using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace BudyBot
{
    internal class DatabaseConnector
    {
        string connectionString;
        ConfigurationBuilder builder;
        IConfiguration configuration;

        public DatabaseConnector()
        {
            connectionString = "Server=localhost;Port=3306;Database=twitch_bot;Uid=root;Pwd=root;";
            Task.WaitAll(connect());
        }

        private async Task connect()
        {
            Console.WriteLine("Connecting to MariaDb...");

            try
            {
                await using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();
                Console.WriteLine("Connected to MariaDB!");

            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public async Task<bool> UserExists(string username)
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            Console.WriteLine($"Looking for: {username}");
            string query = $"SELECT id FROM user WHERE name = @name;";
            await using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", username);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Console.WriteLine("user found!");
                return true;
            }
            return false;
        }

        public async Task AddUser(string username)
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            Console.WriteLine($"New user! adding {username} to list.");

            string query = $"INSERT INTO user (name) VALUES (@name);";
            await using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", username);
            int rowsAffected = await command.ExecuteNonQueryAsync();

            Console.WriteLine($"\nRows inserted into user: {rowsAffected}");
        }

        public async Task IncrementMessages(string username, int id, string message, long timestamp)
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string query = $"UPDATE user SET msgCount = msgCount+1 WHERE name = @name;";
            await using var c1 = new MySqlCommand(query, connection);
            c1.Parameters.AddWithValue("@name", username);
            await c1.ExecuteNonQueryAsync();

            query = $"UPDATE last_message SET message = @message, timestamp = @timestamp WHERE twitch_id = @twitch_id;";
            await using var c3 = new MySqlCommand(query, connection);
            c3.Parameters.AddWithValue("@message", message);
            c3.Parameters.AddWithValue("@timestamp", timestamp);
            c3.Parameters.AddWithValue("@id", id);
            await c3.ExecuteNonQueryAsync();

        }

        public async Task<(int, int, string, int)> SelectUser(string username)
        {
            Console.WriteLine("RUNNING SELECTUSER");
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string query = $"SELECT * FROM user WHERE name = @name;";
            await using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", "budywudy9");
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"FOUND ID: {reader.GetInt32("id")}");
                if (reader.GetInt32("id") > 0)
                    return (reader.GetInt32("id"), reader.GetInt32("twitch_id"), reader.GetString("name"), reader.GetInt32("msgCount"));
            }
            return (0, 0, "", 0);
        }

        public async Task AddMessage(string user, string message, long timestamp)
        {

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            int id = SelectUser(user).Result.Item2;

            string query = $"INSERT INTO top_message (twitch_id, message, timestamp) VALUES (@twitch_id, @message, @timestamp);";
            await using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@twitch_id", id);
            command.Parameters.AddWithValue("@message", message);
            command.Parameters.AddWithValue("@sent", timestamp);

            int RowsAffected = await command.ExecuteNonQueryAsync();
            Console.WriteLine($"\nRows Affected in top_message: {RowsAffected}");
        }

        public async Task<(string, long)> GetMessage(string username = "")
        {
            Random r = new Random();
            List<string> messages = new List<string>();
            List<long> times = new List<long>();

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            if (username == "")
            {
                // Selects random user 
                List<string> users = new List<string>();
                string q3 = $"SELECT name FROM user INNER JOIN top_message ON top_message.twitch_id = user.twitch_id GROUP BY name";
                await using var c3 = new MySqlCommand(q3, connection);
                await using var r3 = await c3.ExecuteReaderAsync();
                while (await r3.ReadAsync())
                    users.Add(r3.GetString("name"));
                if (users.Count < 1)
                    return ("", 0);
                username = users[r.Next(users.Count - 1)];
            }

            int id = SelectUser(username).Result.Item2;
            string query = $"SELECT message, timestamp FROM top_message WHERE twitch_id = @twitch_id;";
            await using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@twitch_id", id);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                messages.Add(reader.GetString("message"));
                times.Add(reader.GetInt64("timestamp"));
            }
            await reader.DisposeAsync();

            if (messages.Count == 0)
                return ("", 0);

            int n = r.Next(messages.Count - 1);
            return (messages[n], times[n]);
        }

        public async Task<int> GetCounter(int id)
        {
            int counter = 0;
            MySqlConnection connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string query = $"SELECT counter FROM cmd_counter WHERE id = @id;";
            await using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                counter = reader.GetInt32("counter");
            await reader.CloseAsync();

            return counter;
        }

        public async Task IncrementCounter(int id)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string query = $"ALTER TABLE cmd_counter SET counter = counter+1 WHERE id = @id;";
            await using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            await command.ExecuteNonQueryAsync();

            return;
        }

    }
}