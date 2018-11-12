#### 0.1.1 November 12 2018 ####
Made the Petabridge.Tracing.ApplicationInsights strong-named using [Public Signing](https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/public-signing.md#public-signing)

#### 0.1.0 September 21 2018 ####
Initial release of Petabridge.Tracing.ApplicationInsights.

We built this driver in order to provide a layer of [OpenTracing](http://opentracing.io/) compatibility on top of [Microsoft's Application Insights](https://azure.microsoft.com/en-us/services/application-insights/) telemetry and analytics service.

This driver follows [the Application Insights to OpenTracing conventions](https://docs.microsoft.com/en-us/azure/application-insights/application-insights-correlation#open-tracing-and-application-insights) outlined by the Application Insights team.

`Petabridge.Tracing.ApplicationInsights` is professionally maintained and tested by [Petabridge](http://petabridge.com/) and is used inside some of our commercial products, such as [Phobos: Enterprise Akka.NET DevOps](https://phobos.petabridge.com/).