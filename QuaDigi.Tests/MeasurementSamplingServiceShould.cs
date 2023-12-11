using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuaDigi.Models;

namespace QuaDigi.Tests;

public class MeasurementSamplingServiceShould
{

    [Fact]
    public async Task SampleItems_ShouldReturnEmpty_WhenSamplingStartIsOlder()
    {
        var service = MockService();
        var unmappedValues = new List<Measurement>()
        {
            new(DateTime.Now.AddMinutes(-5), 15, MeasurementType.HRATE),
            new(DateTime.Now.AddMinutes(-20), 15, MeasurementType.TEMP),
            new(DateTime.Now.AddMinutes(-51), 15, MeasurementType.SPO2),
            new(DateTime.Now.AddMinutes(-50), 15, MeasurementType.HRATE),
        };

        var result = service.Sample(DateTime.Now, unmappedValues);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SampleItems_ShouldGroup()
    {
        var service = MockService();
        var unmappedValues = new List<Measurement>()
        {
            new(new DateTime(2002,10,11,10,14,0), 1, MeasurementType.HRATE),
            new(new DateTime(2002,10,11,10,11,0), 2, MeasurementType.HRATE),
            new(new DateTime(2002,10,11,10,15,0), 3, MeasurementType.HRATE),
            new(new DateTime(2002,10,11,10,20,0), 4, MeasurementType.TEMP),
            new(new DateTime(2002,10,11,10,21,0), 5, MeasurementType.HRATE),
            new(new DateTime(2002,10,11,10,20,5), 6, MeasurementType.TEMP),
            new(new DateTime(2002,10,11,10,10,0), 7, MeasurementType.TEMP),
        };

        var result = service.Sample(new DateTime(2002, 10, 11, 10, 0, 0), unmappedValues);
        Assert.Equal(2, result.Count);
        Assert.Equal(3, result[MeasurementType.HRATE][0].MeasurementValue); 
        Assert.Equal(5, result[MeasurementType.HRATE][1].MeasurementValue);
        Assert.Equal(7, result[MeasurementType.TEMP][0].MeasurementValue); 
        Assert.Equal(4, result[MeasurementType.TEMP][1].MeasurementValue); 
        Assert.Equal(6, result[MeasurementType.TEMP][2].MeasurementValue); 
    }

    [Fact]
    public async Task SampleItems_ShouldOrderAscending()
    {
        var service = MockService();
        var unmappedValues = new List<Measurement>()
        {
            new(new DateTime(2002,10,11,10,14,0), 1, MeasurementType.HRATE),
            new(new DateTime(2002,10,11,10,11,0), 2, MeasurementType.HRATE),
            new(new DateTime(2002,10,11,10,15,0), 3, MeasurementType.HRATE),
            new(new DateTime(2002,10,11,10,20,0), 4, MeasurementType.TEMP),
            new(new DateTime(2002,10,11,10,21,0), 5, MeasurementType.HRATE),
            new(new DateTime(2002,10,11,10,20,5), 6, MeasurementType.TEMP),
            new(new DateTime(2002,10,11,10,10,0), 7, MeasurementType.TEMP),
            new(new DateTime(2002,10,11,10,20,1), 8, MeasurementType.TEMP),
            new(new DateTime(2002,10,11,10,25,0), 9, MeasurementType.TEMP),

        };

        var result = service.Sample(new DateTime(2002, 10, 11, 10, 0, 0), unmappedValues);
        Assert.Equivalent(new Measurement(new DateTime(2002, 10, 11, 10, 15, 0), 3, MeasurementType.HRATE), result[MeasurementType.HRATE][0]);
        Assert.Equivalent(new Measurement(new DateTime(2002, 10, 11, 10, 25, 0), 5, MeasurementType.HRATE), result[MeasurementType.HRATE][1]);
        Assert.Equivalent(new Measurement(new DateTime(2002, 10, 11, 10, 10, 0), 7, MeasurementType.TEMP) , result[MeasurementType.TEMP][0]);
        Assert.Equivalent(new Measurement(new DateTime(2002, 10, 11, 10, 20, 0), 4, MeasurementType.TEMP), result[MeasurementType.TEMP][1]);
    }

    private MeasurementSamplingService MockService()
    {
        var service = new MeasurementSamplingService();

        return service;
    }
}

