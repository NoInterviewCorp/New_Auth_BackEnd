using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System;

namespace sample
{

    public class UserRepository : IUserRepository
      {
          static List<User> TestUsers = new List<User>{
              new User() { Email = "ro.ri@gmail.com", Password  = "Pass11",FullName="bro"},
              new User() { Email = "rr.rr@gmail.com", Password = "Pass22",FullName="bro1"},
              new User() { Email = "a@gmail.com", Password = "abc",FullName="bro2"}
          };
          public UserRepository()
          {
              
          }
            public User GetUser(string email)
            {
              try
              {
                  return TestUsers.First(user => user.Email.Equals(email));
              } catch
              {
                  return null;
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
                TestUsers.Add(obj);
                

            }

        }

}