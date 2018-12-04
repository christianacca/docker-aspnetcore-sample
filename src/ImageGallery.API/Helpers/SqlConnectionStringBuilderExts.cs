using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ImageGallery.API.Helpers
{
    public static class SqlConnectionStringBuilderExts
    {
        public static SqlConnectionStringBuilder Clone(this SqlConnectionStringBuilder source)
        {
            return new SqlConnectionStringBuilder(source.ConnectionString);
        }

        public static string SanitizedConnectionString(this SqlConnectionStringBuilder source)
        {
            if (string.IsNullOrWhiteSpace(source.Password))
            {
                return source.ConnectionString;
            }

            var clone = source.Clone();
            clone.Password = "******";
            return clone.ConnectionString;
        }
    }
}