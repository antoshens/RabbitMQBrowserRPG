using BrowserTextRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.DTOModel.User
{
    public class GetUserDto
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }
}
