using EntityFrameworkTask.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityFrameworkTask.Interfaces
{
    public interface IEntityFrameworkService
    {
        Task AddRecordAsync(string value);
        Task UpdateRecordAsync(int id, string value);

        Task<List<SimpleTable>> GetAllRecordsAsync();
    }
}
