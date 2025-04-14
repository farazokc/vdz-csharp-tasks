using EntityFrameworkTask.Context;
using EntityFrameworkTask.Interfaces;
using EntityFrameworkTask.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityFrameworkTask.Services
{
    public class EntityFrameworkService: IEntityFrameworkService
    {
        private readonly onboardingContext _context;

        public EntityFrameworkService(onboardingContext context)
        {
            _context = context;
        }

        public async Task AddRecordAsync(string value)
        {
            var record = new SimpleTable {Value = value };

            try
            {
                _context.SimpleTables.Add(record);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in inserting data: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateRecordAsync(int id, string value)
        {
            var record = new SimpleTable { Id = id, Value = value };
            
            _context.SimpleTables.Attach(record);

            _context.Entry(record).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<List<SimpleTable>> GetAllRecordsAsync()
        {
            try
            {
                return await _context.SimpleTables.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in fetching data: {ex.Message}");
                throw;
            }
        }
    }
}

