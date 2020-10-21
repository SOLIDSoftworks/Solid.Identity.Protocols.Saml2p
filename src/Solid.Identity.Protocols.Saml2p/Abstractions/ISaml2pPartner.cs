using Solid.Identity.Protocols.Saml2p.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Abstractions
{
    public interface ISaml2pPartner
    {
        string Id { get; }
        string ExpectedIssuer { get; }
        string Name { get; }
        Uri BaseUrl { get; }
        bool Enabled { get; }
        bool CanInitiateSso { get; }
        ICollection<BindingType> SupportedBindings { get; }
    }
}
