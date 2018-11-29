using System.Collections.Generic;


namespace JwtTokenSpace
{

    public interface IUserRepository
    {
        User GetUser(string Email);

        string Hash(string password);

         void Register(User obj);


        
        
    }
}
