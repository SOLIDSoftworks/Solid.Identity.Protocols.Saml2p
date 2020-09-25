using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Protocol
{
    public class RequestedAuthnContext
    {
        public Comparison Comparison { get; set; } = Comparison.Exact;
        public Uri AuthnContextClassRef { get; set; }
        public string AuthnContextDeclRef { get; set; }
    }
    public enum Comparison
    {
        Exact,
        Minimum,
        Maximum,
        Better
    }
}
