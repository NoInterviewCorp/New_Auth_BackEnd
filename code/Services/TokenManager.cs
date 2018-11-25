using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Chilkat;
using Consul;
using Jose;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;


namespace sample
{
    public class TokenManager
    {
    
        //private static string publicKey;
        //private static string privateKey;

        //HMACSHA256 hmac = new HMACSHA256();
        //string key = Convert.ToBase64String(hmac.Key);
        private static string Secret1 = "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ==";
        //private static string token="";

        public static string GenerateToken(string Email)
        {
            Chilkat.Global glob = new Chilkat.Global();
            glob.UnlockBundle("Anything for 30-day trial");

            string token ="";

            Chilkat.JsonObject jwtHeader = new Chilkat.JsonObject();
            jwtHeader.AppendString("alg", "RS256");
            jwtHeader.AppendString("typ", "JWT");

            Chilkat.JsonObject claims = new Chilkat.JsonObject();
            claims.AppendString("Email", Email);
            //claims.AddIntAt(-1,"exp",DateTime.UtcNow.AddMinutes(20));


            Chilkat.Jwt jwt = new Chilkat.Jwt();
            int curDateTime = jwt.GenNumericDate(0);
            claims.AddIntAt(-1, "exp", curDateTime + 720);

            

            using (var client = new ConsulClient())
            {
                client.Config.Address = new Uri("http://172.23.238.173:8500");
                
                var getPair = client.KV.Get("myPrivateKey");


                if (getPair.Result.Response != null)
                {
                    
                    string secret = System.Text.Encoding.UTF8.GetString(getPair.Result.Response.Value);
                    Chilkat.Rsa rsaExportedPrivateKey = new Chilkat.Rsa();
                    rsaExportedPrivateKey.ImportPrivateKey(secret);
                    var rsaPrivKey = rsaExportedPrivateKey.ExportPrivateKeyObj();
                    
                    token = jwt.CreateJwtPk(jwtHeader.Emit(), claims.Emit(), rsaPrivKey);
                    Console.WriteLine("newly created token =" + token);

                    Console.WriteLine("Getting Back the Stored String");
                    Console.WriteLine(Encoding.UTF8.GetString(getPair.Result.Response.Value, 0, getPair.Result.Response.Value.Length));
                }
                else
                {
                    TokenManager.KeyGenerator();
                    var getPair1 = client.KV.Get("myPrivateKey");

                    string secret = System.Text.Encoding.UTF8.GetString(getPair1.Result.Response.Value);
                    Chilkat.Rsa rsaExportedPrivateKey = new Chilkat.Rsa();
                    rsaExportedPrivateKey.ImportPrivateKey(secret);
                    
                   // token = jwt.CreateJwtPk(jwtHeader.Emit(), claims.Emit(), rsaExportedPrivateKey.ExportPrivateKeyObj());
                    Console.WriteLine("newly created token with no token already present in consul =" + token);

                }


                 var getpair2 = client.KV.Get("myPublicKey");
                if(getpair2.Result.Response !=null)
                {
                    string secret = System.Text.Encoding.UTF8.GetString(getpair2.Result.Response.Value);
                    Chilkat.Rsa rsaExportedPublicKey = new Chilkat.Rsa();
                    rsaExportedPublicKey.ImportPublicKey(secret);

                    if(jwt.VerifyJwtPk(token, rsaExportedPublicKey.ExportPublicKeyObj()))
                    {
                        Console.WriteLine("the token is verifieddddddddddddddddddddddddddddddddddddddddddddd");
                    }
                    
                    // token = jwt.CreateJwtPk(jwtHeader.Emit(), claims.Emit(), rsaExportedPublicKey.ExportPublicKeyObj());
                    // Console.WriteLine("newly created token =" + token);

                }

            }


            //jwt.AutoCompact = true;
            return JsonConvert.SerializeObject(token);


        }


        public static ClaimsPrincipal ValidateMyToken(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);

                if (jwtToken == null)
                    return null;

                byte[] key = Convert.FromBase64String(Secret1);

                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
                return principal;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static void KeyGenerator()
        {
            Console.WriteLine("Entered in key generatorrrrrrrrrrrrrrrrrrrrrrrrrr");
            Chilkat.Global glob = new Chilkat.Global();
            glob.UnlockBundle("Anything for 30-day trial");

            Chilkat.Rsa rsaKey = new Chilkat.Rsa();

            rsaKey.GenerateKey(1024);
            var rsaPrivKey = rsaKey.ExportPrivateKeyObj();
            var rsaPrivKeyAsString = rsaKey.ExportPrivateKey();
            Console.WriteLine("PrivateKey= " + rsaPrivKeyAsString);

            var rsaPublicKey = rsaKey.ExportPublicKeyObj();
            var rsaPublicKeyAsString = rsaKey.ExportPublicKey();
            Console.WriteLine("PublicKey= " + rsaPublicKeyAsString);
                       
            using (var client = new ConsulClient())
            {
                client.Config.Address = new Uri("http://172.23.238.173:8500");
                var putPair = new KVPair("myPublicKey")
                {
                    Value = Encoding.UTF8.GetBytes(rsaPublicKeyAsString)

                };

                var putPair1 = new KVPair("myPrivateKey")
                {
                    Value = Encoding.UTF8.GetBytes(rsaPrivKeyAsString)

                };

                var putAttempt = client.KV.Put(putPair);
                var putAttempt1 = client.KV.Put(putPair1);                

            }



        }


    }
}