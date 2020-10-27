using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models
{
    internal static class EnumExtensions
    {
        public static string ToBindingTypeString(this BindingType bindingType)
        {
            switch(bindingType)
            {
                case BindingType.Post: return Saml2pConstants.Bindings.Post;
                case BindingType.Redirect: return Saml2pConstants.Bindings.Redirect;
            }

            throw new ArgumentException($"Unsupported binding type: {bindingType}");
        }

        //public static string ToStatusString(this SamlResponseStatus status)
        //{

        //}
    }
}
