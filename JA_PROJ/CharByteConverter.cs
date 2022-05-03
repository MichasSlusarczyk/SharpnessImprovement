using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JA_PROJ
{
    class CharByteConverter
    {
        public static char[] ByteToCharConverter(byte[] bytes)
        {
            char[] chars = new char[bytes.Length];

            for(int i = 0; i < bytes.Length; i++)
            {
                chars[i] = (char)bytes[i];
            }

            return chars;
        }

        public static byte[] CharToByteConverter(char[] chars)
        {
            byte[] bytes = new byte[chars.Length];

            for (int i = 0; i < chars.Length; i++)
            {
                bytes[i] = (byte)chars[i];
            }

            return bytes;
        }
    }
}
