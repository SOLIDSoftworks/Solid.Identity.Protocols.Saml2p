using Solid.Identity.Protocols.Saml2p.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Services
{
    internal class Saml2pEncodingService
    {
        public string Encode(MemoryStream stream, BindingType binding)
        {
            if (binding == BindingType.Post)
            {
                return Convert.ToBase64String(stream.ToArray());
            }
            else if (binding == BindingType.Redirect)
            {
                using (var memory = new MemoryStream())
                {
                    using (var deflate = new DeflateStream(memory, CompressionMode.Compress, true))
                    {
                        stream.CopyTo(deflate);
                    }
                    var base64 = Convert.ToBase64String(memory.ToArray());
                    return base64;
                }
            }

            throw new ArgumentException("Unsupported binding type.");
        }

        public MemoryStream Decode(string base64, BindingType binding)
        {
            if (binding == BindingType.Post) 
                return new MemoryStream(Convert.FromBase64String(base64));
            
            var bytes = Convert.FromBase64String(base64);
            using (var memory = new MemoryStream(bytes))
            using (var stream = new DeflateStream(memory, CompressionMode.Decompress))
            {
                var deflated = new MemoryStream();
                stream.CopyTo(deflated);
                deflated.Position = 0;
                return deflated;
            }
        }
    }
}
