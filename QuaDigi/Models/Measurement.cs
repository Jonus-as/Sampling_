namespace QuaDigi.Models;

public class Measurement(DateTime measurementTime, Double measurementValue, MeasurementType type)
{
    public DateTime MeasurementTime { get; set; } = measurementTime;
    public Double MeasurementValue { get; } = measurementValue;
    public MeasurementType Type { get; } = type;
}

