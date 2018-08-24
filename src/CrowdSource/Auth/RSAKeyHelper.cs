using System.Security.Cryptography;


namespace CrowdSource.Auth
{
    public class RSAKeyHelper
    {
        public static RSAParameters GenerateKey()
        {
            
            using (var key = RSA.Create())
            {
                return key.ExportParameters(true);
            }
        }
    }
}
