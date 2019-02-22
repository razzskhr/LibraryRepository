using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public static class CollectionConstant
    {
        public const string Book_Collection = "bookDetails";
        public const string User_Collection = "userDetails";
        public const string Config_Collection = "configDetails";
        public const string Login_Collection = "loginDetails";
        public const string Roles_Collection = "RolesDetails";
        public const string userDetails_copy = "userDetails_copy";
        public const string bookDetails_copy = "bookDetails_copy";
    }

    public enum RoleType
    {
        Student,
        Admin
    }

    public enum GenderType
    {
        Male,
        Female
    }
}
