using Dapper;
using Microsoft.Extensions.Configuration;
using MonitoreoHortalizasApp.entities;
using MySql.Data.MySqlClient;

namespace MonitoreoHortalizasApp.Services;

public interface ISowingRepository
{
    Task<List<Sowing>> GetSowings();
    Task<Sowing> AddSowing(Sowing sowing);
    Task<Sowing> UpdateSowing(Sowing sowing);
}

public class SowingRepository : ISowingRepository
{
    private readonly string _connectionString;
    
    private const string SqlGetSowings = "SELECT * FROM cultivo";
    private const string SqlAddSowing = "INSERT INTO cultivo (nombreCultivo, germinacion, fechaSiembra, fechaCosecha, tipoRiego, gramaje, alturaMaxima, alturaMinima, temperaturaAmbienteMaxima, temperaturaAmbienteMinima, humedadAmbienteMaxima, humedadAmbienteMinima, humedadMinimaTierra, humedadMaximaTierra, presionBarometricaMaxima, presionBarometricaMinima, cicloId) VALUES (@NombreCultivo, @Germinacion, @FechaSiembra, @FechaCosecha, @TipoRiego, @Gramaje, @AlturaMaxima, @AlturaMinima, @TemperaturaAmbienteMaxima, @TemperaturaAmbienteMinima, @HumedadAmbienteMaxima, @HumedadAmbienteMinima, @HumedadMinimaTierra, @HumedadMaximaTierra, @PresionBarometricaMaxima, @PresionBarometricaMinima, @cicloId)";
    private const string SqlUpdateSowing = "UPDATE cultivo SET nombreCultivo = @NombreCultivo, germinacion = @Germinacion, fechaSiembra = @FechaSiembra, fechaCosecha = @FechaCosecha, tipoRiego = @TipoRiego, gramaje = @Gramaje, alturaMaxima = @AlturaMaxima, alturaMinima = @AlturaMinima, temperaturaAmbienteMaxima = @TemperaturaAmbienteMaxima, temperaturaAmbienteMinima = @TemperaturaAmbienteMinima, humedadAmbienteMaxima = @HumedadAmbienteMaxima, humedadAmbienteMinima = @HumedadAmbienteMinima, humedadMinimaTierra = @HumedadMinimaTierra, humedadMaximaTierra = @HumedadMaximaTierra, presionBarometricaMaxima = @PresionBarometricaMaxima, presionBarometricaMinima = @PresionBarometricaMinima WHERE cultivoId = @CultivoId";

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
    
    public async Task<Sowing> AddSowing(Sowing sowing)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.ExecuteAsync(SqlAddSowing, sowing);
        return sowing;
    }
    
    public async Task<Sowing> UpdateSowing(Sowing sowing)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.ExecuteAsync(SqlUpdateSowing, sowing);
        return sowing;
    }
}