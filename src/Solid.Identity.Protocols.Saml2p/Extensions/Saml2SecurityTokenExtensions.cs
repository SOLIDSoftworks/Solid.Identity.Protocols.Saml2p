using Solid.Identity.Tokens.Saml2;
using System;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.IdentityModel.Tokens.Saml2
{
    internal static class Saml2SecurityTokenExtensions
    {
        public static void SetRecipient(this Saml2SecurityToken token, Uri recipient) => token.SetRecipient(recipient, null as Saml2Id);

        public static void SetRecipient(this Saml2SecurityToken token, Uri recipient, string inResponseTo) => token.SetRecipient(recipient, inResponseTo != null ? new Saml2Id(inResponseTo) : null);

        public static void SetRecipient(this Saml2SecurityToken token, Uri recipient, Saml2Id inResponseTo)
        {
            var confirmation = token.Assertion.Subject.SubjectConfirmations.FirstOrDefault(c => c.Method == Saml2Constants.ConfirmationMethods.Bearer);
            if (confirmation == null)
                token.Assertion.Subject.SubjectConfirmations.Add(confirmation = new Saml2SubjectConfirmation(Saml2Constants.ConfirmationMethods.Bearer));
            if (confirmation.SubjectConfirmationData == null)
                confirmation.SubjectConfirmationData = new Saml2SubjectConfirmationData();
            confirmation.SubjectConfirmationData.Recipient = recipient;
            confirmation.SubjectConfirmationData.InResponseTo = inResponseTo;
        }

        public static ClaimsPrincipal ToClaimsPrincipal(this Saml2SecurityToken token, TokenValidationParameters parameters) => new SolidSaml2SecurityTokenHandler().CreateClaimsPrincipal(token, parameters);
    }
}
