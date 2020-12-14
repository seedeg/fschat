using System;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;

namespace FsChat.Utilities.Common
{
    public static class Ip
    {
        public static string RetrieveClientIp(HttpRequestMessage request)
        {
            try
            {
                if (request.Properties.ContainsKey("MS_HttpContext"))
                {
                    var httpContext = (HttpContextWrapper)request.Properties["MS_HttpContext"];

                    return RetrieveClientIp(httpContext.Request);
                }

                if (!request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
                {
                    return string.Empty;
                }

                var prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];

                return (prop.Address) ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string RetrieveClientIp(HttpRequestBase httpContext)
        {
            var ipAddress = httpContext["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return ipAddress ?? httpContext.UserHostAddress;
            }

            var splitForwardedFor = ipAddress.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            ipAddress = splitForwardedFor.Any() ? splitForwardedFor.First().Trim() : null;

            return ipAddress ?? httpContext.UserHostAddress;
        }
    }
}
