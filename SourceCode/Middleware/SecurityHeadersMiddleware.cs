using Middleware.HttpHeaders;
using Microsoft.AspNetCore.Http;

namespace Middleware.HttpHeaders
{
    public static class HeaderNames
    {
        public const string XFrameOptions = "X-Frame-Options";
        public const string XContentTypeOptions = "X-Content-Type-Options";
        public const string ContentSecurityPolicy = "Content-Security-Policy";
        public const string ReferrerPolicy = "Referrer-Policy";
        public const string Referer = "Referer";
        public const string StrictTransportSecurity = "Strict-Transport-Security";
        public const string Server = "Server";
    }

    public static class HeaderValues
    {
        public const string Deny = "DENY";
        public const string Unknown = "unknown";
        public const string NoSniff = "nosniff";
        public const string DefaultSourceSelf = "default-src 'self'";
        public const string NoReferrerPolicy = "no-referrer";
        public const string TransportSecurityWithSubdomains = "max-age=31536000; includeSubDomains";
    }

}

namespace Middleware
{
    

    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.Add(HeaderNames.XFrameOptions, HeaderValues.Deny);
            context.Response.Headers.Add(HeaderNames.XContentTypeOptions, HeaderValues.NoSniff);
            context.Response.Headers.Add(HeaderNames.ContentSecurityPolicy, HeaderValues.DefaultSourceSelf);
            context.Response.Headers.Add(HeaderNames.ReferrerPolicy, HeaderValues.NoReferrerPolicy);
            context.Response.Headers.Add(HeaderNames.StrictTransportSecurity, HeaderValues.TransportSecurityWithSubdomains);
            context.Response.Headers.Add(HeaderNames.Server, HeaderValues.Unknown);

            await _next(context);
        }
    }
}