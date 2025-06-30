namespace ChallengeApiAtm.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de generación de números únicos
/// </summary>
public interface INumberGeneratorService
{
    /// <summary>
    /// Genera un número de cuenta único
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de cuenta generado</returns>
    Task<string> GenerateUniqueAccountNumberAsync(CancellationToken cancellationToken = default);
}