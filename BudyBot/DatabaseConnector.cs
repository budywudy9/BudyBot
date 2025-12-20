using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace BudyBot
{
    internal class DatabaseConnector
    {
        string connectionString;

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
            if (reader.HasRows)
                return true;
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
            Console.WriteLine($"\nRows inserted: {rowsAffected}");
            
        }

        public async Task IncrementMessages(string username, string message)
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string query = $"UPDATE user SET msgCount = msgCount+1 WHERE name = @name;";
            await using var c1 = new MySqlCommand(query, connection);
            c1.Parameters.AddWithValue("@name", username);
            await c1.ExecuteNonQueryAsync();

            int id = 0;
            query = $"SELECT id FROM user WHERE name = @name";
            await using var c2 = new MySqlCommand(query, connection);
            c2.Parameters.AddWithValue("@name", username);
            await using var reader = await c2.ExecuteReaderAsync();
            if (reader.HasRows)
                id = reader.GetInt32("id");

            query = $"UPDATE last_message SET message = @message, sent = @sent WHERE id = @id;";
            await using var c3 = new MySqlCommand(query, connection);
            c3.Parameters.AddWithValue("@message", message);
            c3.Parameters.AddWithValue("@sent", username);
            c3.Parameters.AddWithValue("@id", id);
        }

        public async Task<(int, string, int)> SelectUser(string username)
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string query = $"SELECT * FROM user WHERE name = @name;";
            await using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", username);
            await using var reader = await command.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                return (reader.GetInt32("id"), reader.GetString("name"), reader.GetInt32("msgCount"));
            }

            return (0, "", 0);
        }

        public async Task AddMessage(string username, string message, string timestamp)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(Double.Parse(timestamp)).ToLocalTime();
            string timeSent = dt.ToString("yyyy-MM-dd HH:mm:ss");
            int userId = SelectUser(username).Result.Item1;

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            if (message == "")
            {
                string q = $"SELECT message FROM last_message WHERE id = @id";
                await using var c2 = new MySqlCommand(q, connection);
                c2.Parameters.AddWithValue("@id", userId);
                await using var reader = await c2.ExecuteReaderAsync();
                if (reader.HasRows)
                    message = reader.GetString("message");
            }

            string query = $"INSERT INTO top_message (userId, message, sent) VALUES (@userId, @message, @sent);";
            await using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@message", message);
            command.Parameters.AddWithValue("@sent", timeSent);

            int RowsAffected = await command.ExecuteNonQueryAsync();
            Console.WriteLine($"\nRows Affected: {RowsAffected}");
        }

        public async Task GetMessage(int id)
        {

        }

    }
}
