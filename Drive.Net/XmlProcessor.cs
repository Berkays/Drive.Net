using System;
using System.Xml.Linq;

namespace DriveNET
{
    internal class XmlProcessor : IDisposable
    {
        private int counter = 0;
        private XDocument doc;
        private string path;

        public XmlProcessor()
        {
            //Get path
            path = string.Format("{0}\\log-{1}.xml", Environment.CurrentDirectory, DateTime.Now.ToShortDateString());
            try
            {
                //If file is not found create file
                LoadXml();
            }
            catch
            {
                CreateXml();
            }
        }

        /// <summary>
        /// Insert a node in to the xml file
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Input"></param>
        public void AddLog(string Message, string Input = "")
        {
            if (doc == null)
                return;

            XElement Node = new XElement("Error");
            XElement Date = new XElement("Date") { Value = DateTime.Now.ToLongTimeString() };
            XElement _Message = new XElement("Message") { Value = Message };

            Node.Add(Date);
            Node.Add(_Message);

            //If Input is empty dont create this node
            if (!string.IsNullOrEmpty(Input))
            {
                XElement _Input = new XElement("Input") { Value = Input };
                Node.Add(Input);
            }

            doc.Root.Add(Node);

            counter++;
            if (counter <= 25)
                return;

            doc.Save(path);
        }

        private void CreateXml()
        {
            XElement xelement = new XElement("root");
            xelement.Add(new XAttribute("Date", DateTime.Now.ToString()));
            doc = new XDocument(xelement);

            doc.Save(path);
        }

        private void LoadXml()
        {
            doc = XDocument.Load(path);
        }

        public void Dispose()
        {
            doc.Save(path);
        }
    }
}