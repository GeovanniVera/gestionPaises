using System.Collections.Generic;
using System.Threading.Tasks;
using gestionpaises.Models;

namespace gestionpaises.Repositories.Interfaces
{
    public interface ICountryRepository
    {
        Task<IEnumerable<Country>> GetAllAsync();
        Task<Country?> GetByCodeAsync(string code);
        Task<Country?> GetByNameAsync(string name);
        Task<Country?> GetCountryDetailsAsync(string code);
        Task<Country?> GetCountryReportByCodeAsync(string code);
        Task<Country?> GetCountryReportByNameAsync(string name);
        Task AddAsync(Country country);
        Task UpdateAsync(Country country);
        Task DeleteAsync(Country country);
        Task<bool> ExistsAsync(string code);
    }
}
