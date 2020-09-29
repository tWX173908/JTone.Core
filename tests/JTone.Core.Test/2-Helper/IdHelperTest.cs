using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace JTone.Core.Test
{
    [TestClass]
    public class IdHelperTest
    {
        /// <summary>
        /// 时间戳id
        /// </summary>
        [TestMethod]
        public void TimeIdHelperTest()
        {
            for (int i = 0; i < 100000; i++)
            {
                Console.WriteLine(TimeIdHelper.NewId());
            }

            //var t1Result = new List<long>();
            //var t1 = Task.Run(() =>
            //{
            //    var i = 0;
            //    while (i++ < 100000)
            //    {
            //        t1Result.Add(TimeIdHelper.NewId());
            //    }
            //});

            //var t2Result = new List<long>();
            //var t2 = Task.Run(() =>
            //{
            //    var i = 0;
            //    while (i++ < 100000)
            //    {
            //        t2Result.Add(TimeIdHelper.NewId());
            //    }
            //});

            //var t3Result = new List<long>();
            //var t3 = Task.Run(() =>
            //{
            //    var i = 0;
            //    while (i++ < 100000)
            //    {
            //        t3Result.Add(TimeIdHelper.NewId());
            //    }
            //});

            //Task.WaitAll(t1, t2, t3);

            //Assert.IsTrue(!t1Result.Intersect(t2Result).Intersect(t3Result).Any());
        }


        /// <summary>
        /// 雪花id
        /// </summary>
        [TestMethod]
        public void SnowFlakeHelperTest()
        {
            var t1Result = new List<long>();
            var t1 = Task.Run(() =>
            {
                var snow = new SnowFlakeIdHelper(0,0);
                var i = 0;
                while (i++ < 100000)
                {
                    t1Result.Add(snow.NextId());
                }
            });

            var t2Result = new List<long>();
            var t2 = Task.Run(() =>
            {
                var snow = new SnowFlakeIdHelper(1, 0);
                var i = 0;
                while (i++ < 100000)
                {
                    t2Result.Add(snow.NextId());
                }
            });

            var t3Result = new List<long>();
            var t3 = Task.Run(() =>
            {
                var snow = new SnowFlakeIdHelper(2, 0);
                var i = 0;
                while (i++ < 100000)
                {
                    t3Result.Add(snow.NextId());
                }
            });

            Task.WaitAll(t1, t2, t3);

            Assert.IsTrue(!t1Result.Intersect(t2Result).Intersect(t3Result).Any());
        }

        
        /// <summary>
        /// 有序的GuiId
        /// </summary>
        [TestMethod]
        public void SequentialGuidHelperTest()
        {
            var t = SequentialGuidHelper.NextId();

            var t1Result = new List<string>();
            var t1 = Task.Run(() =>
            {
                var i = 0;
                while (i++ < 100000)
                {
                    t1Result.Add(SequentialGuidHelper.NextId());
                }
            });

            var t2Result = new List<string>();
            var t2 = Task.Run(() =>
            {
                var i = 0;
                while (i++ < 100000)
                {
                    t2Result.Add(SequentialGuidHelper.NextId());
                }
            });

            var t3Result = new List<string>();
            var t3 = Task.Run(() =>
            {
                var i = 0;
                while (i++ < 100000)
                {
                    t3Result.Add(SequentialGuidHelper.NextId());
                }
            });

            Task.WaitAll(t1, t2, t3);

            Assert.IsTrue(!t1Result.Intersect(t2Result).Intersect(t3Result).Any());
        }
    }
}
