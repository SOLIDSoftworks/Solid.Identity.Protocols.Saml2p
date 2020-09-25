using Solid.Identity.Protocols.Saml2p.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Abstractions.Configuration
{
    public interface IBinding
    {
        Uri Endpoint { get; }
        BindingType Type { get; }
    }
}
