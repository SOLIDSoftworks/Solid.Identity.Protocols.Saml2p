using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    public class Saml2pProvider
    {
        public string Id { get; internal set; }
        public string Name { get; set; }
        public TimeSpan? MaxClockSkew { get; set; }
        public ICollection<string> Bindings { get; } = new List<string> {Saml2pConstants.Bindings.Post };
        public bool Enabled { get; set; } = true;
        public bool CanInitiateSso { get; set; } = true;
    }
}
