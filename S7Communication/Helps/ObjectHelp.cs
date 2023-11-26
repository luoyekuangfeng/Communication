using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication
{
    /// <summary>
    /// 对象帮助类
    /// </summary>
    public class ObjectHelp
    {
        /// <summary>
        /// 根据字段名称设置字段值
        /// </summary>
        /// <typeparam name="T">字段所属类的类型</typeparam>
        /// <typeparam name="F">字段的类型</typeparam>
        /// <param name="t">字段所属类的实例</param>
        /// <param name="fileName">字段名称</param>
        /// <param name="value">要给字段赋的值</param>
        public static void SetPropertyByName<T, F>(T t, string fileName, F value)
        {

            FieldInfo field = t.GetType().GetField(fileName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (field != null)
            {
                field.SetValue(t, value);
            }

        }

        /// <summary>
        /// 根据字段名称设置字段值
        /// </summary>
        /// <typeparam name="T">字段所属类的类型</typeparam>
        /// <typeparam name="F">字段的类型</typeparam>
        /// <param name="t">字段所属类的实例</param>
        /// <param name="fileName">字段名称</param>
        /// <param name="value">要给字段赋的值</param>
        public static void SetPropertyByName<T>(T t, string fileName, object value)
        {
            FieldInfo field = t.GetType().GetField(fileName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (field != null)
            {
                field.SetValue(t, value);
            }
        }

        /// <summary>
        /// 根据字段名称设置字段值
        /// </summary>
        /// <typeparam name="T">字段所属类的类型</typeparam>
        /// <typeparam name="F">字段的类型</typeparam>
        /// <param name="t">字段所属类的实例</param>
        /// <param name="fileName">字段名称</param>
        /// <param name="value">要给字段赋的值</param>
        public static void SetPropertyByName<T>(T t, FieldInfo field, string fileName, object value)
        {
            if (field != null)
            {
                field.SetValue(t, value);
            }
        }



        /// <summary>
        /// 根据属性名称get属性值(是Get对象，而不是新创建的对象)
        /// </summary>
        /// <typeparam name="T">属性所在类</typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="t">属性所在类的实例</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>属性的值</returns>
        public static F GetValueByProperName<T, F>(T t, string propertyName)
        {
            Type type = t.GetType();
            F obj = default(F);
            PropertyInfo property = type.GetProperty(propertyName);
            if (property != null)
            {
                obj = (F)property.GetValue(t, null);
            }
            return obj;
        }

        /// <summary>
        /// 根据属性名称get属性值(是Get对象，而不是新创建的对象)
        /// </summary>
        /// <typeparam name="T">属性所在类</typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="t">属性所在类的实例</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>属性的值</returns>
        public static object GetValueByProperName<T>(T t, string propertyName)
        {
            Type type = t.GetType();
            object obj = default(object);
            PropertyInfo property = type.GetProperty(propertyName);
            if (property != null)
            {
                obj = property.GetValue(t, null);
            }
            return obj;
        }

        /// <summary>
        /// 根据属性名称get属性值(是Get对象，而不是新创建的对象)
        /// </summary>
        /// <typeparam name="T">属性所在类</typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="t">属性所在类的实例</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>属性的值</returns>
        public static object GetValueByProperName<T>(T t, Expression<Func<T>> action)
        {
            string propertyName = PropertyHelp.GetPropertyName<T>(action);
            Type type = t.GetType();
            object obj = default(object);
            PropertyInfo property = type.GetProperty(propertyName);
            if (property != null)
            {
                obj = property.GetValue(t, null);
            }
            return obj;
        }

        /// <summary>
        /// 根据属性名称get属性值(是Get对象，而不是新创建的对象)
        /// </summary>
        /// <typeparam name="T">属性所在类</typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="t">属性所在类的实例</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>属性的值</returns>
        public static F GetValueByFieldName<T, F>(T t, string fieldName)
        {
            Type type = t.GetType();
            F obj = default(F);
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (fieldInfo != null)
            {
                obj = (F)fieldInfo.GetValue(t);

            }
            return obj;
        }




        /// <summary>
        /// 更新对象(不算拷贝，对象不变，修改内容 相当于逐个用等于赋值)(字典集合不能赋值)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldInstance">旧值</param>
        /// <param name="newInstance">新值</param>
        public static void UpdateInstance<T>(T oldInstance, T newInstance)
        {
            //保证输入对象一致
            UpdateInstance(oldInstance, newInstance, typeof(T));
        }

        private static void UpdateFiled(FieldInfo[] filedInfos, object oldInstance, object newInstance)
        {
            if (filedInfos.Count() == 0)
            {
                return;
            }

            foreach (FieldInfo filedInfo in filedInfos)
            {
                object oldValue = filedInfo.GetValue(oldInstance);
                //新值
                object newValue = filedInfo.GetValue(newInstance);

                if (oldValue == newValue)
                {
                    continue;
                }

                Type childrenFiledType = filedInfo.FieldType;
                if (childrenFiledType.IsValueType || childrenFiledType.Equals(typeof(System.String)) ||
                    childrenFiledType.IsEnum)
                {
                    filedInfo.SetValue(oldInstance, newValue);
                }
                else
                {
                    //处理null值，空值无法用反射
                    if (newValue == null)
                    {
                        filedInfo.SetValue(oldInstance, null);
                        //filedInfo.SetValue(oldInstance, DeepCopyByXml(newValue, childrenFiledType));
                    }
                    else if (oldValue == null)
                    {
                        //创建一个对象 将对象赋值给旧的值 然后用新值替换旧的值
                        oldValue = Activator.CreateInstance(childrenFiledType);
                        filedInfo.SetValue(oldInstance, oldValue);
                        UpdateInstance(oldValue, newValue, childrenFiledType);
                    }
                    else
                    {
                        FieldInfo[] childreFieldInfos = childrenFiledType.GetFields();
                        UpdateInstance(oldValue, newValue, childrenFiledType);
                    }
                }
            }
        }

        private static void UpdateProperty(PropertyInfo[] propertyInfos, object oldInstance, object newInstance)
        {
            try
            {
                if (propertyInfos.Count() == 0)
                {
                    return;
                }
                foreach (var item in propertyInfos)
                {
                    try
                    {
                        string propertyName = item.Name;
                        Console.WriteLine(propertyName);
                        //老值
                        object oldValue = item.GetValue(oldInstance, null);
                        //新值
                        object newValue = item.GetValue(newInstance, null);

                        Type childrenFiledType = item.PropertyType;


                        if (childrenFiledType.IsValueType || childrenFiledType.Equals(typeof(System.String)) ||
                            childrenFiledType.IsEnum)
                        {
                            //新值和旧值相等，则不更新
                            if (oldValue != null && newValue != null && oldValue.ToString().Equals(newValue.ToString()))
                                continue;
                            var vre = oldValue?.GetType().Name;
                            var ewq = newValue?.GetType().Name;
                            try
                            {
                                //为值类型设置值
                                item.SetValue(oldInstance, newValue, null);
                            }
                            catch (Exception e)
                            {

                            }

                        }
                        else
                        {
                            //引用类型 接着遍历
                            if (newValue == null)
                            {
                                item.SetValue(oldInstance, null, null);
                            }
                            else if (oldValue == null)
                            {
                                //创建一个对象 将对象赋值给旧的值 然后用新值替换旧的值
                                oldValue = Activator.CreateInstance(childrenFiledType);
                                item.SetValue(oldInstance, oldValue);
                                UpdateInstance(oldValue, newValue, childrenFiledType);
                            }
                            else
                            {
                                try
                                {
                                    if (typeof(IEnumerable).IsAssignableFrom(childrenFiledType))
                                    {
                                        SetListValue(oldValue, newValue);
                                        continue;
                                    }
                                }
                                catch (Exception e)
                                {

                                }

                                UpdateInstance(oldValue, newValue, childrenFiledType);
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            catch (Exception e)
            {
            }

        }

        /// <summary>
        /// 通过反射进行拷贝
        /// </summary>
        /// <param name="oldInstance">需要赋值得对象</param>
        /// <param name="newInstance">数据源</param>
        /// <param name="type">目标数据(需要赋值得对象)的类型</param>
        /// <returns>如果时值类型，则返回值类型数据(包括字符串)，否则返回空</returns>
        public static object UpdateInstance(object oldInstance, object newInstance, Type type)
        {
            try
            {
                if (newInstance == null)
                {
                    oldInstance = null;
                    return null;
                }

                //枚举类型 值类型 string类型直接返回
                if (type.IsValueType || type.Equals(typeof(System.String)) ||
                    type.IsEnum)
                {
                    oldInstance = newInstance;
                    return oldInstance;
                }

                if (!typeof(IEnumerable).IsAssignableFrom(type))
                {

                    //获取字段
                    FieldInfo[] filedInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                    //获取属性
                    PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                    #region 字段
                    UpdateFiled(filedInfos, oldInstance, newInstance);
                    #endregion

                    #region 属性
                    //更新属性
                    UpdateProperty(propertyInfos, oldInstance, newInstance);
                    #endregion  
                }
                else
                {
                    SetListValue(oldInstance, newInstance);
                }

            }
            catch (Exception EX)
            {

            }

            return null;

        }

        private static void SetListValue(object oldInstance, object newInstance)
        {
            var oldValue_collect = (IList)oldInstance;
            var newValue_collect = (IList)newInstance;
            if (newValue_collect.Count == 0)
            {
                oldValue_collect.Clear();
            }
            else
            {
                oldValue_collect.Clear();
                Type? newValueType = newValue_collect[0]?.GetType();
                if (newValueType != null)
                {
                    for (int i = 0; i < newValue_collect.Count; i++)
                    {
                        var old_ValueItem = Activator.CreateInstance(newValueType);
                        var result = UpdateInstance(old_ValueItem, newValue_collect[i], newValueType);

                        if (result != null)
                        {
                            oldValue_collect.Add(result);
                        }
                        else
                        {
                            oldValue_collect.Add(old_ValueItem);
                        }
                    }
                }

            }
        }

    }
}
