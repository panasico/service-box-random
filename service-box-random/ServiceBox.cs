using CsvHelper.Configuration.Attributes;

namespace ServiceBoxRandom;

public sealed class ServiceBox
{
    [Index(0)]
    public string Name { get; set; }
    
    [Index(1)]
    public int Price { get; set; }
    
    [Index(2)]
    public int MaxPrice { get; set; }
    
    [Index(3)]
    public int MinPrice { get; set; }

    [Ignore]
    public List<Service> Services { get; set; } = new();

    public ServiceBox Clone() => (ServiceBox)MemberwiseClone();

    public override string ToString()
    {
        return $"{Name}, {Price}р. ({MinPrice}-{MaxPrice})\n{string.Join("\n", Services.Select(s => s.ToString()))}";
    }
}