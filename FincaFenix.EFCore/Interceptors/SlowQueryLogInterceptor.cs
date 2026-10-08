using System.Data.Common;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace FincaFenix.EFCore.Interceptors
{
    public class SlowQueryLogInterceptor : DbCommandInterceptor
    {
        private readonly ILogger<SlowQueryLogInterceptor> logger;
        private readonly int thresholdMs;
        private readonly AsyncLocal<Stopwatch> stopwatch = new();

        public SlowQueryLogInterceptor(ILogger<SlowQueryLogInterceptor> logger, int thresholdMs)
        {
            this.logger = logger;
            this.thresholdMs = thresholdMs;
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            StartTimer();
            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            StartTimer();
            return ValueTask.FromResult(result);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            LogIfSlow(command);
            return result;
        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            LogIfSlow(command);
            return ValueTask.FromResult(result);
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            StartTimer();
            return result;
        }

        public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            StartTimer();
            return ValueTask.FromResult(result);
        }

        public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
        {
            LogIfSlow(command);
            return result;
        }

        public override ValueTask<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result, CancellationToken cancellationToken = default)
        {
            LogIfSlow(command);
            return ValueTask.FromResult(result);
        }

        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            StartTimer();
            return result;
        }

        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            StartTimer();
            return ValueTask.FromResult(result);
        }

        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            LogIfSlow(command);
            return result;
        }

        public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            LogIfSlow(command);
            return ValueTask.FromResult(result);
        }

        public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            ClearTimer();
        }

        public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            ClearTimer();
            return Task.CompletedTask;
        }

        private void StartTimer() => stopwatch.Value = Stopwatch.StartNew();

        private void ClearTimer() => stopwatch.Value = null;

        private void LogIfSlow(DbCommand command)
        {
            var watch = stopwatch.Value;
            stopwatch.Value = null;
            if (watch is null)
                return;

            watch.Stop();
            if (watch.ElapsedMilliseconds < thresholdMs)
                return;

            logger.LogWarning(
                "Slow query {ElapsedMs}ms (threshold {ThresholdMs}ms): {CommandText}",
                watch.ElapsedMilliseconds,
                thresholdMs,
                command.CommandText);
        }
    }
}
