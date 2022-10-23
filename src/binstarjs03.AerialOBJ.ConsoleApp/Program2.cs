using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.Core;

namespace binstarjs03.AerialOBJ.ConsoleApp;
public static class Program2
{
    static void MainDisabled(string[] args)
    {
        DateTime startTime;
        TimeSpan duration;

        int bitLength = 4;

        int bitsInByte = 8;
        int longBitLength = sizeof(long) * bitsInByte;
        int bufferSize = longBitLength / bitLength;

        Random random = new();
        int runCount = (int)Math.Pow(10, 6);
        int[] buffer = new int[bufferSize];

        startTime = DateTime.Now;
        RunFast(bitLength, buffer, random, runCount);
        duration = DateTime.Now - startTime;
        Console.WriteLine($"Fast method took {duration.TotalMilliseconds} ms / {duration.Seconds} s to execute");

        startTime = DateTime.Now;
        RunFastNoCheck(bitLength, buffer, random, runCount);
        duration = DateTime.Now - startTime;
        Console.WriteLine($"Fast method (no checking) took {duration.TotalMilliseconds} ms / {duration.Seconds} s to execute");

        startTime = DateTime.Now;
        RunSlow(bitLength, random, runCount);
        duration = DateTime.Now - startTime;
        Console.WriteLine($"Slow method took {duration.TotalMilliseconds} ms / {duration.Seconds} s to execute");
    }

    public static void RunFast(int bitLength, int[] buffer, Random random, int runCount)
    {
        long num;
        for (int i = 0; i < runCount; i++)
        {
            num = random.NextInt64(long.MinValue, long.MaxValue);
            num.SplitSubnumberFast(buffer, bitLength);
        }
    }

    public static void RunFastNoCheck(int bitLength, int[] buffer, Random random, int runCount)
    {
        long num;
        for (int i = 0; i < runCount; i++)
        {
            num = random.NextInt64(long.MinValue, long.MaxValue);
            num.SplitSubnumberFastNoCheck(buffer, bitLength);
        }
    }

    public static void RunSlow(int bitLength, Random random, int runCount)
    {
        long num;
        for (int i = 0; i < runCount; i++)
        {
            num = random.NextInt64(long.MinValue, long.MaxValue);
            num.SplitSubnumberSlow(bitLength);
        }
    }

    // here buffer is int, means each element may be 32 bits, so bit length can be 32 at most
    public static void SplitSubnumberFast(this long num, int[] buffer, int bitLength)
    {
        int bitsInByte = 8;

        // sizeof unit is byte, convert to bit
        int longBitLength = sizeof(long) * bitsInByte;

        // total subnumber count is total subnumber long number can hold from given bitlength
        int totalSubnumberCount = longBitLength / bitLength;

        // minimum buffer size is minimum buffer size required to convert
        // requested bit-length of given long number
        int minimumBufferSize = totalSubnumberCount;

        // total bits is total bits required for all subnumbers
        int totalBits = totalSubnumberCount * bitLength;

        // empty bits count is count of empty, redundant bits from leftmost side
        int emptyBitsCount = longBitLength - totalBits;

        if (minimumBufferSize > buffer.Length)
            throw new OverflowException("Buffer is too small for given bit-length");

        if (bitLength > 32)
            throw new OverflowException("bit-length is too big, 32 is at most");


        /* pseudocode:
         * case 1:
         * example we want to take the index 0 (which is sub-number of long at rightmost),
         * and bit length is 4, that makes total possible 16 sub-numbers (minimum buffer size count)
         * 
         * first, we left shift it 60 times, this will left only 4 bytes left remaining, which
         * is the subnumber index 0, then we left shift it 60 times back so the only bits
         * left is the subumber index 0, therefore we got the subnumber index 0
         * 
         * 
         * case 2:
         * example we want to take the index 15 (which is sub-number of long at leftmost),
         * and bit length is 4, that makes total possible 16 sub-numbers (minimum buffer size count)
         * 
         * first, we right shift it 60 times, this will left only 4 bytes left remaining of the right side, 
         * which is the subnumber index 15, therefore we got the subnumber index 15
         * 
         * 
         * case 3:
         * example we want to take the index 3, which is the 4th sub-number of long.
         * given resulting bit array example:
         * [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0]
         * 15          14          13          12          11          10          9           8
         * [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,1,0,1] - [0,0,1,0] - [1,1,0,0] - [0,0,0,1]
         * 7           6           5           4           3           2           1           0
         * 
         * the third index has bit-array of [0,1,0,1], that equals to 5 if we convert it to base-10 numbering system
         * there are total of 16 indices, and we want to remove all bits from left side, to do that we need to make
         * index 3 as the leftmost index.
         * To do that we need to left shift it 15 - 3 = 12 indices, bit length is 4
         * so 12 * 4 = 48 times. 
         * After bit-shifted left 48 times, the resulting bits will be looks like below:
         * 
         * [0,1,0,1] - [0,0,1,0] - [1,1,0,0] - [0,0,0,1] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0]
         * 15          14          13          12          11          10          9           8
         * [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0]
         * 7           6           5           4           3           2           1           0
         * 
         * Finally, the subnumber is finally at leftmost of the bits, now its time to move it to the rightmost
         * to remove all the bits of its right side.
         * To do that, we need to right shift it 15 indices since it is at leftmost, all we need to make it rightmost.
         * Bit length is 4 so 15 * 4 = 60 times.
         * After doing all of these bit-shifting, the resulting bits will be looks like below:
         * 
         * [0,0,0,0] - [0,0,1,0] - [1,1,0,0] - [0,0,0,1] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0]
         * 15          14          13          12          11          10          9           8
         * [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,0,0,0] - [0,1,0,1]
         * 7           6           5           4           3           2           1           0
         * 
         * Finally, we got the final result of taking the subnumber index 3, which is [0,1,0,1], which is 5 :D
         * 
         * case 4:
         * consider more complex example: we want to take index 7, which is the 8th subnumber index
         * and bit length is 6, that makes total possible 10 sub numbers.
         * 
         * given following example bits, we want to take the element t, which is located at bits
         * 42 through 48
         * 
         * [0,0,0,0] - [0,0,0,0,0,0] - [0,0,0,0,0,0] - [1,0,1,1,0,1] - [0,0,0,0,0,0] - [0,0,0,0,0,0]
         * redundant   9 - 60          8 - 54          7 - 48 - t      6 - 42          5 - 36
         *             [0,0,0,0,0,0] - [0,0,0,0,0,0] - [0,0,0,0,0,0] - [0,0,0,0,0,0] - [0,0,0,0,0,0]
         *             4 - 30          3 - 24          2 - 18          1 - 12          0 - 6
         *             
         * first, we make it to the leftmost bits, even occupying the redundants. we need to bit shift it
         * 10 - 8 = 2 subnumbers, 2 * 6 bitlength = 12, then we also need to bitshift the redundant bits,
         * which is 4. 4 is calculated from long bit length - total sub number bits = 64 - (10 * 4),
         * 64 - 60 = 4, so the final formula for left shifting is:
         * 
         * redundant bits count + ((total subnumber - 1 - selected subnumber index) * bitlength)
         * 
         * [1,0,1,1,0,1] - [0,0,0,0,0,0] - [0,0,0,0,0,0] - [0,0,0,0,0,0] - [0,0,0,0,0,0]
         * 9 - 60          8 - 54          7 - 48 - t      6 - 42          5 - 36
         * [0,0,0,0,0,0] - [0,0,0,0,0,0] - [0,0,0,0,0,0] - [0,0,0,0,0,0] - [0,0,0,0,0,0] - [0,0,0,0]
         * 4 - 30          3 - 24          2 - 18          1 - 12          0 - 6           redundant
         * 
         * then we want to make it to the rightmost side, to do that the formula is:
         * (total bits - bitlength) + redundant bits count
         */

        // iterate through all subnumber indices
        for (int index = 0; index < totalSubnumberCount; index++)
        {
            // pass 1, make the selected subnumber to the leftmost of bits
            int leftShiftAmount = emptyBitsCount + ((totalSubnumberCount - 1 - index) * bitLength);
            long numLeftShifted = num << leftShiftAmount;

            // pass 2, make the selected subnumber to the rightmost of bits
            int rightShiftAmount = (totalBits - bitLength) + emptyBitsCount;
            int numRightShifted = (int)((ulong)numLeftShifted >> rightShiftAmount);
            buffer[index] = numRightShifted;
        }

        //string binNum = Convert.ToString(num, toBase: 2);
        //binNum = binNum.PadLeft(64, '0');

        ////int index = 9;
        //int index = 3;

        //// pass 1, make the selected subnumber to the leftmost of bits
        //// subtract by one to give space so it doesnt overflow,
        //// otherwise the given subnumber will goes outside bits left side boundary
        //int leftShiftAmount = emptyBitsCount + ((totalSubnumberCount - 1 - index) * bitLength);
        //long numLeftShifted = num << leftShiftAmount;
        //string binNumLeftShifted = Convert.ToString(numLeftShifted, toBase: 2);
        //binNumLeftShifted = binNumLeftShifted.PadLeft(64, '0');

        //// pass 2, make the selected subnumber to the rightmost of bits
        //int rightShiftAmount = (totalBits - bitLength) + emptyBitsCount;
        //ulong numRightShifted = (ulong)numLeftShifted >> rightShiftAmount;
        //string binNumRightShifted = Convert.ToString((long)numRightShifted, toBase: 2);
        //binNumRightShifted = binNumRightShifted.PadLeft(64, '0');
    }

    // this method is same as normal one, except that execution for this method is completely linear,
    // no if statements are exist as checkings are disabled.
    // Invoking this method have greater risk of generating bugs
    // unless if your arguments are pre-checked before and calling this will be done
    // thousand times, check is only done once so speed is improved greatly
    public static void SplitSubnumberFastNoCheck(this long num, int[] buffer, int bitLength)
    {
        // total subnumber count is total subnumber long number can hold from given bitlength
        int totalSubnumberCount = 64 / bitLength;

        // total bits is total bits required for all subnumbers
        int totalBits = totalSubnumberCount * bitLength;

        // empty bits count is count of empty, redundant bits from leftmost side
        int emptyBitsCount = 64 - totalBits;

        // iterate through all subnumber indices
        for (int index = 0; index < totalSubnumberCount; index++)
        {
            // pass 1, make the selected subnumber to the leftmost of bits
            int leftShiftAmount = emptyBitsCount + ((totalSubnumberCount - 1 - index) * bitLength);
            long numLeftShifted = num << leftShiftAmount;

            // pass 2, make the selected subnumber to the rightmost of bits
            int rightShiftAmount = (totalBits - bitLength) + emptyBitsCount;
            int numRightShifted = (int)((ulong)numLeftShifted >> rightShiftAmount);
            buffer[index] = numRightShifted;
        }
    }

    public static int[] SplitSubnumberSlow(this long num, int bitLength)
    {
        int bitsInByte = 8;
        int longBitLength = sizeof(long) * bitsInByte;
        int bufferSize = longBitLength / bitLength;

        List<int> intTableFragment = new(bufferSize);
        byte[] bytes = num.ToBinaryArray(longBitLength).Reverse().ToArray();
        using (MemoryStream binStream = new(bytes))
        using (BinaryReader binReader = new(binStream))
        {
            for (int i = 0; i < bufferSize; i++)
            {
                // bin buffer for current, single block id.
                byte[] buff = binReader.ReadBytes(bitLength);

                // convert bin to int, this will return us the block palette index
                // for current block
                int blockId = buff.ToIntLE();
                intTableFragment.Add(blockId);
            }
        }
        return intTableFragment.ToArray();
    }

    public static byte[] ToBinaryArray(this long num, int bitLength)
    {
        string binNum = Convert.ToString(num, toBase: 2);
        //if (binNum.Length > bitLength)
        //    throw new OverflowException("Bit-length is not enough");
        binNum = binNum.PadLeft(bitLength, '0');
        byte[] ret = new byte[bitLength];
        for (int i = 0; i < bitLength; i++)
        {
            // TODO parsing is slow especially in tight-loops.
            // maybe we should calculate it by our own using binary shifting?
            ret[i] = byte.Parse($"{binNum[i]}");
        }
        return ret;
    }
}
