using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTSocket
{
    public enum MTMessageType : byte
    {
        Logon = 1,
        DataPackage = 2,
        EndMessage = 4,
        KeepAlive = 8,
        Logout = 16,
    }
}
