using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core
{
    public class ImageUtility
    {
        public static byte[] ConvertBgraToRgba(byte[] bytes)
        {
            if (bytes == null)
                throw new Exception("Data block returned null. Make sure the parameters and image properties are correct!");

            byte[] copy = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length; i += 4)
            {
                copy[i + 2] = bytes[i + 0];
                copy[i + 0] = bytes[i + 2];
                copy[i + 1] = bytes[i + 1];
                copy[i + 3] = bytes[i + 3];
            }
            return copy;
        }
    }
}
