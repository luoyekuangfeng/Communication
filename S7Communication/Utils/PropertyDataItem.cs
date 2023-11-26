using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication
{
    public class PropertyDataItem : DataItem
    {
        public PropertyInfo PropertyInfo { get; set; }
    }
}
