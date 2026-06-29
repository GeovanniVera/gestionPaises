using System.Collections.Generic;
using System.Threading.Tasks;
using gestionpaises.Models;

namespace gestionpaises.Repositories.Interfaces
{
    public interface ICountryLanguageRepository
    {
        Task<IEnumerable<CountryLanguage>> GetAllAsync();
        Task<CountryLanguage?> GetByKeyAsync(string countryCode, string language);
        Task AddAsync(CountryLanguage countryLanguage);
        Task UpdateAsync(CountryLanguage countryLanguage);
        Task DeleteAsync(CountryLanguage countryLanguage);
        Task<bool> ExistsAsync(string countryCode, string language);
    }
}
