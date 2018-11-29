using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using JwtTokenSpace;


namespace JwtTokenSpace
{
    [Route("api/[controller]")]
    [ApiController]
    public class loginController : ControllerBase
    {

        IUserRepository IUserobj;
        public loginController(IUserRepository UserRepository)
        {
            this.IUserobj = UserRepository;
        }
       

        // This post api handles the social login through facebook and google

        [HttpPost]

        [Route("/socialSignIn")]

        public async Task<string> SocialSignInAsync(socialSignIn signIn)
        {
            if (ModelState.IsValid)
            {
                                
                var token = await TokenManager.GenerateTokenAsync(signIn.email); 
                return JsonConvert.SerializeObject(token);
               
            }
            else
            {
                return "Invalid state";
            }
            
            
           
        }

        // This post handles the local server signIn part
        
        [HttpPost]

        [Route("/signIn")]

        public async Task<string> SignInAsync(signIn signIn)
        {
            if (ModelState.IsValid)
            {
                 
                User u = IUserobj.GetUser(signIn.email);
                if (u == null)
                {
                    string x="user does not exist";
                    return JsonConvert.SerializeObject(x);
                }

                if(!(IUserobj.Hash(signIn.password).Equals(u.Password)))
                {
                    string x = "Password entered is not correct";
                    return JsonConvert.SerializeObject(x) ;
                }
                else
                { 
                     var token = await TokenManager.GenerateTokenAsync(signIn.email); 
                    return JsonConvert.SerializeObject(token);
                }
                
        
            }
            else
            {
                return "Invalid state";
            }
            
            
           
        }

        // This post method handles the local server signUp part

        [HttpPost]

        [Route("/signUp")]

        public string SignUp(signUp signUp)
        {
            if(ModelState.IsValid)
            {
                User u = IUserobj.GetUser(signUp.email);
            
            
                if(u != null)
                {
                    string x = "User already exist";
                    return JsonConvert.SerializeObject(x) ;
                }
            
                else
                {
                    User newUser=new User();
                    newUser.Email = signUp.email;
                    newUser.Password = IUserobj.Hash(signUp.password);
                    // newUser.Password= signUp.password;
                    newUser.FullName = signUp.fullName;
                    IUserobj.Register(newUser);
                    string x = "Success";
                    return JsonConvert.SerializeObject(x) ;
                }

            }
            else
            {
                return "Invalid state";
            }

            
           

        }


        
    }
}
