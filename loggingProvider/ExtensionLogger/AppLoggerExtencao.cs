using loggingProvider.Entidades;
using loggingProvider.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace loggingProvider.ExtensionLogger
{
    public static class AppLoggerExtencao
    {
        private static dynamic IdRepositorio;
        public static ILoggerFactory AddContext(this ILoggerFactory factory, IServiceProvider services, Func<string, LogLevel, bool> filter = null)
        {
            Type serviceType = typeof(IRepositorioBase<Log>);
            factory.AddProvider(new AppLoggerProvider(filter, repositorio: (IRepositorioBase<Log>)services.GetService(serviceType)));
            return factory;
        }

        public static ILoggerFactory AddContext(this ILoggerFactory factory, LogLevel minLevel, IServiceProvider services)
            => AddContext(factory, services, (_, logLevel) => logLevel >= minLevel);        

        public static void SetarId(dynamic id) => IdRepositorio = id;
        public static dynamic PegarId(this ILogger _) => IdRepositorio;
    }

    public class AppLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filtro;
        private readonly IRepositorioBase<Log> _repositorio;

        public AppLoggerProvider(Func<string, LogLevel, bool> filtro, IRepositorioBase<Log> repositorio)
        {
            _filtro = filtro;
            _repositorio = repositorio;
        }

        public ILogger CreateLogger(string nomeCategoria)
            => new AppLogger(nomeCategoria, _filtro, _repositorio);

        public void Dispose() { GC.SuppressFinalize(this);  }
    }

    public class AppLogger : ILogger
    {
        private readonly string _nomeCategoria;
        private readonly Func<string, LogLevel, bool> _filtro;
        private readonly IRepositorioBase<Log> _repositorio;
        private IExternalScopeProvider ScopeProvider { get; set; }

        public AppLogger(string nomeCategoria,
                         Func<string, LogLevel, bool> filtro,
                         IRepositorioBase<Log> repositorio)
        {
            _nomeCategoria = nomeCategoria;
            _filtro = filtro;
            _repositorio = repositorio;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventoId, TState state, Exception exception, Func<TState, Exception, string> formato)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formato == null)
                throw new ArgumentNullException(nameof(formato));

            if (!_nomeCategoria.Contains(AppDomain.CurrentDomain.FriendlyName) && logLevel == LogLevel.Information)
                return;

            var logBuilder = new StringBuilder();

            var mensagem = formato(state, exception);
            if (!string.IsNullOrEmpty(mensagem))
            {
                logBuilder.Append(mensagem);
                logBuilder.Append(Environment.NewLine);
            }

            GetScope(logBuilder);

            if (exception != null)
                mensagem = logBuilder.Append(exception.ToString()).ToString();

            var log = new Log()
            {
                Mensagem = mensagem,
                IdEvento = eventoId.Id,
                Categoria = _nomeCategoria,
                LogLevel = logLevel.ToString(),
                DataCadastro = DateTime.UtcNow
            };
            AppLoggerExtencao.SetarId(log.Id);
            _repositorio.Adicionar(log);
        }

        public bool IsEnabled(LogLevel logLevel) => _filtro == null || _filtro(_nomeCategoria, logLevel);

        public IDisposable BeginScope<TState>(TState state) => null;

        private void GetScope(StringBuilder stringBuilder)
        {
            var scopeProvider = ScopeProvider;
            if (scopeProvider != null)
            {
                var initialLength = stringBuilder.Length;

                scopeProvider.ForEachScope((scope, state) =>
                {
                    var (builder, length) = state;
                    var first = length == builder.Length;
                    builder.Append(first ? "=> " : " => ").Append(scope);
                }, (stringBuilder, initialLength));

                stringBuilder.AppendLine();
            }
        }
    }

}
