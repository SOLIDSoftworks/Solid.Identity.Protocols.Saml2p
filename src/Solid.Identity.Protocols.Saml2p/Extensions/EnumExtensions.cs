using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models
{
    internal static class EnumExtensions
    {
        public static string ToProtocolBindingString(this BindingType bindingType)
        {
            switch(bindingType)
            {
                case BindingType.Post: return Saml2pConstants.Bindings.Post;
                case BindingType.Redirect: return Saml2pConstants.Bindings.Redirect;
            }

            throw new ArgumentException($"Unsupported binding type: {bindingType}");
        }

        public static Status ToStatus(this SamlResponseStatus status, SamlResponseStatus? subStatus = null)
        {
            var s = new Status
            {
                StatusCode = new StatusCode
                {
                    Value = status.ToStatusUri()
                }
            };
            if (subStatus != null)
                s.StatusCode.SubCode = new StatusCode
                {
                    Value = subStatus.Value.ToStatusUri()
                };
            return s;
        }

        public static Uri ToStatusUri(this SamlResponseStatus status)
        {
            switch (status)
            {
                case SamlResponseStatus.Success: return Saml2pConstants.Statuses.Success;
                case SamlResponseStatus.Requester: return Saml2pConstants.Statuses.Requester;
                case SamlResponseStatus.Responder: return Saml2pConstants.Statuses.Responder;
                case SamlResponseStatus.AuthnFailed: return Saml2pConstants.Statuses.AuthnFailed;
                case SamlResponseStatus.VersionMismatch: return Saml2pConstants.Statuses.VersionMismatch;
                case SamlResponseStatus.InvalidAttrNameOrValue: return Saml2pConstants.Statuses.InvalidAttrNameOrValue;
                case SamlResponseStatus.InvalidNameIDPolicy: return Saml2pConstants.Statuses.InvalidNameIDPolicy;
                case SamlResponseStatus.NoAuthnContext: return Saml2pConstants.Statuses.NoAuthnContext;
                case SamlResponseStatus.NoAvailableIDP: return Saml2pConstants.Statuses.NoAvailableIDP;
                case SamlResponseStatus.NoPassive: return Saml2pConstants.Statuses.NoPassive;
                case SamlResponseStatus.NoSupportedIDP: return Saml2pConstants.Statuses.NoSupportedIDP;
                case SamlResponseStatus.PartialLogout: return Saml2pConstants.Statuses.PartialLogout;
                case SamlResponseStatus.ProxyCountExceeded: return Saml2pConstants.Statuses.ProxyCountExceeded;
                case SamlResponseStatus.RequestDenied: return Saml2pConstants.Statuses.RequestDenied;
                case SamlResponseStatus.RequestUnsupported: return Saml2pConstants.Statuses.RequestUnsupported;
                case SamlResponseStatus.RequestVersionDeprecated: return Saml2pConstants.Statuses.RequestVersionDeprecated;
                case SamlResponseStatus.RequestVersionTooHigh: return Saml2pConstants.Statuses.RequestVersionTooHigh;
                case SamlResponseStatus.RequestVersionTooLow: return Saml2pConstants.Statuses.RequestVersionTooLow;
                case SamlResponseStatus.ResourceNotRecognized: return Saml2pConstants.Statuses.ResourceNotRecognized;
                case SamlResponseStatus.TooManyResponses: return Saml2pConstants.Statuses.TooManyResponses;
                case SamlResponseStatus.UnknownAttrProfile: return Saml2pConstants.Statuses.UnknownAttrProfile;
                case SamlResponseStatus.UnknownPrincipal: return Saml2pConstants.Statuses.UnknownPrincipal;
                case SamlResponseStatus.UnsupportedBinding: return Saml2pConstants.Statuses.UnsupportedBinding;
            }
            throw new ArgumentException($"Unknown status: {status}");
        }

        public static string ToStatusString(this SamlResponseStatus status)
        {
            switch (status)
            {
                case SamlResponseStatus.Success: return Saml2pConstants.Statuses.SuccessString;
                case SamlResponseStatus.Requester: return Saml2pConstants.Statuses.RequesterString;
                case SamlResponseStatus.Responder: return Saml2pConstants.Statuses.ResponderString;
                case SamlResponseStatus.AuthnFailed: return Saml2pConstants.Statuses.AuthnFailedString;
                case SamlResponseStatus.VersionMismatch: return Saml2pConstants.Statuses.VersionMismatchString;
                case SamlResponseStatus.InvalidAttrNameOrValue: return Saml2pConstants.Statuses.InvalidAttrNameOrValueString;
                case SamlResponseStatus.InvalidNameIDPolicy: return Saml2pConstants.Statuses.InvalidNameIDPolicyString;
                case SamlResponseStatus.NoAuthnContext: return Saml2pConstants.Statuses.NoAuthnContextString;
                case SamlResponseStatus.NoAvailableIDP: return Saml2pConstants.Statuses.NoAvailableIDPString;
                case SamlResponseStatus.NoPassive: return Saml2pConstants.Statuses.NoPassiveString;
                case SamlResponseStatus.NoSupportedIDP: return Saml2pConstants.Statuses.NoSupportedIDPString;
                case SamlResponseStatus.PartialLogout: return Saml2pConstants.Statuses.PartialLogoutString;
                case SamlResponseStatus.ProxyCountExceeded: return Saml2pConstants.Statuses.ProxyCountExceededString;
                case SamlResponseStatus.RequestDenied: return Saml2pConstants.Statuses.RequestDeniedString;
                case SamlResponseStatus.RequestUnsupported: return Saml2pConstants.Statuses.RequestUnsupportedString;
                case SamlResponseStatus.RequestVersionDeprecated: return Saml2pConstants.Statuses.RequestVersionDeprecatedString;
                case SamlResponseStatus.RequestVersionTooHigh: return Saml2pConstants.Statuses.RequestVersionTooHighString;
                case SamlResponseStatus.RequestVersionTooLow: return Saml2pConstants.Statuses.RequestVersionTooLowString;
                case SamlResponseStatus.ResourceNotRecognized: return Saml2pConstants.Statuses.ResourceNotRecognizedString;
                case SamlResponseStatus.TooManyResponses: return Saml2pConstants.Statuses.TooManyResponsesString;
                case SamlResponseStatus.UnknownAttrProfile: return Saml2pConstants.Statuses.UnknownAttrProfileString;
                case SamlResponseStatus.UnknownPrincipal: return Saml2pConstants.Statuses.UnknownPrincipalString;
                case SamlResponseStatus.UnsupportedBinding: return Saml2pConstants.Statuses.UnsupportedBindingString;
            }
            throw new ArgumentException($"Unknown status: {status}");
        }
    }
}
