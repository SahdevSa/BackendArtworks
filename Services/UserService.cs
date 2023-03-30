using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using ArtworkProvider.Backend.Models;
using System.Text;
using System.Security.Cryptography;
using Timer = System.Timers.Timer;
using ArtworkProvider.Backend.Providers;

namespace ArtworkProvider.Backend.Services;

public class UserService
{
    private readonly IMongoCollection<UsersModel> _UsersCollection;
    private static Dictionary<string, UserSessionInfo> m_validSessions = new Dictionary<string, UserSessionInfo>();
    private const int SALT_SIZE = 16;
    private const int DISCONNECT_TIMER = 14400000;

    public UserService(IOptions<MongoDBSettings> MongoDBSettings)
    {
        MongoClient client = new MongoClient(MongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(MongoDBSettings.Value.DatabaseName);
        _UsersCollection = database.GetCollection<UsersModel>(MongoDBSettings.Value.UsersCollection);
    }

    public async Task CreateUser(UsersModel UsersModel)
    {
        await _UsersCollection.InsertOneAsync(UsersModel);
        return;
    }

    public async Task<List<UsersModel>> GetUsers()
    {
        return await _UsersCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<LoginResult> AuthenticateUser(string username, string password)
    {
        var users = await GetUsers();
        return AuthenticateCore(users, username, password);
    }

    public bool IsValidToken(string token)
    {
        return m_validSessions.ContainsKey(token);
    }

    #region private methods

    private LoginResult AuthenticateCore(List<UsersModel> users, string username, string password, bool doNotGenerateToken = false)
    {
        var user = users.FirstOrDefault(u => u.Username == username);
        if (user != null && user.Active)
        {
            if ((user.RemoveDate == DateTime.MinValue || user.RemoveDate.CompareTo(DateTime.Now) > 0) && PasswordMatches(password, user.Password))
            {
                return new LoginResult
                {
                    Result = LoginResult.ResultType.Successful,
                    Token = doNotGenerateToken ? null : GenerateUserSession(username),

                };
            }
        }

        return new LoginResult
        {
            Result = LoginResult.ResultType.UserNotFound
        };
    }
    private string ToHexString(byte[] buff)
    {
        return BitConverter.ToString(buff).Replace("-", "");
    }

    private bool PasswordMatches(string password, string hashedPassword)
    {
        var salt = hashedPassword.Substring(0, SALT_SIZE * 2);

        return SaltAndHashPassword(password, salt) == hashedPassword;
    }

    private string SaltAndHashPassword(string password, string? salt = null)
    {
        if (salt == null)
            salt = CreateSalt(SALT_SIZE);

        byte[] bytes = Encoding.UTF8.GetBytes(password + salt);
        SHA256 sHA256ManagedString = SHA256.Create();
        byte[] hash = sHA256ManagedString.ComputeHash(bytes);
        return salt + ToHexString(hash);
    }

    private string CreateSalt(int size)
    {
        //Generate a cryptographic random number.
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] buff = new byte[size];
        rng.GetBytes(buff);
        return ToHexString(buff);
    }

    private Guid? GenerateUserSession(string username, bool isLdapAuth = false, string bestMatchedGroup = null)
    {
        var guid = Guid.NewGuid();
        var timer = CreateDisconnectTimer(guid.ToString());
        m_validSessions.Add(guid.ToString(), new UserSessionInfo()
        {
            Username = username,
            IsLdapAuthenticated = isLdapAuth,
            BestMatchedGroup = bestMatchedGroup,
            Timer = timer
        });
        timer.Start();
        return guid;
    }

    private Timer CreateDisconnectTimer(string token)
    {
        var t = new Timer(DISCONNECT_TIMER);
        t.AutoReset = false;
        t.Elapsed += (sender, e) =>
        {
            if (IsValidToken(token))
            {
                m_validSessions.Remove(token);
            }
            t.Dispose();
        };
        return t;
    }
    #endregion
}

public class UserSessionInfo
{
    public UserSessionInfo()
    {
    }

    public string Username { get; set; }
    public bool IsLdapAuthenticated { get; set; }
    public string BestMatchedGroup { get; set; }
    public Timer Timer { get; set; }
}

public struct LoginResult
{
    public enum ResultType
    {
        Successful,
        UserNotFound,
        ClientLimitReached,
        NoGroupMatching
    }

    public ResultType Result { get; set; }
    public Guid? Token { get; set; }
}