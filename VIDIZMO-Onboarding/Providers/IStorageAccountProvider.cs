using Onboarding.Models;
using System.Threading.Tasks;

namespace Onboarding.Providers
{
    public interface IStorageAccountProvider
    {
        StorageAccountConfig Config { get; }
        StorageAccountConfig GetStorageAccountConfig();
        Task<bool> AuthenticateAsync();
    }
}