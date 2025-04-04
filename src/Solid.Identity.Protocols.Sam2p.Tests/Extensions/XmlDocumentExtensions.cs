using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Xml;
using KeyInfo = System.Security.Cryptography.Xml.KeyInfo;
using Reference = System.Security.Cryptography.Xml.Reference;
using RSAKeyValue = System.Security.Cryptography.Xml.RSAKeyValue;

namespace System.Xml;

public static class XmlDocumentExtensions
{
    public static void SignXml(this XmlDocument document, RSA key, string id = null)
    {
        var signer = new SignedXml(document);
        var root = document.DocumentElement;
        if (root == null)
            throw new ArgumentException("Invalid document", nameof(document));
        
        var reference = new Reference(id ?? string.Empty);
        reference.AddTransform(new XmlDsigExcC14NTransform());
        signer.Signature.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
        signer.Signature.SignedInfo.AddReference(reference);
        
        var keyInfo = new KeyInfo();
        keyInfo.AddClause(new RSAKeyValue(key));
        signer.Signature.KeyInfo = keyInfo;
        
        signer.SigningKey = key;
        signer.ComputeSignature();
        
        var signature = signer.GetXml();

        // Append the element to the XML document.
        root.AppendChild(document.ImportNode(signature, true));

        if (document.FirstChild is XmlDeclaration)
            document.RemoveChild(document.FirstChild);
    }
}