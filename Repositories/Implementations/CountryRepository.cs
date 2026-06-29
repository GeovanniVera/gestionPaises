using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gestionpaises.Data;
using gestionpaises.Models;
using gestionpaises.Repositories.Interfaces;

namespace gestionpaises.Repositories.Implementations
{
    public class CountryRepository : ICountryRepository
    {
        private readonly ApplicationDbContext _context;

        public CountryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            return await _context.Countries
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Country?> GetByCodeAsync(string code)
        {
            return await _context.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<Country?> GetByNameAsync(string name)
        {
            return await _context.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<Country?> GetCountryDetailsAsync(string code)
        {
            return await _context.Countries
                .AsNoTracking()
                .Include(c => c.Cities)
                .Include(c => c.CountryLanguages)
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<Country?> GetCountryReportByCodeAsync(string code)
        {
            return await _context.Countries
                .AsNoTracking()
                .Include(c => c.Cities)
                .Include(c => c.CountryLanguages)
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<Country?> GetCountryReportByNameAsync(string name)
        {
            return await _context.Countries
                .AsNoTracking()
                .Include(c => c.Cities)
                .Include(c => c.CountryLanguages)
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task AddAsync(Country country)
        {
            await _context.Countries.AddAsync(country);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Country country)
        {
            _context.Countries.Update(country);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Country country)
        {
            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string code)
        {
            return await _context.Countries.AsNoTracking().AnyAsync(e => e.Code == code);
        }
    }
}
