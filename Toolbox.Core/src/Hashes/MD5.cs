using System;
using System.Collections.Generic;
using System.IO;

namespace Toolbox.Core.Hashes
{
    public class MD5
    {
        public static string Calculate(string filename)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
