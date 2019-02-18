﻿using Models;
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

        Task<List<string>> GetUserMailList();
    }
}
