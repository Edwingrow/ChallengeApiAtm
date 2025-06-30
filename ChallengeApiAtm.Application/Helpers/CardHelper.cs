namespace ChallengeApiAtm.Application.Helpers;

/// <summary>
/// Helper para operaciones relacionadas con tarjetas
/// </summary>
public static class CardHelper
{
    /// <summary>
    /// Enmascara el número de tarjeta mostrando solo los últimos 4 dígitos
    /// </summary>
    /// <param name="cardNumber">Número de tarjeta completo</param>
    /// <returns>Número de tarjeta enmascarado</returns>
    public static string MaskCardNumber(string cardNumber)
    {
        if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
            return "****";

        return "****-****-****-" + cardNumber[^4..];
    }
} 