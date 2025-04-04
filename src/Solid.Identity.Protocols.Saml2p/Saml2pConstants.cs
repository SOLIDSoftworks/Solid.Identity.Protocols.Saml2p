using Solid.Identity.Protocols.Saml2p.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace Solid.Identity.Protocols.Saml2p
{
    /// <summary>
    /// Constants used for Saml2p.
    /// </summary>
    public static class Saml2pConstants
    {
        /// <summary>
        /// The name of the token that is stored in <see cref="AuthenticationProperties"/>.
        /// </summary>
        public const string TokenName = "saml2";
        
        internal const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static class Namespaces
        {
            public const string ProtocolPrefix = "samlp";
            public const string ProtocolNamespace = "urn:oasis:names:tc:SAML:2.0:protocol";
            public const string StatusNamespace = "urn:oasis:names:tc:SAML:2.0:status";
            public const string BindingNamespace = "urn:oasis:names:tc:SAML:2.0:bindings";
            public const string ClassNamespace = "urn:oasis:names:tc:SAML:2.0:ac:classes";
        }

        public static class Statuses
        {
            public const string SuccessString = Namespaces.StatusNamespace + ":Success";
            public const string RequesterString = Namespaces.StatusNamespace + ":Requester";
            public const string ResponderString = Namespaces.StatusNamespace + ":Responder";
            public const string VersionMismatchString = Namespaces.StatusNamespace + ":VersionMismatch";
            public const string AuthnFailedString = Namespaces.StatusNamespace + ":AuthnFailed";
            public const string InvalidAttrNameOrValueString = Namespaces.StatusNamespace + ":InvalidAttrNameOrValue";
            public const string InvalidNameIDPolicyString = Namespaces.StatusNamespace + ":InvalidNameIDPolicy";
            public const string NoAuthnContextString = Namespaces.StatusNamespace + ":NoAuthnContext";
            public const string NoAvailableIDPString = Namespaces.StatusNamespace + ":NoAvailableIDP";
            public const string NoPassiveString = Namespaces.StatusNamespace + ":NoPassive";
            public const string NoSupportedIDPString = Namespaces.StatusNamespace + ":NoSupportedIDP";
            public const string PartialLogoutString = Namespaces.StatusNamespace + ":PartialLogout";
            public const string ProxyCountExceededString = Namespaces.StatusNamespace + ":ProxyCountExceeded";
            public const string RequestDeniedString = Namespaces.StatusNamespace + ":RequestDenied";
            public const string RequestUnsupportedString = Namespaces.StatusNamespace + ":RequestUnsupported";
            public const string RequestVersionDeprecatedString = Namespaces.StatusNamespace + ":RequestVersionDeprecated";
            public const string RequestVersionTooHighString = Namespaces.StatusNamespace + ":RequestVersionTooHigh";
            public const string RequestVersionTooLowString = Namespaces.StatusNamespace + ":RequestVersionTooLow";
            public const string ResourceNotRecognizedString = Namespaces.StatusNamespace + ":ResourceNotRecognized";
            public const string TooManyResponsesString = Namespaces.StatusNamespace + ":TooManyResponses";
            public const string UnknownAttrProfileString = Namespaces.StatusNamespace + ":UnknownAttrProfile";
            public const string UnknownPrincipalString = Namespaces.StatusNamespace + ":UnknownPrincipal";
            public const string UnsupportedBindingString = Namespaces.StatusNamespace + ":UnsupportedBinding";

            public static readonly Uri Success = new Uri(SuccessString);
            public static readonly Uri Requester = new Uri(RequesterString);
            public static readonly Uri Responder = new Uri(ResponderString);
            public static readonly Uri VersionMismatch = new Uri(VersionMismatchString);
            public static readonly Uri AuthnFailed = new Uri(AuthnFailedString);
            public static readonly Uri InvalidAttrNameOrValue = new Uri(InvalidAttrNameOrValueString);
            public static readonly Uri InvalidNameIDPolicy = new Uri(InvalidNameIDPolicyString);
            public static readonly Uri NoAuthnContext = new Uri(NoAuthnContextString);
            public static readonly Uri NoAvailableIDP = new Uri(NoAvailableIDPString);
            public static readonly Uri NoPassive = new Uri(NoPassiveString);
            public static readonly Uri NoSupportedIDP = new Uri(NoSupportedIDPString);
            public static readonly Uri PartialLogout = new Uri(PartialLogoutString);
            public static readonly Uri ProxyCountExceeded = new Uri(ProxyCountExceededString);
            public static readonly Uri RequestDenied = new Uri(RequestDeniedString);
            public static readonly Uri RequestUnsupported = new Uri(RequestUnsupportedString);
            public static readonly Uri RequestVersionDeprecated = new Uri(RequestVersionDeprecatedString);
            public static readonly Uri RequestVersionTooHigh = new Uri(RequestVersionTooHighString);
            public static readonly Uri RequestVersionTooLow = new Uri(RequestVersionTooLowString);
            public static readonly Uri ResourceNotRecognized = new Uri(ResourceNotRecognizedString);
            public static readonly Uri TooManyResponses = new Uri(TooManyResponsesString);
            public static readonly Uri UnknownAttrProfile = new Uri(UnknownAttrProfileString);
            public static readonly Uri UnknownPrincipal = new Uri(UnknownPrincipalString);
            public static readonly Uri UnsupportedBinding = new Uri(UnsupportedBindingString);
        }

        // https://docs.oasis-open.org/security/saml/v2.0/saml-authn-context-2.0-os.pdf
        public static class Classes
        {
            /*
             * InternetProtocol (urn:oasis:names:tc:SAML:2.0:ac:classes:InternetProtocol)
             * InternetProtocolPassword (urn:oasis:names:tc:SAML:2.0:ac:classes:InternetProtocolPassword)
             * Kerberos (urn:oasis:names:tc:SAML:2.0:ac:classes:Kerberos)
             * MobileOneFactorUnregistered (urn:oasis:names:tc:SAML:2.0:ac:classes:MobileOneFactorUnregistered)
             * MobileTwoFactorUnregistered (urn:oasis:names:tc:SAML:2.0:ac:classes:MobileTwoFactorUnregistered)
             * MobileOneFactorContract (urn:oasis:names:tc:SAML:2.0:ac:classes:MobileOneFactorContract)
             * MobileTwoFactorContract (urn:oasis:names:tc:SAML:2.0:ac:classes:MobileTwoFactorContract)
             * Password (urn:oasis:names:tc:SAML:2.0:ac:classes:Password)
             * PasswordProtectedTransport (urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport)
             * PreviousSession (urn:oasis:names:tc:SAML:2.0:ac:classes:PreviousSession)
             * Public Key – X.509 (urn:oasis:names:tc:SAML:2.0:ac:classes:X509)
             * Public Key – PGP (urn:oasis:names:tc:SAML:2.0:ac:classes:PGP)
             * Public Key – SPKI (urn:oasis:names:tc:SAML:2.0:ac:classes:SPKI)
             * Public Key - XML Digital Signature (urn:oasis:names:tc:SAML:2.0:ac:classes:XMLDSig)
             * Smartcard (urn:oasis:names:tc:SAML:2.0:ac:classes:Smartcard)
             * SmartcardPKI (urn:oasis:names:tc:SAML:2.0:ac:classes:SmartcardPKI)
             * SoftwarePKI (urn:oasis:names:tc:SAML:2.0:ac:classes:SoftwarePKI)
             * Telephony (urn:oasis:names:tc:SAML:2.0:ac:classes:Telephony)
             * Telephony ("Nomadic") (urn:oasis:names:tc:SAML:2.0:ac:classes:NomadTelephony)
             * Telephony (Personalized) (urn:oasis:names:tc:SAML:2.0:ac:classes:PersonalTelephony)
             * Telephony (Authenticated) (urn:oasis:names:tc:SAML:2.0:ac:classes:AuthenticatedTelephony)
             * Secure Remote Password (urn:oasis:names:tc:SAML:2.0:ac:classes:SecureRemotePassword)
             * SSL/TLS Certificate-Based Client Authentication (urn:oasis:names:tc:SAML:2.0:ac:classes:TLSClient)
             * TimeSyncToken (urn:oasis:names:tc:SAML:2.0:ac:classes:TimeSyncToken)
             * Unspecified (urn:oasis:names:tc:SAML:2.0:ac:classes:unspecified)
             */

            public const string UnspecifiedString = Namespaces.ClassNamespace + ":unspecified";
            public const string PasswordString = Namespaces.ClassNamespace + ":Password";
            public const string KerberosString = Namespaces.ClassNamespace + ":Kerberos";

            public static readonly Uri Unspecified = new Uri(UnspecifiedString);
            public static readonly Uri Password = new Uri(PasswordString);
            public static readonly Uri Kerberos = new Uri(KerberosString);
        }

        public static class Attributes
        {
            public const string Comparison = nameof(Comparison);
            public const string AllowCreate = nameof(AllowCreate);
            public const string IsPassive = nameof(IsPassive);
            public const string ForceAuthn = nameof(ForceAuthn);
            public const string ProtocolBinding = nameof(ProtocolBinding);
            public const string AssertionConsumerServiceUrl = "AssertionConsumerServiceURL";
            public const string Destination = nameof(Destination);
            public const string ProviderName = nameof(ProviderName);
            public const string Value = nameof(Value);
        }

        public static class Elements
        {
            public const string RequestedAuthnContext = nameof(RequestedAuthnContext);
            public const string AuthnRequest = nameof(AuthnRequest);
            public const string Response = nameof(Response);
            public const string Status = nameof(Status);
            public const string StatusCode = nameof(StatusCode);
            public const string NameIdPolicy = "NameIDPolicy";
        }

        public static class Bindings
        {
            public const string Post = Namespaces.BindingNamespace + ":HTTP-POST";
            //public const string Artifact = Namespaces.BindingNamespace + ":HTTP-Artifact";
            public const string Redirect = Namespaces.BindingNamespace + ":HTTP-Redirect";
            //public const string Soap = Namespaces.BindingNamespace + ":SOAP";

            public static ICollection<BindingType> All => new List<BindingType>
            {
                BindingType.Redirect,
                BindingType.Post
            };
        }

        public static class Tracing
        {
            private static readonly string AssemblyVersion = GenerateAssemblyVersion();
            public static ActivitySource Saml2p { get; } = new (Names.Saml2p, AssemblyVersion);
            public static ActivitySource Cache { get; } = new (Names.Cache, AssemblyVersion);
            public static ActivitySource Factories { get; } = new (Names.Factories, AssemblyVersion);
            public static ActivitySource Providers { get; } = new (Names.Providers, AssemblyVersion);
            public static ActivitySource Validation { get; } = new (Names.Validation, AssemblyVersion);
            public static class Names
            {
                public const string Saml2p = "Solid.Identity.Protocols.Saml2p";
                public const string Cache = Saml2p + ".Cache";
                public const string Factories = Saml2p + ".Factories";
                public const string Providers = Saml2p + ".Providers";
                public const string Validation = Saml2p + ".Validation";
            }

            private static string GenerateAssemblyVersion()
            {
                var version = typeof(Saml2pConstants).Assembly.GetName().Version;
                return version == null ? "0.0.0" : $"{version.Major}.{version.Minor}.{version.Build}";
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
