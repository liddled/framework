using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace DL.Framework.Web
{
    public class WhiteSpaceFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Filter = new WhiteSpaceStream(filterContext.HttpContext.Response.Filter);
        }
    }

    internal class WhiteSpaceStream : Stream
    {
        private readonly Stream _sink;
        private readonly Regex _regex = new Regex(@"(?<=[^])\t{2,}|(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,11}(?=[<])|(?=[\n])\s{2,}");
        //private static Regex m_regex = new Regex(@"^\s+", RegexOptions.Multiline | RegexOptions.Compiled); 

        public WhiteSpaceStream(Stream sink)
        {
            _sink = sink;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            _sink.Flush();
        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position
        {
            get; set;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _sink.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _sink.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _sink.SetLength(value);
        }

        public override void Close()
        {
            _sink.Close();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            var text = Encoding.Default.GetString(buffer);

            text = _regex.Replace(text, string.Empty);

            var outdata = Encoding.Default.GetBytes(text);
            _sink.Write(outdata, 0, outdata.GetLength(0));
        }
    }
}
