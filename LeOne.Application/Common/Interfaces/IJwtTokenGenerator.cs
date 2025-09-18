using LeOne.Domain.Entities;

namespace LeOne.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Generates a short-lived access token (JWT) for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the access token is generated.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        ///   <item>
        ///     <description><see cref="string"/> <c>Token</c>: The signed JWT string that should be used as a Bearer token in the <c>Authorization</c> header.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="DateTimeOffset"/> <c>ExpiresAt</c>: The UTC time when the access token expires.</description>
        ///   </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// The generated token is intended for authorizing API requests and typically has a short lifetime
        /// (e.g., 5–30 minutes). It may include claims such as <c>sub</c> (subject), <c>jti</c> (token id),
        /// <c>iat</c> (issued at), role/permissions, and a custom <c>token_use=access</c>.
        /// </remarks>
        /// <example>
        /// <code>
        /// var (access, accessExp) = jwt.GenerateAccessToken(user);
        /// httpClient.DefaultRequestHeaders.Authorization =
        ///     new AuthenticationHeaderValue("Bearer", access);
        /// </code>
        /// </example>
        (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(User user);

        /// <summary>
        /// Generates a long-lived refresh token (JWT) for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the refresh token is generated.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        ///   <item>
        ///     <description><see cref="string"/> <c>Token</c>: The signed JWT string to be stored securely (e.g., HttpOnly cookie).</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="DateTimeOffset"/> <c>ExpiresAt</c>: The UTC time when the refresh token expires.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="string"/> <c>Jti</c>: A unique token identifier (JWT ID) used for rotation and revocation.</description>
        ///   </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// Refresh tokens should contain minimal claims (e.g., <c>sub</c>, <c>jti</c>, <c>iat</c>, and <c>token_use=refresh</c>)
        /// and are used to obtain new access tokens without re-authentication. Store them securely and rotate on every use.
        /// </remarks>
        /// <example>
        /// <code>
        /// var (refresh, refreshExp, jti) = jwt.GenerateRefreshToken(user);
        /// // persist the (userId, jti, refreshExp) in your token/session store for rotation &amp; revocation
        /// </code>
        /// </example>
        (string Token, DateTimeOffset ExpiresAt, string Jti) GenerateRefreshToken(User user);

        /// <summary>
        /// Validates the provided refresh token and ensures it is indeed a refresh token.
        /// </summary>
        /// <param name="token">The refresh token (JWT) to validate.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        ///   <item>
        ///     <description><see cref="Guid?"/> <c>UserId</c>: The user identifier extracted from the token's <c>sub</c> claim, or <c>null</c> if invalid.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="string"/>? <c>Jti</c>: The token's unique identifier, or <c>null</c> if invalid.</description>
        ///   </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// This method performs signature, issuer, audience, lifetime validation and additionally checks a custom claim
        /// (e.g., <c>token_use</c>) to ensure the token is of type <c>refresh</c>. For full protection, combine this with
        /// server-side checks against your refresh-token store (rotation &amp; reuse detection, revocation).
        /// </remarks>
        /// <example>
        /// <code>
        /// var (userId, jti) = jwt.ValidateRefreshTokenRich(tokenFromClient);
        /// if (userId is null || jti is null) return Unauthorized();
        /// // Lookup jti in DB, enforce rotation, issue new tokens, revoke old one, etc.
        /// </code>
        /// </example>
        (Guid? UserId, string? Jti) ValidateRefreshTokenRich(string token);
    }
}
