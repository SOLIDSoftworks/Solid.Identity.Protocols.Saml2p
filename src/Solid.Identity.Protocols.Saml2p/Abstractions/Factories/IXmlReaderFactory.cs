using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Solid.Identity.Protocols.Saml2p.Abstractions.Factories
{
    public interface IXmlReaderFactory
    {
        XmlReader CreateXmlReader(Stream stream);
        XmlReader CreateXmlReader(TextReader reader);
    }
}
