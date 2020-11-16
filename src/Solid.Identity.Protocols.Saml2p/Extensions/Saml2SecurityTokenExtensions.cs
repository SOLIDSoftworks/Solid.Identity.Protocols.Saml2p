using Solid.Identity.Tokens.Saml2;
using System;
using System.Linq;
using System.Security;
using System.Security.Claims;

namespace Microsoft.IdentityModel.Tokens.Saml2
{
    internal static class Saml2SecurityTokenExtensions
    {
        public static void SetRecipient(this Saml2SecurityToken token, Uri recipient) => token.SetRecipient(recipient, null as Saml2Id);

        public static void SetRecipient(this Saml2SecurityToken token, Uri recipient, string inResponseTo) => token.SetRecipient(recipient, inResponseTo != null ? new Saml2Id(inResponseTo) : null);

        public static void SetRecipient(this Saml2SecurityToken token, Uri recipient, Saml2Id inResponseTo)
        {
            var data = token.GetBearerSubjectConfirmationData();
            if (data == null) return;

            data.Recipient = recipient;
            data.InResponseTo = inResponseTo;
        }

        public static void SetNotBefore(this Saml2SecurityToken token)
            => token.SetNotBefore(token.ValidFrom);

        public static void SetNotBefore(this Saml2SecurityToken token, DateTime? notBefore)
        {
            var data = token.GetBearerSubjectConfirmationData();
            if (data == null) return;

            data.NotBefore = notBefore;
        }

        public static void SetNotOnOrAfter(this Saml2SecurityToken token)
            => token.SetNotOnOrAfter(token.ValidTo);

        public static void SetNotOnOrAfter(this Saml2SecurityToken token, DateTime? notOnOrAfter)
        {
            var data = token.GetBearerSubjectConfirmationData();
            if (data == null) return;

            data.NotOnOrAfter = notOnOrAfter;
        }

        public static void ValidateResponseToken(this Saml2SecurityToken token, string authnRequestId, DateTime now)
        {
            var data = token.GetBearerSubjectConfirmationData();
            if (data == null)
                throw new SecurityException("Missing bearer subject confirmation data.");

            if(data.InResponseTo?.Value != authnRequestId)
                throw new SecurityException($"Invalid InResponseTo. Expected '{authnRequestId}' but got '{data.InResponseTo?.Value}'.");

            if (data.NotBefore != null && now < data.NotBefore)
                throw new SecurityException($"NotBefore validation failed.");
            if (data.NotOnOrAfter != null && data.NotOnOrAfter <= now)
                throw new SecurityException($"NotOnOrAfter validation failed.");
        }

        public static Saml2Id GetInResponseTo(this Saml2SecurityToken token)
        {
            var data = token.GetBearerSubjectConfirmationData();
            return data?.InResponseTo;
        }

        //public static ClaimsPrincipal ToClaimsPrincipal(this Saml2SecurityToken token, TokenValidationParameters parameters) => new SolidSaml2SecurityTokenHandler().CreateClaimsPrincipal(token, parameters);

        static Saml2SubjectConfirmationData GetBearerSubjectConfirmationData(this Saml2SecurityToken token)
        {
            var confirmation = token.Assertion.Subject.SubjectConfirmations.FirstOrDefault(c => c.Method == Saml2Constants.ConfirmationMethods.Bearer);
            if (confirmation == null)
                token.Assertion.Subject.SubjectConfirmations.Add(confirmation = new Saml2SubjectConfirmation(Saml2Constants.ConfirmationMethods.Bearer));
            if (confirmation.SubjectConfirmationData == null)
                confirmation.SubjectConfirmationData = new Saml2SubjectConfirmationData();
            return confirmation.SubjectConfirmationData;
        }
    }
}
