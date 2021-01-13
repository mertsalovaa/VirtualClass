using System;
using System.Collections.Generic;
using System.Text;
using VirtualClass.DAL.Entities;

namespace VirtualClass.DLL.JWT
{
    public interface IJWTTokenService
    {
        string CreateToken(DbUser dbUser);
    }
}
