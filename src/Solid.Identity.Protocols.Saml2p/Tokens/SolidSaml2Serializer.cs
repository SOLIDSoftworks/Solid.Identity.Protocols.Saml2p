using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Solid.Identity.Tokens.Saml2
{
    internal class SolidSaml2Serializer : Saml2Serializer
    {
        public override void WriteAttribute(XmlWriter writer, Saml2Attribute attribute)
        {
            var w = XmlDictionaryWriter.CreateDictionaryWriter(writer);
            var saml2assertion = "urn:oasis:names:tc:SAML:2.0:assertion";
            var xsi = "http://www.w3.org/2001/XMLSchema-instance";
            w.WriteStartElement("Attribute", saml2assertion);
            w.WriteAttributeString("Name", attribute.Name);
            w.WriteAttributeString("NameFormat", attribute.NameFormat?.ToString() ?? Saml2Constants.NameIdentifierFormats.UnspecifiedString);
            foreach (var value in attribute.Values)
            {
                w.WriteStartElement("AttributeValue", saml2assertion);


                var fqtn = attribute.AttributeValueXsiType?.Split('#');
                if (fqtn?.Length == 2)
                {
                    if(string.IsNullOrEmpty(w.LookupPrefix("xs")))
                        w.WriteXmlnsAttribute("xs", fqtn[0]);

                    w.WriteStartAttribute("xsi", "type", xsi);
                    w.WriteQualifiedName(fqtn[1], fqtn[0]);
                    w.WriteEndAttribute();
                }

                w.WriteString(value);

                w.WriteEndElement();
            }
            w.WriteEndElement();
        }
    }
}
