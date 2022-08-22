using System;
using System.Linq;
namespace binstarjs03.MinecraftSharpOBJ.Utils;

public static class BinaryUtils {
    public static int Bitlength(this int num) {
        int bitLength = 0;
        for (; num > 0; num >>= 1) {
            bitLength++;
        }
        return bitLength;
    }

    public static char[] ToBinaryChar(this long num) {
        return Convert.ToString(num, toBase: 2).ToCharArray();
    }

    public static string ToBinaryString(this long num) {
        return Convert.ToString(num, toBase: 2);
    }

    public static byte[] ToBinaryArray(this long num, int bitLength) {
        string binNum = Convert.ToString(num, toBase: 2);
        if (binNum.Length > bitLength)
            throw new OverflowException("Bit-length is not enough");
        binNum = binNum.PadLeft(bitLength, '0');
        byte[] ret = new byte[bitLength];
        for (int i = 0; i < bitLength; i++) {
            ret[i] = byte.Parse($"{binNum[i]}");
        }
        return ret;
    }

    public static int ToIntLE(this byte[] buffer) {
        int ret = 0;
        int length = buffer.Length;
        if (length == 0)
            return ret;
        int add;
        for (int i = 0; i < length; i++) {
            add = (int)Math.Pow(2, i);
            if (buffer[i] == 1)
                ret += add;
        }
        return ret;
    }

    public static int ToIntBE(this byte[] buffer) {
        return ToIntLE(buffer.Reverse().ToArray());
    }
}
