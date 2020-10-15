using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Xml;

namespace Solid.Identity.Tokens.Saml2
{
    internal class SolidSaml2SecurityTokenHandler : Saml2SecurityTokenHandler
    {
        public SolidSaml2SecurityTokenHandler() : this(new SolidSaml2Serializer())
        {
        }
        public SolidSaml2SecurityTokenHandler(Saml2Serializer serializer)
        {
            Serializer = serializer;
        }

        public override SecurityToken CreateToken(SecurityTokenDescriptor tokenDescriptor)
        {
            var methodString = tokenDescriptor.Subject?.FindFirst(ClaimTypes.AuthenticationMethod)?.Value;
            var instantString = tokenDescriptor.Subject?.FindFirst(ClaimTypes.AuthenticationInstant)?.Value;

            if (methodString != null && Uri.TryCreate(methodString, UriKind.Absolute, out var method) &&
                instantString != null && DateTime.TryParse(instantString, out var instant))
            {
                var information = new AuthenticationInformation(method, instant);
                return CreateToken(tokenDescriptor, information);
            }
            return base.CreateToken(tokenDescriptor);
        }

        public override SecurityToken CreateToken(SecurityTokenDescriptor tokenDescriptor, AuthenticationInformation authenticationInformation)
        {
            var token = base.CreateToken(tokenDescriptor, null) as Saml2SecurityToken;
            if (authenticationInformation != null)
            {
                if (authenticationInformation.Session == null)
                    authenticationInformation.Session = token.Assertion.Id.Value;

                var authnStatement = CreateAuthenticationStatement(authenticationInformation);
                token.Assertion.Statements.Add(authnStatement);
            }
            return token;
        }

        protected override Saml2Subject CreateSubject(SecurityTokenDescriptor tokenDescriptor)
        {
            var subject = base.CreateSubject(tokenDescriptor);
            var bearer = subject.SubjectConfirmations.FirstOrDefault(c => c.Method == Saml2Constants.ConfirmationMethods.Bearer);
            if (bearer != null)
            {
                if (bearer.SubjectConfirmationData == null) 
                    bearer.SubjectConfirmationData = new Saml2SubjectConfirmationData();
                bearer.SubjectConfirmationData.NotBefore = tokenDescriptor.NotBefore;
                bearer.SubjectConfirmationData.NotOnOrAfter = tokenDescriptor.Expires;
            }
            return subject;
        }

        public override SecurityToken ReadToken(XmlReader reader, TokenValidationParameters validationParameters)
        {
            ValidateToken(reader, validationParameters, out var token);
            return token;
        }

        public ClaimsPrincipal CreateClaimsPrincipal(Saml2SecurityToken token, TokenValidationParameters validationParameters)
        {
            var identity = CreateClaimsIdentity(token, token.Issuer, validationParameters);
            return new ClaimsPrincipal(identity);
        }
    }
}
