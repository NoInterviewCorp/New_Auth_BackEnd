using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;


namespace JwtTokenSpace
{
    public class UserDatabaseManager : IUserRepository
    {

        UserDatabase db_obj = null;

        public UserDatabaseManager(UserDatabase db_obj1)
        {
            this.db_obj = db_obj1;
        }

        // This fuction get the user from the Database

        public User GetUser(string email)
        {

            try
            {
                return db_obj.users.First(user => user.Email.Equals(email));
            }
            catch
            {
                return null;
            }



        }

        // This function Hash the password

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


        // This function register the user in the DataBase

        public void Register(User obj)
        {

            db_obj.users.Add(obj);
            db_obj.SaveChanges();

        }


    }
}