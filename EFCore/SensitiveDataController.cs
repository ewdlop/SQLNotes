using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Security.Cryptography;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using System.Text;

namespace YijingSQL.Encryption
{
    public enum TrigramProtectionLevel
    {
        Qian,   // Heaven - Highest protection
        Kun,    // Earth - Foundation data
        Zhen,   // Thunder - Dynamic data
        Xun,    // Wind - Flexible data
        Kan,    // Water - Flowing data
        Li,     // Fire - Critical data
        Gen,    // Mountain - Stable data
        Dui     // Lake - Audit data
    }

    public enum YijingState
    {
        Peace,      // Normal operations
        Danger,     // Enhanced security
        Change,     // Key rotation
        Return,     // Recovery
        Advance     // Progressive enhancement
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class TrigramProtectionAttribute : Attribute
    {
        public TrigramProtectionLevel Level { get; }
        public bool RequireEncryption { get; }

        public TrigramProtectionAttribute(TrigramProtectionLevel level, bool requireEncryption = true)
        {
            Level = level;
            RequireEncryption = requireEncryption;
        }
    }

    // Example entity with trigram protection
    public class SensitiveData
    {
        public int Id { get; set; }

        [TrigramProtection(TrigramProtectionLevel.Qian)]
        public string HighlyConfidential { get; set; }

        [TrigramProtection(TrigramProtectionLevel.Kun)]
        public string BaselineData { get; set; }

        [TrigramProtection(TrigramProtectionLevel.Li)]
        public string CriticalData { get; set; }

        public DateTime LastModified { get; set; }
    }

    public class YijingEncryptionProvider : IEncryptionProvider
    {
        private readonly Dictionary<TrigramProtectionLevel, IEncryptionProvider> _providers;
        private readonly YijingState _currentState;
        private readonly KeyClient _keyClient;
        private readonly string _keyName;

        public YijingEncryptionProvider(
            string keyVaultUrl,
            string keyName,
            YijingState initialState = YijingState.Peace)
        {
            var credential = new DefaultAzureCredential();
            _keyClient = new KeyClient(new Uri(keyVaultUrl), credential);
            _keyName = keyName;
            _currentState = initialState;

            // Initialize providers for each trigram
            _providers = InitializeProviders();
        }

        private Dictionary<TrigramProtectionLevel, IEncryptionProvider> InitializeProviders()
        {
            var providers = new Dictionary<TrigramProtectionLevel, IEncryptionProvider>();
            
            foreach (TrigramProtectionLevel level in Enum.GetValues(typeof(TrigramProtectionLevel)))
            {
                var key = _keyClient.GetKey(_keyName).Value.Key;
                var encryptionKey = new byte[32];
                var authenticationKey = new byte[32];
                
                // Derive keys for each trigram
                using (var hmac = new HMACSHA256(Convert.FromBase64String(key.K)))
                {
                    var derivedKey = hmac.ComputeHash(Encoding.UTF8.GetBytes(level.ToString()));
                    Array.Copy(derivedKey, 0, encryptionKey, 0, 16);
                    Array.Copy(derivedKey, 16, authenticationKey, 0, 16);
                }

                providers[level] = new AesProvider(encryptionKey, authenticationKey);
            }

            return providers;
        }

        public string Encrypt(string plainText, string purpose = null)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            var level = GetProtectionLevel(purpose);
            return _providers[level].Encrypt(plainText, purpose);
        }

        public string Decrypt(string cipherText, string purpose = null)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            var level = GetProtectionLevel(purpose);
            return _providers[level].Decrypt(cipherText, purpose);
        }

        private TrigramProtectionLevel GetProtectionLevel(string purpose)
        {
            if (Enum.TryParse<TrigramProtectionLevel>(purpose, out var level))
                return level;
            
            return TrigramProtectionLevel.Kun; // Default to foundation level
        }
    }

    public class YijingDbContext : DbContext
    {
        private readonly YijingEncryptionProvider _encryptionProvider;
        private readonly YijingState _currentState;

        public YijingDbContext(
            DbContextOptions<YijingDbContext> options,
            YijingEncryptionProvider encryptionProvider,
            YijingState initialState = YijingState.Peace)
            : base(options)
        {
            _encryptionProvider = encryptionProvider;
            _currentState = initialState;
        }

        public DbSet<SensitiveData> SensitiveData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply encryption to properties with TrigramProtection attribute
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    var attribute = property.PropertyInfo?.GetCustomAttribute<TrigramProtectionAttribute>();
                    if (attribute != null && attribute.RequireEncryption)
                    {
                        // Create converter for the property
                        var converter = new ValueConverter<string, string>(
                            v => _encryptionProvider.Encrypt(v, attribute.Level.ToString()),
                            v => _encryptionProvider.Decrypt(v, attribute.Level.ToString())
                        );

                        property.SetValueConverter(converter);
                    }
                }
            }

            // Additional security based on current state
            if (_currentState == YijingState.Danger || _currentState == YijingState.Change)
            {
                // Enable row-level security
                modelBuilder.Entity<SensitiveData>()
                    .HasQueryFilter(e => EF.Property<string>(e, "UserScope") == _currentUserScope);
            }
        }

        private string _currentUserScope => 
            // Implementation for getting current user scope
            "default";
    }

    // Extension methods for easier configuration
    public static class YijingEncryptionExtensions
    {
        public static IServiceCollection AddYijingEncryption(
            this IServiceCollection services,
            string keyVaultUrl,
            string keyName,
            YijingState initialState = YijingState.Peace)
        {
            services.AddSingleton(new YijingEncryptionProvider(keyVaultUrl, keyName, initialState));
            return services;
        }
    }

    // Example repository implementing the encryption
    public class SensitiveDataRepository
    {
        private readonly YijingDbContext _context;
        private YijingState _currentState;

        public SensitiveDataRepository(YijingDbContext context)
        {
            _context = context;
            _currentState = YijingState.Peace;
        }

        public async Task<SensitiveData> CreateAsync(SensitiveData data)
        {
            data.LastModified = DateTime.UtcNow;
            _context.SensitiveData.Add(data);
            await _context.SaveChangesAsync();
            return data;
        }

        public async Task<SensitiveData> GetByIdAsync(int id)
        {
            return await _context.SensitiveData.FindAsync(id);
        }

        public async Task UpdateAsync(SensitiveData data)
        {
            data.LastModified = DateTime.UtcNow;
            _context.SensitiveData.Update(data);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var data = await GetByIdAsync(id);
            if (data != null)
            {
                _context.SensitiveData.Remove(data);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateSecurityStateAsync(YijingState newState)
        {
            _currentState = newState;
            
            // Handle state-specific actions
            switch (newState)
            {
                case YijingState.Change:
                    await RotateEncryptionKeysAsync();
                    break;
                case YijingState.Danger:
                    await EnhanceSecurityAsync();
                    break;
                case YijingState.Return:
                    await RestoreNormalOperationsAsync();
                    break;
            }
        }

        private async Task RotateEncryptionKeysAsync()
        {
            // Implement key rotation logic
        }

        private async Task EnhanceSecurityAsync()
        {
            // Implement enhanced security measures
        }

        private async Task RestoreNormalOperationsAsync()
        {
            // Implement normal operations restoration
        }
    }
}

// Example usage in Program.cs or Startup.cs
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Configure Yijing encryption
        services.AddYijingEncryption(
            keyVaultUrl: "https://your-keyvault.vault.azure.net/",
            keyName: "YourKeyName"
        );

        // Configure DbContext
        services.AddDbContext<YijingDbContext>((provider, options) =>
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            var encryptionProvider = provider.GetRequiredService<YijingEncryptionProvider>();
            return new YijingDbContext(options, encryptionProvider);
        });
    }
}

// Example controller usage
public class SensitiveDataController : ControllerBase
{
    private readonly SensitiveDataRepository _repository;

    public SensitiveDataController(SensitiveDataRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SensitiveData data)
    {
        var result = await _repository.CreateAsync(data);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _repository.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] SensitiveData data)
    {
        await _repository.UpdateAsync(data);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return Ok();
    }

    [HttpPost("security-state")]
    public async Task<IActionResult> UpdateSecurityState([FromBody] YijingState newState)
    {
        await _repository.UpdateSecurityStateAsync(newState);
        return Ok();
    }
}
