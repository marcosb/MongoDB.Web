using System;
using System.Collections.Specialized;
using System.Configuration;

using MongoDB.Driver;
using MongoDB.Web.Config;

namespace MongoDB.Web.Providers
{
    internal class ConnectionHelper
    {
        /// <summary>
        /// Gets the configured connection string.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        internal static string GetConnectionString(MongoDbWebSection mongoDbWebSection, NameValueCollection config)
        {
            string connectionString = config["connectionString"];
            if (!string.IsNullOrWhiteSpace(connectionString))
                return connectionString;

            string appSettingsKey = config["connectionStringKey"];
            if (string.IsNullOrWhiteSpace(appSettingsKey))
                return (mongoDbWebSection != null) ? mongoDbWebSection.ConnectionString : "mongodb://localhost";
            else
                return ConfigurationManager.ConnectionStrings[appSettingsKey].ConnectionString;
        }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        internal static string GetDatabaseName(MongoDbWebSection mongoDbWebSection, string connectionString, NameValueCollection config)
        {
            MongoUrl mongoUrl = MongoUrl.Create(connectionString);
            if (!string.IsNullOrEmpty(mongoUrl.DatabaseName))
                return mongoUrl.DatabaseName;

            return (mongoDbWebSection != null)
                ? mongoDbWebSection.DatabaseName
                : config["database"] ?? "ASPNETDB";
        }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        internal static string GetDatabaseName(MongoDbWebSection mongoDbWebSection, NameValueCollection config)
        {
            return GetDatabaseName(mongoDbWebSection, GetConnectionString(mongoDbWebSection, config), config);
        }

        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        internal static string GetDatabaseConnectionString(NameValueCollection config)
        {
            var mongoDbWebSection = ConfigurationManager.GetSection("mongoDbWeb") as MongoDbWebSection;
            return GetDatabaseConnectionString(mongoDbWebSection, config);
        }

        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        internal static string GetDatabaseConnectionString(MongoDbWebSection mongoDbWebSection, NameValueCollection config)
        {
            string connectionString = GetConnectionString(mongoDbWebSection, config);
            var builder = new MongoUrlBuilder(connectionString);
            builder.DatabaseName = GetDatabaseName(mongoDbWebSection, connectionString, config);

            return builder.ToString();
        }

        internal static MongoDatabase GetDatabase(NameValueCollection config)
        {
            var mongoDbWebSection = ConfigurationManager.GetSection("mongoDbWeb") as MongoDbWebSection;

            return new MongoClient(new MongoUrl(GetDatabaseConnectionString(mongoDbWebSection, config)))
                    .GetServer().GetDatabase(GetDatabaseName(mongoDbWebSection, config));
        }
    }
}