using APIDocumentationSample.Models;

namespace APIDocumentationSample.Interfaces;

public interface ICarBusinessLogic
{
    Task<List<Car>> GetCarDetailsAsync();
}
