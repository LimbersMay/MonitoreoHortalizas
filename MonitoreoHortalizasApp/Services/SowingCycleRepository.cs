using Dapper;
using Microsoft.Extensions.Configuration;
using MonitoreoHortalizasApp.Entities;
using MySql.Data.MySqlClient;

namespace MonitoreoHortalizasApp.Services;

public interface ISowingCycleRepository
{
    Task<List<SowingCycle>> GetSowingCycles();
    Task<SowingCycle> GetSowingCycle(int id);
    Task<string> AddSowingCycle(SowingCycle sowingCycle);
    Task<SowingCycle> UpdateSowingCycle(SowingCycle sowingCycle);
}

public class SowingCycleRepository: ISowingCycleRepository
{
    private readonly string _connectionString;
    
    private readonly string _getSowingCycles = "SELECT * FROM cicloSiembra ORDER BY ciclo";
    private readonly string _getSowingCycle = "SELECT * FROM cicloSiembra WHERE cicloId = {0}";
    private readonly string _addSowingCycle = "INSERT INTO cicloSiembra (cicloId, ciclo, descripcion, fechaInicio, fechaFin) VALUES (@cicloId, @Ciclo, @Descripcion, @FechaInicio, @FechaFin)";
    private readonly string _updateSowingCycle = "UPDATE cicloSiembra SET ciclo = @Ciclo, descripcion = @Descripcion, fechaInicio = @FechaInicio, fechaFin = @FechaFin WHERE cicloId = @CicloId";
    
    public SowingCycleRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<List<SowingCycle>> GetSowingCycles()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryAsync<SowingCycle>(_getSowingCycles);
        return result.ToList();
    }
    
    public async Task<SowingCycle> GetSowingCycle(int id)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryFirstOrDefaultAsync<SowingCycle>(string.Format(_getSowingCycle, id));
        return result;
    }
    
    public async Task<string> AddSowingCycle(SowingCycle sowingCycle)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.ExecuteAsync(_addSowingCycle, sowingCycle);
        
        return  await connection.ExecuteScalarAsync<string>("SELECT LAST_INSERT_ID()");
    }
    
    public async Task<SowingCycle> UpdateSowingCycle(SowingCycle sowingCycle)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.ExecuteAsync(_updateSowingCycle, sowingCycle);
        return sowingCycle;
    }
}