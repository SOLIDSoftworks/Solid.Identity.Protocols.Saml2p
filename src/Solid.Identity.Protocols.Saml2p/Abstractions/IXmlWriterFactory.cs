using System;
using System.IO;
using System.Xml;

namespace Solid.Identity.Protocols.Saml2p.Abstractions
{
    /// <summary>
    /// An interface describing a factory for creating <see cref="XmlWriter"/>s.
    /// </summary>
    public interface IXmlWriterFactory
    {
        /// <summary>
        /// Creates an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write the XML to.</param>
        /// <returns>An <see cref="XmlWriter"/> instance.</returns>
        XmlWriter CreateXmlWriter(Stream stream);

        /// <summary>
        /// Creates an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write the XML to.</param>
        /// <returns>An <see cref="XmlWriter"/> instance.</returns>
        XmlWriter CreateXmlWriter(TextWriter writer);
    }
}
