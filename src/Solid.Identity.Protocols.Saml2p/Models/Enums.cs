using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public enum BindingType
    {
        Post,
        //Artifact,
        Redirect,
        //Soap
    }

    public enum SamlResponseStatus
    {
        Success,
        Requester,
        Responder,
        VersionMismatch,
        AuthnFailed,
        InvalidAttrNameOrValue,
        InvalidNameIDPolicy,
        NoAuthnContext,
        NoAvailableIDP,
        NoPassive,
        NoSupportedIDP,
        PartialLogout,
        ProxyCountExceeded,
        RequestDenied,
        RequestUnsupported,
        RequestVersionDeprecated,
        RequestVersionTooHigh,
        RequestVersionTooLow,
        ResourceNotRecognized,
        TooManyResponses,
        UnknownAttrProfile,
        UnknownPrincipal,
        UnsupportedBinding
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}