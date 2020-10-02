using Solid.Identity.Protocols.Saml2p.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Solid.Identity.Protocols.Saml2p.Factories
{
    internal class XmlReaderFactory : IXmlReaderFactory
    {
        private static readonly XmlReaderSettings _settings = new XmlReaderSettings
        {
            Async = true
        };

        public XmlReader CreateXmlReader(Stream stream) => XmlReader.Create(stream, _settings);

        public XmlReader CreateXmlReader(TextReader reader) => XmlReader.Create(reader, _settings);
    }
}
