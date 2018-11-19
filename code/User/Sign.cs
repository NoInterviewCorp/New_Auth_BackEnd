using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sample
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



}