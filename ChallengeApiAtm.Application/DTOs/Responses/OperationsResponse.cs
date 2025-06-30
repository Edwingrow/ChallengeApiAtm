using ChallengeApiAtm.Domain.Enums;

namespace ChallengeApiAtm.Application.DTOs.Responses;

/// <summary>
/// DTO para la respuesta de historial de operaciones paginado
/// </summary>
public sealed class OperationsResponse
{
    /// <summary>
    /// Lista de transacciones de la página actual
    /// </summary>
    public IEnumerable<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();

    /// <summary>
    /// Información de paginación
    /// </summary>
    public PaginationInfoDto Pagination { get; set; } = null!;

    /// <summary>
    /// Información de la cuenta
    /// </summary>
    public AccountSummaryDto AccountInfo { get; set; } = null!;
}

/// <summary>
/// DTO para información de una transacción
/// </summary>
public sealed class TransactionDto
{
    /// <summary>
    /// ID de la transacción
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Tipo de transacción
    /// </summary>
    public TransactionType Type { get; set; }

    /// <summary>
    /// Descripción del tipo de transacción
    /// </summary>
    public string TypeDescription { get; set; } = string.Empty;

    /// <summary>
    /// Monto de la transacción
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Descripción de la transacción
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de la transacción
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Saldo después de la transacción
    /// </summary>
    public decimal? BalanceAfterTransaction { get; set; }

    /// <summary>
    /// Estado de la transacción
    /// </summary>
    public TransactionStatus Status { get; set; }
}

/// <summary>
/// DTO para información de paginación
/// </summary>
public sealed class PaginationInfoDto
{
    /// <summary>
    /// Página actual
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Tamaño de página
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de registros
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Indica si hay página anterior
    /// </summary>
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Indica si hay página siguiente
    /// </summary>
    public bool HasNextPage { get; set; }
}

/// <summary>
/// DTO para resumen de la cuenta
/// </summary>
public sealed class AccountSummaryDto
{
    /// <summary>
    /// Número de cuenta
    /// </summary>
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del titular
    /// </summary>
    public string AccountHolderName { get; set; } = string.Empty;

    /// <summary>
    /// Saldo actual
    /// </summary>
    public decimal CurrentBalance { get; set; }
} 