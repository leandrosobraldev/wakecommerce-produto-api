namespace WakeCommerce.API.Logging;

public sealed class MemoryLogEntry
{
    public DateTime DataHora { get; init; }
    public string Nivel { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public string Mensagem { get; init; } = string.Empty;
    public string? Metodo { get; init; }
    public string? Caminho { get; init; }
    public string? Excecao { get; init; }
}
