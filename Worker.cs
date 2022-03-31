namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Configuration _options;
       // private Timer _t;
        private int count = 0;
        private double delay = 0;

       
        public Worker(ILogger<Worker> logger, Configuration options)
        {
            _logger = logger;
            _options = options;       
        }
        private double FirstCycle()
        {
            DateTime _scheduleTime;
            _scheduleTime = DateTime.Today.AddDays(_options.T1Day).AddHours(_options.T1Hour).AddMinutes(_options.T1Minutes);
            _scheduleTime = GetNextWeekday(_scheduleTime, _options.T1DayName);
            double tillNextInterval = _scheduleTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            if (_options.T1Day == 0)
            {
                if (tillNextInterval < 0) tillNextInterval += new TimeSpan(24, 0, 0).TotalSeconds * 1000;
            }
            return tillNextInterval;
        }
        private double NextCycle()
        {
            DateTime _scheduleTime;
            _scheduleTime = DateTime.Today.AddDays(7).AddHours(_options.T1Hour).AddMinutes(_options.T1Minutes);
            double tillNextInterval = _scheduleTime.Subtract(DateTime.Now).TotalSeconds * 1000;

            if (tillNextInterval < 0) tillNextInterval += new TimeSpan(7, 0, 0, 0).TotalSeconds * 1000;
            return tillNextInterval;
        }
        public static DateTime GetNextWeekday(DateTime start, int dayValue)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)dayValue - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //// set up a timer to be non-reentrant
            //_t = new Timer(async _ => await OnTimerFiredAsync(cancellationToken),
            //    null, FirstCycle(), Timeout.Infinite);
            delay = FirstCycle();
            _logger.LogInformation("Service started at: {time}", DateTimeOffset.Now);
            
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //_t?.Dispose();
            _logger.LogInformation("Service stopped at: {time}", DateTimeOffset.Now);
            return base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            while (!stoppingToken.IsCancellationRequested)
            {
                if (count == 0)
                {
                    count = 1;
                }
                else
                {
                    delay= NextCycle();
                    if (GetResponse())
                    {
                        _logger.LogInformation("Response OK : {time}", DateTimeOffset.Now);
                    }
                    else
                    {
                        _logger.LogInformation("No Response: {time}", DateTimeOffset.Now);
                    }
                }
                await Task.Delay(TimeSpan.FromMilliseconds(delay), stoppingToken);
            }
        }
       
        public bool GetResponse() {
            return false;
        }


        //private async Task OnTimerFiredAsync(CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        if (GetResponse())
        //        {
        //            _logger.LogInformation("Response OK : {time}", DateTimeOffset.Now);
        //        }
        //        else
        //        {
        //            _logger.LogInformation("No Response: {time}", DateTimeOffset.Now);
        //        }
        //        await Task.Delay(0, cancellationToken);
        //    }
        //    finally
        //    {
        //        // set timer to fire off again
        //        _t?.Change(FirstCycle(), Timeout.Infinite);
        //    }
        //}

       
    }
}