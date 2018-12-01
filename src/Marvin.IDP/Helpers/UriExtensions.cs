using System;
using System.Linq;

namespace Marvin.IDP.Helpers
{
    public static class UriExtensions
    {
        public static Uri AppendPath(this Uri uri, params string[] paths)
        {
            return new Uri(paths.Aggregate(uri.AbsoluteUri,
                (current, path) => string.Format("{0}/{1}", current.TrimEnd('/'), path.TrimStart('/'))));
        }
    }
}