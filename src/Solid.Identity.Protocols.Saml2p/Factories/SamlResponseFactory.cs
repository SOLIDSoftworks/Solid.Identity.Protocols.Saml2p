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
    internal class SamlResponseFactory
    {
        private Saml2pOptions _options;

        public SamlResponseFactory(IOptions<Saml2pOptions> options)
        {
            _options = options.Value;
        }

        public SamlResponse Create(ISaml2pServiceProvider partner, string authnRequestId = null, string relayState = null, SamlResponseStatus status = SamlResponseStatus.Success, SamlResponseStatus? subStatus = null, Saml2SecurityToken token = null)
        {
            var destination = new Uri(partner.BaseUrl, partner.AssertionConsumerServiceEndpoint);
            if (token != null)
            {
                if (authnRequestId != null)
                    token.SetRecipient(destination, authnRequestId);
                else
                    token.SetRecipient(destination);
                token.SetNotOnOrAfter();
            }

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

            var sub = Convert(subStatus);
            if (sub != null)
                converted.StatusCode.SubCode = new StatusCode
                {
                    Value = sub
                };

            return converted;
        }

        private Uri Convert(SamlResponseStatus? status)
        {
            if (status.HasValue) return status.Value.ToStatusUri();
            return null;
        }
    }
}
