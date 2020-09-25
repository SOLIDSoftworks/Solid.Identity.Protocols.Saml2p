using System;
using System.Collections.Generic;
using System.Text;

namespace System.Xml
{
    public static class XmlDictionaryReaderExtensions
    {
        public static IEnumerable<XmlDictionaryReader> GetChildElementReaders(this XmlDictionaryReader reader)
        {
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    using (var child = XmlDictionaryReader.CreateDictionaryReader(reader.ReadSubtree()))
                    {
                        if (!child.Read()) continue;
                        yield return child;
                    }
                }
            }
        }
    }
}
