using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace JTone.Core.Test
{
    [TestClass]
    public class EnumExtensionTest
    {


        [TestMethod]
        public void GetDes()
        {
            Assert.AreEqual(TestEnum.Test.Value(), 1);

            Assert.AreEqual(TestEnum.Default.Text(), "Default");

            Assert.AreEqual(TestDescEnum.Default.Desc(), "默认");

            Assert.AreEqual(1.ToEnum<int, TestEnum>(), TestEnum.Test);

            Assert.ThrowsException<NotSupportedException>(()=>999.ToEnum<int, TestEnum>());

            Assert.AreEqual("1".ToEnum<TestEnum>(), TestEnum.Test);

            Assert.AreEqual("建筑".ToEnum(StrConvertToEnumTestClass.MajorConvert), JianZhuMajorEnum.JianZhu);

            Assert.AreEqual("结构".ToEnum(StrConvertToEnumTestClass.MajorConvert2), JieGouMajorEnum.JieGou);
        }
    }


    public class StrConvertToEnumTestClass
    {
        public static JianZhuMajorEnum MajorConvert(string major)
        {
            switch (major)
            {
                case "建筑":
                    return JianZhuMajorEnum.JianZhu;
                default:
                    return JianZhuMajorEnum.Default;
            }
        }

        public static JieGouMajorEnum MajorConvert2(string major)
        {
            switch (major)
            {
                case "结构":
                    return JieGouMajorEnum.JieGou;
                default:
                    return JieGouMajorEnum.Default;
            }
        }
    }

    public enum JianZhuMajorEnum
    {
        Default = 0,

        JianZhu = 1
    }


    public enum JieGouMajorEnum
    {
        Default = 0,

        JieGou = 1
    }


    public enum TestEnum
    {
        Default = 0,

        Test = 1
    }

    public enum TestDescEnum
    {
        [System.ComponentModel.Description("默认")]
        Default = 0
    }
}
