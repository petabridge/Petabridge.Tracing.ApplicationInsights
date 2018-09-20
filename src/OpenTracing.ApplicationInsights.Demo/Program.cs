using System;
using Akka.Actor;
using Akka.Event;
using Microsoft.ApplicationInsights.Extensibility;

namespace OpenTracing.ApplicationInsights.Demo
{
    public class TracerActor : ReceiveActor
    {
        private readonly ITracer _tracer;
        private readonly ILoggingAdapter _loggingAdapter = Context.GetLogger();
        private ICancelable _scheduled;

        public TracerActor(ITracer tracer)
        {
            _tracer = tracer;

            Receive<string>(str =>
            {
                using (var current = _tracer.BuildSpan(Context.Self.Path.ToString()).StartActive())
                {
                    _loggingAdapter.Info(str);
                    current.Span.Log(str);
                    current.Span.SetTag("strLen", str.Length);

                    using (var subOp = _tracer.BuildSpan(Context.Self.Path.ToString() + ".subOp").StartActive())
                    {
                        subOp.Span.Log("Do nested operations work?");
                        subOp.Span.SetTag("nested", true);
                    }
                }
            });
        }

        protected override void PreStart()
        {
            _scheduled = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromMilliseconds(40),
                TimeSpan.FromMilliseconds(40), Self, "foo", ActorRefs.NoSender);
        }

        protected override void PostStop()
        {
            _scheduled.Cancel();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var appId = Environment.GetEnvironmentVariable("APP_INSIGHTS_APPID");
            if (string.IsNullOrEmpty(appId))
            {
                Console.WriteLine("Unable to read environment variable [APP_INSIGHTS_APPID].");
                Console.WriteLine("Please set this value equal to your Application Insights AppId");
                return;
            }

            var actorSystem = ActorSystem.Create("MySys");
            var tracer = new ApplicationInsightsTracer(new TelemetryConfiguration(appId));
            var loggingActor = actorSystem.ActorOf(Props.Create(() => new TracerActor(tracer)));

            Console.WriteLine("Press enter at any time to end this sample.");
            Console.ReadLine();
        }
    }
}
