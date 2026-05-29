using APIDocumentationSample.Models;

namespace APIDocumentationSample.Interfaces;

public interface ICarDataAccess
{
    Task<List<Car>> GetCarsFromStoredProcedureAsync();
}
