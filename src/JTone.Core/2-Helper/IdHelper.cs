using System;
using System.Threading;


namespace JTone.Core
{
    /// <summary>
    /// Id生成 TODO
    /// </summary>
    /// <remarks>
    /// 高并发，整形趋势递增，可读
    /// 其核心思想是：
    /// 使用41bit作为毫秒数，
    /// 10bit作为机器的ID（5个bit是数据中心，5个bit的机器ID），
    /// 12bit作为毫秒内的流水号（意味着每个节点在每毫秒可以产生 4096 个 ID），
    /// 最后还有一个符号位，永远是0。
    /// </remarks>
    public class TimeIdHelper
    {
        /// <summary>
        /// 毫秒计数器
        /// </summary>
        private static long _sequence;

        //数据库
        private static long _workId;
        public static long WorkId {
            get => _workId;
            set
            {
                if (value > 10)
                {
                    throw new  ArgumentOutOfRangeException(nameof(WorkId));
                }

                _workId = value;
            }
        }

        /// <summary>
        /// 获取新的ID
        /// </summary>
        /// <returns></returns>
        public static long NewId()
        {
            return LongTime(DateTime.Now) * 1000 + Interlocked.Increment(ref _sequence);
        }


        /// <summary>
        /// 时间的长整型表示
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static long LongTime(DateTime time)
        {
            return time.Millisecond +
                   time.Second * 1000L +
                   time.Minute * 100000L +
                   time.Hour * 10000000L +
                   time.Day * 1000000000L +
                   time.Month * 100000000000L +
                   time.Year % 100 * 10000000000000L;
        }
    }


    /// <summary>
    /// 雪花算法 TODO
    /// </summary>
    /// <remarks>
    /// 其核心思想是：
    /// 使用41bit作为毫秒数，
    /// 10bit作为机器的ID（5个bit是数据中心，5个bit的机器ID），
    /// 12bit作为毫秒内的流水号（意味着每个节点在每毫秒可以产生 4096 个 ID），
    /// 最后还有一个符号位，永远是0。
    /// </remarks>
    public class SnowFlakeIdHelper
    {
        //UTC基准时间
        private static readonly DateTime BaseUtcTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //基准时间
        public const long BaseTime = 1288834974657L;
        //机器标识位数
        const int WorkerIdBits = 5;
        //数据标志位数
        const int DatacenterIdBits = 5;
        //序列号识位数
        const int SequenceBits = 12;
        //机器ID最大值
        const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        //数据标志ID最大值
        const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);
        //序列号ID最大值
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);
        //机器ID偏左移12位
        private const int WorkerIdShift = SequenceBits;
        //数据ID偏左移17位
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        //时间毫秒左移22位
        public const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

        private long _sequence;
        private long _lastTimestamp = -1L;

        public long WorkerId { get; protected set; }
        public long DatacenterId { get; protected set; }


        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="workerId"></param>
        /// <param name="datacenterId"></param>
        public SnowFlakeIdHelper(long workerId, long datacenterId)
        {
            if (workerId > MaxWorkerId || workerId < 0)
            {
                throw new ArgumentException($"worker Id 必须大于0，且不能大于MaxWorkerId： {MaxWorkerId}");
            }

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException($"region Id 必须大于0，且不能大于MaxWorkerId： {MaxDatacenterId}");
            }

            WorkerId = workerId;
            DatacenterId = datacenterId;
        }

        readonly object _lock = new object();


        /// <summary>
        /// 生产id
        /// </summary>
        /// <returns></returns>
        public long NextId()
        {
            lock (_lock)
            {
                var timestamp = TilNextMillis(_lastTimestamp);

                //如果上次生成时间和当前时间相同,在同一毫秒内
                if (_lastTimestamp == timestamp)
                {
                    //sequence自增，和sequenceMask相与一下，去掉高位
                    _sequence = (_sequence + 1) & SequenceMask;
                    //判断是否溢出,也就是每毫秒内超过1024，当为1024时，与sequenceMask相与，sequence就等于0
                    if (_sequence == 0)
                    {
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    //如果和上次生成时间不同,重置sequence，就是下一毫秒开始，sequence计数重新从0开始累加,
                    //为了保证尾数随机性更大一些,最后一位可以设置一个随机数
                    _sequence = 0;//new Random().Next(10);
                }

                _lastTimestamp = timestamp;
                return ((timestamp - BaseTime) << TimestampLeftShift) | (DatacenterId << DatacenterIdShift) | (WorkerId << WorkerIdShift) | _sequence;
            }
        }


        /// <summary>
        /// 防止产生的时间比之前的时间还要小（由于NTP回拨等问题）,保持增量的趋势.
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        /// <remarks>
        /// 只能解决单机时钟回拨问题，不能保证全数据中心整体是趋势递增的
        /// </remarks>
        private long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }


        /// <summary>
        /// 获取当前的时间戳
        /// </summary>
        /// <returns></returns>
        private long TimeGen()
        {
            return (long)(DateTime.UtcNow - BaseUtcTime).TotalMilliseconds;
        }
    }


    /// <summary>
    /// 有序GuiId TODO
    /// </summary>
    /// <remarks>
    /// 分片：
    /// 一致性HASH
    /// 区间范围分片
    /// 时间范围分片
    /// 目录分片（额外维护查找表）
    /// 分区：
    /// 时间范围分区，归档
    /// </remarks>
    public class SequentialGuidHelper
    {
        private static long _counter;

        /// <summary>
        /// 生产id
        /// </summary>
        /// <returns></returns>
        public static string NextId()
        {
            var guid = Guid.NewGuid().ToByteArray();
            var ticks = BitConverter.GetBytes(GetTicks());

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(ticks);

            return new Guid(new[]
                {
                    ticks[1], ticks[0], ticks[7], ticks[6],
                    ticks[5], ticks[4], ticks[3], ticks[2],
                    guid[0], guid[1], guid[2], guid[3],
                    guid[4], guid[5], guid[6], guid[7]
                }).ToString();
        }


        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        private static long GetTicks()
        {
            if (_counter == 0)
                _counter = DateTime.UtcNow.Ticks;

            return Interlocked.Increment(ref _counter);
        }
    }
}
