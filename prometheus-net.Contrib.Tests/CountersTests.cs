using System;
using System.Collections.Generic;
using Prometheus.Contrib.EventListeners.Counters;
using Xunit;

namespace prometheus_net.Contrib.Tests
{
    public class CountersTests
    {
        [Fact]
        public void MeanCounter_WhenMeanIsGreaterThanZero_ThenUseMeanForValue()
        {
            var meanCounter = new MeanCounter("test1", "test2", "test3");

            var eventData = new Dictionary<string, object>
            {
                ["Name"] = "cpu-usage",
                ["DisplayName"] = "CPU Usage",
                ["Mean"] = 15d,
                ["StandardDeviation"] = 0,
                ["Count"] = 1,
                ["Min"] = 0,
                ["Max"] = 0,
                ["InvervalSec"] = 9.9996233,
                ["Series"] = "Interval=10000",
                ["CounterType"] = "Mean",
                ["Metadata"] = "",
                ["DisplayUnits"] = "%",
            };

            meanCounter.TryReadEventCounterData(eventData);
            
            Assert.Equal(15, meanCounter.Metric.Value);
        }
        
        [Fact]
        public void MeanCounter_WhenMeanIsZero_ThenUseCountForValue()
        {
            var meanCounter = new MeanCounter("test1", "test2", "test3");

            var eventData = new Dictionary<string, object>
            {
                ["Name"] = "active-db-contexts",
                ["DisplayName"] = "Active DbContexts",
                ["Mean"] = 0d,
                ["StandardDeviation"] = 0,
                ["Count"] = 5,
                ["Min"] = 0,
                ["Max"] = 0,
                ["InvervalSec"] = 9.9996233,
                ["Series"] = "Interval=10000",
                ["CounterType"] = "Mean",
                ["Metadata"] = "",
                ["DisplayUnits"] = "",
            };

            meanCounter.TryReadEventCounterData(eventData);
            
            Assert.Equal(5, meanCounter.Metric.Value);
        }
        
        [Fact]
        public void MeanCounter_WhenMeanIsNaN_ThenUseZeroForValue()
        {
            var meanCounter = new MeanCounter("test1", "test2", "test3");

            var eventData = new Dictionary<string, object>
            {
                ["Name"] = "compiled-query-cache-hit-rate",
                ["DisplayName"] = "Query Cache Hit Rate",
                ["Mean"] = double.NaN,
                ["StandardDeviation"] = 0,
                ["Count"] = 1,
                ["Min"] = double.NaN,
                ["Max"] = double.NaN,
                ["InvervalSec"] = 9.9996233,
                ["Series"] = "Interval=10000",
                ["CounterType"] = "Mean",
                ["Metadata"] = "",
                ["DisplayUnits"] = "%"
            };

            meanCounter.TryReadEventCounterData(eventData);
            
            Assert.Equal(0, meanCounter.Metric.Value);
        }
        
        [Fact]
        public void IncrementCounter_WhenIncrement_ThenUseIncrementForValue()
        {
            var incrementCounter = new IncrementCounter("test1", "test2", "test3");

            var eventData = new Dictionary<string, object>
            {
                ["Name"] = "optimistic-concurrency-failures-per-second",
                ["DisplayName"] = "Optimistic Concurrency Failures",
                ["DisplayRateTimeScale"] = "00:00:01",
                ["Increment"] = 1d,
                ["IntervalSec"] = 675.144775,
                ["Metadata"] = "",
                ["Series"] = "Interval=10000",
                ["CounterType"] = "Sum",
                ["DisplayUnits"] = ""
            };

            incrementCounter.TryReadEventCounterData(eventData);
            
            Assert.Equal(1, incrementCounter.Metric.Value);
        }
    }
}