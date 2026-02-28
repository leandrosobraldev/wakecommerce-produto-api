using System.Collections.Concurrent;

namespace WakeCommerce.API.Logging;

public interface IMemoryLogStore
{
    void Add(MemoryLogEntry entry);
    IReadOnlyList<MemoryLogEntry> GetEntries();
}

public sealed class MemoryLogStore : IMemoryLogStore
{
    private const int MaxEntries = 500;
    private readonly ConcurrentQueue<MemoryLogEntry> _entries = new();

    public void Add(MemoryLogEntry entry)
    {
        _entries.Enqueue(entry);
        while (_entries.Count > MaxEntries && _entries.TryDequeue(out _)) { }
    }

    public IReadOnlyList<MemoryLogEntry> GetEntries() => _entries.ToArray();
}
