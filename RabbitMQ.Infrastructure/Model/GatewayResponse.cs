using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BrowserTextRPG.Model
{
#nullable enable
    public class GatewayResponse<T>
    {
        public T Data { get; set; } = default(T);
        public Fault? Fault { get; set; } = default(Fault);
    }
#nullable disable
}
