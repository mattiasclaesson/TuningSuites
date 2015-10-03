using System;
using System.Collections.Generic;
using System.Linq;

using BKSystem.IO;

namespace CommonSuite
{
    public class TrionicSymbolDecompressor
    {
        private static readonly int[] outputFileOffsetHigh = new int[]
        {
            0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000,
            0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000,
            0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000,
            0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000,
            0x0040, 0x0040, 0x0040, 0x0040, 0x0040, 0x0040, 0x0040, 0x0040,
            0x0040, 0x0040, 0x0040, 0x0040, 0x0040, 0x0040, 0x0040, 0x0040,
            0x0080, 0x0080, 0x0080, 0x0080, 0x0080, 0x0080, 0x0080, 0x0080,
            0x0080, 0x0080, 0x0080, 0x0080, 0x0080, 0x0080, 0x0080, 0x0080,
            0x00C0, 0x00C0, 0x00C0, 0x00C0, 0x00C0, 0x00C0, 0x00C0, 0x00C0,
            0x00C0, 0x00C0, 0x00C0, 0x00C0, 0x00C0, 0x00C0, 0x00C0, 0x00C0,
            0x0100, 0x0100, 0x0100, 0x0100, 0x0100, 0x0100, 0x0100, 0x0100,
            0x0140, 0x0140, 0x0140, 0x0140, 0x0140, 0x0140, 0x0140, 0x0140,
            0x0180, 0x0180, 0x0180, 0x0180, 0x0180, 0x0180, 0x0180, 0x0180,
            0x01C0, 0x01C0, 0x01C0, 0x01C0, 0x01C0, 0x01C0, 0x01C0, 0x01C0,
            0x0200, 0x0200, 0x0200, 0x0200, 0x0200, 0x0200, 0x0200, 0x0200,
            0x0240, 0x0240, 0x0240, 0x0240, 0x0240, 0x0240, 0x0240, 0x0240,
            0x0280, 0x0280, 0x0280, 0x0280, 0x0280, 0x0280, 0x0280, 0x0280,
            0x02C0, 0x02C0, 0x02C0, 0x02C0, 0x02C0, 0x02C0, 0x02C0, 0x02C0,
            0x0300, 0x0300, 0x0300, 0x0300, 0x0340, 0x0340, 0x0340, 0x0340,
            0x0380, 0x0380, 0x0380, 0x0380, 0x03C0, 0x03C0, 0x03C0, 0x03C0,
            0x0400, 0x0400, 0x0400, 0x0400, 0x0440, 0x0440, 0x0440, 0x0440,
            0x0480, 0x0480, 0x0480, 0x0480, 0x04C0, 0x04C0, 0x04C0, 0x04C0,
            0x0500, 0x0500, 0x0500, 0x0500, 0x0540, 0x0540, 0x0540, 0x0540,
            0x0580, 0x0580, 0x0580, 0x0580, 0x05C0, 0x05C0, 0x05C0, 0x05C0,
            0x0600, 0x0600, 0x0640, 0x0640, 0x0680, 0x0680, 0x06C0, 0x06C0, 
            0x0700, 0x0700, 0x0740, 0x0740, 0x0780, 0x0780, 0x07C0, 0x07C0, 
            0x0800, 0x0800, 0x0840, 0x0840, 0x0880, 0x0880, 0x08C0, 0x08C0, 
            0x0900, 0x0900, 0x0940, 0x0940, 0x0980, 0x0980, 0x09C0, 0x09C0, 
            0x0A00, 0x0A00, 0x0A40, 0x0A40, 0x0A80, 0x0A80, 0x0AC0, 0x0AC0, 
            0x0B00, 0x0B00, 0x0B40, 0x0B40, 0x0B80, 0x0B80, 0x0BC0, 0x0BC0,
            0x0C00, 0x0C40, 0x0C80, 0x0CC0, 0x0D00, 0x0D40, 0x0D80, 0x0DC0,
            0x0E00, 0x0E40, 0x0E80, 0x0EC0, 0x0F00, 0x0F40, 0x0F80, 0x0FC0
        };

        private static readonly int[] outputFileOffsetLow = new int[]
        {
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02,
            0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02,
            0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02,
            0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02,
            0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02,
            0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02,
            0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03,
            0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03,
            0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03,
            0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03,
            0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03,
            0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03,
            0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03,
            0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03,
            0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04,
            0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04,
            0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04,
            0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04,
            0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04,
            0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04,
            0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
            0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
            0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
            0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
            0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
            0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
            0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06,
            0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06
        };

        // I think that the values in this array relate to a weighting given to symbols
        // I'm not sure if it supposed to have 0x273 or 0x274 values - the original structures
        // are contiguous in memory and it's hard to tell when one ends and the next begins.
        // array1 6Af8-6FDD ( 0x273 16-bit words )
        // var_01 6FDE-6FDF ( = 0xFFFF )
        //
        private static uint[] nodeWeights = new uint[0x274];

        // Looks like an array of node tree successor/predecessor pairs
        // array2 6FE0-74C3 ( 0x272 16-bit words )
        // 
        private static int[] nodeTreePairs = new int[0x273];

        // something goes here
        // var_02 74C4-74C5 ( = 0x0 )
        // 

        // Looks like an array of 
        // array3 74C6-7739 ( 0x13A 16-bit words )
        //
        private static int[] symbolContents = new int[0x13A];

        // This may actually be 2 arrays
        // The first one has 0x13A elements - the deciphered symbol numbers
        // The second has 0x139 elements: 0, 2... 0x270 - 
        // array4 8776-8C5B ( 0x273 (0x13A + 0x139) 16-bit words )
        private static int[] symbolDecipherTable = new int[0x273];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="symbols"></param>
        public static void ExpandComprStream(byte[] bytes, out string[] symbols)               // sub_107DE
        {

            // Convert byte array to a BitStream
            BitStream bstrm = new BitStream(bytes.Length << 8);
            for (int i = 0; i < bytes.Length; i++)
            {
                bstrm.WriteByte(bytes[i]);
            }
            // make sure we start at the beginning of the compressed bitstream
            bstrm.Position = 0x0;
            // Create a byte array large enough to hold the expanded COMPR file
            int expandedFileSize = 0x0;
            for (int i = 0; i < 4; i++)
            {
                expandedFileSize |= bstrm.ReadByte() << (8 * i);
            }
            byte []outputFile = new byte[expandedFileSize];
            // Initialise the Huffman(?) Table structure
            initialiseTables();
            // 
            int decodedBytesSoFar = 0;
            while (decodedBytesSoFar < expandedFileSize)
            {
                int currentSymbol = getSymbolFromFile(bstrm);
                if (currentSymbol < 0x100)
                {
                    // The symbol was an ASCII character, simply put it in the output file buffer.
                    outputFile[decodedBytesSoFar++] = (byte)currentSymbol;
                }
                else
                {
                    // The symbol is a reference backwards to something that has already been expanded.
                    // Get a start position offset and character count and repeat the earlier block.
                    int outputFileRepeatBlockOffset = getOffsetFromFile(bstrm) + 1;
                    int outputFileRepeatBlockSize = currentSymbol - 0xFD;
                    for (int i = 0; i < outputFileRepeatBlockSize; i++)
                    {
                        outputFile[decodedBytesSoFar] = outputFile[decodedBytesSoFar++ - outputFileRepeatBlockOffset];
                    }
                    //Array.Copy(outputFile,
                    //    decodedBytesSoFar - getOffsetFromFile(bstrm) - 1,
                    //    outputFile,
                    //    decodedBytesSoFar,
                    //    currentSymbol - 0xFD);
                    //decodedBytesSoFar += currentSymbol - 0xFD;
                }
            }
            symbols = System.Text.Encoding.UTF8.GetString(outputFile).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void initialiseTables()              // sub_104C8
        {
            for (int i = 0; i < 0x13A; i++)
            {
                nodeWeights[i] = 0x1;                       // array1
                symbolContents[i] = i;                      // array3
                symbolDecipherTable[i] = i + 0x273;         // array4
            }

            int nodeValue = 0;
            for (int i = 0x13A; i < 0x273; i++)
            {
                nodeWeights[i] = nodeWeights[nodeValue];    // array1
                nodeTreePairs[nodeValue] = i;               // array2
                symbolDecipherTable[i] = nodeValue;         // array4
                nodeValue++;
                nodeWeights[i] += nodeWeights[nodeValue];   // array1
                nodeTreePairs[nodeValue] = i;               // array2
                nodeValue++;
            }

            nodeWeights[0x273] = 0xFFFF;                    // var_01 = -1 :- array1 - var_01 - array2
            nodeTreePairs[0x272] = 0;                       // var_02 =  0 :- array2 - var_02 - array3

            return;
        }

        /// <summary>
        /// Traverse the decipher table depending on the value of bits from the COMPR file. Stop when a
        /// symbol has been fully deciphered, update the symbol node tree and return the deciphered symbol.
        /// 
        /// The symbol is deciphered when the value held in the decipher table is > 0x273. The actual symbol
        /// value is obtained by subtracting 0x273 from the value in the decipher table.
        /// </summary>
        /// <param name="bstrm" a BitStream of encoded symbols></param>
        /// <returns int - a single deciphered symbol></returns>
        /// 
        private static int getSymbolFromFile(BitStream bstrm)   // sub_10756
        {
            int decipheredSymbol = symbolDecipherTable[0x272];
            int bitValue;
            while (decipheredSymbol < 0x273)
            {
                bstrm.Read(out bitValue, 0, 1);                       // was sub_102F2
                decipheredSymbol += bitValue;
                decipheredSymbol = symbolDecipherTable[decipheredSymbol];
            }
            decipheredSymbol -= 0x273;
            updateNodes(decipheredSymbol);

            return decipheredSymbol;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bstrm" a BitStream of encoded symbols></param>
        /// <returns int - an offset from the end of the output file.></returns>
        /// 
        private static int getOffsetFromFile(BitStream bstrm)       // sub_1078E
        {

            int offsetFromOutputFileEndLowBits = bstrm.ReadByte();                   // was sub_10364
            int offsetFromOutputFileEndHighBits = outputFileOffsetHigh[offsetFromOutputFileEndLowBits];
            int extraEncodedOffsetBits = outputFileOffsetLow[offsetFromOutputFileEndLowBits];

            int extraEncodedOffsetValue;
            bstrm.Read(out extraEncodedOffsetValue, 0, extraEncodedOffsetBits);     // was sub_102F2 + a loop
            offsetFromOutputFileEndLowBits <<= extraEncodedOffsetBits;
            offsetFromOutputFileEndLowBits |= extraEncodedOffsetValue;
            offsetFromOutputFileEndLowBits &= 0x3F;

            return offsetFromOutputFileEndHighBits | offsetFromOutputFileEndLowBits;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisSymbol"></param>
        private static void updateNodes(int thisSymbol)                         // sub_10678
        {
            if (nodeWeights[0x272] == 0x8000)                                   // 0x8000 is short.minvalue (-32,768)
            {
                rejigTables();
            }
            thisSymbol = symbolContents[thisSymbol];
            do
            {
                nodeWeights[thisSymbol]++;
                uint thisSymbolsWeight = nodeWeights[thisSymbol];
                int nextSymbol = thisSymbol + 1;
                if (nodeWeights[nextSymbol] < thisSymbolsWeight)
                {
                    while (nodeWeights[++nextSymbol] < thisSymbolsWeight) ;
                    nextSymbol--;               // last symbol whose weight is < this symbol's weight

                    nodeWeights[thisSymbol] = nodeWeights[nextSymbol];
                    nodeWeights[nextSymbol] = thisSymbolsWeight;

                    int thisSymbolsDecipherValue = symbolDecipherTable[thisSymbol];
                    int nextSymbolsDecipherValue = symbolDecipherTable[nextSymbol];

                    if (thisSymbolsDecipherValue < 0x273)
                    {
                        nodeTreePairs[thisSymbolsDecipherValue] = nextSymbol;
                        nodeTreePairs[thisSymbolsDecipherValue + 1] = nextSymbol;
                    }
                    else
                    {
                        symbolContents[thisSymbolsDecipherValue - 0x273] = nextSymbol;
                    }
                    if (nextSymbolsDecipherValue < 0x273)
                    {
                        nodeTreePairs[nextSymbolsDecipherValue] = thisSymbol;
                        nodeTreePairs[nextSymbolsDecipherValue + 1] = thisSymbol;
                    }
                    else
                    {
                        symbolContents[nextSymbolsDecipherValue - 0x273] = thisSymbol;
                    }

                    symbolDecipherTable[thisSymbol] = nextSymbolsDecipherValue;
                    symbolDecipherTable[nextSymbol] = thisSymbolsDecipherValue;

                    thisSymbol = nextSymbol;
                }
                thisSymbol = nodeTreePairs[thisSymbol];
            } while (thisSymbol > 0);                           // ??? not sure about this
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        private static void swapEntries<T>(ref T obj1, ref T obj2)
        {
            var temp = obj1;
            obj1 = obj2;
            obj2 = temp;
        }

        /// <summary>
        /// !!! DANGER !!!
        /// I have not tested this function as I have never seen it called
        /// Check the Array copy works because it didn't in ExpandComprStream 
        /// </summary>
        private static void rejigTables()                           // sub_1054E
        {
            int currSymbol = 0;
            for (int i = 0; i < 0x273; i++)
            {
                if (symbolDecipherTable[i] < 0x273)
                {
                    nodeWeights[currSymbol] = (nodeWeights[i] + 1) >> 1;          // not at all sure about this
                    symbolDecipherTable[currSymbol] = symbolDecipherTable[i];
                    currSymbol++;
                }
            }

            int nodeValue = 0;
            for (int thisSymbol = 0x13A; thisSymbol < 0x273; thisSymbol++)
            {
                nodeWeights[thisSymbol] = nodeWeights[nodeValue] + nodeWeights[nodeValue + 1];
                uint thisSymbolsWeight = nodeWeights[thisSymbol];
                int prevSymbol = thisSymbol - 1;
                while (nodeWeights[prevSymbol] > thisSymbolsWeight)
                {
                    prevSymbol--;
                }
                prevSymbol++;

                Array.Copy(nodeWeights, prevSymbol, nodeWeights, prevSymbol + 1, thisSymbol - prevSymbol);
                nodeWeights[prevSymbol] = thisSymbolsWeight;
                Array.Copy(symbolDecipherTable, prevSymbol, symbolDecipherTable, prevSymbol + 1, thisSymbol - prevSymbol);
                symbolDecipherTable[prevSymbol] = nodeValue;
                nodeValue += 2;
            }

            for (int i = 0; i < 0x273; i++)
            {
                int symbolDecipherValue = symbolDecipherTable[i];
                if (symbolDecipherValue < 0x273)
                {
                    nodeTreePairs[symbolDecipherValue] = i;
                    nodeTreePairs[symbolDecipherValue + 1] = i;
                }
                else
                {
                    symbolContents[symbolDecipherValue - 0x273] = i;
                }
            }
            return;
        }
    }
}
