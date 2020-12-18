using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace JTone.Core
{
    /// <summary>
    /// 类型扩展
    /// </summary>
    public static class TypeExtension
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, List<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, List<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, List<FieldInfo>> TypeFields = new ConcurrentDictionary<RuntimeTypeHandle, List<FieldInfo>>();

        //基本类型
        private static readonly List<Type> SimpleTypes = new List<Type>
        {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(bool),
            typeof(string),
            typeof(char),
            typeof(Guid),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(byte[])
        };


        /// <summary>
        /// 当前类型是否是简单类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSimpleType(this Type type)
        {
            return SimpleTypes.Contains(type);
        }


        /// <summary>
        /// 获取类型属性信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetTypeProperties(this Type type)
        {
            if (TypeProperties.TryGetValue(type.TypeHandle, out var typePropertyInfos))
            {
                return typePropertyInfos;
            }

            var props = type.GetProperties().ToList();

            TypeProperties[type.TypeHandle] = props;
            return props;
        }


        /// <summary>
        /// 获取类型字段信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<FieldInfo> GetTypeFields(this Type type)
        {
            if (TypeFields.TryGetValue(type.TypeHandle, out var typeFields))
            {
                return typeFields;
            }

            var fields = type.GetFields().ToList();

            TypeFields[type.TypeHandle] = fields;
            return fields;
        }
    }
}
