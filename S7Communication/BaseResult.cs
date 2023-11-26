using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication
{
    /// <summary>
    /// 基础结果类型
    /// </summary>
    public class BaseResult
    {
        public BaseResult() { }

        public BaseResult(bool res) 
        {
            Result = res;
        }
        /// <summary>
        /// 返回结果
        /// </summary>
        public bool Result { get; set; } = false;

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; } = "";

        public static implicit operator bool(BaseResult res) => res.Result;

        public static implicit operator BaseResult(bool res) => new BaseResult(res);
    }
}
