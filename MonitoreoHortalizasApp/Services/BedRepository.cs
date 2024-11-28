using Dapper;
using GestionHortalizasApp.entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace MonitoreoHortalizasApp.Services;

public interface IBedRepository
{
    // Get all the beds
    Task<IEnumerable<BedHumidity>> GetBed1HumidityLog();
    Task<IEnumerable<BedHumidity>> GetBed2HumidityLog();
    Task<IEnumerable<BedHumidity>> GetBed3HumidityLog();
    Task<IEnumerable<BedHumidity>> GetBed4HumidityLog();
    
    // Get the average of the humidity of a bed between two dates
    Task<List<int>> GetBed1HumiditiesByDates(DateTime startDate, DateTime endDate);
    Task<List<int>> GetBed2HumiditiesByDates(DateTime startDate, DateTime endDate);
    Task<List<int>> GetBed3HumiditiesByDates(DateTime startDate, DateTime endDate);
    Task<List<int>> GetBed4HumiditiesByDates(DateTime startDate, DateTime endDate);
    
    // Add a bed
    Task<int> AddBed1HumidityLog(BedHumidity bedHumidity);
    Task<int> AddBed2HumidityLog(BedHumidity bedHumidity);
    Task<int> AddBed3HumidityLog(BedHumidity bedHumidity);
    Task<int> AddBed4HumidityLog(BedHumidity bedHumidity);
    
    // Calculate the amount of water that a bed has received
    Task<decimal> CalculateBed1WaterAmount(DateTime startDate, DateTime endDate);
    Task<decimal> CalculateBed2WaterAmount(DateTime startDate, DateTime endDate);
    Task<decimal> CalculateBed3WaterAmount(DateTime startDate, DateTime endDate);
    Task<decimal> CalculateBed4WaterAmount(DateTime startDate, DateTime endDate);
}

public class BedRepository: IBedRepository
{
    private readonly string _connectionString;
    
    private const string SqlGetBeds = "SELECT humedad, fecha, hora FROM {0} ORDER BY fecha DESC, hora DESC LIMIT 500";
    private const string SqlAddBed = "INSERT INTO {0} (humedad, fecha, hora) VALUES (@humedad, @fecha, @hora)";
    
    private const string SqlGetBedByDates = "SELECT humedad FROM {0} WHERE DATE(fecha) BETWEEN DATE(@startDate) AND DATE(@endDate)";
    private const string SqlGetBedWaterAmountByDates = "SELECT SUM(Volumen) FROM valvula WHERE DATE(fechaEncendido) BETWEEN DATE(@startDate) AND DATE(@endDate) AND CultivoId = {0}";
    private const string SqlGetManualWaterAmountByDates = "SELECT SUM(Volumen) FROM riegomanual WHERE DATE(fechaEncendido) BETWEEN DATE(@startDate) AND DATE(@endDate) AND CultivoId = {0}";
        
    public BedRepository(IConfiguration configuration) {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<BedHumidity>> GetBed1HumidityLog()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetBeds, "cama1");
        var result = await connection.QueryAsync<BedHumidity>(query);
        return result;
    }

    public async Task<List<int>> GetBed1HumiditiesByDates(DateTime startDate, DateTime endDate)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetBedByDates, "cama1");
        return (await connection.QueryAsync<int>(query, new { startDate, endDate })).ToList();
    }

    public async Task<IEnumerable<BedHumidity>> GetBed2HumidityLog()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetBeds, "cama2");
        var result = await connection.QueryAsync<BedHumidity>(query);
        return result;
    }
    
    public async Task<List<int>> GetBed2HumiditiesByDates(DateTime startDate, DateTime endDate)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetBedByDates, "cama2");
        return (await connection.QueryAsync<int>(query, new { startDate, endDate })).ToList();
    }

    public async Task<IEnumerable<BedHumidity>> GetBed3HumidityLog()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetBeds, "cama3");
        var result = await connection.QueryAsync<BedHumidity>(query);
        return result;
    }
    
    public async Task<List<int>> GetBed3HumiditiesByDates(DateTime startDate, DateTime endDate)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetBedByDates, "cama3");
        return (await connection.QueryAsync<int>(query, new { startDate, endDate })).ToList();
    }

    public async Task<IEnumerable<BedHumidity>> GetBed4HumidityLog()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetBeds, "cama4");
        var result = await connection.QueryAsync<BedHumidity>(query);
        return result;
    }
    
    public async Task<List<int>> GetBed4HumiditiesByDates(DateTime startDate, DateTime endDate)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetBedByDates, "cama4");
        return (await connection.QueryAsync<int>(query, new { startDate, endDate })).ToList();
    }

    public async Task<int> AddBed1HumidityLog(BedHumidity bedHumidity)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlAddBed, "cama1");
        var result = await connection.ExecuteAsync(query, bedHumidity);
        return result;
    }

    public async Task<int> AddBed2HumidityLog(BedHumidity bedHumidity)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlAddBed, "cama2");
        var result = await connection.ExecuteAsync(query, bedHumidity);
        return result;
    }

    public async Task<int> AddBed3HumidityLog(BedHumidity bedHumidity)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlAddBed, "cama3");
        var result = await connection.ExecuteAsync(query, bedHumidity);
        return result;
    }

    public async Task<int> AddBed4HumidityLog(BedHumidity bedHumidity)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlAddBed, "cama4");
        var result = await connection.ExecuteAsync(query, bedHumidity);
        return result;
    }
    
    // CALCULATE THE AMOUNT OF WATER THAT A BED HAS RECEIVED
    public async Task<decimal> CalculateBed1WaterAmount(DateTime startDate, DateTime endDate)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetBedWaterAmountByDates, 1);
        return await connection.ExecuteScalarAsync<decimal>(query, new { startDate, endDate });
    }
    
    public async Task<decimal> CalculateBed2WaterAmount(DateTime startDate, DateTime endDate)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetBedWaterAmountByDates, 2);
        return await connection.ExecuteScalarAsync<decimal>(query, new { startDate, endDate });
    }
    
    public async Task<decimal> CalculateBed3WaterAmount(DateTime startDate, DateTime endDate)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetManualWaterAmountByDates, 3);
        return await connection.ExecuteScalarAsync<decimal>(query, new { startDate, endDate });
    }
    
    public async Task<decimal> CalculateBed4WaterAmount(DateTime startDate, DateTime endDate)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetManualWaterAmountByDates, 4);
        return await connection.ExecuteScalarAsync<decimal>(query, new { startDate, endDate });
    }
}