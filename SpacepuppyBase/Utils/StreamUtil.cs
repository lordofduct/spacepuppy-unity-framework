using System;

namespace com.spacepuppy.Utils
{
    public static class StreamUtil
    {

        public static void CopyTo(this System.IO.Stream source, System.IO.Stream destination)
        {
            const int BUFFER = 4096;

            byte[] buffer = new byte[BUFFER];
            int count;
            while((count = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, count);
            }
        }

        public static byte[] ToByteArray(this System.IO.Stream strm)
        {
            if(strm is System.IO.MemoryStream)
            {
                return (strm as System.IO.MemoryStream).ToArray();
            }

            long originalPosition = -1;
            if(strm.CanSeek)
            {
                originalPosition = strm.Position;
            }

            try
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    ms.CopyTo(ms);
                    return ms.ToArray();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (originalPosition >= 0) strm.Position = originalPosition;
            }
        }

    }
}
