    
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
    
    
    namespace JwtTokenSpace
    {

         public class User
      {
          [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
          [Required]
          public string Email{get;set;}
          public string UserId{get;set;}

          [Required]
          public string Password { get; set; }

          [Required]
           public string FullName{get;set;}
         
      }

    }
    
   