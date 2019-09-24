using System;
using System.Collections.Generic;
using System.Text;

namespace HorsDA
{
   /// <summary>
   /// Based on: https://stackoverflow.com/questions/22860356/how-to-generate-a-crc-16-from-c-sharp
   /// </summary>
   public static class CRC16
   {
      const ushort polynomial = 0xA001;
      static readonly ushort[] table = new ushort[256];

      public static ushort ComputeChecksum(string Text)
      {
         return ComputeChecksum(Encoding.UTF8.GetBytes(Text));
      }

      public static ushort ComputeChecksum(byte[] bytes)
      {
         ushort crc = 0;
         for (int i = 0; i < bytes.Length; ++i)
         {
            byte index = (byte)(crc ^ bytes[i]);
            crc = (ushort)((crc >> 8) ^ table[index]);
         }
         return crc;
      }

      static CRC16()
      {
         ushort value;
         ushort temp;

         for (ushort i = 0; i < table.Length; ++i)
         {
            value = 0;
            temp = i;

            for (byte j = 0; j < 8; ++j)
            {
               if (((value ^ temp) & 0x0001) != 0)
               {
                  value = (ushort)((value >> 1) ^ polynomial);
               }
               else
               {
                  value >>= 1;
               }
               temp >>= 1;
            }

            table[i] = value;
         }
      }
   }
}
