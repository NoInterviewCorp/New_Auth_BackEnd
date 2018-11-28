using System.Collections.Generic;

//namespace sample
namespace JwtTokenSpace
{

    public interface IUserRepository
    {
        User GetUser(string Email);

        string Hash(string password);

         void Register(User obj);


        
        
    }
}
