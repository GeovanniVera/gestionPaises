using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gestionpaises.Data;
using gestionpaises.Models;
using gestionpaises.Repositories.Interfaces;

namespace gestionpaises.Repositories.Implementations
{
    public class CountryLanguageRepository : ICountryLanguageRepository
    {
        private readonly ApplicationDbContext _context;

        public CountryLanguageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CountryLanguage>> GetAllAsync()
        {
            return await _context.CountryLanguages
                .AsNoTracking()
                .Include(cl => cl.Country)
                .ToListAsync();
        }

        public async Task<CountryLanguage?> GetByKeyAsync(string countryCode, string language)
        {
            return await _context.CountryLanguages
                .AsNoTracking()
                .Include(cl => cl.Country)
                .FirstOrDefaultAsync(cl => cl.CountryCode == countryCode && cl.Language == language);
        }

        public async Task AddAsync(CountryLanguage countryLanguage)
        {
            await _context.CountryLanguages.AddAsync(countryLanguage);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CountryLanguage countryLanguage)
        {
            _context.CountryLanguages.Update(countryLanguage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CountryLanguage countryLanguage)
        {
            _context.CountryLanguages.Remove(countryLanguage);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string countryCode, string language)
        {
            return await _context.CountryLanguages.AsNoTracking().AnyAsync(e => e.CountryCode == countryCode && e.Language == language);
        }
    }
}
