using balancer;

var fileName = "C:/Users/Paul_/Documents/Minecraft Packs/MultiMC/instances/No More Kings/.minecraft/config/spawnbalanceutility/BiomeMobWeight.txt";
Console.WriteLine($"reading {fileName}");
var entries = CsvHandler.ParseCsv(fileName);
var groupedEntries = entries.GroupBy(entry => entry.Biome + entry.EntityType);
foreach (var groupedEntry in groupedEntries)
{
    WeightFixer.FixWeight(groupedEntry);
}

Console.WriteLine($"parsed {entries.Count()} entities");
