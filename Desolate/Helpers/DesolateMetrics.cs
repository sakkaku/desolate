using System.Diagnostics.Metrics;
using Microsoft.Extensions.Diagnostics.Metrics;

namespace Desolate.Helpers;

internal struct SystemProcessingTimeTags(string Name);

internal static partial class DesolateMetrics
{
    [Histogram<double>(typeof(SystemProcessingTimeTags))]
    public static partial SystemProcessingTimeMetric CreateSystemProcessingTime(Meter meter);

    [Histogram<double>]
    public static partial FrameTimeMetric CreateFrameTime(Meter meter);
    
    [Counter<int>]
    public static partial EntityCountMetric CreateEntityCount(Meter meter);
}