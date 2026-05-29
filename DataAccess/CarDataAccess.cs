using APIDocumentationSample.Interfaces;
using APIDocumentationSample.Models;
using System.Data;
using System.Data.SqlClient;

namespace APIDocumentationSample.DataAccess;

public class CarDataAccess : ICarDataAccess
{
    private readonly string _connectionString;
    private readonly ILogger<CarDataAccess> _logger;

    public CarDataAccess(string connectionString, ILogger<CarDataAccess> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    /// <summary>
    /// Calls the stored procedure 'sp_GetCarDetails' to fetch car data from the database.
    /// </summary>
    public async Task<List<Car>> GetCarsFromStoredProcedureAsync()
    {
        var cars = new List<Car>();

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_GetCarDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var car = new Car
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Make = reader.IsDBNull(reader.GetOrdinal("Make")) ? null : reader.GetString(reader.GetOrdinal("Make")),
                                Model = reader.IsDBNull(reader.GetOrdinal("Model")) ? null : reader.GetString(reader.GetOrdinal("Model")),
                                Year = reader.GetInt32(reader.GetOrdinal("Year")),
                                Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? null : reader.GetString(reader.GetOrdinal("Color"))
                            };
                            cars.Add(car);
                        }
                    }
                }
            }

            _logger.LogInformation("Retrieved {Count} cars from database via sp_GetCarDetails", cars.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling sp_GetCarDetails stored procedure");
            throw;
        }

        return cars;
    }
}
