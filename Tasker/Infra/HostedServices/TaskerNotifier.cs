using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Tasker.App.Services;
using Tasker.Infra.Options;
using Timer = System.Timers.Timer;

namespace Tasker.Infra.HostedServices
{
    internal class TaskerNotifier : IHostedService, IDisposable, IAsyncDisposable
    {
        private readonly ILogger<TaskerNotifier> mLogger;

        private bool mDisposed;
        private readonly INotifierService mNotifierService;
        private readonly IOptionsMonitor<TaskerConfiguration> mTaskerOptions;
        private Timer mTriangleNotifierTimer;
        private Timer mDailyNotifierTimer;

        public TaskerNotifier(INotifierService notifierService, IOptionsMonitor<TaskerConfiguration> options,
            ILogger<TaskerNotifier> logger)
        {
            mNotifierService = notifierService ?? throw new ArgumentNullException(nameof(notifierService));
            mTaskerOptions = options ?? throw new ArgumentNullException(nameof(options));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            mLogger.LogDebug($"Initializing timer with interval of {mTaskerOptions.CurrentValue.NotifierInterval}");
            mTriangleNotifierTimer = new Timer
            {
                Interval = mTaskerOptions.CurrentValue.NotifierInterval.TotalMilliseconds,
                Enabled = true,
            };

            mDailyNotifierTimer = new Timer
            {
                Interval = mTaskerOptions.CurrentValue.SummaryEmailInterval.TotalMilliseconds,
                Enabled = true,
            };

            mTriangleNotifierTimer.Elapsed += OnTriangleNotifierElapsed;
            mDailyNotifierTimer.Elapsed += OnDailyNotifierElapsed;

            return Task.CompletedTask;
        }

        private async void OnTriangleNotifierElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            await mNotifierService.NotifyTriangleTasks().ConfigureAwait(false);
        }

        private async void OnDailyNotifierElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            await mNotifierService.NotifySummary().ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            mLogger.LogDebug($"Stopping timer");
            mTriangleNotifierTimer.Stop();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            mLogger.LogDebug("Closing timer");

            if (mDisposed)
                return;

            if (disposing)
                mTriangleNotifierTimer.Dispose();

            mDisposed = true;
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return default;
        }
    }
}