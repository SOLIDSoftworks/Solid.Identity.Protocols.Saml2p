using System;
using System.Collections.Generic;
using System.Text;

namespace System.Xml
{
    internal static class XmlWriterExtensions
    {
        public static IDisposable WriteElement(this XmlWriter writer, string localName, string ns) => new SelfClosingXmlElement(writer, localName, ns);
        public static IDisposable WriteElement(this XmlWriter writer, string localName) => new SelfClosingXmlElement(writer, localName);
        public static IDisposable WriteElement(this XmlWriter writer, string prefix, string localName, string ns) => new SelfClosingXmlElement(writer, prefix, localName, ns);
        
        class SelfClosingXmlElement : IDisposable
        {
            private XmlWriter _writer;

            public SelfClosingXmlElement(XmlWriter writer, string localName, string ns) : this(writer) => writer.WriteStartElement(localName, ns);
            public SelfClosingXmlElement(XmlWriter writer, string localName) : this(writer) => writer.WriteStartElement(localName);
            public SelfClosingXmlElement(XmlWriter writer, string prefix, string localName, string ns) : this(writer) => writer.WriteStartElement(prefix, localName, ns);

            private SelfClosingXmlElement(XmlWriter writer)
            {
                _writer = writer;
            }

            public void Dispose()
            {
                _writer.WriteEndElement();
            }
        }
    }
}
