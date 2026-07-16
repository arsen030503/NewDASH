using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using LiveChartsDashboard.Models;

namespace LiveChartsDashboard.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
           
            var databasePath = Path.Combine(AppContext.BaseDirectory, "database.db");
            _connectionString = $"Data Source={databasePath}";
        }

        public async Task<List<Activity>> GetActivitiesAsync()
        {
            var activities = new List<Activity>();

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Date, HoursWorked, MachineName FROM Activity";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    activities.Add(new Activity
                    {
                        Id = reader.GetInt32(0),
                        Date = DateTime.Parse(reader.GetString(1)),
                        HoursWorked = reader.GetDouble(2),
                        MachineName = reader.GetString(3)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error (GetActivitiesAsync): {ex.Message}");
            }

            return activities;
        }

        public async Task AddActivityAsync(Activity activity)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO Activity (Date, HoursWorked, MachineName)
                                         VALUES ($date, $hoursWorked, $machineName)";
                command.Parameters.AddWithValue("$date", activity.Date.ToString("o"));
                command.Parameters.AddWithValue("$hoursWorked", activity.HoursWorked);
                command.Parameters.AddWithValue("$machineName", activity.MachineName);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error (AddActivityAsync): {ex.Message}");
            }
        }

        public async Task UpdateActivityAsync(Activity activity)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = @"UPDATE Activity
                                         SET Date = $date, HoursWorked = $hoursWorked, MachineName = $machineName
                                         WHERE Id = $id";
                command.Parameters.AddWithValue("$id", activity.Id);
                command.Parameters.AddWithValue("$date", activity.Date.ToString("o"));
                command.Parameters.AddWithValue("$hoursWorked", activity.HoursWorked);
                command.Parameters.AddWithValue("$machineName", activity.MachineName);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error (UpdateActivityAsync): {ex.Message}");
            }
        }

        public async Task DeleteActivityAsync(int activityId)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Activity WHERE Id = $id";
                command.Parameters.AddWithValue("$id", activityId);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error (DeleteActivityAsync): {ex.Message}");
            }
        }
    }
}
