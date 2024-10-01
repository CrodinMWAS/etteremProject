using Api.Database;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace Api.Helpers;

public class LoginHelper
{
    private readonly DatabaseContext _databaseContext;

    public LoginHelper(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task IncrementLoginAttemptsAsync(string ipAddress)
        {
            var query = "SELECT AttemptCount, LastAttempt FROM loginattempts WHERE IPAddress = @IPAddress";
            int attemptCount = 0;
            DateTime? lastAttempt = null;

            var command = _databaseContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.Parameters.Add(new MySqlParameter("@IPAddress", ipAddress));

            await _databaseContext.Database.OpenConnectionAsync();
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                attemptCount = reader.GetInt32(0);
                lastAttempt = reader.GetDateTime(1);
            }
            await _databaseContext.Database.CloseConnectionAsync();

            if (lastAttempt == null)
            {
                // New IP, insert into the table
                await InsertLoginAttemptAsync(ipAddress, 1);  // First failed attempt
                return;
            }

            if ((DateTime.Now - lastAttempt.Value).TotalMinutes < 5)
            {
                attemptCount++;
            }
            else
            {
                attemptCount = 1;  // Reset count after 5 minutes
            }

            if (attemptCount >= 5)
            {
                // Lock the IP and no further action
                return;
            }

            await UpdateLoginAttemptAsync(ipAddress, attemptCount);
        }
        
        public async Task InsertLoginAttemptAsync(string ipAddress, int attemptCount)
        {
            var query = "INSERT INTO LoginAttempts (IPAddress, AttemptCount, LastAttempt) VALUES (@IPAddress, @AttemptCount, @LastAttempt)";
            var command = _databaseContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.Parameters.Add(new MySqlParameter("@IPAddress", ipAddress));
            command.Parameters.Add(new MySqlParameter("@AttemptCount", attemptCount));
            command.Parameters.Add(new MySqlParameter("@LastAttempt", DateTime.Now));

            await _databaseContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            await _databaseContext.Database.CloseConnectionAsync();
        } 

        public async Task UpdateLoginAttemptAsync(string ipAddress, int attemptCount)
        {
            var query = "UPDATE LoginAttempts SET AttemptCount = @AttemptCount, LastAttempt = @LastAttempt WHERE IPAddress = @IPAddress";
            var command = _databaseContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.Parameters.Add(new MySqlParameter("@AttemptCount", attemptCount));
            command.Parameters.Add(new MySqlParameter("@LastAttempt", DateTime.Now));
            command.Parameters.Add(new MySqlParameter("@IPAddress", ipAddress));

            await _databaseContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            await _databaseContext.Database.CloseConnectionAsync();
        }

        public async Task ResetLoginAttemptsAsync(string ipAddress)
        {
            var query = "DELETE FROM LoginAttempts WHERE IPAddress = @IPAddress";
            var command = _databaseContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.Parameters.Add(new MySqlParameter("@IPAddress", ipAddress));

            await _databaseContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            await _databaseContext.Database.CloseConnectionAsync();
        }

        public async Task<bool> IsIpTimedOutAsync(string ipAddress)
        {
            var query = "SELECT AttemptCount, LastAttempt FROM LoginAttempts WHERE IPAddress = @IPAddress";
            int attemptCount = 0;
            DateTime? lastAttempt = null;

            var command = _databaseContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.Parameters.Add(new MySqlParameter("@IPAddress", ipAddress));

            await _databaseContext.Database.OpenConnectionAsync();
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                attemptCount = reader.GetInt32(0);
                lastAttempt = reader.GetDateTime(1);
            }
            await _databaseContext.Database.CloseConnectionAsync();

            if (attemptCount >= 5 && (DateTime.Now - lastAttempt.Value).TotalMinutes < 5)
            {
                return true;
            }

            return false;
        }
}