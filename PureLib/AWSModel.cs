using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Anomaly
{
    class AWSModel
    {
        static byte[] HmacSHA256(String data, byte[] key)
        {
            String algorithm = "HmacSHA256";
            KeyedHashAlgorithm kha = KeyedHashAlgorithm.Create(algorithm);
            kha.Key = key;

            return kha.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        static byte[] getSignatureKey(String key, String dateStamp, String regionName, String serviceName)
        {
            byte[] kSecret = Encoding.UTF8.GetBytes(("AWS4" + key).ToCharArray());
            byte[] kDate = HmacSHA256(dateStamp, kSecret);
            byte[] kRegion = HmacSHA256(regionName, kDate);
            byte[] kService = HmacSHA256(serviceName, kRegion);
            byte[] kSigning = HmacSHA256("aws4_request", kService);
            return kSigning;
        }
    }
}
