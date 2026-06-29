using System.Collections.Generic;
using System.Threading.Tasks;
using gestionpaises.Models;

namespace gestionpaises.Repositories.Interfaces
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetAllAsync();
        Task<City?> GetByIdAsync(int id);
        Task AddAsync(City city);
        Task UpdateAsync(City city);
        Task DeleteAsync(City city);
        Task<bool> ExistsAsync(int id);
    }
}
