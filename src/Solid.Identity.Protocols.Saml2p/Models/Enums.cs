using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models
{
    public enum BindingType
    {
        Post,
        //Artifact,
        Redirect,
        //Soap
    }

    public enum SamlResponseStatus
    {
        Success
    }
}
