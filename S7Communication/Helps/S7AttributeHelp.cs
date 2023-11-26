using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication
{
    public static class S7AttributeHelp
    {
        /// <summary>
        /// 获取数据块的地址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int GetDB<T>() where T : BaseDB
        {
            Type type = typeof(T);
            S7DBAttribute sprayingDb = (S7DBAttribute)Attribute.GetCustomAttribute(type, typeof(S7DBAttribute));
            if (sprayingDb == null)
            {
                throw new Exception($"{type.FullName}  没有设置DB标签");
            }
            return sprayingDb.DB;
        }

        /// <summary>
        /// 获取数据块的地址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int GetDB(Type type)
        {
            S7DBAttribute sprayingDb = (S7DBAttribute)Attribute.GetCustomAttribute(type, typeof(S7DBAttribute));
            if (sprayingDb == null)
            {
                throw new Exception($"{type.FullName}  没有设置DB标签");
            }
            return sprayingDb.DB;
        }

        /// <summary>
        /// 获取在DB块里面的位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static (int StartByteAdr, Byte BitAdr) GetAdress<T>(string name) where T : BaseDB
        {
            Type type = typeof(T);
            PropertyInfo propertyInfo = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            if (propertyInfo == null)
            {
                throw new Exception($"{type.FullName}  的 {name}  属性不存在");
            }
            S7PropertyAttribute s7Property = (S7PropertyAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(S7PropertyAttribute));
            if (s7Property == null)
            {
                throw new Exception($"{type.FullName}  没有设置DB标签");
            }
            return (s7Property.StartByteAdr, s7Property.BitAdr);
        }

        /// <summary>
        /// 获取在DB块里面的位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static (int StartByteAdr, Byte BitAdr) GetAdress(Type type, string name)
        {
            PropertyInfo propertyInfo = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            if (propertyInfo == null)
            {
                throw new Exception($"{type.FullName}  的 {name}  属性不存在");
            }
            S7PropertyAttribute s7Property = (S7PropertyAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(S7PropertyAttribute));
            if (s7Property == null)
            {
                throw new Exception($"{type.FullName}  没有设置DB标签");
            }
            return (s7Property.StartByteAdr, s7Property.BitAdr);
        }
    }
}
