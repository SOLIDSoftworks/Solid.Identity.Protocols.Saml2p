using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Solid.Identity.Protocols.Saml2p.Abstractions;

namespace Solid.Identity.Protocols.Saml2p.Factories
{
    public class SamlResponseFactory
    {
        private Saml2pOptions _options;

        public SamlResponseFactory(IOptions<Saml2pOptions> options)
        {
            _options = options.Value;
        }

        public SamlResponse Create(ISaml2pServiceProvider partner, string authnRequestId = null, string relayState = null, SamlResponseStatus status = SamlResponseStatus.Success, SamlResponseStatus? subStatus = null, Saml2SecurityToken token = null)
        {
            var destination = new Uri(partner.BaseUrl, partner.AssertionConsumerServiceEndpoint);
            if (authnRequestId != null)
                token.SetRecipient(destination, authnRequestId);
            else
                token.SetRecipient(destination);
            token.SetNotOnOrAfter();

            var response = new SamlResponse
            {
                Id = $"_{Guid.NewGuid()}", // TODO: create id factory
                SecurityToken = token, 
                Destination = destination,
                IssueInstant = token?.Assertion.IssueInstant,
                Issuer = partner.ExpectedIssuer ?? _options.DefaultIssuer,
                Status = Convert(status, subStatus),
                InResponseTo = authnRequestId,
                RelayState = relayState
            };

            return response;
        }

        private Status Convert(SamlResponseStatus status, SamlResponseStatus? subStatus)
        {
            var converted = new Status
            {
                StatusCode = new StatusCode
                {
                    Value = Convert(status)
                }
            };

            return converted;
        }

        private Uri Convert(SamlResponseStatus? status)
        { 
            switch(status)
            {
                case SamlResponseStatus.Success: return Saml2pConstants.Statuses.Success;
                case SamlResponseStatus.AuthnFailed: return Saml2pConstants.Statuses.AuthnFailed;
            }
            return null;
            //"urn:oasis:names:tc:SAML:2.0:status:Success"
            //"urn:oasis:names:tc:SAML:2.0:status:Requester"
            //"urn:oasis:names:tc:SAML:2.0:status:Responder"
            //"urn:oasis:names:tc:SAML:2.0:status:VersionMismatch"
            //"urn:oasis:names:tc:SAML:2.0:status:AuthnFailed"
            //"urn:oasis:names:tc:SAML:2.0:status:InvalidAttrNameOrValue"
            //"urn:oasis:names:tc:SAML:2.0:status:InvalidNameIDPolicy"
            //"urn:oasis:names:tc:SAML:2.0:status:NoAuthnContext"
            //"urn:oasis:names:tc:SAML:2.0:status:NoAvailableIDP"
            //"urn:oasis:names:tc:SAML:2.0:status:NoPassive"
            //"urn:oasis:names:tc:SAML:2.0:status:NoSupportedIDP"
            //"urn:oasis:names:tc:SAML:2.0:status:PartialLogout"
            //"urn:oasis:names:tc:SAML:2.0:status:ProxyCountExceeded"
            //"urn:oasis:names:tc:SAML:2.0:status:RequestDenied"
            //"urn:oasis:names:tc:SAML:2.0:status:RequestUnsupported"
            //"urn:oasis:names:tc:SAML:2.0:status:RequestVersionDeprecated"
            //"urn:oasis:names:tc:SAML:2.0:status:RequestVersionTooHigh"
            //"urn:oasis:names:tc:SAML:2.0:status:RequestVersionTooLow"
            //"urn:oasis:names:tc:SAML:2.0:status:ResourceNotRecognized"
            //"urn:oasis:names:tc:SAML:2.0:status:TooManyResponses"
            //"urn:oasis:names:tc:SAML:2.0:status:UnknownAttrProfile"
            //"urn:oasis:names:tc:SAML:2.0:status:UnknownPrincipal"
            //"urn:oasis:names:tc:SAML:2.0:status:UnsupportedBinding"
        }
    }
}
