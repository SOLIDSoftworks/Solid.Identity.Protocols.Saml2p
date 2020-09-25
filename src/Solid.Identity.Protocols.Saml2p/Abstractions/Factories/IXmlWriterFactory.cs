using System;
using System.IO;
using System.Xml;

namespace Solid.Identity.Protocols.Saml2p.Abstractions.Factories
{
    public interface IXmlWriterFactory
    {
        XmlWriter CreateXmlWriter(Stream stream);
        XmlWriter CreateXmlWriter(TextWriter writer);
    }
}
