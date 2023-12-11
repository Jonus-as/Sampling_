
using QuaDigi.Models;

namespace QuaDigi;

public class MeasurementSamplingService
{

    public Dictionary<MeasurementType, List<Measurement>> Sample(DateTime startOfSampling, List<Measurement> unsampledMeasurements)
    {
        var result = new Dictionary<MeasurementType, List<Measurement>>();
        var positionDiff = Position(startOfSampling);

        foreach (var unsampledMeasurement in unsampledMeasurements)
        {
            if (unsampledMeasurement.MeasurementTime < startOfSampling)
                continue;

            if (!result.ContainsKey(unsampledMeasurement.Type))
            {
                result[unsampledMeasurement.Type] = new List<Measurement> { unsampledMeasurement };
                continue;
            }

            var position = Position(unsampledMeasurement.MeasurementTime, positionDiff);

            if (position - Position(result[unsampledMeasurement.Type][0].MeasurementTime, positionDiff) <= -1)
            {
                result[unsampledMeasurement.Type].Insert(0, unsampledMeasurement);
            }
            else if (position > result[unsampledMeasurement.Type].Count)
            {
                for (int i = result[unsampledMeasurement.Type].Count - 1; i >= 0; i--)
                {
                    var currentPosition = Position(result[unsampledMeasurement.Type][i].MeasurementTime, positionDiff);
                    if (position - currentPosition < 0)
                        continue;

                    if (position - currentPosition >= 1)
                    {
                        result[unsampledMeasurement.Type].Insert(i + 1, unsampledMeasurement);
                        break; // insert value after current item and skip iterating
                    }

                    // if item is bigger than existing change it
                    if ((unsampledMeasurement.MeasurementTime - result[unsampledMeasurement.Type][i].MeasurementTime).TotalSeconds > 0)
                    {
                        result[unsampledMeasurement.Type][i] = unsampledMeasurement;
                    }

                    break;
                }
            }

            else if (unsampledMeasurement.MeasurementTime > result[unsampledMeasurement.Type][position].MeasurementTime)
            {
                result[unsampledMeasurement.Type][position] = unsampledMeasurement;
            }
        }

        FormatRangeDates(result);

        return result;
    }

    private static void FormatRangeDates(Dictionary<MeasurementType, List<Measurement>> rangedMeasurements)
    {
        foreach (var measurements in rangedMeasurements.Values)
        {
            foreach (var measurement in measurements)
            {
                measurement.MeasurementTime = ConvertToIntervalDate(measurement.MeasurementTime);
            }
        }
    }

    private static DateTime ConvertToIntervalDate(DateTime date)
    {
        return new DateTime((int)Math.Ceiling((double)date.Ticks / TimeSpan.TicksPerMinute / 5) * (TimeSpan.TicksPerMinute * 5));
    }

    private static int Position(DateTime time, double startingPosition = 0)
    {
        return (int) Math.Ceiling(((double)time.Ticks / TimeSpan.TicksPerMinute / 5) - startingPosition);
    }

}

