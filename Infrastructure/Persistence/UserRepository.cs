using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Application.Interfaces;

namespace Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task AddUserAsync(User user)
        {
            var query = @"INSERT INTO Users (Username, PasswordHash, FirstName, LastName, Device, IpAddress, CreatedAt, Balance)
                      VALUES (@Username, @PasswordHash, @FirstName, @LastName, @Device, @IpAddress, @CreatedAt, @Balance)";
            await _dbConnection.ExecuteAsync(query, user);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var query = "SELECT * FROM Users WHERE Username = @Username";
            return await _dbConnection.QuerySingleOrDefaultAsync<User>(query, new { Username = username });
        }

        public async Task UpdateUserAsync(User user)
        {
            var query = "UPDATE Users SET Balance = @Balance WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(query, new { user.Balance, user.Id });
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var query = "SELECT * FROM Users WHERE Id = @Id";
            return await _dbConnection.QuerySingleOrDefaultAsync<User>(query, new { Id = userId });
        }

        public async Task RecordLoginAsync(int userId, string ipAddress, string device, string browser)
        {
            var query = @"INSERT INTO UserLogins (UserId, IpAddress, Device, Browser, LoginTime)
                      VALUES (@UserId, @IpAddress, @Device, @Browser, @LoginTime)";
            await _dbConnection.ExecuteAsync(query, new { UserId = userId, IpAddress = ipAddress, Device = device, Browser = browser, LoginTime = DateTime.UtcNow });
        }
    }



}
