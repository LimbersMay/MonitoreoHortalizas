using Dapper;
using Microsoft.Extensions.Configuration;
using MonitoreoHortalizasApp.entities;
using MySql.Data.MySqlClient;

namespace MonitoreoHortalizasApp.Services;

public interface IGerminationLogRepository
{
    Task<List<GerminationLog>> GetGerminationLogs();
    Task<GerminationLog> AddGerminationLog(GerminationLog germinationLog);
    Task<GerminationLog> UpdateGerminationLog(GerminationLog germinationLog);
}

public class GerminationLogRepository: IGerminationLogRepository
{
    private readonly string _connectionString;
    
    private const string SqlGetGerminationLogs = @"
    SELECT 
        rg.registroGerminacionId,
        rg.cultivoId,
        c.nombreCultivo AS NombreCama,
        cs.ciclo AS Ciclo,
        rg.temperaturaAmbiente,
        rg.humedadAmbiente,
        rg.numeroZurcosGerminados,
        rg.broteAlturaMinima,
        rg.broteAlturaMaxima,
        rg.numeroMortandad,
        rg.hojasAlturaMaxima,
        rg.hojasAlturaMinima,
        rg.linea,
        rg.observaciones,
        rg.fechaRegistro
    FROM 
        registrogerminacion rg
    JOIN 
        cultivo c ON rg.cultivoId = c.cultivoId
    JOIN 
        ciclosiembra cs ON c.cicloId = cs.cicloId";
    
    private const string SqlGetGerminationLogById = SqlGetGerminationLogs + " WHERE registroGerminacionId = @RegistroGerminacionId";

    
    public GerminationLogRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<List<GerminationLog>> GetGerminationLogs()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryAsync<GerminationLog>(SqlGetGerminationLogs);
        return result.ToList();
    }
    
    public async Task<GerminationLog> AddGerminationLog(GerminationLog germinationLog)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.ExecuteAsync("INSERT INTO registrogerminacion" +
                                                   "(temperaturaAmbiente, humedadAmbiente, numeroZurcosGerminados, broteAlturaMaxima, broteAlturaMinima, numeroMortandad, observaciones, hojasAlturaMinima, hojasAlturaMaxima, linea, fechaRegistro, cultivoId) " +
                                                   "VALUES " +
                                                   "(@temperaturaAmbiente, @humedadAmbiente, @numeroZurcosGerminados, @broteAlturaMaxima, @broteAlturaMinima, @numeroMortandad, @observaciones, @hojasAlturaMinima, @hojasAlturaMaxima, @linea, @fechaRegistro, @cultivoId)"
                                                    , germinationLog);
        
        var lastId = await connection.QueryFirstOrDefaultAsync<int>("SELECT LAST_INSERT_ID()");
        
        return await connection.QueryFirstOrDefaultAsync<GerminationLog>(SqlGetGerminationLogById, new { RegistroGerminacionId = lastId });
    }
    
    public async Task<GerminationLog> UpdateGerminationLog(GerminationLog germinationLog)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.ExecuteAsync("UPDATE registrogerminacion SET " +
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
        
        return await connection.QueryFirstOrDefaultAsync<GerminationLog>(SqlGetGerminationLogById, new { germinationLog.RegistroGerminacionId });
    }
}