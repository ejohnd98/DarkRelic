/* Decompression: */
/*      Zlib deflation. This is a direct port of zlib puff.c reference code that is meant to be
 *      an addendum to RFC 1951: DEFLATE Compressed Data Format Specification version 1.3.
 *
 *      Additional code added for decoding zlib and gzip headers before passing compressed data to
 *      Puff().
 *
 *      puff.c used in this port can be found at:
 *      https://github.com/madler/zlib/tree/master/contrib/puff
 *      commit hash used: 03614c56ad299f9b238c75aa1e66f0c08fc4fc8b
 *
 *      Copyright notice for puff.c reference implementation follows:
 */

/*
        Copyright (C) 2002-2013 Mark Adler, all rights reserved
        version 2.3, 21 Jan 2013

        This software is provided 'as-is', without any express or implied
        warranty.  In no event will the author be held liable for any damages
        arising from the use of this software.

        Permission is granted to anyone to use this software for any purpose,
        including commercial applications, and to alter it and redistribute it
        freely, subject to the following restrictions:

        1. The origin of this software must not be misrepresented; you must not
           claim that you wrote the original software. If you use this software
           in a product, an acknowledgment in the product documentation would be
           appreciated but is not required.
        2. Altered source versions must be plainly marked as such, and must not be
           misrepresented as being the original software.
        3. This notice may not be removed or altered from any source distribution.

        Mark Adler    madler@alumni.caltech.edu
*/

/* Compression: */
/*      Deflate compression ported from stb_image_write.h from stb headers at https://github.com/nothings/stb
 *      The code was additionally modified to add GZip header support.
 *      stb_image_write.h license is as follows: */

/*
        ------------------------------------------------------------------------------
        Public Domain (www.unlicense.org)
        This is free and unencumbered software released into the public domain.
        Anyone is free to copy, modify, publish, use, compile, sell, or distribute this
        software, either in source code form or as a compiled binary, for any purpose,
        commercial or non-commercial, and by any means.
        In jurisdictions that recognize copyright laws, the author or authors of this
        software dedicate any and all copyright interest in the software to the public
        domain. We make this dedication for the benefit of the public at large and to
        the detriment of our heirs and successors. We intend this dedication to be an
        overt act of relinquishment in perpetuity of all present and future rights to
        this software under copyright law.
        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
        ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
        WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
        ------------------------------------------------------------------------------
*/

namespace RetroBlitInternal
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// An implementation of Deflate compression and decompression.
    /// </summary>
    public class RBDeflate
    {
        /// <summary>
        /// Decompress the given data buffer
        /// </summary>
        /// <param name="compressedData">Compressed data</param>
        /// <returns>Decompressed data</returns>
        public static byte[] Decompress(byte[] compressedData)
        {
            return DeflateDecompressor.Decompress(compressedData, 0, compressedData.Length);
        }

        /// <summary>
        /// Decompress a part of given data buffer with given offset and length
        /// </summary>
        /// <param name="compressedData">Compressed data</param>
        /// <param name="startOffset">Start offset</param>
        /// <param name="length">Length</param>
        /// <returns>Decompressed data</returns>
        public static byte[] Decompress(byte[] compressedData, long startOffset, int length)
        {
            return DeflateDecompressor.Decompress(compressedData, startOffset, length);
        }

        /// <summary>
        /// Compressed the given data buffer
        /// </summary>
        /// <param name="decompressedData">Uncompressed data</param>
        /// <returns>Compressed data</returns>
        public static byte[] Compress(byte[] decompressedData)
        {
            return DeflateCompressor.Compress(decompressedData);
        }

        private class DeflateDecompressor
        {
            private const int COMPRESSION_METHOD_DEFLATE = 8;
            private const int MAXBITS = 15;            /* maximum bits in a code */
            private const int MAXLCODES = 286;           /* maximum number of literal/length codes */
            private const int MAXDCODES = 30;            /* maximum number of distance codes */
            private const int MAXCODES = MAXLCODES + MAXDCODES;  /* maximum codes lengths to read */
            private const int FIXLCODES = 288;           /* number of fixed literal/length codes */

            private static readonly short[] lens = new short[]
            {
            /* Size base for length codes 257..285 */
            3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27, 31,
            35, 43, 51, 59, 67, 83, 99, 115, 131, 163, 195, 227, 258
            };

            private static readonly short[] lext = new short[]
            {
            /* Extra bits for length codes 257..285 */
            0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2,
            3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0
            };

            private static readonly short[] dists = new short[]
            {
            /* Offset base for distance codes 0..29 */
            1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193,
            257, 385, 513, 769, 1025, 1537, 2049, 3073, 4097, 6145,
            8193, 12289, 16385, 24577
            };

            private static readonly short[] dext = new short[]
            {
            /* Extra bits for distance codes 0..29 */
            0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6,
            7, 7, 8, 8, 9, 9, 10, 10, 11, 11,
            12, 12, 13, 13
            };

            private static readonly short[] lencnt = new short[MAXBITS + 1];
            private static readonly short[] lensym = new short[FIXLCODES];
            private static readonly short[] distcnt = new short[MAXBITS + 1];
            private static readonly short[] distsym = new short[MAXDCODES];

            private static readonly short[] order = new short[] /* permutation of code length codes */
            {
                16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15
            };

            private static int virgin = 1;
            private static Huffman lencode = new Huffman();
            private static Huffman distcode = new Huffman();

            /// <summary>
            /// Deflate compressed zlib data
            /// </summary>
            /// <param name="compressedData">Compressed data</param>
            /// <param name="startOffset">Start offset of the data in the buffer</param>
            /// <param name="length">Length of the data</param>
            /// <returns>Deflated data</returns>
            public static byte[] Decompress(byte[] compressedData, long startOffset, int length)
            {
                long bufStart;
                long bufEnd;

                if (compressedData.Length - startOffset < length)
                {
                    throw new Exception("Compressed data too short for given offset and length");
                }

                // Minimum size is 10 header bytes, and 8 bytes for crc and original length
                if (length < 1)
                {
                    throw new Exception("Compressed data too short to fit any kind of header");
                }

                // GZIP
                if (compressedData[0 + startOffset] == 0x1f && compressedData[1 + startOffset] == 0x8b)
                {
                    // Minimum size is 10 header bytes, and 8 bytes for crc and original length
                    if (length < 18)
                    {
                        throw new Exception("Compressed data too short to fit a gzip header");
                    }

                    int compressionMethod = compressedData[2 + startOffset];

                    if (compressionMethod != COMPRESSION_METHOD_DEFLATE)
                    {
                        throw new Exception("Compression method 0x" + compressionMethod.ToString("X") + " not supported");
                    }

                    int flags = compressedData[3 + startOffset];

                    // Default data offset, skiping a bunch of header fields
                    long i = 10 + startOffset;

                    // FLG.FEXTRA set
                    if ((flags & (0x1 << 2)) != 0)
                    {
                        int xlen = (compressedData[i] << 8) + (compressedData[i + 1] << 8);
                        i += 2 + xlen;

                        if (i >= compressedData.Length)
                        {
                            throw new Exception("GZip format is invalid or corrupt");
                        }
                    }

                    // FLG_FNAME
                    if ((flags & (0x1 << 3)) != 0)
                    {
                        while (compressedData[i] != 0 && i < compressedData.Length)
                        {
                            i++;
                        }
                    }

                    // FLG_COMMENT
                    if ((flags & (0x1 << 4)) != 0)
                    {
                        while (compressedData[i] != 0 && i < compressedData.Length)
                        {
                            i++;
                        }
                    }

                    // FLG_FHCRC
                    if ((flags & (0x1 << 1)) != 0)
                    {
                        i += 2;
                    }

                    if (compressedData.Length - i < 8 + 1)
                    {
                        throw new Exception("Invalid gzip format, not enough bytes after header for data + crc32 + isize");
                    }

                    // Extract only the compression data (minus header, minus crc at end)
                    bufStart = i;
                    bufEnd = i + compressedData.Length - i - 8;
                }
                else
                {
                    // Minimum size is 2 header bytes
                    if (length < 2)
                    {
                        throw new Exception("Compressed data too short to fit a zlib header");
                    }

                    // Other, assume ZLIB
                    long i = startOffset;

                    // Read Compression Method and CompressionInfo (containing window size)
                    byte cmf = compressedData[i++];

                    int compressionMethod = cmf & 0x0F;
                    int compressionInfo = (cmf & 0xF0) >> 4;

                    if (compressionMethod != COMPRESSION_METHOD_DEFLATE)
                    {
                        throw new Exception("Compression method 0x" + compressionMethod.ToString("X") + " not supported");
                    }

                    if (compressionInfo > 7 || compressionInfo < 0)
                    {
                        throw new Exception("Invalid compression info " + compressionInfo);
                    }

                    // Read flags
                    int flg = compressedData[i++];

                    bool presetDictionary = (flg & 0x20) > 0;

                    if (presetDictionary)
                    {
                        throw new Exception("Preset compression dictionary not supported");
                    }

                    bool headerCheck = (cmf << 8 | flg) % 31 == 0;
                    if (!headerCheck)
                    {
                        throw new Exception("Compression header check failed");
                    }

                    // Extract only the compression data (minus header, minus crc at end)
                    bufStart = i;
                    bufEnd = i + compressedData.Length - i - 4;
                }

                return DecompressData(compressedData, bufStart, bufEnd);
            }

            /*
             * Return need bits from the input stream.  This always leaves less than
             * eight bits in the buffer.  bits() works properly for need == 0.
             *
             * Format notes:
             *
             * - Bits are stored in bytes from the least significant bit to the most
             *   significant bit.  Therefore bits are dropped from the bottom of the bit
             *   buffer, using shift right, and new bytes are appended to the top of the
             *   bit buffer, using shift left.
             */
            private static int Bits(State s, int need)
            {
                long val;           /* bit accumulator (can use up to 20 bits) */

                /* load at least need bits into val */
                val = s.bitbuf;
                while (s.bitcnt < need)
                {
                    if (s.incnt == s.inlen)
                    {
                        throw new Exception("Decompression overran input buffer");
                    }

                    val |= (long)s.inbuf[s.instart + s.incnt++] << s.bitcnt;  /* load eight bits */
                    s.bitcnt += 8;
                }

                /* drop need bits and update buffer, always zero to seven bits left */
                s.bitbuf = (int)(val >> need);
                s.bitcnt -= need;

                /* return need bits, zeroing the bits above that */
                return (int)(val & ((1L << need) - 1));
            }

            /*
             * Process a stored block.
             *
             * Format notes:
             *
             * - After the two-bit stored block type (00), the stored block length and
             *   stored bytes are byte-aligned for fast copying.  Therefore any leftover
             *   bits in the byte that has the last bit of the type, as many as seven, are
             *   discarded.  The value of the discarded bits are not defined and should not
             *   be checked against any expectation.
             *
             * - The second inverted copy of the stored block length does not have to be
             *   checked, but it's probably a good idea to do so anyway.
             *
             * - A stored block can have zero length.  This is sometimes used to byte-align
             *   subsets of the compressed data for random access or partial recovery.
             */
            private static int Stored(State s)
            {
                int len;       /* length of stored block */

                /* discard leftover bits from current byte (assumes s.bitcnt < 8) */
                s.bitbuf = 0;
                s.bitcnt = 0;

                /* get length and check against its one's complement */
                if (s.incnt + 4 > s.inlen)
                {
                    return 2;                               /* not enough input */
                }

                len = s.inbuf[s.instart + s.incnt++];
                len |= s.inbuf[s.instart + s.incnt++] << 8;
                if (s.inbuf[s.instart + s.incnt++] != (~len & 0xff) ||
                    s.inbuf[s.instart + s.incnt++] != ((~len >> 8) & 0xff))
                {
                    return -2;                              /* didn't match complement! */
                }

                /* copy len bytes from in to out */
                if (s.incnt + (ulong)len > s.inlen)
                {
                    return 2;                               /* not enough input */
                }

                if (s.outbuf != null)
                {
                    if (s.outcnt + (ulong)len > s.outlen)
                    {
                        return 1;                           /* not enough output space */
                    }

                    while (len-- > 0)
                    {
                        s.outbuf[s.outcnt++] = s.inbuf[s.instart + s.incnt++];
                    }
                }
                else
                {                                      /* just scanning */
                    s.outcnt += (ulong)len;
                    s.incnt += (ulong)len;
                }

                /* done with a valid stored block */
                return 0;
            }

            /*
             * Decode a code from the stream s using huffman table h.  Return the symbol or
             * a negative value if there is an error.  If all of the lengths are zero, i.e.
             * an empty code, or if the code is incomplete and an invalid code is received,
             * then -10 is returned after reading MAXBITS bits.
             *
             * Format notes:
             *
             * - The codes as stored in the compressed data are bit-reversed relative to
             *   a simple integer ordering of codes of the same lengths.  Hence below the
             *   bits are pulled from the compressed data one at a time and used to
             *   build the code value reversed from what is in the stream in order to
             *   permit simple integer comparisons for decoding.  A table-based decoding
             *   scheme (as used in zlib) does not need to do this reversal.
             *
             * - The first code for the shortest length is all zeros.  Subsequent codes of
             *   the same length are simply integer increments of the previous code.  When
             *   moving up a length, a zero bit is appended to the code.  For a complete
             *   code, the last code of the longest length will be all ones.
             *
             * - Incomplete codes are handled by this decoder, since they are permitted
             *   in the deflate format.  See the format notes for fixed() and dynamic().
             */
            /*
             * A faster version of decode() for real applications of this code.   It's not
             * as readable, but it makes puff() twice as fast.  And it only makes the code
             * a few percent larger.
             */
            private static int Decode(State s, Huffman h)
            {
                int len;            /* current number of bits in code */
                int code;           /* len bits being decoded */
                int first;          /* first code of length len */
                int count;          /* number of codes of length len */
                int index;          /* index of first code of length len in symbol table */
                int bitbuf;         /* bits from stream */
                int left;           /* bits left in next or left to process */
                int next;        /* next number of codes */

                bitbuf = s.bitbuf;
                left = s.bitcnt;
                code = first = index = 0;
                len = 1;
                next = 1;
                while (true)
                {
                    while (left-- > 0)
                    {
                        code |= bitbuf & 1;
                        bitbuf >>= 1;
                        count = h.count[next++];

                        if (code - count < first)
                        { /* if length len, return symbol */
                            s.bitbuf = bitbuf;
                            s.bitcnt = (s.bitcnt - len) & 7;
                            return h.symbol[index + (code - first)];
                        }

                        index += count;             /* else update for next length */
                        first += count;
                        first <<= 1;
                        code <<= 1;
                        len++;
                    }

                    left = (MAXBITS + 1) - len;
                    if (left == 0)
                    {
                        break;
                    }

                    if (s.incnt == s.inlen)
                    {
                        throw new Exception("Decompression overran input buffer");
                    }

                    bitbuf = s.inbuf[s.instart + s.incnt++];

                    if (left > 8)
                    {
                        left = 8;
                    }
                }

                return -10;                         /* ran out of codes */
            }

            /*
             * Given the list of code lengths length[0..n-1] representing a canonical
             * Huffman code for n symbols, construct the tables required to decode those
             * codes.  Those tables are the number of codes of each length, and the symbols
             * sorted by length, retaining their original order within each length.  The
             * return value is zero for a complete code set, negative for an over-
             * subscribed code set, and positive for an incomplete code set.  The tables
             * can be used if the return value is zero or positive, but they cannot be used
             * if the return value is negative.  If the return value is zero, it is not
             * possible for decode() using that table to return an error--any stream of
             * enough bits will resolve to a symbol.  If the return value is positive, then
             * it is possible for decode() using that table to return an error for received
             * codes past the end of the incomplete lengths.
             *
             * Not used by decode(), but used for error checking, h->count[0] is the number
             * of the n symbols not in the code.  So n - h->count[0] is the number of
             * codes.  This is useful for checking for incomplete codes that have more than
             * one symbol, which is an error in a dynamic block.
             *
             * Assumption: for all i in 0..n-1, 0 <= length[i] <= MAXBITS
             * This is assured by the construction of the length arrays in dynamic() and
             * fixed() and is not verified by construct().
             *
             * Format notes:
             *
             * - Permitted and expected examples of incomplete codes are one of the fixed
             *   codes and any code with a single symbol which in deflate is coded as one
             *   bit instead of zero bits.  See the format notes for fixed() and dynamic().
             *
             * - Within a given code length, the symbols are kept in ascending order for
             *   the code bits definition.
             */
            private static int Construct(Huffman h, short[] length, int n, int offset = 0)
            {
                int symbol;         /* current symbol when stepping through length[] */
                int len;            /* current length when stepping through h.count[] */
                int left;           /* number of possible codes left of current length */
                short[] offs = new short[MAXBITS + 1];      /* offsets in symbol table for each length */

                /* count number of codes of each length */
                for (len = 0; len <= MAXBITS; len++)
                {
                    h.count[len] = 0;
                }

                for (symbol = 0; symbol < n; symbol++)
                {
                    h.count[length[symbol + offset]]++;   /* assumes lengths are within bounds */
                }

                if (h.count[0] == n)
                {
                    /* no codes! */
                    return 0;                       /* complete, but decode() will fail */
                }

                /* check for an over-subscribed or incomplete set of lengths */
                left = 1;                           /* one possible code of zero length */
                for (len = 1; len <= MAXBITS; len++)
                {
                    left <<= 1;                     /* one more bit, double codes left */
                    left -= h.count[len];          /* deduct count from possible codes */
                    if (left < 0)
                    {
                        return left;                /* over-subscribed--return negative */
                    }
                }                                   /* left > 0 means incomplete */

                /* generate offsets into symbol table for each length for sorting */
                offs[1] = 0;
                for (len = 1; len < MAXBITS; len++)
                {
                    offs[len + 1] = (short)(offs[len] + h.count[len]);
                }

                /*
                 * put symbols in table sorted by length, by symbol order within each
                 * length
                 */
                for (symbol = 0; symbol < n; symbol++)
                {
                    if (length[symbol + offset] != 0)
                    {
                        h.symbol[offs[length[symbol + offset]]++] = (short)symbol;
                    }
                }

                /* return zero for complete set, positive for incomplete set */
                return left;
            }

            /*
             * Decode literal/length and distance codes until an end-of-block code.
             *
             * Format notes:
             *
             * - Compressed data that is after the block type if fixed or after the code
             *   description if dynamic is a combination of literals and length/distance
             *   pairs terminated by and end-of-block code.  Literals are simply Huffman
             *   coded bytes.  A length/distance pair is a coded length followed by a
             *   coded distance to represent a string that occurs earlier in the
             *   uncompressed data that occurs again at the current location.
             *
             * - Literals, lengths, and the end-of-block code are combined into a single
             *   code of up to 286 symbols.  They are 256 literals (0..255), 29 length
             *   symbols (257..285), and the end-of-block symbol (256).
             *
             * - There are 256 possible lengths (3..258), and so 29 symbols are not enough
             *   to represent all of those.  Lengths 3..10 and 258 are in fact represented
             *   by just a length symbol.  Lengths 11..257 are represented as a symbol and
             *   some number of extra bits that are added as an integer to the base length
             *   of the length symbol.  The number of extra bits is determined by the base
             *   length symbol.  These are in the static arrays below, lens[] for the base
             *   lengths and lext[] for the corresponding number of extra bits.
             *
             * - The reason that 258 gets its own symbol is that the longest length is used
             *   often in highly redundant files.  Note that 258 can also be coded as the
             *   base value 227 plus the maximum extra value of 31.  While a good deflate
             *   should never do this, it is not an error, and should be decoded properly.
             *
             * - If a length is decoded, including its extra bits if any, then it is
             *   followed a distance code.  There are up to 30 distance symbols.  Again
             *   there are many more possible distances (1..32768), so extra bits are added
             *   to a base value represented by the symbol.  The distances 1..4 get their
             *   own symbol, but the rest require extra bits.  The base distances and
             *   corresponding number of extra bits are below in the static arrays dist[]
             *   and dext[].
             *
             * - Literal bytes are simply written to the output.  A length/distance pair is
             *   an instruction to copy previously uncompressed bytes to the output.  The
             *   copy is from distance bytes back in the output stream, copying for length
             *   bytes.
             *
             * - Distances pointing before the beginning of the output data are not
             *   permitted.
             *
             * - Overlapped copies, where the length is greater than the distance, are
             *   allowed and common.  For example, a distance of one and a length of 258
             *   simply copies the last byte 258 times.  A distance of four and a length of
             *   twelve copies the last four bytes three times.  A simple forward copy
             *   ignoring whether the length is greater than the distance or not implements
             *   this correctly.  You should not use memcpy() since its behavior is not
             *   defined for overlapped arrays.  You should not use memmove() or bcopy()
             *   since though their behavior -is- defined for overlapping arrays, it is
             *   defined to do the wrong thing in this case.
             */
            private static int Codes(State s, Huffman lencode, Huffman distcode)
            {
                int symbol;         /* decoded symbol */
                int len;            /* length for copy */
                int dist;      /* distance for copy */

                /* decode literals and length/distance pairs */
                do
                {
                    symbol = Decode(s, lencode);
                    if (symbol < 0)
                    {
                        return symbol;              /* invalid symbol */
                    }

                    if (symbol < 256)
                    {             /* literal: symbol is the byte */
                                  /* write out the literal */
                        if (s.outbuf != null)
                        {
                            if (s.outcnt == s.outlen)
                            {
                                return 1;
                            }

                            s.outbuf[s.outcnt] = (byte)symbol;
                        }

                        s.outcnt++;
                    }
                    else if (symbol > 256)
                    {        /* length */
                             /* get and compute length */
                        symbol -= 257;

                        if (symbol >= 29)
                        {
                            return -10;             /* invalid fixed code */
                        }

                        len = lens[symbol] + Bits(s, lext[symbol]);

                        /* get and check distance */
                        symbol = Decode(s, distcode);
                        if (symbol < 0)
                        {
                            return symbol;          /* invalid symbol */
                        }

                        dist = dists[symbol] + Bits(s, dext[symbol]);

                        /* copy length bytes from distance bytes back */
                        if (s.outbuf != null)
                        {
                            if (s.outcnt + (ulong)len > s.outlen)
                            {
                                return 1;
                            }

                            while (len-- > 0)
                            {
                                s.outbuf[s.outcnt] =
                            s.outbuf[s.outcnt - (ulong)dist];
                                s.outcnt++;
                            }
                        }
                        else
                        {
                            s.outcnt += (ulong)len;
                        }
                    }
                }
                while (symbol != 256);            /* end of block symbol */

                /* done with a valid fixed or dynamic block */
                return 0;
            }

            /*
             * Process a fixed codes block.
             *
             * Format notes:
             *
             * - This block type can be useful for compressing small amounts of data for
             *   which the size of the code descriptions in a dynamic block exceeds the
             *   benefit of custom codes for that block.  For fixed codes, no bits are
             *   spent on code descriptions.  Instead the code lengths for literal/length
             *   codes and distance codes are fixed.  The specific lengths for each symbol
             *   can be seen in the "for" loops below.
             *
             * - The literal/length code is complete, but has two symbols that are invalid
             *   and should result in an error if received.  This cannot be implemented
             *   simply as an incomplete code since those two symbols are in the "middle"
             *   of the code.  They are eight bits long and the longest literal/length\
             *   code is nine bits.  Therefore the code must be constructed with those
             *   symbols, and the invalid symbols must be detected after decoding.
             *
             * - The fixed distance codes also have two invalid symbols that should result
             *   in an error if received.  Since all of the distance codes are the same
             *   length, this can be implemented as an incomplete code.  Then the invalid
             *   codes are detected while decoding.
             */
            private static int Fixed(State s)
            {
                /* build fixed huffman tables if first call (may not be thread safe) */
                if (virgin == 1)
                {
                    int symbol;
                    short[] lengths = new short[FIXLCODES];

                    /* construct lencode and distcode */
                    lencode.count = lencnt;
                    lencode.symbol = lensym;
                    distcode.count = distcnt;
                    distcode.symbol = distsym;

                    /* literal/length table */
                    for (symbol = 0; symbol < 144; symbol++)
                    {
                        lengths[symbol] = 8;
                    }

                    for (; symbol < 256; symbol++)
                    {
                        lengths[symbol] = 9;
                    }

                    for (; symbol < 280; symbol++)
                    {
                        lengths[symbol] = 7;
                    }

                    for (; symbol < FIXLCODES; symbol++)
                    {
                        lengths[symbol] = 8;
                    }

                    Construct(lencode, lengths, FIXLCODES);

                    /* distance table */
                    for (symbol = 0; symbol < MAXDCODES; symbol++)
                    {
                        lengths[symbol] = 5;
                    }

                    Construct(distcode, lengths, MAXDCODES);

                    /* do this just once */
                    virgin = 0;
                }

                /* decode data until end-of-block code */
                return Codes(s, lencode, distcode);
            }

            /*
             * Process a dynamic codes block.
             *
             * Format notes:
             *
             * - A dynamic block starts with a description of the literal/length and
             *   distance codes for that block.  New dynamic blocks allow the compressor to
             *   rapidly adapt to changing data with new codes optimized for that data.
             *
             * - The codes used by the deflate format are "canonical", which means that
             *   the actual bits of the codes are generated in an unambiguous way simply
             *   from the number of bits in each code.  Therefore the code descriptions
             *   are simply a list of code lengths for each symbol.
             *
             * - The code lengths are stored in order for the symbols, so lengths are
             *   provided for each of the literal/length symbols, and for each of the
             *   distance symbols.
             *
             * - If a symbol is not used in the block, this is represented by a zero as
             *   as the code length.  This does not mean a zero-length code, but rather
             *   that no code should be created for this symbol.  There is no way in the
             *   deflate format to represent a zero-length code.
             *
             * - The maximum number of bits in a code is 15, so the possible lengths for
             *   any code are 1..15.
             *
             * - The fact that a length of zero is not permitted for a code has an
             *   interesting consequence.  Normally if only one symbol is used for a given
             *   code, then in fact that code could be represented with zero bits.  However
             *   in deflate, that code has to be at least one bit.  So for example, if
             *   only a single distance base symbol appears in a block, then it will be
             *   represented by a single code of length one, in particular one 0 bit.  This
             *   is an incomplete code, since if a 1 bit is received, it has no meaning,
             *   and should result in an error.  So incomplete distance codes of one symbol
             *   should be permitted, and the receipt of invalid codes should be handled.
             *
             * - It is also possible to have a single literal/length code, but that code
             *   must be the end-of-block code, since every dynamic block has one.  This
             *   is not the most efficient way to create an empty block (an empty fixed
             *   block is fewer bits), but it is allowed by the format.  So incomplete
             *   literal/length codes of one symbol should also be permitted.
             *
             * - If there are only literal codes and no lengths, then there are no distance
             *   codes.  This is represented by one distance code with zero bits.
             *
             * - The list of up to 286 length/literal lengths and up to 30 distance lengths
             *   are themselves compressed using Huffman codes and run-length encoding.  In
             *   the list of code lengths, a 0 symbol means no code, a 1..15 symbol means
             *   that length, and the symbols 16, 17, and 18 are run-length instructions.
             *   Each of 16, 17, and 18 are follwed by extra bits to define the length of
             *   the run.  16 copies the last length 3 to 6 times.  17 represents 3 to 10
             *   zero lengths, and 18 represents 11 to 138 zero lengths.  Unused symbols
             *   are common, hence the special coding for zero lengths.
             *
             * - The symbols for 0..18 are Huffman coded, and so that code must be
             *   described first.  This is simply a sequence of up to 19 three-bit values
             *   representing no code (0) or the code length for that symbol (1..7).
             *
             * - A dynamic block starts with three fixed-size counts from which is computed
             *   the number of literal/length code lengths, the number of distance code
             *   lengths, and the number of code length code lengths (ok, you come up with
             *   a better name!) in the code descriptions.  For the literal/length and
             *   distance codes, lengths after those provided are considered zero, i.e. no
             *   code.  The code length code lengths are received in a permuted order (see
             *   the order[] array below) to make a short code length code length list more
             *   likely.  As it turns out, very short and very long codes are less likely
             *   to be seen in a dynamic code description, hence what may appear initially
             *   to be a peculiar ordering.
             *
             * - Given the number of literal/length code lengths (nlen) and distance code
             *   lengths (ndist), then they are treated as one long list of nlen + ndist
             *   code lengths.  Therefore run-length coding can and often does cross the
             *   boundary between the two sets of lengths.
             *
             * - So to summarize, the code description at the start of a dynamic block is
             *   three counts for the number of code lengths for the literal/length codes,
             *   the distance codes, and the code length codes.  This is followed by the
             *   code length code lengths, three bits each.  This is used to construct the
             *   code length code which is used to read the remainder of the lengths.  Then
             *   the literal/length code lengths and distance lengths are read as a single
             *   set of lengths using the code length codes.  Codes are constructed from
             *   the resulting two sets of lengths, and then finally you can start
             *   decoding actual compressed data in the block.
             *
             * - For reference, a "typical" size for the code description in a dynamic
             *   block is around 80 bytes.
             */
            private static int Dynamic(State s)
            {
                int nlen, ndist, ncode;             /* number of lengths in descriptor */
                int index;                          /* index of lengths[] */
                int err;                            /* construct() return value */
                short[] lengths = new short[MAXCODES];            /* descriptor code lengths */
                short[] lencnt = new short[MAXBITS + 1];
                short[] lensym = new short[MAXLCODES];         /* lencode memory */
                short[] distcnt = new short[MAXBITS + 1];
                short[] distsym = new short[MAXDCODES];       /* distcode memory */
                Huffman lencode = new Huffman();
                Huffman distcode = new Huffman();   /* length and distance codes */

                /* construct lencode and distcode */
                lencode.count = lencnt;
                lencode.symbol = lensym;
                distcode.count = distcnt;
                distcode.symbol = distsym;

                /* get number of lengths in each table, check lengths */
                nlen = Bits(s, 5) + 257;
                ndist = Bits(s, 5) + 1;
                ncode = Bits(s, 4) + 4;
                if (nlen > MAXLCODES || ndist > MAXDCODES)
                {
                    return -3;                      /* bad counts */
                }

                /* read code length code lengths (really), missing lengths are zero */
                for (index = 0; index < ncode; index++)
                {
                    lengths[order[index]] = (short)Bits(s, 3);
                }

                for (; index < 19; index++)
                {
                    lengths[order[index]] = 0;
                }

                /* build huffman table for code lengths codes (use lencode temporarily) */
                err = Construct(lencode, lengths, 19);
                if (err != 0)
                {
                    /* require complete code set here */
                    return -4;
                }

                /* read length/literal and distance code length tables */
                index = 0;
                while (index < nlen + ndist)
                {
                    int symbol;             /* decoded value */
                    int len;                /* last length to repeat */

                    symbol = Decode(s, lencode);
                    if (symbol < 0)
                    {
                        return symbol;          /* invalid symbol */
                    }

                    if (symbol < 16)
                    {
                        /* length in 0..15 */
                        lengths[index++] = (short)symbol;
                    }
                    else
                    {                          /* repeat instruction */
                        len = 0;                    /* assume repeating zeros */
                        if (symbol == 16)
                        {         /* repeat last length 3..6 times */
                            if (index == 0)
                            {
                                return -5;          /* no last length! */
                            }

                            len = lengths[index - 1];       /* last length */
                            symbol = 3 + Bits(s, 2);
                        }
                        else if (symbol == 17)
                        {
                            /* repeat zero 3..10 times */
                            symbol = 3 + Bits(s, 3);
                        }
                        else
                        {
                            /* == 18, repeat zero 11..138 times */
                            symbol = 11 + Bits(s, 7);
                        }

                        if (index + symbol > nlen + ndist)
                        {
                            return -6;              /* too many lengths! */
                        }

                        while (symbol-- > 0)
                        {
                            /* repeat last or zero symbol times */
                            lengths[index++] = (short)len;
                        }
                    }
                }

                /* check for end-of-block code -- there better be one! */
                if (lengths[256] == 0)
                {
                    return -9;
                }

                /* build huffman table for literal/length codes */
                err = Construct(lencode, lengths, nlen);
                if (err != 0 && (err < 0 || nlen != lencode.count[0] + lencode.count[1]))
                {
                    return -7;      /* incomplete code ok only for single length 1 code */
                }

                /* build huffman table for distance codes */
                err = Construct(distcode, lengths, ndist, nlen);
                if (err != 0 && (err < 0 || ndist != distcode.count[0] + distcode.count[1]))
                {
                    return -8;      /* incomplete code ok only for single length 1 code */
                }

                /* decode data until end-of-block code */
                return Codes(s, lencode, distcode);
            }

            /*
             * Inflate source to dest.  On return, destlen and sourcelen are updated to the
             * size of the uncompressed data and the size of the deflate data respectively.
             * On success, the return value of puff() is zero.  If there is an error in the
             * source data, i.e. it is not in the deflate format, then a negative value is
             * returned.  If there is not enough input available or there is not enough
             * output space, then a positive error is returned.  In that case, destlen and
             * sourcelen are not updated to facilitate retrying from the beginning with the
             * provision of more input data or more output space.  In the case of invalid
             * inflate data (a negative error), the dest and source pointers are updated to
             * facilitate the debugging of deflators.
             *
             * puff() also has a mode to determine the size of the uncompressed output with
             * no output written.  For this dest must be (unsigned char *)0.  In this case,
             * the input value of *destlen is ignored, and on return *destlen is set to the
             * size of the uncompressed output.
             *
             * The return codes are:
             *
             *   2:  available inflate data did not terminate
             *   1:  output space exhausted before completing inflate
             *   0:  successful inflate
             *  -1:  invalid block type (type == 3)
             *  -2:  stored block length did not match one's complement
             *  -3:  dynamic block code description: too many length or distance codes
             *  -4:  dynamic block code description: code lengths codes incomplete
             *  -5:  dynamic block code description: repeat lengths with no first length
             *  -6:  dynamic block code description: repeat more than specified lengths
             *  -7:  dynamic block code description: invalid literal/length code lengths
             *  -8:  dynamic block code description: invalid distance code lengths
             *  -9:  dynamic block code description: missing end-of-block code
             * -10:  invalid literal/length or distance code in fixed or dynamic block
             * -11:  distance is too far back in fixed or dynamic block
             *
             * Format notes:
             *
             * - Three bits are read for each block to determine the kind of block and
             *   whether or not it is the last block.  Then the block is decoded and the
             *   process repeated if it was not the last block.
             *
             * - The leftover bits in the last byte of the deflate data after the last
             *   block (if it was a fixed or dynamic block) are undefined and have no
             *   expected values to check.
             */
            private static int Puff(
                byte[] dest,           /* pointer to destination pointer */
                out ulong destlen,        /* amount of output space */
                ulong indestlen,
                byte[] source,   /* pointer to source data pointer */
                long bufStart,
                out ulong sourcelen,
                ulong insourcelen)      /* amount of input available */
            {
                destlen = sourcelen = 0;

                State s = new State();             /* input/output state */
                int last, type;             /* block information */
                int err;                    /* return value */

                /* initialize output state */
                s.outbuf = dest;
                s.outlen = indestlen;                /* ignored if dest is NIL */
                s.outcnt = 0;

                /* initialize input state */
                s.inbuf = source;
                s.inlen = insourcelen;
                s.incnt = 0;
                s.instart = (ulong)bufStart;
                s.bitbuf = 0;
                s.bitcnt = 0;

                /* return if bits() or decode() tries to read past available input */
                {
                    /* process blocks until last block or error */
                    do
                    {
                        last = Bits(s, 1);         /* one if last block */
                        type = Bits(s, 2);         /* block type 0..3 */
                        err = type == 0 ?
                            Stored(s) :
                                        (type == 1 ?
                                            Fixed(s) :
                                            (type == 2 ?
                                                Dynamic(s) :
                                                -1));       /* type == 3, invalid */
                        if (err != 0)
                        {
                            break;                  /* return with error */
                        }
                    }
                    while (last == 0);
                }

                /* update the lengths and return */
                if (err <= 0)
                {
                    destlen = s.outcnt;
                    sourcelen = s.incnt;
                }

                return err;
            }

            private static byte[] DecompressData(byte[] data, long bufStart, long bufEnd)
            {
                ulong outLen;
                ulong inLen;

                // First determine output length
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                int ret = Puff(null, out outLen, 0, data, bufStart, out inLen, (ulong)(bufEnd - bufStart));
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                if (ret != 0)
                {
                    throw new Exception("Decompression failed");
                }

                byte[] uncompressed = new byte[outLen];
                if (uncompressed == null)
                {
                    throw new Exception("Decompression ran out of memory allocating output buffer");
                }

#pragma warning disable IDE0059 // Unnecessary assignment of a value
                ret = Puff(uncompressed, out outLen, (ulong)uncompressed.Length, data, bufStart, out inLen, (ulong)(bufEnd - bufStart));
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                if (ret != 0)
                {
                    throw new Exception("Decompression failed");
                }

                return uncompressed;
            }

            /*
             * Huffman code decoding tables.  count[1..MAXBITS] is the number of symbols of
             * each length, which for a canonical code are stepped through in order.
             * symbol[] are the symbol values in canonical order, where the number of
             * entries is the sum of the counts in count[].  The decoding process can be
             * seen in the function decode() below.
             */
            private struct Huffman
            {
                public short[] count;       /* number of symbols of each length */
                public short[] symbol;      /* canonically ordered symbols */
            }

            /* input and output state */
            private class State
            {
                /* output state */
                public byte[] outbuf;         /* output buffer */
                public ulong outlen;       /* available space at out */
                public ulong outcnt;       /* bytes written to out so far */

                /* input state */
                public byte[] inbuf;    /* input buffer */
                public ulong inlen;        /* available input at in */
                public ulong instart;
                public ulong incnt;        /* bytes read so far */
                public int bitbuf;                 /* bit buffer */
                public int bitcnt;                 /* number of bits in bit buffer */
            }
        }

        private class DeflateCompressor
        {
            private const uint HASH_SIZE = 16384;

            private static readonly ushort[] mLengthC = new ushort[]
                {
                    3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19,
                    23, 27, 31, 35, 43, 51, 59, 67, 83, 99, 115,
                    131, 163, 195, 227, 258, 259
                };

            private static readonly byte[] mLengthEB = new byte[]
                {
                    0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2,
                    2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0
                };

            private static readonly ushort[] mDistC = new ushort[]
                {
                    1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65,
                    97, 129, 193, 257, 385, 513, 769, 1025, 1537,
                    2049, 3073, 4097, 6145, 8193, 12289, 16385,
                    24577, 32768
                };

            private static readonly byte[] mDistB = new byte[]
                {
                    0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6,
                    6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12,
                    13, 13
                };

            public static byte[] Compress(byte[] data)
            {
                int dataLen = data.Length;
                int quality = 8;
                int i, j;
                var bitStream = new BitStream();

                List<int>[] hashTable = new List<int>[(int)HASH_SIZE];

                if (hashTable == null)
                {
                    return null;
                }

                // Gzip header
                bitStream.WriteByte(0x1f); // GZIP magic num
                bitStream.WriteByte(0x8b); // GZIP magic num

                bitStream.WriteByte(0x08); // Compression format
                bitStream.WriteByte(0); // Flags

                bitStream.WriteByte(0); // MTime
                bitStream.WriteByte(0); // MTime
                bitStream.WriteByte(0); // MTime
                bitStream.WriteByte(0); // MTime

                bitStream.WriteByte(4); // XFL flags, fastest algorithm
                bitStream.WriteByte(255); // Unknown OS

                // Deflate header
                bitStream.WriteBits(1, 1); // BFINAL = 1
                bitStream.WriteBits(1, 2); // BTYPE = 1 -- fixed huffman

                for (i = 0; i < HASH_SIZE; ++i)
                {
                    hashTable[i] = null;
                }

                i = 0;
                while (i < dataLen - 3)
                {
                    // hash next 3 bytes of data to be compressed
                    uint h = Hash(ref data, i) & (HASH_SIZE - 1);
                    int best = 3;

                    int bestloc = -1;
                    List<int> hashList = hashTable[h];
                    int n = hashList != null ? hashList.Count : 0;
                    for (j = 0; j < n; ++j)
                    {
                        if (hashList[j] > i - 32768)
                        { // if entry lies within window
                            int d = CountM(ref data, hashList[j], i, dataLen - i);
                            if (d >= best)
                            {
                                best = d;
                                bestloc = hashList[j];
                            }
                        }
                    }

                    // when hash table entry is too long, delete half the entries
                    if (hashTable[h] != null && hashTable[h].Count == 2 * quality)
                    {
                        hashTable[h] = hashTable[h].GetRange(quality, quality);
                    }

                    if (hashTable[h] == null)
                    {
                        hashTable[h] = new List<int>();
                    }

                    hashTable[h].Add(i);

                    if (bestloc != -1)
                    {
                        // "lazy matching" - check match at *next* byte, and if it's better, do cur byte as literal
                        h = Hash(ref data, i + 1) & (HASH_SIZE - 1);
                        hashList = hashTable[h];
                        n = hashList != null ? hashList.Count : 0;
                        for (j = 0; j < n; ++j)
                        {
                            if (hashList[j] > i - 32767)
                            {
                                int e = CountM(ref data, hashList[j], i + 1, dataLen - i - 1);
                                if (e > best)
                                {
                                    // if next match is better, bail on current match
                                    bestloc = -1;
                                    break;
                                }
                            }
                        }
                    }

                    if (bestloc != -1)
                    {
                        int d = (int)(i - bestloc); // distance back

                        for (j = 0; best > mLengthC[j + 1] - 1; ++j)
                        {
                            // Do nothing
                        }

                        Huff(bitStream, j + 257);

                        if (mLengthEB[j] != 0)
                        {
                            bitStream.WriteBits(best - mLengthC[j], mLengthEB[j]);
                        }

                        for (j = 0; d > mDistC[j + 1] - 1; ++j)
                        {
                            // Do nothing
                        }

                        bitStream.WriteBits(BitRev(j, 5), 5);

                        if (mDistB[j] != 0)
                        {
                            bitStream.WriteBits(d - mDistC[j], mDistB[j]);
                        }

                        i += best;
                    }
                    else
                    {
                        HuffB(bitStream, data[i]);
                        ++i;
                    }
                }

                // write out final bytes
                for (; i < dataLen; ++i)
                {
                    HuffB(bitStream, data[i]);
                }

                Huff(bitStream, 256); // end of block

                // pad with 0 bits to byte boundary
                bitStream.PadBits();

                {
                    // compute adler32 on input
                    uint s1 = 1, s2 = 0;
                    int blocklen = (int)(dataLen % 5552);
                    j = 0;
                    while (j < dataLen)
                    {
                        for (i = 0; i < blocklen; ++i)
                        {
                            s1 += data[j + i];
                            s2 += s1;
                        }

                        s1 %= 65521;
                        s2 %= 65521;
                        j += blocklen;
                        blocklen = 5552;
                    }

                    bitStream.WriteByte((byte)(s2 >> 8));
                    bitStream.WriteByte((byte)s2);
                    bitStream.WriteByte((byte)(s1 >> 8));
                    bitStream.WriteByte((byte)s1);

                    // CRC32 for GZIP
                    bitStream.WriteByte((byte)(s2 >> 8));
                    bitStream.WriteByte((byte)s2);
                    bitStream.WriteByte((byte)(s1 >> 8));
                    bitStream.WriteByte((byte)s1);

                    // Size for GZIP
                    bitStream.WriteByte((byte)((dataLen & 0xFF000000) >> 24));
                    bitStream.WriteByte((byte)((dataLen & 0x00FF0000) >> 16));
                    bitStream.WriteByte((byte)((dataLen & 0x0000FF00) >> 8));
                    bitStream.WriteByte((byte)((dataLen & 0x000000FF) >> 0));
                }

                return bitStream.ToArray();
            }

            private static uint Hash(ref byte[] data, int offset)
            {
                uint hash = (uint)(data[0 + offset] + (data[1 + offset] << 8) + (data[2 + offset] << 16));
                hash ^= hash << 3;
                hash += hash >> 5;
                hash ^= hash << 4;
                hash += hash >> 17;
                hash ^= hash << 25;
                hash += hash >> 6;
                return hash;
            }

            private static int CountM(ref byte[] data, int a_offset, int b_offset, int limit)
            {
                int i;
                for (i = 0; i < limit && i < 258; ++i)
                {
                    if (data[i + a_offset] != data[i + b_offset])
                    {
                        break;
                    }
                }

                return i;
            }

            private static int BitRev(int code, int codebits)
            {
                int res = 0;
                while ((codebits--) != 0)
                {
                    res = (res << 1) | (code & 1);
                    code >>= 1;
                }

                return res;
            }

            private static void HuffA(BitStream bitStream, int b, int c)
            {
                bitStream.WriteBits(BitRev(b, c), c);
            }

            private static void Huff1(BitStream bitStream, int n)
            {
                HuffA(bitStream, 0x30 + n, 8);
            }

            private static void Huff2(BitStream bitStream, int n)
            {
                HuffA(bitStream, 0x190 + n - 144, 9);
            }

            private static void Huff3(BitStream bitStream, int n)
            {
                HuffA(bitStream, 0 + n - 256, 7);
            }

            private static void Huff4(BitStream bitStream, int n)
            {
                HuffA(bitStream, 0xc0 + n - 280, 8);
            }

            private static void HuffB(BitStream bitStream, int n)
            {
                if (n <= 143)
                {
                    Huff1(bitStream, n);
                }
                else
                {
                    Huff2(bitStream, n);
                }
            }

            private static void Huff(BitStream bitStream, int n)
            {
                if (n <= 143)
                {
                    Huff1(bitStream, n);
                }
                else
                {
                    if (n <= 255)
                    {
                        Huff2(bitStream, n);
                    }
                    else
                    {
                        if (n <= 279)
                        {
                            Huff3(bitStream, n);
                        }
                        else
                        {
                            Huff4(bitStream, n);
                        }
                    }
                }
            }

            private class BitStream : MemoryStream
            {
                private int bitPattern = 0;
                private int bitOffset = 0;

                public void WriteBits(int bits, int count)
                {
                    for (int i = 0; i < count; i++)
                    {
                        int bit = (bits & (1 << i)) >> i;
                        bitPattern |= bit << bitOffset;
                        bitOffset++;

                        if (bitOffset == 8)
                        {
                            base.WriteByte((byte)bitPattern);
                            bitOffset = 0;
                            bitPattern = 0;
                        }
                    }
                }

                public override void WriteByte(byte b)
                {
                    // Flush partial bit pattern if there is one
                    PadBits();

                    base.WriteByte(b);
                }

                public void PadBits()
                {
                    if (bitOffset > 0)
                    {
                        base.WriteByte((byte)bitPattern);
                        bitOffset = 0;
                        bitPattern = 0;
                    }
                }
            }
        }
    }
}
