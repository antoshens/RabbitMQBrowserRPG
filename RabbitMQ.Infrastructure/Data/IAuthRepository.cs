using BrowserTextRPG.DTOModel.User;
using BrowserTextRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.Data
{
    public interface IAuthRepository
    {
        Task<GatewayResponse<int>> Register(GetUserDto user);
        Task<GatewayResponse<string>> Login(string username, string password);
        Task<GatewayResponse<bool>> UserExist(string username);
    }
}
