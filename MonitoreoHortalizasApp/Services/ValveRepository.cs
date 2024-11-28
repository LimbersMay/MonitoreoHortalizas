using Dapper;
using GestionHortalizasApp.entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace MonitoreoHortalizasApp.Services;

public interface IValveRepository
{
    Task<IEnumerable<Valve>> GetReadings();
    Task<int> AddReading(Valve barometricPressure);
    
    Task<IEnumerable<Valve>> GetManualWateringReadings();
    Task<int> AddManualWateringReading(Valve barometricPressure);
}

public class ValveRepository: IValveRepository
{
    private string _connectionString;

    public ValveRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<IEnumerable<Valve>> GetReadings()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryAsync<Valve>(@"
            SELECT fechaEncendido, fechaApagado, volumen, cultivo.nombreCultivo as NombreSembrado 
            FROM valvula INNER JOIN cultivo ON valvula.cultivoId = cultivo.cultivoId ORDER BY fechaEncendido DESC LIMIT 500");
        return result;
    }
    
    public async Task<int> AddReading(Valve waterFlowReading)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.ExecuteAsync("INSERT INTO valvula (fechaEncendido, fechaApagado, volumen, cultivoId) VALUES (@fechaEncendido, @fechaApagado, @volumen, @cultivoId)", waterFlowReading);
        return result;
    }
    
    public async Task<IEnumerable<Valve>> GetManualWateringReadings()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryAsync<Valve>(@"
            SELECT fechaEncendido, fechaApagado, volumen, cultivo.nombreCultivo as NombreSembrado 
            FROM riegoManual INNER JOIN cultivo ON riegoManual.cultivoId = cultivo.cultivoId ORDER BY fechaEncendido DESC LIMIT 500");
        return result;
    }
    
    public async Task<int> AddManualWateringReading(Valve waterFlowReading)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.ExecuteAsync("INSERT INTO riegoManual (fechaEncendido, fechaApagado, volumen, cultivoId) VALUES (@fechaEncendido, @fechaApagado, @volumen, @cultivoId)", waterFlowReading);
        return result;
    }
}