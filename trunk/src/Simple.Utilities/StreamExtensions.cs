using System.Text;
using System.IO;

namespace Simple.Utilities
{
    public static class StreamExtensions
    {

        public static string ReadAllText(this Stream sourceStream)
        {
            return sourceStream.ReadAllText(Encoding.UTF8);
        }

        public static string ReadAllText(this Stream sourceStream, Encoding encoding)
        {
            Requires.ArgumentNotNull(sourceStream, "sourceStream");

            using (var streamReader = new StreamReader(sourceStream,encoding))
            {
                return streamReader.ReadToEnd();
            }
        }

        public static void MoveToStart(this Stream sourceStream)
        {
            Requires.ArgumentNotNull(sourceStream, "sourceStream");
            sourceStream.Seek(0, SeekOrigin.Begin);
        }

        public static void CopyTo(this Stream source, Stream dest)
        {
            Requires.ArgumentNotNull(source, "source");
            Requires.ArgumentNotNull(dest, "dest");
            if (source.CanSeek)
            {
                source.MoveToStart();
            }

            var buffer = new byte[1024];
            var bytesRead = 1;
            while (source.CanRead && bytesRead > 0)
            {
                bytesRead = source.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    dest.Write(buffer, 0, bytesRead);
                }
            }
        }

        public static void ToFile(this Stream source, string fileName)
        {
            using (var fileStream = File.OpenWrite(fileName))
            {
                source.CopyTo(fileStream);
            }
        }
    }
}
