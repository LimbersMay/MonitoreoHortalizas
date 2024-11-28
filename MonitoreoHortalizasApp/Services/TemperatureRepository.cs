using Dapper;
using GestionHortalizasApp.entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace MonitoreoHortalizasApp.Services;

public interface ITemperatureRepository
{
    Task<IEnumerable<Temperature>> GetReadings();
    Task<int> AddReading(Temperature temperature);
}

public class TemperatureRepository: ITemperatureRepository
{
    private readonly string _connectionString;
    
    public TemperatureRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<IEnumerable<Temperature>> GetReadings()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryAsync<Temperature>("SELECT * FROM temperatura ORDER BY fecha DESC, hora DESC LIMIT 500");
        return result;
    }

    public async Task<int> AddReading(Temperature temperature)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.ExecuteAsync("INSERT INTO temperatura (temperatura, humedad, fecha, hora) VALUES (@temperatura, @humedad, @fecha, @hora)", temperature);
        return result;
    }
}