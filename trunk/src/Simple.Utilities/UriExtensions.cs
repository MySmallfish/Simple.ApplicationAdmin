using System;

namespace Simple.Utilities
{
    public static class UriExtensions
    {
        public static string SubDomain(this Uri uri)
        {
            var host = uri.Host;
            var parts = host.Split('.');
            var subDomain = default(string);
            if (parts.Length > 1)
            {
                subDomain = parts[0];
            }

            return subDomain;
        
        }

    }
}
