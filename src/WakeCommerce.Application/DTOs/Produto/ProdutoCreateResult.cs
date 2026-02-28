namespace WakeCommerce.Application.DTOs.Produto;

/// <summary>
/// Resultado interno do create: Id para o Location e o DTO (sem Id) para o corpo da resposta.
/// </summary>
public sealed record ProdutoCreateResult(int Id, ProdutoResponseDTO Produto);
