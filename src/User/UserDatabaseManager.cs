using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;


namespace JwtTokenSpace
{
    public class UserDatabaseManager :IUserRepository
    {

        UserDatabase db_obj =null;

        public UserDatabaseManager(UserDatabase db_obj1)
        {
            this.db_obj=db_obj1;
        }

        public User GetUser(string email)
        {
            using(db_obj)
            {
                 try
              {
                  return db_obj.users.First(user => user.Email.Equals(email));
              } catch
              {
                  return null;
              }

            }
             
        }

            public string Hash(string password)
            {
                using(var sha256 = SHA256.Create())  
                {  
                    // Converting Password to Hash.  
                    var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));  
                    // Get the hashed string.  
                    return System.BitConverter.ToString(hashedBytes);  
                }  
            }

            

            public void Register(User obj)
            {  
                using(db_obj)
                {
                    db_obj.users.Add(obj);
                    db_obj.SaveChanges();

                }
               
                

            }

    ~UserDatabaseManager()
    {

    db_obj.Dispose();

    }

    }
}