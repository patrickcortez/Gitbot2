using System;
using System.Collections.Generic;
using System.Text;

namespace Gitbot2.Source
{
    internal enum Channels : ulong
    {
        Genchat = 1461357770308583597 
    }

    internal enum Roles : ulong
    {
        Owner = 1528288849778704414,
        Developer = 1463104698558054401
    }

    internal enum RoleStatus
    {
        Allowed,
        NotAllowed,
        Error
    }

}
