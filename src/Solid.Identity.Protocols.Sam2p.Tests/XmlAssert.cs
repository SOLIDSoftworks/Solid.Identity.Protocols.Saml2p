using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Xunit;

namespace Solid.Identity.Protocols.Saml2p.Tests
{
    internal static class XmlAssert
    {
        public static void Equal(string expectedXml, string actualXml)
        {
            var expected = XDocument.Parse(expectedXml);
            var actual = XDocument.Parse(actualXml);

            ElementEqual(expected.Root, actual.Root);
        }

        private static void ElementEqual(XElement expected, XElement actual)
        {
            Assert.Equal(expected.Name, actual.Name);
            AssertAttributes(expected.Attributes(), actual.Attributes());
        }

        private static void AssertAttributes(IEnumerable<XAttribute> expected, IEnumerable<XAttribute> actual)
        {
            Assert.Equal(expected.Count(), actual.Count());

            expected = expected.OrderBy(a => a.Name);
            actual = actual.OrderBy(a => a.Name);

            foreach (var attribute in expected)
            {

            }
        }
    }
}
