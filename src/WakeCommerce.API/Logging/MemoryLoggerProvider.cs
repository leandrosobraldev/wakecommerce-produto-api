using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WakeCommerce.API.Logging;

public sealed class MemoryLoggerProvider : ILoggerProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMemoryLogStore _store;

    public MemoryLoggerProvider(IHttpContextAccessor httpContextAccessor, IMemoryLogStore store)
    {
        _httpContextAccessor = httpContextAccessor;
        _store = store;
    }

    public ILogger CreateLogger(string categoryName) => new MemoryLogger(categoryName, _httpContextAccessor, _store);

    public void Dispose() { }

    private sealed class MemoryLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryLogStore _store;

        public MemoryLogger(string categoryName, IHttpContextAccessor httpContextAccessor, IMemoryLogStore store)
        {
            _categoryName = categoryName;
            _httpContextAccessor = httpContextAccessor;
            _store = store;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            var message = formatter(state, exception);

            _store.Add(new MemoryLogEntry
            {
                DataHora = DateTime.Now,
                Nivel = logLevel.ToString(),
                Categoria = _categoryName,
                Mensagem = message,
                Metodo = request?.Method,
                Caminho = request?.Path.Value,
                Excecao = exception?.Message
            });
        }
    }
}
