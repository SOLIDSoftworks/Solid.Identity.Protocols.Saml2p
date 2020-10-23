using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Results
{
    public class FinishSsoResult
    {
        private FinishSsoResult() { }
        public static FinishSsoResult Success(Saml2SecurityToken token, ClaimsPrincipal subject)
        {
            return new FinishSsoResult
            {
                SecurityToken = token,
                Subject = subject
            };
        }
        public static FinishSsoResult Fail()
        {
            return new FinishSsoResult
            {
            };
        }
        public bool IsSuccessful => SecurityToken != null && Subject != null;
        public Saml2SecurityToken SecurityToken { get; private set; }
        public ClaimsPrincipal Subject { get; private set; }
    }
}
