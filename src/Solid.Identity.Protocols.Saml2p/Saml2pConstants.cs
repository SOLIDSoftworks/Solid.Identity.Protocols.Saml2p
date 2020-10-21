using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p
{
    public static class Saml2pConstants
    {
        internal const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        public static class Namespaces
        {
            public const string ProtocolPrefix = "samlp";
            public const string ProtocolNamespace = "urn:oasis:names:tc:SAML:2.0:protocol";
            public const string StatusNamespace = "urn:oasis:names:tc:SAML:2.0:status";
            public const string BindingNamespace = "urn:oasis:names:tc:SAML:2.0:bindings";
            public const string ClassNamespace = "urn:oasis:names:tc:SAML:2.0:ac:classes";
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

        public static class Status
        {
            public const string Success = Namespaces.StatusNamespace + ":" + nameof(Success);
        }

        public static class Bindings
        {
            public const string Post = Namespaces.BindingNamespace + ":HTTP-POST";
            //public const string Artifact = Namespaces.BindingNamespace + ":HTTP-Artifact";
            public const string Redirect = Namespaces.BindingNamespace + ":HTTP-Redirect";
            //public const string Soap = Namespaces.BindingNamespace + ":SOAP";

            public static ICollection<string> All => new List<string>
            {
                Redirect,
                Post
            };
        }
    }
}
