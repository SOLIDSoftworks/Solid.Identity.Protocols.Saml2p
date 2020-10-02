﻿using System;
using System.IO;
using System.Xml;
using Solid.Identity.Protocols.Saml2p.Abstractions;

namespace Solid.Identity.Protocols.Saml2p.Factories
{
    internal class XmlWriterFactory : IXmlWriterFactory
    {
        private static readonly XmlWriterSettings _settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Async = true
        };

        public XmlWriter CreateXmlWriter(Stream stream) => XmlWriter.Create(stream, _settings);

        public XmlWriter CreateXmlWriter(TextWriter writer) => XmlWriter.Create(writer, _settings);
    }
}
