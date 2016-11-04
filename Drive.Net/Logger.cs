using GoogleException = Google.GoogleApiException;
using System;

namespace DriveNET
{
    class Logger : IDisposable
    {
        private XmlProcessor Xml;

        public Logger()
        {
            Xml = new XmlProcessor();
        }

        public void Log(Exception exception)
        {
            Console.WriteLine(exception.ToString());

            if (exception is GoogleException)
            {
                var googleException = (GoogleException)exception;

                Console.WriteLine("Error (" + googleException.Error.Message + ")");
                Xml.AddLog(googleException.Error.Message);
            }
            else
            {
                Console.WriteLine("Error (" + exception.Message + ")");
                Xml.AddLog(exception.Message);
            }
        }

        public void Log(Exception exception, string Input)
        {
            if (exception is GoogleException)
            {
                var googleException = (GoogleException)exception;

                Console.WriteLine("Error (" + googleException.Error.Message + ")");
                Xml.AddLog(googleException.Error.Message, Input);
            }
            else
            {
                Console.WriteLine("Error (" + exception.Message + ")");
                Xml.AddLog(exception.Message, Input);
            }
        }

        public void Dispose()
        {
            Xml.Dispose();
        }
    }
}