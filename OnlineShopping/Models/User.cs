using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopping.Models
{
    public class User
    {
        public string Ip { get; set; }

        public int Id { get; set; }

        public virtual List<Video> Videos { get; set; }
    }

    public static class VideoOwned
    {
        public static List<User> Users = new List<User>();

        public static List<User> AddUser(string ip, int id)
        {
            if (Users.FirstOrDefault(x => x.Ip == ip && x.Id == id) != null) return Users;

            var user = new User { Ip = ip, Id = id };
            Users.Add(user);

            return Users;
        }
    }
}
