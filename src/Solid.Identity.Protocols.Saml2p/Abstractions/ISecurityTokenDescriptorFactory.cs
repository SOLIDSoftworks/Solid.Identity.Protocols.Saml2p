﻿using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Abstractions
{
    public interface ISecurityTokenDescriptorFactory
    {
        ValueTask<SecurityTokenDescriptor> CreateSecurityTokenDescriptorAsync(ClaimsIdentity identity, ISaml2pServiceProvider partner);
    }
}
