using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    public class Saml2pProvider
    {
        public TimeSpan MaxClockSkew { get; set; } = new TimeSpan(5);
        public string Binding { get; set; } = Saml2pConstants.Bindings.Post;
        public bool Enabled { get; set; } = true;
    }
}
