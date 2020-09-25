using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Configuration
{
    public class Saml2pServiceProvider : Saml2pProvider
    {
        public string Id { get; set; }
        public Uri AssertionConsumerServiceUrl { get; set; }
    }
}
