using Dapper;
using Microsoft.Extensions.Configuration;
using MonitoreoHortalizasApp.entities;
using MySql.Data.MySqlClient;

namespace MonitoreoHortalizasApp.Services;

public interface ISowingRepository
{
    Task<List<Sowing>> GetSowings();
    Task<List<Sowing>> GetSowingsByCicleId(string cicleId);
    Task<Sowing> AddSowing(Sowing sowing);
    Task<Sowing> UpdateSowing(Sowing sowing);
}

public class SowingRepository : ISowingRepository
{
    private readonly string _connectionString;

    private const string SqlGetSowings = @"
    SELECT 
        c.cultivoId, 
        c.descripcion,
        c.nombreCultivo, 
        c.germinacion, 
        c.fechaSiembra, 
        c.fechaCosecha, 
        c.tipoRiego, 
        c.gramaje, 
        c.alturaMaxima, 
        c.alturaMinima, 
        c.temperaturaAmbienteMaxima, 
        c.temperaturaAmbienteMinima, 
        c.humedadAmbienteMaxima, 
        c.humedadAmbienteMinima, 
        c.humedadMinimaTierra, 
        c.humedadMaximaTierra, 
        c.presionBarometricaMaxima, 
        c.presionBarometricaMinima, 
        c.cicloId,
        ci.ciclo AS Ciclo 
    FROM cultivo c
    JOIN ciclosiembra ci ON c.cicloId = ci.cicloId";
    
    private const string SqlGetSowingById = SqlGetSowings + " WHERE cultivoId = @CultivoId";
    
    private const string SqlGetSowingsByCicleId = SqlGetSowings + " WHERE c.cicloId = @CicloId";

    private const string SqlAddSowing = "INSERT INTO cultivo (cultivoId, nombreCultivo, germinacion, fechaSiembra, fechaCosecha, tipoRiego, gramaje, alturaMaxima, alturaMinima, temperaturaAmbienteMaxima, temperaturaAmbienteMinima, humedadAmbienteMaxima, humedadAmbienteMinima, humedadMinimaTierra, humedadMaximaTierra, presionBarometricaMaxima, presionBarometricaMinima, descripcion, cicloId) VALUES (@CultivoId, @NombreCultivo, @Germinacion, @FechaSiembra, @FechaCosecha, @TipoRiego, @Gramaje, @AlturaMaxima, @AlturaMinima, @TemperaturaAmbienteMaxima, @TemperaturaAmbienteMinima, @HumedadAmbienteMaxima, @HumedadAmbienteMinima, @HumedadMinimaTierra, @HumedadMaximaTierra, @PresionBarometricaMaxima, @PresionBarometricaMinima, @descripcion, @cicloId)";
    private const string SqlUpdateSowing = "UPDATE cultivo SET descripcion = @descripcion, cicloId = @CicloId, nombreCultivo = @NombreCultivo, germinacion = @Germinacion, fechaSiembra = @FechaSiembra, fechaCosecha = @FechaCosecha, tipoRiego = @TipoRiego, gramaje = @Gramaje, alturaMaxima = @AlturaMaxima, alturaMinima = @AlturaMinima, temperaturaAmbienteMaxima = @TemperaturaAmbienteMaxima, temperaturaAmbienteMinima = @TemperaturaAmbienteMinima, humedadAmbienteMaxima = @HumedadAmbienteMaxima, humedadAmbienteMinima = @HumedadAmbienteMinima, humedadMinimaTierra = @HumedadMinimaTierra, humedadMaximaTierra = @HumedadMaximaTierra, presionBarometricaMaxima = @PresionBarometricaMaxima, presionBarometricaMinima = @PresionBarometricaMinima WHERE cultivoId = @CultivoId";

    public SowingRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<List<Sowing>> GetSowings()
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryAsync<Sowing>(SqlGetSowings);
        return result.ToList();
    }
    
    public async Task<List<Sowing>> GetSowingsByCicleId(string cicleId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryAsync<Sowing>(SqlGetSowingsByCicleId, new { CicloId = cicleId });
        return result.ToList();
    }
    
    public async Task<Sowing> AddSowing(Sowing sowing)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.ExecuteAsync(SqlAddSowing, sowing); 
        
        return await connection.QueryFirstOrDefaultAsync<Sowing>(SqlGetSowingById, new { sowing.CultivoId });
    }
    
    public async Task<Sowing> UpdateSowing(Sowing sowing)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.ExecuteAsync(SqlUpdateSowing, sowing);
        
        return await connection.QueryFirstOrDefaultAsync<Sowing>(SqlGetSowingById, new { sowing.CultivoId });
    }
}