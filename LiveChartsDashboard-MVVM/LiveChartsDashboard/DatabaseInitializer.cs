using System.IO;
using Microsoft.Data.Sqlite;

namespace LiveChartsDashboard
{
    public class DatabaseInitializer
    {
        public static void InitializeDatabase(string databasePath)
        {
            if (!File.Exists(databasePath))
            {
                using var connection = new SqliteConnection($"Data Source={databasePath}");
                connection.Open();

                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = @"
                    CREATE TABLE Activity (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT NOT NULL,
                        HoursWorked REAL NOT NULL,
                        MachineName TEXT NOT NULL
                    );";
                createTableCommand.ExecuteNonQuery();

                System.Console.WriteLine($"База данных создана по пути: {databasePath}");
            }
            else
            {
                System.Console.WriteLine($"База данных уже существует по пути: {databasePath}");
            }
        }
    }
}
