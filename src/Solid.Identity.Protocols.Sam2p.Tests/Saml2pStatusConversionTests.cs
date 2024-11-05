using Solid.Identity.Protocols.Saml2p;
using Solid.Identity.Protocols.Saml2p.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Solid.Identity.Protocols.Saml2p.Tests
{
    public class Saml2pStatusConversionTests
    {
        [Theory]
        [InlineData(SamlResponseStatus.Success, Saml2pConstants.Statuses.SuccessString)]
        [InlineData(SamlResponseStatus.Requester, Saml2pConstants.Statuses.RequesterString)]
        [InlineData(SamlResponseStatus.Responder, Saml2pConstants.Statuses.ResponderString)]
        [InlineData(SamlResponseStatus.VersionMismatch, Saml2pConstants.Statuses.VersionMismatchString)]
        [InlineData(SamlResponseStatus.AuthnFailed, Saml2pConstants.Statuses.AuthnFailedString)]
        [InlineData(SamlResponseStatus.InvalidAttrNameOrValue, Saml2pConstants.Statuses.InvalidAttrNameOrValueString)]
        [InlineData(SamlResponseStatus.InvalidNameIDPolicy, Saml2pConstants.Statuses.InvalidNameIDPolicyString)]
        [InlineData(SamlResponseStatus.NoAuthnContext, Saml2pConstants.Statuses.NoAuthnContextString)]
        [InlineData(SamlResponseStatus.NoAvailableIDP, Saml2pConstants.Statuses.NoAvailableIDPString)]
        [InlineData(SamlResponseStatus.NoPassive, Saml2pConstants.Statuses.NoPassiveString)]
        [InlineData(SamlResponseStatus.NoSupportedIDP, Saml2pConstants.Statuses.NoSupportedIDPString)]
        [InlineData(SamlResponseStatus.PartialLogout, Saml2pConstants.Statuses.PartialLogoutString)]
        [InlineData(SamlResponseStatus.ProxyCountExceeded, Saml2pConstants.Statuses.ProxyCountExceededString)]
        [InlineData(SamlResponseStatus.RequestDenied, Saml2pConstants.Statuses.RequestDeniedString)]
        [InlineData(SamlResponseStatus.RequestUnsupported, Saml2pConstants.Statuses.RequestUnsupportedString)]
        [InlineData(SamlResponseStatus.RequestVersionDeprecated, Saml2pConstants.Statuses.RequestVersionDeprecatedString)]
        [InlineData(SamlResponseStatus.RequestVersionTooHigh, Saml2pConstants.Statuses.RequestVersionTooHighString)]
        [InlineData(SamlResponseStatus.RequestVersionTooLow, Saml2pConstants.Statuses.RequestVersionTooLowString)]
        [InlineData(SamlResponseStatus.ResourceNotRecognized, Saml2pConstants.Statuses.ResourceNotRecognizedString)]
        [InlineData(SamlResponseStatus.TooManyResponses, Saml2pConstants.Statuses.TooManyResponsesString)]
        [InlineData(SamlResponseStatus.UnknownAttrProfile, Saml2pConstants.Statuses.UnknownAttrProfileString)]
        [InlineData(SamlResponseStatus.UnknownPrincipal, Saml2pConstants.Statuses.UnknownPrincipalString)]
        [InlineData(SamlResponseStatus.UnsupportedBinding, Saml2pConstants.Statuses.UnsupportedBindingString)]
        public void ShouldConvertToString(SamlResponseStatus status, string expected)
        {
            var str = status.ToStatusString();
            Assert.Equal(expected, str);
        }

        [Theory]
        [InlineData(SamlResponseStatus.Success, Saml2pConstants.Statuses.SuccessString)]
        [InlineData(SamlResponseStatus.Requester, Saml2pConstants.Statuses.RequesterString)]
        [InlineData(SamlResponseStatus.Responder, Saml2pConstants.Statuses.ResponderString)]
        [InlineData(SamlResponseStatus.VersionMismatch, Saml2pConstants.Statuses.VersionMismatchString)]
        [InlineData(SamlResponseStatus.AuthnFailed, Saml2pConstants.Statuses.AuthnFailedString)]
        [InlineData(SamlResponseStatus.InvalidAttrNameOrValue, Saml2pConstants.Statuses.InvalidAttrNameOrValueString)]
        [InlineData(SamlResponseStatus.InvalidNameIDPolicy, Saml2pConstants.Statuses.InvalidNameIDPolicyString)]
        [InlineData(SamlResponseStatus.NoAuthnContext, Saml2pConstants.Statuses.NoAuthnContextString)]
        [InlineData(SamlResponseStatus.NoAvailableIDP, Saml2pConstants.Statuses.NoAvailableIDPString)]
        [InlineData(SamlResponseStatus.NoPassive, Saml2pConstants.Statuses.NoPassiveString)]
        [InlineData(SamlResponseStatus.NoSupportedIDP, Saml2pConstants.Statuses.NoSupportedIDPString)]
        [InlineData(SamlResponseStatus.PartialLogout, Saml2pConstants.Statuses.PartialLogoutString)]
        [InlineData(SamlResponseStatus.ProxyCountExceeded, Saml2pConstants.Statuses.ProxyCountExceededString)]
        [InlineData(SamlResponseStatus.RequestDenied, Saml2pConstants.Statuses.RequestDeniedString)]
        [InlineData(SamlResponseStatus.RequestUnsupported, Saml2pConstants.Statuses.RequestUnsupportedString)]
        [InlineData(SamlResponseStatus.RequestVersionDeprecated, Saml2pConstants.Statuses.RequestVersionDeprecatedString)]
        [InlineData(SamlResponseStatus.RequestVersionTooHigh, Saml2pConstants.Statuses.RequestVersionTooHighString)]
        [InlineData(SamlResponseStatus.RequestVersionTooLow, Saml2pConstants.Statuses.RequestVersionTooLowString)]
        [InlineData(SamlResponseStatus.ResourceNotRecognized, Saml2pConstants.Statuses.ResourceNotRecognizedString)]
        [InlineData(SamlResponseStatus.TooManyResponses, Saml2pConstants.Statuses.TooManyResponsesString)]
        [InlineData(SamlResponseStatus.UnknownAttrProfile, Saml2pConstants.Statuses.UnknownAttrProfileString)]
        [InlineData(SamlResponseStatus.UnknownPrincipal, Saml2pConstants.Statuses.UnknownPrincipalString)]
        [InlineData(SamlResponseStatus.UnsupportedBinding, Saml2pConstants.Statuses.UnsupportedBindingString)]
        public void ShouldConvertToUri(SamlResponseStatus status, string expected)
        {
            var uri = status.ToStatusUri();
            Assert.Equal(new Uri(expected), uri);
        }
    }
}
