using Serilog.Debugging;
using Serilog.Sinks.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JimJiangApp.Commons
{
    internal class LogFileLifecycleHooks : FileLifecycleHooks
    {
        private const int DEFAULT_BUFFER_SIZE = 1024;

        private readonly string _header;
        private string _lastFilePath;
        private readonly Action<string> _fileNameChanged;

        public LogFileLifecycleHooks(Action<string> fileNameChanged, string header)
        {
            _fileNameChanged = fileNameChanged;
            _header = header;
        }

        public override Stream OnFileOpened(string path, Stream underlyingStream, Encoding encoding)
        {
            if (_lastFilePath != path)
            {
                _lastFilePath = path;
                _fileNameChanged?.Invoke(_lastFilePath);
            }
            try
            {
                if (underlyingStream.Length != 0L)
                {
                    SelfLog.WriteLine($"File header will not be written, as the stream already contains {underlyingStream.Length} bytes of content");
                    return base.OnFileOpened(underlyingStream, encoding);
                }
            }
            catch (NotSupportedException)
            {
            }
            using (StreamWriter streamWriter = new StreamWriter(underlyingStream, encoding, DEFAULT_BUFFER_SIZE, leaveOpen: true))
            {
                streamWriter.WriteLine(_header);
                streamWriter.Flush();
            }
            return base.OnFileOpened(path, underlyingStream, encoding);
        }
    }
}