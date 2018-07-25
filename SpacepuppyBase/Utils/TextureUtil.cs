using UnityEngine;
using System.IO;

namespace com.spacepuppy.Utils
{
    
    public static class TextureUtil
    {

        #region EncodeToTGA

        /*
         * This section is taken from this openly available code whose license (at time of my using it) is 
         * 'free and unencumbered software released into the public domain' and can be found here:
         * https://github.com/phwitti/EncodeToTGA/blob/master/EncodeToTGAExtension.cs
         * 
         * License as it was found on the original source code:
         * 
        // This is free and unencumbered software released into the public domain.
        //
        // Anyone is free to copy, modify, publish, use, compile, sell, or
        // distribute this software, either in source code form or as a compiled
        // binary, for any purpose, commercial or non-commercial, and by any
        // means.
        //
        // In jurisdictions that recognize copyright laws, the author or authors
        // of this software dedicate any and all copyright interest in the
        // software to the public domain. We make this dedication for the benefit
        // of the public at large and to the detriment of our heirs and
        // successors. We intend this dedication to be an overt act of
        // relinquishment in perpetuity of all present and future rights to this
        // software under copyright law.
        //
        // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
        // EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
        // MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
        // IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
        // OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
        // ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
        // OTHER DEALINGS IN THE SOFTWARE.
        //
        // For more information, please refer to <http://unlicense.org/>
         * 
         */

        public static byte[] EncodeToTGA(this Texture2D _texture2D)
        {
            const int iTgaHeaderSize = 18;
            const int iBytesPerPixelRGB24 = 3; // 1 byte per channel (rgb)
            const int iBytesPerPixelARGB32 = 4; // ~ (rgba)

            var useAlpha = SupportsAlpha(_texture2D.format);
            int iBytesPerPixel = useAlpha ? iBytesPerPixelARGB32 : iBytesPerPixelRGB24;

            //

            using (MemoryStream memoryStream = new MemoryStream(iTgaHeaderSize + _texture2D.width * _texture2D.height * iBytesPerPixel))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    // Write TGA Header

                    binaryWriter.Write((byte)0);                    // IDLength (not in use)
                    binaryWriter.Write((byte)0);                    // ColorMapType (not in use)
                    binaryWriter.Write((byte)10);                   // DataTypeCode == 10 (Runlength encoded RGB images)
                    binaryWriter.Write((short)0);                   // ColorMapOrigin (not in use)
                    binaryWriter.Write((short)0);                   // ColorMapLength (not in use)
                    binaryWriter.Write((byte)0);                    // ColorMapDepth (not in use)
                    binaryWriter.Write((short)0);                   // Origin X
                    binaryWriter.Write((short)0);                   // Origin Y
                    binaryWriter.Write((short)_texture2D.width);    // Width
                    binaryWriter.Write((short)_texture2D.height);   // Height
                    binaryWriter.Write((byte)(iBytesPerPixel * 8)); // Bits Per Pixel
                    binaryWriter.Write((byte)0);                    // ImageDescriptor (not in use)

                    // Write RLE Encoded Pixels

                    Color32[] arPixels = _texture2D.GetPixels32();

                    const int iMaxPacketLength = 128;
                    int iPacketStart = 0;
                    int iPacketEnd = 0;

                    while (iPacketStart < arPixels.Length)
                    {
                        Color32 c32PreviousPixel = arPixels[iPacketStart];

                        // Get current Packet Type
                        RLEPacketType packetType = TextureUtil.PacketType(arPixels, iPacketStart);

                        // Find Packet End
                        int iReadEnd = Mathf.Min(iPacketStart + iMaxPacketLength, arPixels.Length);
                        for (iPacketEnd = iPacketStart + 1; iPacketEnd < iReadEnd; ++iPacketEnd)
                        {
                            bool bPreviousEqualsCurrent = TextureUtil.Equals(arPixels[iPacketEnd - 1], arPixels[iPacketEnd]);

                            // Packet End if change in Packet Type or if max Packet-Size reached
                            if (packetType == RLEPacketType.RAW && bPreviousEqualsCurrent ||
                                packetType == RLEPacketType.RLE && !bPreviousEqualsCurrent)
                            {
                                break;
                            }
                        }

                        // Write Packet

                        int iPacketLength = iPacketEnd - iPacketStart;

                        switch (packetType)
                        {
                            case RLEPacketType.RLE:

                                // Add RLE-Bit to PacketLength
                                binaryWriter.Write((byte)((iPacketLength - 1) | (1 << 7)));

                                binaryWriter.Write(c32PreviousPixel.b);
                                binaryWriter.Write(c32PreviousPixel.g);
                                binaryWriter.Write(c32PreviousPixel.r);

                                if (useAlpha)
                                    binaryWriter.Write(c32PreviousPixel.a);

                                break;
                            case RLEPacketType.RAW:

                                binaryWriter.Write((byte)(iPacketLength - 1));

                                for (int iPacketPosition = iPacketStart; iPacketPosition < iPacketEnd; ++iPacketPosition)
                                {
                                    binaryWriter.Write(arPixels[iPacketPosition].b);
                                    binaryWriter.Write(arPixels[iPacketPosition].g);
                                    binaryWriter.Write(arPixels[iPacketPosition].r);

                                    if (useAlpha)
                                        binaryWriter.Write(arPixels[iPacketPosition].a);
                                }

                                break;
                        }

                        iPacketStart = iPacketEnd;
                    }
                }

                return memoryStream.GetBuffer();
            }
        }

        //

        // RLE Helper

        private enum RLEPacketType { RLE, RAW }

        private static bool Equals(Color32 _first, Color32 _second)
        {
            return _first.r == _second.r && _first.g == _second.g && _first.b == _second.b && _first.a == _second.a;
        }

        private static RLEPacketType PacketType(Color32[] _arData, int _iPacketPosition)
        {
            if ((_iPacketPosition != _arData.Length - 1) && TextureUtil.Equals(_arData[_iPacketPosition], _arData[_iPacketPosition + 1]))
            {
                return RLEPacketType.RLE;
            }
            else
            {
                return RLEPacketType.RAW;
            }
        }

        private static bool SupportsAlpha(TextureFormat format)
        {
            switch(format)
            {
                case TextureFormat.RGBA32:
                case TextureFormat.ARGB32:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

    }
}
