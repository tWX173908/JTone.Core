﻿using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace JTone.Core.Test
{
    [TestClass]
    public class EnumExtensionTest
    {
        [TestMethod]
        public void GetDes()
        {
            var text = TestEnum.Default.Text();

            var des = TestDescEnum.Default.Desc();
        }
    }

    public enum TestEnum
    {
        Default = 0
    }

    public enum TestDescEnum
    {
        [System.ComponentModel.Description("默认")]
        Default = 0
    }
}
