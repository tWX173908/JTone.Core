using System;
using System.Collections.Concurrent;
using System.Threading;


namespace JTone.Core
{
    /// <summary>
    /// 时间Id生成
    /// </summary>
    /// <remarks>
    /// 高并发，整形趋势递增，可读
    /// 其核心思想是：2020年10月29日23点59分59秒999毫秒 表示为
    /// 10 29 20 23 59 59 999  + 9位服务器 + 单毫秒最大999
    /// 月 日 年 时 分 秒 毫秒
    /// </remarks>
    public class TimeId
    {
        /// <summary>
        /// 毫秒计数器
        /// </summary>
        private long _sequence;

        //工作线程
        private readonly long _workId;

        //每毫秒最大值
        private const long PerMillisecond = 1000;

        //工作线程设置
        public const long MaxWorkId = 10;
        public const long MinWorkId = 0;

        public TimeId(long workId = 0)
        {
            _workId = workId;
        }

        /// <summary>
        /// 生成ID
        /// </summary>
        /// <returns></returns>
        public long NewId()
        {
            Interlocked.Increment(ref _sequence);
            _sequence = 1000 * (_workId % MaxWorkId) + _sequence % PerMillisecond;
            return LongTime(DateTime.Now) * 10000 + _sequence;
        }


        /// <summary>
        /// 时间的长整型表示
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private long LongTime(DateTime time)
        {
            return time.Millisecond +
                   time.Second * 1000L +
                   time.Minute * 100000L +
                   time.Hour * 10000000L +
                   time.Year % 100 * 1000000000L +
                   time.Day *        100000000000L +
                   time.Month *      10000000000000L;
        }
    }


    /// <summary>
    /// 时间Id帮助类
    /// </summary>
    public class TimeIdHelper
    {
        private static readonly ConcurrentDictionary<long, TimeId> TimeIds = new ConcurrentDictionary<long, TimeId>();


        /// <summary>
        /// 生成ID
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        public static long NewId(long workId = 1)
        {
            if (workId <= TimeId.MinWorkId || workId >= TimeId.MaxWorkId)
            {
                throw new ArgumentOutOfRangeException(nameof(workId));
            }

            if (TimeIds.ContainsKey(workId))
            {
                return TimeIds[workId].NewId();
            }

            if (TimeIds.TryAdd(workId, new TimeId(workId)))
            {
                return TimeIds[workId].NewId();
            }

            return 0;
        }
    }


    /// <summary>
    /// 雪花算法
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
    /// 有序GuiId 
    /// </summary>
    /// <remarks>
    /// https://stackoverflow.com/questions/1752004/sequential-guid-generator
    /// </remarks>
    public class SequentialGuid
    {
        private Guid _currentGuid;
        public Guid CurrentGuid => _currentGuid;

        private static readonly object Lock = new object();

        public SequentialGuid()
        {
            _currentGuid = Guid.NewGuid();
        }

        public static SequentialGuid operator ++(SequentialGuid sequentialGuid)
        {
            lock (Lock)
            {
                var bytes = sequentialGuid._currentGuid.ToByteArray();
                for (var mapIndex = 0; mapIndex < 16; mapIndex++)
                {
                    var bytesIndex = SqlOrderMap[mapIndex];
                    bytes[bytesIndex]++;
                    if (bytes[bytesIndex] != 0)
                    {
                        break; // No need to increment more significant bytes
                    }
                }
                sequentialGuid._currentGuid = new Guid(bytes);
                return sequentialGuid;
            }
        }

        private static int[] _sqlOrderMap;
        private static int[] SqlOrderMap
        {
            get
            {
                return _sqlOrderMap ?? (_sqlOrderMap = new []
                {
                    3, 2, 1, 0, 5, 4, 7, 6, 9, 8, 15, 14, 13, 12, 11, 10
                });
            }
        }
    }


    /// <summary>
    /// 有序GuiId帮助类
    /// </summary>
    public class SequentialGuidHelper
    {
        private static SequentialGuid _sequentialGuid = new SequentialGuid();

        public static string NewId()
        {
            return (_sequentialGuid++).CurrentGuid.ToString();
        }
    }
}
