using Augustus.Infra.Core.DAL.Redis;
using Augustus.Infra.Core.Shared.Interfaces;
using Coucher.Shared;
using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.Internal.Authentication;
using StackExchange.Redis;

namespace Coucher.Lib.DAL.Providers;

public sealed class AuthorizationCacheRedisProvider : CommonRedisProvider, IAuthorizationCacheProvider
{
    private readonly string _servicePrefix;
    private readonly string _sessionIdPrefix;
    private readonly string _userIdPrefix;
    private readonly TimeSpan _sessionDuration;
    private readonly TimeSpan _sessionDurationExpansion;
    private readonly LuaScript _getUserIdBySessionAndExpandDurationScript;

    public AuthorizationCacheRedisProvider(
        RedisCommunicationFactory factory,
        IAugustusLogger logger,
        IAugustusConfiguration config
    ) : base(ConfigurationKeys.RedisSection, factory, logger)
    {
        _servicePrefix = config.GetOrThrow<string>(
            ConfigurationKeys.AuthenticationSection,
            ConfigurationKeys.ServicePrefix
        ).TrimEnd(ConstantValues.RedisKeySeparator);

        _sessionIdPrefix = config.GetOrThrow<string>(
            ConfigurationKeys.AuthenticationSection,
            ConfigurationKeys.SessionIdPrefix
        ).TrimEnd(ConstantValues.RedisKeySeparator);

        _userIdPrefix = config.GetOrThrow<string>(
            ConfigurationKeys.AuthenticationSection,
            ConfigurationKeys.UserIdPrefix
        ).TrimEnd(ConstantValues.RedisKeySeparator);

        _sessionDuration = TimeSpan.FromHours(config.GetOrThrow<int>(
            ConfigurationKeys.AuthenticationSection,
            ConfigurationKeys.SessionDurationInHours
        ));

        _sessionDurationExpansion = TimeSpan.FromSeconds(config.GetOrThrow<int>(
            ConfigurationKeys.AuthenticationSection,
            ConfigurationKeys.SessionDurationExpansionInSeconds
        ));

        _getUserIdBySessionAndExpandDurationScript = BuildGetUserIdBySessionAndExpandDurationScript();
    }

    public async Task<string> CreateUserSessionAsync(Guid userId)
    {
        var sessionId = Guid.NewGuid().ToString();
        var sessionKey = BuildSessionKey(sessionId);
        var userKey = BuildUserKey(userId);
        var transaction = RedisConnection.Database.CreateTransaction();

        _ = transaction.StringSetAsync(sessionKey, userId.ToString(), _sessionDuration);
        _ = transaction.StringSetAsync(userKey, sessionId, _sessionDuration);
        await transaction.ExecuteAsync();

        return sessionId;
    }

    public async Task RemoveUserSessionByUserIdAsync(Guid userId)
    {
        var userKey = BuildUserKey(userId);
        var sessionId = await GetStringAsync(userKey);
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return;
        }

        var sessionKey = BuildSessionKey(sessionId);
        var transaction = RedisConnection.Database.CreateTransaction();

        _ = transaction.KeyDeleteAsync(sessionKey);
        _ = transaction.KeyDeleteAsync(userKey);
        await transaction.ExecuteAsync();
    }

    public async Task<UserAuthenticationResult> GetUserAuthenticationResultBySessionAsync(string sessionId)
    {
        var sessionKey = BuildSessionKey(sessionId);
        var scriptResult = await RedisConnection.Database.ScriptEvaluateAsync(
            _getUserIdBySessionAndExpandDurationScript,
            new
            {
                sessionKey,
                durationExpansionInSeconds = _sessionDurationExpansion.TotalSeconds
            }
        );

        var redisValues = (RedisValue[])scriptResult!;
        if (!redisValues.Any() || redisValues[0].IsNullOrEmpty)
        {
            return new UserAuthenticationResult { IsValid = false };
        }

        var userIdValue = redisValues[0].ToString();
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return new UserAuthenticationResult { IsValid = false };
        }

        var userKey = BuildUserKey(userId);
        await RedisConnection.Database.KeyExpireAsync(userKey, _sessionDuration);

        return new UserAuthenticationResult
        {
            UserId = userId,
            IsValid = true
        };
    }

    private string BuildJoinedKey(params string[] parts)
    {
        var key = string.Join(ConstantValues.RedisKeySeparator, new[] { _servicePrefix }.Concat(parts));

        return key;
    }

    private string BuildSessionKey(string sessionId)
    {
        var key = BuildJoinedKey(_sessionIdPrefix, sessionId);

        return key;
    }

    private string BuildUserKey(Guid userId)
    {
        var key = BuildJoinedKey(_userIdPrefix, userId.ToString());

        return key;
    }

    private static LuaScript BuildGetUserIdBySessionAndExpandDurationScript()
    {
        var stringScript =
            """
            local value = redis.call('GET', @sessionKey)

            if value == nil then
                return value
            end

            local ttl = redis.call('TTL', @sessionKey)
            local durationExpansion = tonumber(@durationExpansionInSeconds)

            if ttl < durationExpansion then
                redis.call('EXPIRE', @sessionKey, durationExpansion)
            end

            return value
            """;

        var script = LuaScript.Prepare(stringScript);

        return script;
    }
}
