using Dapper;
using MonitoreoHortalizasApp.entities;
using Microsoft.Extensions.Configuration;
using MonitoreoHortalizasApp.Entities;
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
    Task<int> AddBed1HumidityLog(Bed1 bedHumidity);
    Task<int> AddBed2HumidityLog(Bed2 bedHumidity);
    Task<int> AddBed3HumidityLog(Bed3 bedHumidity);
    Task<int> AddBed4HumidityLog(Bed4 bedHumidity);
    
    // Calculate the amount of water that a bed has received
    Task<decimal> CalculateBed1WaterAmount(DateTime startDate, DateTime endDate);
    Task<decimal> CalculateBed2WaterAmount(DateTime startDate, DateTime endDate);
    Task<decimal> CalculateBed3WaterAmount(DateTime startDate, DateTime endDate);
    Task<decimal> CalculateBed4WaterAmount(DateTime startDate, DateTime endDate);

    Task UpdateBedPrimaryKey();
}

class RiegoManual : Valve
{
    public string idRiegoManual { get; set; }
}

public class BedRepository: IBedRepository
{
    private readonly string _connectionString;
    
    private const string SqlGetBeds = "SELECT humedad, fecha, hora FROM {0} ORDER BY fecha DESC, hora DESC LIMIT 500";
    private const string SqlAddBed = "INSERT INTO {0} ({1}, humedad, fecha, hora) VALUES (@{1}, @humedad, @fecha, @hora)";
    
    private const string SqlGetBedByDates = "SELECT humedad FROM {0} WHERE DATE(fecha) BETWEEN DATE(@startDate) AND DATE(@endDate)";
    private const string SqlGetBedWaterAmountByDates = "SELECT SUM(Volumen) FROM valvula WHERE DATE(fechaEncendido) BETWEEN DATE(@startDate) AND DATE(@endDate)";
    private const string SqlGetManualWaterAmountByDates = "SELECT SUM(Volumen) FROM riegomanual WHERE DATE(fechaEncendido) BETWEEN DATE(@startDate) AND DATE(@endDate)";
        
    public BedRepository(IConfiguration configuration) {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task UpdateBedPrimaryKey()
    {
        await using var connection = new MySqlConnection(_connectionString);
        
        var query = $"SELECT idCama1 FROM cama1";
        
        /*var bed1s = await connection.QueryAsync<Bed1>(query);
        
        foreach (var bed in bed1s)
        {
            Console.WriteLine("Updating bed 1: " + bed.idCama1);
            var newId = Guid.NewGuid().ToString();
            
            query = $"UPDATE cama1 SET idCama1 = '{newId}' WHERE idCama1 = '{bed.idCama1}'";
            await connection.ExecuteAsync(query);
        }
        
        query = $"SELECT idCama2 FROM cama2";
        var result2 = await connection.QueryAsync<Bed2>(query);
        
        foreach (var bed in result2)
        {
            Console.WriteLine("Updating bed 2: " + bed.idCama2);
            var newId = Guid.NewGuid().ToString();
            
            query = $"UPDATE cama2 SET idCama2 = '{newId}' WHERE idCama2 = '{bed.idCama2}'";
            await connection.ExecuteAsync(query);
        }
        
        query = $"SELECT idCama3 FROM cama3";
        var result3 = await connection.QueryAsync<Bed3>(query);
        
        foreach (var bed in result3)
        {
            Console.WriteLine("Updating bed 3: " + bed.idCama3);
            var newId = Guid.NewGuid().ToString();
            
            query = $"UPDATE cama3 SET idCama3 = '{newId}' WHERE idCama3 = '{bed.idCama3}'";
            await connection.ExecuteAsync(query);
        }
        
        query = $"SELECT idCama4 FROM cama4";
        var result4 = await connection.QueryAsync<Bed4>(query);
        
        foreach (var bed in result4)
        {
            Console.WriteLine("Updating bed 4: " + bed.idCama4);
            var newId = Guid.NewGuid().ToString();
            
            query = $"UPDATE cama4 SET idCama4 = '{newId}' WHERE idCama4 = '{bed.idCama4}'";
            await connection.ExecuteAsync(query);
        }
        
        // TEMPERATURE TABLE
        query = "SELECT idTemperatura FROM temperatura";
        var resultTemperature = await connection.QueryAsync<Temperature>(query);
        
        foreach (var temperature in resultTemperature)
        {
            Console.WriteLine("Updating temperature: " + temperature.idTemperatura);
            var newId = Guid.NewGuid().ToString();
            
            query = $"UPDATE temperatura SET idTemperatura = '{newId}' WHERE idTemperatura = '{temperature.idTemperatura}'";
            await connection.ExecuteAsync(query);
        }*/
        
        /*// BAROMETRIC PRESSURE TABLE
        query = "SELECT idPresionBarometrica FROM presionbarometrica";
        var resultBarometricPressure = await connection.QueryAsync<BarometricPressure>(query);
        
        foreach (var barometricPressure in resultBarometricPressure)
        {
            Console.WriteLine("Updating barometric pressure: " + barometricPressure.idPresionBarometrica);
            var newId = Guid.NewGuid().ToString();
            
            query = $"UPDATE presionbarometrica SET idPresionBarometrica = '{newId}' WHERE idPresionBarometrica = '{barometricPressure.idPresionBarometrica}'";
            await connection.ExecuteAsync(query);
        }
        
        query = "SELECT idValvula FROM valvula";
        var resultValve = await connection.QueryAsync<AutomaticWatering>(query);
        
        foreach (var valve in resultValve)
        {
            Console.WriteLine("Updating valve: " + valve.IdValvula);
            var newId = Guid.NewGuid().ToString();
            
            query = $"UPDATE valvula SET idValvula = '{newId}' WHERE idValvula = '{valve.IdValvula}'";
            await connection.ExecuteAsync(query);
        }#1#
                
        query = "SELECT idRiegoManual FROM riegomanual";
        var resultManualWatering = await connection.QueryAsync<RiegoManual>(query);
        
        foreach (var manualWatering in resultManualWatering)
        {
            Console.WriteLine("Updating manual watering: " + manualWatering.idRiegoManual);
            var newId = Guid.NewGuid().ToString();
            
            query = $"UPDATE riegomanual SET idRiegoManual = '{newId}' WHERE idRiegoManual = '{manualWatering.idRiegoManual}'";
            await connection.ExecuteAsync(query);
        }*/
        
        // Update sowing cycle
        // UPDATE ON CASCADE: 
        /*
         * Cultivo
         *
         * UPDATE CultivoId -> UPDATE registroGerminacion AND UPDATE valvula AND UPDATE riegomanual
         */
        
        /*var queryUpdate = "SELECT cicloId FROM cicloSiembra";
        
        var result = await connection.QueryAsync<SowingCycle>(queryUpdate);
        
        var disableFKChecksQuery = "SET FOREIGN_KEY_CHECKS = 0";
        await connection.ExecuteAsync(disableFKChecksQuery);
        
        foreach (var cycle in result)
        {
            Console.WriteLine("Updating cycle: " + cycle.CicloId);
            var newId = Guid.NewGuid().ToString();
            
            // Update sowing cycle
            query = $"UPDATE cicloSiembra SET cicloId = '{newId}' WHERE cicloId = '{cycle.CicloId}'";
            await connection.ExecuteAsync(query);
            
            // Update sowing
            query = $"UPDATE cultivo SET cicloId = '{newId}' WHERE cicloId = '{cycle.CicloId}'";
            await connection.ExecuteAsync(query);
        }
        
        // Update cultivo
        query = "SELECT cultivoId FROM cultivo";
        var resultCultivo = await connection.QueryAsync<Sowing>(query);
        
        foreach (var sowing in resultCultivo)
        {
            Console.WriteLine("Updating sowing: " + sowing.CultivoId);
            var newId = Guid.NewGuid().ToString();
            
            query = $"UPDATE cultivo SET cultivoId = '{newId}' WHERE cultivoId = '{sowing.CultivoId}'";
            await connection.ExecuteAsync(query);
            
            // ON CASCADE UPDATE GERMINATION LOGS AND WATERING LOGS
            query = $"UPDATE registrogerminacion SET cultivoId = '{newId}' WHERE cultivoId = '{sowing.CultivoId}'";
            await connection.ExecuteAsync(query);
            
            query = $"UPDATE valvula SET cultivoId = '{newId}' WHERE cultivoId = '{sowing.CultivoId}'";
            await connection.ExecuteAsync(query);
            
            query = $"UPDATE riegomanual SET cultivoId = '{newId}' WHERE cultivoId = '{sowing.CultivoId}'";
            await connection.ExecuteAsync(query);
        }
        
        query = "SELECT registroGerminacionId FROM registrogerminacion";
        var resultGermination = await connection.QueryAsync<GerminationLog>(query);
        
        foreach (var germination in resultGermination)
        {
            Console.WriteLine("Updating germination: " + germination.RegistroGerminacionId);
            var newId = Guid.NewGuid().ToString();
            
            query = $"UPDATE registrogerminacion SET registroGerminacionId = '{newId}' WHERE registroGerminacionId = '{germination.RegistroGerminacionId}'";
            await connection.ExecuteAsync(query);
        }*/
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

    public async Task<int> AddBed1HumidityLog(Bed1 bedHumidity)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlAddBed, "cama1", "idCama1");
        var result = await connection.ExecuteAsync(query, new { idCama1 = Guid.NewGuid().ToString(), bedHumidity.Humedad, bedHumidity.Fecha, bedHumidity.Hora });
        return result;
    }

    public async Task<int> AddBed2HumidityLog(Bed2 bedHumidity)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlAddBed, "cama2", "idCama2");
        var result = await connection.ExecuteAsync(query, bedHumidity);
        return result;
    }

    public async Task<int> AddBed3HumidityLog(Bed3 bedHumidity)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlAddBed, "cama3", "idCama3");
        var result = await connection.ExecuteAsync(query, bedHumidity);
        return result;
    }

    public async Task<int> AddBed4HumidityLog(Bed4 bedHumidity)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlAddBed, "cama4", "idCama4");
        var result = await connection.ExecuteAsync(query, bedHumidity);
        return result;
    }
    
    // CALCULATE THE AMOUNT OF WATER THAT A BED HAS RECEIVED
    public async Task<decimal> CalculateBed1WaterAmount(DateTime startDate, DateTime endDate)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetBedWaterAmountByDates);
        return await connection.ExecuteScalarAsync<decimal>(query, new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") });
    }
    
    public async Task<decimal> CalculateBed2WaterAmount(DateTime startDate, DateTime endDate)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetBedWaterAmountByDates);
        return await connection.ExecuteScalarAsync<decimal>(query, new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") });
    }
    
    public async Task<decimal> CalculateBed3WaterAmount(DateTime startDate, DateTime endDate)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetManualWaterAmountByDates);
        return await connection.ExecuteScalarAsync<decimal>(query, new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") });
    }
    
    public async Task<decimal> CalculateBed4WaterAmount(DateTime startDate, DateTime endDate)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var query = string.Format(SqlGetManualWaterAmountByDates);
        return await connection.ExecuteScalarAsync<decimal>(query, new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") });
    }
}