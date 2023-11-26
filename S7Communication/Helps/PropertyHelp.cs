using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication
{
    /// <summary>
    /// 属性帮助类
    /// </summary>
    public class PropertyHelp
    {
        public static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        /// <summary>
        /// 根据Lambda表达式获取字段名称字符串
        /// </summary>
        /// <typeparam name="T">要获取的类型</typeparam>
        /// <param name="expression">Lambda表达式树对象</param>
        /// <returns>字段名称</returns>
        public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null) throw new ArgumentNullException();
            string propertyName = "";
            switch (expression.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var bodyExpression = (MemberExpression)expression.Body;
                    propertyName = bodyExpression.Member.Name;
                    break;
                case ExpressionType.Convert:
                    propertyName = GetPropertyVlaue("Body.Operand.Member.Name", expression) as string;
                    break;
                default:
                    throw new Exception("表达式过于复杂无法解析！");
                    break;
            }

            //只有一个值 NodeType 类型为 ExpressionType.Convert
            return propertyName;
        }

        /// <summary>
        /// 根据Lambda表达式获取字段名称字符串
        /// </summary>
        /// <typeparam name="T">要获取的类型</typeparam>
        /// <param name="expression">Lambda表达式树对象</param>
        /// <returns>字段名称</returns>
        public static string GetPropertyName<T, F>(Expression<Func<T, F>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException();
            string propertyName = "";
            switch (expression.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var bodyExpression = (MemberExpression)expression.Body;
                    propertyName = bodyExpression.Member.Name;
                    break;
                case ExpressionType.Convert:
                    propertyName = GetPropertyVlaue("Body.Operand.Member.Name", expression) as string;
                    break;
                default:
                    throw new Exception("表达式过于复杂无法解析！");
                    break;
            }

            //只有一个值 NodeType 类型为 ExpressionType.Convert
            return propertyName;
        }

        /// <summary>
        /// 根据Lambda表达式获取字段名称字符串
        /// </summary>
        /// <typeparam name="T">要获取的类型</typeparam>
        /// <param name="expression">Lambda表达式树对象</param>
        /// <returns>字段名称数组</returns>
        public static string[] GetPropertyNames<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null) throw new ArgumentNullException();
            //只有一个值 NodeType 类型为 ExpressionType.Convert
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                return new[] { GetPropertyVlaue("Body.Operand.Member.Name", expression) as string };
            }
            //返回的是一个数组 NodeType 类型为 ExpressionType.NewArrayInit
            if (expression.Body.NodeType == ExpressionType.NewArrayInit)
            {
                var list = (IEnumerable<Expression>)GetPropertyVlaue("Body.Expressions", expression);
                return list.Select(l =>
                {
                    //他的第一项类型为 Convert 后面的均为 MemberAccess
                    if (l.NodeType == ExpressionType.Convert)
                        return GetPropertyVlaue("Operand.Member.Name", l) as string;
                    return GetPropertyVlaue("Member.Name", l) as string;
                }).ToArray();
            }
            throw new Exception("表达式过于复杂无法解析！");
        }

        /// <summary>
        /// 根据路径获取字段值
        /// </summary>
        /// <param name="fullPath">路径</param>
        /// <param name="obj">对象</param>
        /// <returns>获取到的值</returns>
        public static object GetPropertyVlaue(string fullPath, object obj)
        {
            var o = obj;
            fullPath.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(point =>
            {
                o = o.GetType().GetProperty(point,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static)
                    .GetValue(o, null);
            });
            return o;
        }

        /// <summary>
        /// 根据属性名称设置属性值
        /// </summary>
        /// <typeparam name="T">属性所属类的类型</typeparam>
        /// <typeparam name="F">属性的类型</typeparam>
        /// <param name="t">属性所属类的实例</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">要给属性赋的值</param>
        public static void SetPropertyByName<T, F>(T t, string propertyName, F value)
        {
            PropertyInfo property = t.GetType().GetProperty(propertyName);
            if (property != null)
            {
                property.SetValue(t, value, null);
            }
        }

        public static object GetPropertyByName<T>(T t, string propertyName)
        {
            object value = new object();

            PropertyInfo property = typeof(T).GetProperty(propertyName);
            if (property != null)
            {
                value = property.GetValue(t);
            }

            return value;
        }
    }
}
