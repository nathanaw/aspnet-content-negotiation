using Microsoft.Data.Sqlite;
using Dapper;
using SqlKata;
using SqlKata.Execution;

namespace CodecExample.Data.Sqlite
{

    /// <summary>
    /// Simple database schema migration logic to created needed tables.
    /// A better approach would be to use something like FluentMigrations.
    /// </summary>
    public class DBSetup
    {

        /// <summary>
        /// Create the necessary database and schema.
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Setup(string connectionString)
        {
            using var connection = new SqliteConnection(connectionString);

            // Check if table already exists.
            var table = connection.Query<string>(@"
                            SELECT  name 
                            FROM    sqlite_master 
                            WHERE   type='table' 
                            AND     name = 'WeatherForecasts';");

            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && tableName == "WeatherForecasts")
            {
                // Exists... No need to create it.
                return;
            }

            // Create the tables.
            connection.Execute(@"
                CREATE TABLE WeatherForecasts (
                    ID              VARCHAR(100)    PRIMARY KEY NOT NULL, 
                    MediaType       VARCHAR(256)    NOT NULL,
                    Content         BLOB            NOT NULL,
                    TempCelcius     INT             NOT NULL 
                );");
        }
    }
}