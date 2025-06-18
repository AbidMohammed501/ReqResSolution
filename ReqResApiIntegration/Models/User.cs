using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReqResApiIntegration.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }

    public class UserDataResponse
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public List<User> Data { get; set; } = new();
    }

    public class SingleUserResponse
    {
        public User Data { get; set; } = new();
    }
}
