using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Solid.Identity.Protocols.Saml2p.Abstractions
{
    /// <summary>
    /// An interface describing a factory for creating <see cref="XmlReader"/>s.
    /// </summary>
    public interface IXmlReaderFactory
    {
        /// <summary>
        /// Creates an <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read the XML from.</param>
        /// <returns>An <see cref="XmlReader"/> instance.</returns>
        XmlReader CreateXmlReader(Stream stream);

        /// <summary>
        /// Creates an <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader"/> to read the XML from.</param>
        /// <returns>An <see cref="XmlReader"/> instance.</returns>
        XmlReader CreateXmlReader(TextReader reader);
    }
}
