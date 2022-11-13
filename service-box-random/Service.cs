using CsvHelper.Configuration.Attributes;

namespace ServiceBoxRandom;

public sealed class Service
{
    [Index(0)]
    public string Id { get; set; }
    
    [Index(1)]
    public string Name { get; set; }
    
    [Index(2)]
    public int Time { get; set; }
    
    [Index(3)]
    public int Person { get; set; }

    [Index(6)]
    public int AgentPrice { get; set; }

    public override string ToString()
    {
        return $"#{Id}, {Name}, {Person}person, {Time}min, {AgentPrice}р.";
    }
}