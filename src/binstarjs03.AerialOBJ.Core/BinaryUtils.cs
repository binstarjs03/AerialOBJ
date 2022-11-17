/*
Copyright (c) 2022, Bintang Jakasurya
All rights reserved. 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;

namespace binstarjs03.AerialOBJ.Core;

public static class BinaryUtils
{
    public static int Bitlength(this int num)
    {
        int bitLength = 0;
        for (; num > 0; num >>= 1)
        {
            bitLength++;
        }
        return bitLength;
    }

    // here buffer is int, means each element may be 32 bits, so bit length can be 32 at most
    // Splitting is done in big-endian (from rightmost side to the left)
    public static void SplitSubnumberFast(this long num, Span<int> buffer, int bitLength)
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
    }

    // this method is same as normal one, except that execution for this method is completely linear,
    // no if statements are exist as checkings are disabled.
    // Invoking this method have greater risk of generating bugs
    // unless if your arguments are pre-checked before and calling this will be done
    // thousand times, check is only done once so speed is improved greatly
    public static void SplitSubnumberFastNoCheck(this long num, Span<int> buffer, int bitLength)
    {
        int totalSubnumberCount = 64 / bitLength;
        int totalBits = totalSubnumberCount * bitLength;
        int emptyBitsCount = 64 - totalBits;

        for (int index = 0; index < totalSubnumberCount; index++)
        {
            int leftShiftAmount = emptyBitsCount + ((totalSubnumberCount - 1 - index) * bitLength);
            long numLeftShifted = num << leftShiftAmount;

            int rightShiftAmount = (totalBits - bitLength) + emptyBitsCount;
            int numRightShifted = (int)((ulong)numLeftShifted >> rightShiftAmount);
            buffer[index] = numRightShifted;
        }
    }
}
