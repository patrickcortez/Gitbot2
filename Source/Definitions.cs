using NetCord.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gitbot2.Source
{

    internal sealed class _Roles
    {
        public string[] Roles { get; set; }

        public string GenId { get; set; }
    }

    

    internal enum RoleStatus
    {
        Allowed,
        NotAllowed,
        Error
    }

}
