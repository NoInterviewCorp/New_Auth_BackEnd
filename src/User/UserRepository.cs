using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System;


namespace JwtTokenSpace
{

    public class UserRepository : IUserRepository
    {
        static List<User> TestUsers = new List<User>();
        public UserRepository() { }
        public User GetUser(string email)
        {
            try
            {
                return TestUsers.First(user => user.Email.Equals(email));
            }
            catch
            {
                return null;
            }
        }

        public string Hash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Converting Password to Hash.  
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                // Get the hashed string.  
                return System.BitConverter.ToString(hashedBytes);
            }
        }



        public void Register(User obj)
        {
            TestUsers.Add(obj);


        }

    }

}