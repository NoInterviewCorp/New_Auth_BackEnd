using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace JwtTokenSpace
{
    public class signIn
    {
        //[Required]
        public string email{get;set;}
        //[Required]
        public string password{get; set;}

    }

    public class signUp
    {
       //[Required]
        public string fullName;
        //[Required]
        public string email;
        //[Required]
        public string password;
        

    }

    public class socialSignIn
    {
        public string email;
        public string id;
        public string image;
        public string name;
        public string provider;
    }




}