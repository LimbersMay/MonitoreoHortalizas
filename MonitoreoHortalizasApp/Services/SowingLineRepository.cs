using Dapper;
using Microsoft.Extensions.Configuration;
using MonitoreoHortalizasApp.Entities;
using MySql.Data.MySqlClient;

namespace MonitoreoHortalizasApp.Services;

public interface ISowingLineRepository
{
    Task<List<SowingLine>> GetSowingLinesBySowingId(string sowingId);
    Task<SowingLine> UpdateSowingLine(SowingLine sowingLine);
    Task<SowingLine> AddSowingLine(SowingLine sowingLine);
}

public class SowingLineRepository : ISowingLineRepository
{

    private const string SqlUpdateSowing = "UPDATE lineaCultivo SET numeroLinea = @numeroLinea, gramaje = @gramaje WHERE lineaCultivoId = @lineaCultivoId";
    private const string SqlGetSowingLineById = "SELECT * FROM lineacultivo WHERE lineaCultivoId = @lineaCultivoId";

    private const string SqlAddSowingLine =
        "INSERT INTO lineacultivo(numeroLinea, gramaje, lineaCultivoId, cultivoId) VALUES (@NumeroLinea, @Gramaje, @LineaCultivoId, @CultivoId)";
    
    private readonly string _connectionString;

    public SowingLineRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    public async Task<List<SowingLine>> GetSowingLinesBySowingId(string sowingId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryAsync<SowingLine>("SELECT * FROM lineacultivo WHERE cultivoId = @sowingId ORDER BY numeroLinea", new { sowingId });
        return result.ToList();
    }
    public async Task<SowingLine> UpdateSowingLine(SowingLine sowingLine)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.ExecuteAsync(SqlUpdateSowing, sowingLine);
        return await connection.QueryFirstOrDefaultAsync<SowingLine>(SqlGetSowingLineById,
            new { sowingLine.LineaCultivoId });
    }

    public async Task<SowingLine> AddSowingLine(SowingLine sowingLine)
    {
        Console.WriteLine("SowingLineId: " + sowingLine.LineaCultivoId);
        Console.WriteLine("SowingLineSowingId: " + sowingLine.CultivoId);
        Console.WriteLine("SowingLineGrama: " + sowingLine.Gramaje);
        Console.WriteLine("SowingLineLine: " + sowingLine.NumeroLinea);
        
        
        await using var connection = new MySqlConnection(_connectionString);
        await connection.ExecuteAsync(SqlAddSowingLine, sowingLine);
        
        return await connection.QueryFirstOrDefaultAsync<SowingLine>(SqlGetSowingLineById,
            new { sowingLine.LineaCultivoId }); 
    }
}