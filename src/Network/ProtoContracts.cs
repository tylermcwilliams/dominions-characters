using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace playerskins
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class SkinChange
    {
        public string part;
        public string variant;
    }
}
