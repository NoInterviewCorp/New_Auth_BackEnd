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
        private static string Secret1 = "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ==";
        
        // This method is resposible of generating JWT token
        public static string GenerateToken(string Email)
        {
            Chilkat.Global glob = new Chilkat.Global();
            glob.UnlockBundle("Anything for 30-day trial");

            string token = "";

            Chilkat.JsonObject jwtHeader = new Chilkat.JsonObject();
            jwtHeader.AppendString("alg", "RS256");
            jwtHeader.AppendString("typ", "JWT");

            Chilkat.JsonObject claims = new Chilkat.JsonObject();
            claims.AppendString("Email", Email);

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
                    
                }
                else
                {
                    TokenManager.KeyGenerator(client);
                    var getPair1 = client.KV.Get("myPrivateKey");

                    string secret = System.Text.Encoding.UTF8.GetString(getPair1.Result.Response.Value);

                    Chilkat.Rsa rsaExportedPrivateKey = new Chilkat.Rsa();
                    rsaExportedPrivateKey.ImportPrivateKey(secret);

                    token = jwt.CreateJwtPk(jwtHeader.Emit(), claims.Emit(), rsaExportedPrivateKey.ExportPrivateKeyObj());
                    
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


        // This method is responsible for generating public and private key if keys are not present in consul
        public static void KeyGenerator(ConsulClient client)
        {
            
            Chilkat.Global glob = new Chilkat.Global();
            glob.UnlockBundle("Anything for 30-day trial");

            Chilkat.Rsa rsaKey = new Chilkat.Rsa();

            rsaKey.GenerateKey(1024);
            var rsaPrivKey = rsaKey.ExportPrivateKeyObj();
            var rsaPrivKeyAsString = rsaKey.ExportPrivateKey();

            var rsaPublicKey = rsaKey.ExportPublicKeyObj();
            var rsaPublicKeyAsString = rsaKey.ExportPublicKey();

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