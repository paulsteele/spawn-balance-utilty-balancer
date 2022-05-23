using CsvHelper.Configuration.Attributes;

namespace balancer;

public class Entry
{
    [Index(0)]
    public int Row { get; set; }
    [Index(1)]
    public string BiomeCategory { get; set; }
    [Index(2)]
    public string Biome { get; set; }
    [Index(3)]
    public string EntityType { get; set; }
    [Index(4)]
    public string Entity { get; set; }
    [Index(5)]
    public int Weight { get; set; }
    [Index(6)]
    public int MinGroup { get; set; }
    [Index(7)]
    public int MaxGroup { get; set; }

    [Ignore]
    public string Source => Entity.Split(":")[0].Trim();
    public override string ToString()
    {
        return $"{Biome}, {Entity}, {Weight}";
    }
}