using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Utility
{
    public static class ArrayHelpers
    {
        public static byte[] IncrementByteArrByOne(byte[] arr)
        {
            byte[] outBytes = new byte[arr.Length];
            byte carry = 0;
            bool first = true;
            for (int i = arr.Length - 1; i >= 0; i--)
            {
                byte procByte = 0;
                procByte = arr[i];
                if (first)
                {
                    if (procByte < byte.MaxValue)
                        procByte++;

                }

                if (carry > 0)
                {
                    procByte += carry;
                    carry = 0;
                }

                if (procByte > byte.MaxValue)
                    carry = 1;

                first = false;
                outBytes[i] = procByte;
            }

            return outBytes;
        }
    }
}
