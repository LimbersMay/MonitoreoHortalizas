using Dapper;
using GestionHortalizasApp.entities;
using MySql.Data.MySqlClient;

namespace MonitoreoHortalizasApp.Services;

public interface IBarometricRepository
{
    Task<IEnumerable<BarometricPressure>> GetReadings();
    Task<int> AddReading(BarometricPressure barometricPressure);
}

public class BarometricRepository: IBarometricRepository
{

    private readonly string _connectionString;
    
    public BarometricRepository(Settings settings)
    {
        _connectionString = settings.DefaultConnection;
    }
    
    public async Task<IEnumerable<BarometricPressure>> GetReadings()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryAsync<BarometricPressure>("SELECT * FROM presionBarometrica ORDER BY fecha DESC, hora DESC");
        return result;
    }

    public async Task<int> AddReading(BarometricPressure barometricPressure)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.ExecuteAsync("INSERT INTO presionBarometrica (presion, temperatura, altitud, fecha, hora) VALUES (@presion, @temperatura, @altitud, @fecha, @hora)", barometricPressure);
        return result;
    }
}