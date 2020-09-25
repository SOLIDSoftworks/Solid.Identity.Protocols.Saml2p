using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Abstractions.Configuration;
using Solid.Identity.Protocols.Saml2p.Configuration;
using Solid.Identity.Protocols.Saml2p.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Abstractions.Factories
{
    public interface ISecurityTokenDescriptorFactory
    {
        SecurityTokenDescriptor CreateSecurityTokenDescriptor(ClaimsIdentity identity, PartnerSaml2pServiceProvider partner);
    }
}
