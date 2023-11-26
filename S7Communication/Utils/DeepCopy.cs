using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace S7Communication
{
    public static class DeepCopy
    {
        #region 经典的深拷贝方法

        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopyByXml<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = xml.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }

        public static T DeepCopyByJson<T>(T obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json);
        }



        public static object DeepCopyByXml(object obj, Type type)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(type);
                xml.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = xml.Deserialize(ms);
                ms.Close();
            }
            return retval;
        }

        public static TOut TransReflection<TIn, TOut>(TIn tIn)
        {
            TOut tOut = Activator.CreateInstance<TOut>();
            var tInType = tIn.GetType();
            foreach (var itemOut in tOut.GetType().GetProperties())
            {
                var itemIn = tInType.GetProperty(itemOut.Name); ;
                if (itemIn != null)
                {
                    itemOut.SetValue(tOut, itemIn.GetValue(tIn));
                }
            }
            return tOut;
        }

        /// <summary>
        /// 对于部分类型实际上不可用(ObserverCollection实际上不可用)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Obsolete("这种写法实际上对于")]
        public static T DeepCopyByReflect<T>(T obj)
        {
            //如果是字符串或值类型则直接返回
            if (obj is string || obj.GetType().IsValueType) return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try
                {
                    field.SetValue(retval, DeepCopyByReflect(field.GetValue(obj)));
                }
                catch (Exception)
                {

                }
            }
            return (T)retval;
        }

        /// <summary>
        /// 对于部分类型实际上不可用(ObserverCollection实际上不可用)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void DeepCopyByRef<T>(this T instance, T obj)
        {
            ObjectHelp.UpdateInstance(instance, obj);
        }

        public static T DeepCopyByRef<T>(T obj, Type type)
        {
            //如果是字符串或值类型则直接返回
            if (type.IsValueType || type.Equals(typeof(System.String)) ||
                type.IsEnum)
            {
                return obj;
            }

            object retval = Activator.CreateInstance(type);

            ObjectHelp.UpdateInstance(retval, obj, type);
            return (T)retval;
        }

        public static T DeepCopyByRef<T>(T obj)
        {
            Type type = typeof(T);
            return DeepCopyByRef<T>(obj, type);
        }

        public static T DeepCopyByBin<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                //序列化成流
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                //反序列化成对象
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }
        #endregion

        /// <summary>
        /// 传入类型B的对象b，将b与a相同名称的值进行赋值给创建的a中        
        /// </summary>
        /// <typeparam name="A">类型A</typeparam>
        /// <typeparam name="B">类型B</typeparam>
        /// <param name="b">类型为B的参数b</param>
        /// <returns>拷贝b中相同属性的值的a</returns>
        public static A MapperTwo<A, B>(B b)
        {
            A a = Activator.CreateInstance<A>();
            try
            {
                Type Typeb = typeof(B);//获得类型  
                Type Typea = typeof(A);
                foreach (PropertyInfo ap in Typea.GetProperties())
                {
                    System.Reflection.PropertyInfo bp = Typeb.GetProperty(ap.Name); //获取指定名称的属性
                    if (bp != null) //如果B对象也有该属性
                    {
                        if (ap.GetSetMethod() != null) //判断A对象是否有能用Set方法
                        {
                            if (bp.GetGetMethod() != null) //判断B对象是否有能用Get方法
                            {
                                ap.SetValue(a, bp.GetValue(b, null), null);//获得b对象属性的值复制给a对象的属性   
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            return a;
        }
    }
}
