using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity;
internal class ApplicationUserManager : UserManager<ApplicationUser>
{
    private readonly UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid, IdentityUserClaim<Guid>, ApplicationUserRole, IdentityUserLogin<Guid>, IdentityUserToken<Guid>, IdentityRoleClaim<Guid>> _userStore;

    //protected readonly IServiceProvider _services;

    public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _userStore = (UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid, IdentityUserClaim<Guid>, ApplicationUserRole, IdentityUserLogin<Guid>, IdentityUserToken<Guid>, IdentityRoleClaim<Guid>>)store;

        //_services = services;
    }

    public async Task<IdentityResult> CreateAndAddToRoleTransactionAsync(ApplicationUser user, string password, string role)
    {
        ThrowIfDisposed();
        //var passwordStore = GetPasswordStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }
        var result = await UpdatePasswordHash(user, password, true).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        return await CreateAndAddToRoleTransactionAsync(user, role).ConfigureAwait(false);
    }

    public async Task<IdentityResult> CreateAndAddToRoleTransactionAsync(ApplicationUser user, string role)
    {
        //CREATING USER
        ThrowIfDisposed();
        await UpdateSecurityStampInternal(user).ConfigureAwait(false);
        var result = await ValidateUserAsync(user).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        if (Options.Lockout.AllowedForNewUsers && SupportsUserLockout)
        {
            await GetUserLockoutStore().SetLockoutEnabledAsync(user, true, CancellationToken).ConfigureAwait(false);
        }
        await UpdateNormalizedUserNameAsync(user).ConfigureAwait(false);
        await UpdateNormalizedEmailAsync(user).ConfigureAwait(false);

        using var transaction = _userStore.Context.Database.BeginTransaction().GetDbTransaction();
        //_userStore.AutoSaveChanges = false;
        try
        {
            await _userStore.CreateAsync(user, CancellationToken).ConfigureAwait(false);

            //ADDING TO ROLE
            var userRoleStore = GetUserRoleStore();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var normalizedRole = NormalizeName(role);
            if (await userRoleStore.IsInRoleAsync(user, normalizedRole, CancellationToken).ConfigureAwait(false))
            {
                return UserAlreadyInRoleError(role);
            }
            await userRoleStore.AddToRoleAsync(user, normalizedRole, CancellationToken).ConfigureAwait(false);
            //_userStore.AutoSaveChanges = true;
            result = await UpdateUserAsync(user).ConfigureAwait(false);
            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
        return result;
    }

    //To reduce queries to database made by UserClaimsPrincipalFactory
    public override bool SupportsUserClaim => false;

    // LIBRARY CODE STARTS
    internal static class LoggerEventIds
    {
        public static readonly EventId RoleValidationFailed = new(0, "RoleValidationFailed");
        public static readonly EventId InvalidPassword = new(0, "InvalidPassword");
        public static readonly EventId UserAlreadyHasPassword = new(1, "UserAlreadyHasPassword");
        public static readonly EventId ChangePasswordFailed = new(2, "ChangePasswordFailed");
        public static readonly EventId AddLoginFailed = new(4, "AddLoginFailed");
        public static readonly EventId UserAlreadyInRole = new(5, "UserAlreadyInRole");
        public static readonly EventId UserNotInRole = new(6, "UserNotInRole");
        public static readonly EventId PhoneNumberChanged = new(7, "PhoneNumberChanged");
        public static readonly EventId VerifyUserTokenFailed = new(9, "VerifyUserTokenFailed");
        public static readonly EventId VerifyTwoFactorTokenFailed = new(10, "VerifyTwoFactorTokenFailed");
        public static readonly EventId LockoutFailed = new(11, "LockoutFailed");
        public static readonly EventId UserLockedOut = new(12, "UserLockedOut");
        public static readonly EventId UserValidationFailed = new(13, "UserValidationFailed");
        public static readonly EventId PasswordValidationFailed = new(14, "PasswordValidationFailed");
        public static readonly EventId GetSecurityStampFailed = new(15, "GetSecurityStampFailed");
    }
    private IUserRoleStore<ApplicationUser> GetUserRoleStore()
    {
        if (Store is not IUserRoleStore<ApplicationUser> cast)
        {
            //throw new NotSupportedException(Resources.StoreNotIUserRoleStore);
            throw new NotSupportedException("StoreNotIUserRoleStore");
        }
        return cast;
    }
    private IdentityResult UserAlreadyInRoleError(string role)
    {
        Logger.LogDebug(LoggerEventIds.UserAlreadyInRole, "ApplicationUser is already in role {role}.", role);
        return IdentityResult.Failed(ErrorDescriber.UserAlreadyInRole(role));
    }
    private IUserSecurityStampStore<ApplicationUser> GetSecurityStore()
    {
        if (Store is not IUserSecurityStampStore<ApplicationUser> cast)
        {
            //throw new NotSupportedException(Resources.StoreNotIUserSecurityStampStore);
            throw new NotSupportedException("StoreNotIUserSecurityStampStore");
        }
        return cast;
    }
    internal static class Base32
    {
#pragma warning disable IDE1006 // Naming Styles
        private const string _base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
#pragma warning restore IDE1006 // Naming Styles

        public static string ToBase32(byte[] input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            StringBuilder sb = new();
            for (int offset = 0; offset < input.Length;)
            {
#pragma warning disable IDE0018 // Inline variable declaration
                byte a, b, c, d, e, f, g, h;
#pragma warning restore IDE0018 // Inline variable declaration
                int numCharsToOutput = GetNextGroup(input, ref offset, out a, out b, out c, out d, out e, out f, out g, out h);

                sb.Append((numCharsToOutput >= 1) ? _base32Chars[a] : '=');
                sb.Append((numCharsToOutput >= 2) ? _base32Chars[b] : '=');
                sb.Append((numCharsToOutput >= 3) ? _base32Chars[c] : '=');
                sb.Append((numCharsToOutput >= 4) ? _base32Chars[d] : '=');
                sb.Append((numCharsToOutput >= 5) ? _base32Chars[e] : '=');
                sb.Append((numCharsToOutput >= 6) ? _base32Chars[f] : '=');
                sb.Append((numCharsToOutput >= 7) ? _base32Chars[g] : '=');
                sb.Append((numCharsToOutput >= 8) ? _base32Chars[h] : '=');
            }

            return sb.ToString();
        }

        public static byte[] FromBase32(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            var trimmedInput = input.AsSpan().TrimEnd('=');
            if (trimmedInput.Length == 0)
            {
                return Array.Empty<byte>();
            }

            var output = new byte[trimmedInput.Length * 5 / 8];
            var bitIndex = 0;
            var inputIndex = 0;
            var outputBits = 0;
            var outputIndex = 0;
            while (outputIndex < output.Length)
            {
                var byteIndex = _base32Chars.IndexOf(char.ToUpperInvariant(trimmedInput[inputIndex]));
                if (byteIndex < 0)
                {
                    throw new FormatException();
                }

                var bits = Math.Min(5 - bitIndex, 8 - outputBits);
                output[outputIndex] <<= bits;
                output[outputIndex] |= (byte)(byteIndex >> (5 - (bitIndex + bits)));

                bitIndex += bits;
                if (bitIndex >= 5)
                {
                    inputIndex++;
                    bitIndex = 0;
                }

                outputBits += bits;
                if (outputBits >= 8)
                {
                    outputIndex++;
                    outputBits = 0;
                }
            }
            return output;
        }

        // returns the number of bytes that were output
        private static int GetNextGroup(byte[] input, ref int offset, out byte a, out byte b, out byte c, out byte d, out byte e, out byte f, out byte g, out byte h)
        {
            uint b1, b2, b3, b4, b5;

            int retVal;
#pragma warning disable IDE0066 // Convert switch statement to expression
            switch (input.Length - offset)
            {
                case 1: retVal = 2; break;
                case 2: retVal = 4; break;
                case 3: retVal = 5; break;
                case 4: retVal = 7; break;
                default: retVal = 8; break;
            }
#pragma warning restore IDE0066 // Convert switch statement to expression

            b1 = (offset < input.Length) ? input[offset++] : 0U;
            b2 = (offset < input.Length) ? input[offset++] : 0U;
            b3 = (offset < input.Length) ? input[offset++] : 0U;
            b4 = (offset < input.Length) ? input[offset++] : 0U;
            b5 = (offset < input.Length) ? input[offset++] : 0U;

            a = (byte)(b1 >> 3);
            b = (byte)(((b1 & 0x07) << 2) | (b2 >> 6));
            c = (byte)((b2 >> 1) & 0x1f);
            d = (byte)(((b2 & 0x01) << 4) | (b3 >> 4));
            e = (byte)(((b3 & 0x0f) << 1) | (b4 >> 7));
            f = (byte)((b4 >> 2) & 0x1f);
            g = (byte)(((b4 & 0x3) << 3) | (b5 >> 5));
            h = (byte)(b5 & 0x1f);

            return retVal;
        }
    }
    private static string NewSecurityStamp()
    {
        byte[] bytes = new byte[20];
#if NETSTANDARD2_0 || NETFRAMEWORK
        _rng.GetBytes(bytes);
#else
        RandomNumberGenerator.Fill(bytes);
#endif
        return Base32.ToBase32(bytes);
    }
    private async Task UpdateSecurityStampInternal(ApplicationUser user)
    {
        if (SupportsUserSecurityStamp)
        {
            await GetSecurityStore().SetSecurityStampAsync(user, NewSecurityStamp(), CancellationToken).ConfigureAwait(false);
        }
    }
    private IUserLockoutStore<ApplicationUser> GetUserLockoutStore()
    {
        if (Store is not IUserLockoutStore<ApplicationUser> cast)
        {
            //throw new NotSupportedException(Resources.StoreNotIUserLockoutStore);
            throw new NotSupportedException("StoreNotIUserLockoutStore");
        }
        return cast;
    }
    //LIBRARY CODE ENDS HERE
}
