using Dapper;
using Microsoft.Extensions.Configuration;
using MonitoreoHortalizasApp.entities;
using MySql.Data.MySqlClient;

namespace MonitoreoHortalizasApp.Services;

public interface IGerminationLogRepository
{
    Task<List<GerminationLog>> GetGerminationLogs();
    Task<List<GerminationLog>> GetBed1GerminationLogs();
    Task<List<GerminationLog>> GetBed2GerminationLogs();
    Task<List<GerminationLog>> GetBed3GerminationLogs();
    Task<List<GerminationLog>> GetBed4GerminationLogs();
    Task<GerminationLog> AddGerminationLog(GerminationLog germinationLog);
    Task<GerminationLog> UpdateGerminationLog(GerminationLog germinationLog);
}

public class GerminationLogRepository: IGerminationLogRepository
{
    private readonly string _connectionString;
    
    private readonly string GET_GERMINATION_LOGS = "SELECT * FROM registrogerminacion WHERE cultivoId = {0} ORDER BY fechaRegistro DESC LIMIT 500";
    
    public GerminationLogRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<List<GerminationLog>> GetGerminationLogs()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryAsync<GerminationLog>("SELECT * FROM registrogerminacion ORDER BY fechaRegistro DESC LIMIT 500");
        return result.ToList();
    }
    
    public async Task<List<GerminationLog>> GetBed1GerminationLogs()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryAsync<GerminationLog>(string.Format(GET_GERMINATION_LOGS, 1));
        return result.ToList();
    }
    
    public async Task<List<GerminationLog>> GetBed2GerminationLogs()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryAsync<GerminationLog>(string.Format(GET_GERMINATION_LOGS, 2));
        return result.ToList();
    }
    
    public async Task<List<GerminationLog>> GetBed3GerminationLogs()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryAsync<GerminationLog>(string.Format(GET_GERMINATION_LOGS, 3));
        return result.ToList();
    }
    
    public async Task<List<GerminationLog>> GetBed4GerminationLogs()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryAsync<GerminationLog>(string.Format(GET_GERMINATION_LOGS, 4));
        return result.ToList();
    }
    
    public async Task<GerminationLog> AddGerminationLog(GerminationLog germinationLog)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.ExecuteAsync("INSERT INTO registrogerminacion" +
                                                   "(temperaturaAmbiente, humedadAmbiente, numeroZurcosGerminados, broteAlturaMaxima, broteAlturaMinima, numeroMortandad, observaciones, hojasAlturaMinima, hojasAlturaMaxima, linea, fechaRegistro, cultivoId) " +
                                                   "VALUES " +
                                                   "(@temperaturaAmbiente, @humedadAmbiente, @numeroZurcosGerminados, @broteAlturaMaxima, @broteAlturaMinima, @numeroMortandad, @observaciones, @hojasAlturaMinima, @hojasAlturaMaxima, @linea, @fechaRegistro, @cultivoId)"
                                                    , germinationLog);
        return germinationLog;
    }
    
    public async Task<GerminationLog> UpdateGerminationLog(GerminationLog germinationLog)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.ExecuteAsync("UPDATE registrogerminacion SET " +
                                                   "temperaturaAmbiente = @temperaturaAmbiente, " +
                                                   "humedadAmbiente = @humedadAmbiente, " +
                                                   "numeroZurcosGerminados = @numeroZurcosGerminados, " +
                                                   "broteAlturaMaxima = @broteAlturaMaxima, " +
                                                   "broteAlturaMinima = @broteAlturaMinima, " +
                                                   "numeroMortandad = @numeroMortandad, " +
                                                   "observaciones = @observaciones, " +
                                                   "hojasAlturaMinima = @hojasAlturaMinima, " +
                                                   "hojasAlturaMaxima = @hojasAlturaMaxima, " +
                                                   "linea = @linea, " +
                                                   "fechaRegistro = @fechaRegistro, " +
                                                   "cultivoId = @cultivoId " +
                                                   "WHERE registroGerminacionId = @registroGerminacionId"
                                                    , germinationLog);
        return germinationLog;
    }
}