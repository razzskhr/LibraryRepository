using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceRepository
{
    public interface IUserRepository
    {
        Task<List<UserDetails>> GetAllUsers();

        Task<Response<string>> RegisterUser(LoginDetails userLoginDetails, UserDetails userdetails);

        UserDetails GetLoggedInUserDetails(string username);


        Task<bool> UpdatePassword(LoginDetails userLoginDetails);

        Task<Response<string>> InsertImageFileName(string UserName, string image);

        Task<List<string>> GetUserMailList();
    }
}
