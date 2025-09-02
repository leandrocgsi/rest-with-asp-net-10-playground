﻿using Microsoft.AspNetCore.Mvc;

namespace RestWithASPNET10Erudio.Hypermedia.Utils
{
    public static class UrlHelper
    {
        private static readonly object _lock = new object();

        public static string BuildBaseUrl(this IUrlHelper urlHelper, string routeName, string path)
        {
            lock (_lock)
            {
                var url = urlHelper.Link(routeName, new { controller = path }) ?? string.Empty;
                return url.Replace("%2F", "/").TrimEnd('/');
            }
        }
    }
}
