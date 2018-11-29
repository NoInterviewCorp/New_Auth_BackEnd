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



namespace JwtTokenSpace
{
    public class TokenManager
    {

        // This method is resposible of generating JWT token
        public async static Task<string> GenerateTokenAsync(string Email)
        {
            Chilkat.Global glob = new Chilkat.Global();
            glob.UnlockBundle("Anything for 30-day trial");

            string token = "";

            //Creating JWT header using chilkat
            Chilkat.JsonObject jwtHeader = new Chilkat.JsonObject();
            jwtHeader.AppendString("alg", "RS256");
            jwtHeader.AppendString("typ", "JWT");

            //Adding Token claims
            Chilkat.JsonObject claims = new Chilkat.JsonObject();
            claims.AppendString("Email", Email);

            //Adding Token Expiration time
            Chilkat.Jwt jwt = new Chilkat.Jwt();
            int curDateTime = jwt.GenNumericDate(0);
            claims.AddIntAt(-1, "exp", curDateTime + 720);

            //Ading consul for putting and getting public and private key
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
                    await TokenManager.KeyGeneratorAsync(client);
                    var getPair1 = client.KV.Get("myPrivateKey");

                    string secret = System.Text.Encoding.UTF8.GetString(getPair1.Result.Response.Value);

                    Chilkat.Rsa rsaExportedPrivateKey = new Chilkat.Rsa();
                    rsaExportedPrivateKey.ImportPrivateKey(secret);

                    token = jwt.CreateJwtPk(jwtHeader.Emit(), claims.Emit(), rsaExportedPrivateKey.ExportPrivateKeyObj());
                    
                }


            }

            //jwt.AutoCompact = true;
            //return JsonConvert.SerializeObject(token);
            return token;


        }


        


        // This method is responsible for generating public and private key if keys are not present in consul
        public static async System.Threading.Tasks.Task KeyGeneratorAsync(ConsulClient client)
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
            var putAttempt = await client.KV.Put(putPair);


            var putPair1 = new KVPair("myPrivateKey")
            {
                Value = Encoding.UTF8.GetBytes(rsaPrivKeyAsString)

            };
            var putAttempt1 = await client.KV.Put(putPair1);



        }


    }
}