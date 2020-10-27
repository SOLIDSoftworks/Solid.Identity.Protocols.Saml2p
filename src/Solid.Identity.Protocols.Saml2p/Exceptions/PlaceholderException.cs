#if DEBUG
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Exceptions
{
    internal class PlaceholderException : Exception
    {
        public PlaceholderException() : base("Replace this exception") { }
    }
}
#endif
