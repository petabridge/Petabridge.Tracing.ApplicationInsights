// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using Akka.Actor;
using Akka.Event;
using Microsoft.ApplicationInsights.Extensibility;

namespace OpenTracing.ApplicationInsights.Demo
{
    public class TracerActor : ReceiveActor
    {
        private readonly ILoggingAdapter _loggingAdapter = Context.GetLogger();
        private readonly ITracer _tracer;
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

    internal class Program
    {
        private static void Main(string[] args)
        {
            var appId = Environment.GetEnvironmentVariable("APP_INSIGHTS_INSTRUMENTATION_KEY");
            if (string.IsNullOrEmpty(appId))
            {
                Console.WriteLine("Unable to read environment variable [APP_INSIGHTS_APPID].");
                Console.WriteLine("Please set this value equal to your Application Insights AppId");
                return;
            }

            var config = new TelemetryConfiguration(appId);

            var actorSystem = ActorSystem.Create("MySys");
            var tracer = new ApplicationInsightsTracer(config);
            var loggingActor = actorSystem.ActorOf(Props.Create(() => new TracerActor(tracer)));

            tracer.Client.TrackRequest("start app", DateTimeOffset.Now, TimeSpan.FromSeconds(1), "200", true);

            Console.WriteLine("Press enter at any time to end this sample.");
            Console.ReadLine();
        }
    }
}