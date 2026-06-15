using Dapper;
using System.Data;
using UserService.Models;

namespace UserService.Data;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _db;
    private static readonly SemaphoreSlim InitializationLock = new(1, 1);
    private static bool _usersTableInitialized;
    private const string EnsureUsersTableSql = @"
        CREATE TABLE IF NOT EXISTS users (
            id SERIAL PRIMARY KEY,
            name TEXT NOT NULL,
            email TEXT NOT NULL UNIQUE,
            passwordhash TEXT NOT NULL
        );";

    public UserRepository(IDbConnection db)
    {
        _db = db;
    }

    private async Task EnsureUsersTableAsync()
    {
        if (_usersTableInitialized)
        {
            return;
        }

        await InitializationLock.WaitAsync();
        try
        {
            if (_usersTableInitialized)
            {
                return;
            }

            await _db.ExecuteAsync(EnsureUsersTableSql);
            _usersTableInitialized = true;
        }
        finally
        {
            InitializationLock.Release();
        }
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        await EnsureUsersTableAsync();

        const string sql = "SELECT id AS Id, name AS Name, email AS Email, passwordhash AS PasswordHash FROM users WHERE id = @Id";
        return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        await EnsureUsersTableAsync();

        const string sql = "SELECT id AS Id, name AS Name, email AS Email, passwordhash AS PasswordHash FROM users WHERE email = @Email";
        return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task<User?> GetByNameAsync(string name)
    {
        await EnsureUsersTableAsync();

        const string sql = "SELECT id AS Id, name AS Name, email AS Email, passwordhash AS PasswordHash FROM users WHERE name = @Name";
        return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Name = name });
    }

    public async Task<int> CreateAsync(User user)
    {
        await EnsureUsersTableAsync();

        const string sql = @"
            INSERT INTO users (name, email, passwordhash)
            VALUES (@Name, @Email, @PasswordHash)
            RETURNING id;";

        var id = await _db.ExecuteScalarAsync<int>(sql, user);
        return id;
    }

    public async Task UpdateAsync(User user)
    {
        await EnsureUsersTableAsync();

        const string sql = "UPDATE users SET name=@Name, email=@Email, passwordhash=@PasswordHash WHERE id=@Id";
        await _db.ExecuteAsync(sql, user);
    }
}
