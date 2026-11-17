using System;
using System.Net;
using System.Xml;

namespace UblSharp.Validation.Internal
{
    public class UblXsdResolver : XmlUrlResolver
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri.IsFile && absoluteUri.Segments.Length >= 2)
            {
                var path = absoluteUri.Segments[absoluteUri.Segments.Length - 2].Trim('/') + "/" +
                           absoluteUri.Segments[absoluteUri.Segments.Length - 1].Trim('/');

                var stream = XsdResources.Open(path);
                if (stream != null)
                {
                    return stream;
                }
            }

            return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }
    }
}
