using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Solid.Identity.Protocols.Saml2p.Tests
{
    public class Saml2pEncodingServiceTests
    {
        private Saml2pEncodingService _encoder = new Saml2pEncodingService();

        [Theory]
        [InlineData("<xml></xml>", "s6nIzbGz0QeRAA==")]
        [InlineData("<test></test>", "sylJLS6xs9EHUwA=")]
        public void ShouldDeflateAndBase64Encode(string xml, string expected)
        {
            using (var stream = new MemoryStream(new UTF8Encoding(false).GetBytes(xml)))
            {
                var encoded = _encoder.Encode(stream, BindingType.Redirect);
                Assert.Equal(expected, encoded);
            }
        }

        [Theory]
        [InlineData("s6nIzbGz0QeRAA==", "<xml></xml>")]
        [InlineData("sylJLS6xs9EHUwA=", "<test></test>")]
        public void ShouldBase64DecodeAndInflate(string encoded, string expected)
        {
            using (var stream = _encoder.Decode(encoded, BindingType.Redirect))
            {
                var xml = new UTF8Encoding(false).GetString(stream.ToArray());
                Assert.Equal(expected, xml);
            }
        }
        
        [Theory]
        [InlineData("PHhtbD48L3htbD4=", "<xml></xml>")]
        [InlineData("PHRlc3Q+PC90ZXN0Pg==", "<test></test>")]
        public void ShouldBase64Decode(string encoded, string expected)
        {
            using (var stream = _encoder.Decode(encoded, BindingType.Post))
            {
                var xml = new UTF8Encoding(false).GetString(stream.ToArray());
                Assert.Equal(expected, xml);
            }
        }

        [Theory]
        [InlineData("<xml></xml>", "PHhtbD48L3htbD4=")]
        [InlineData("<test></test>", "PHRlc3Q+PC90ZXN0Pg==")]
        public void ShouldBase64Encode(string xml, string expected)
        {
            using (var stream = new MemoryStream(new UTF8Encoding(false).GetBytes(xml)))
            {
                var encoded = _encoder.Encode(stream, BindingType.Post);
                Assert.Equal(expected, encoded);
            }
        }
    }
}
