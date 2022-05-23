using balancer;

var inputFileName = "C:/Users/Paul_/Documents/Minecraft Packs/MultiMC/instances/No More Kings/.minecraft/config/spawnbalanceutility/BiomeMobWeight.txt";
var outputFileName = "C:/Users/Paul_/Documents/Minecraft Packs/MultiMC/instances/No More Kings/.minecraft/config/spawnbalanceutility/BiomeMobWeight.csv";
Console.WriteLine($"reading {inputFileName}");
var entries = CsvHandler.ParseCsv(inputFileName);
var groupedEntries = entries.GroupBy(entry => entry.Biome + entry.EntityType);
foreach (var groupedEntry in groupedEntries)
{
    WeightFixer.FixWeight(groupedEntry);
}
Console.WriteLine($"writing {outputFileName}");
CsvHandler.WriteCsv(outputFileName, entries);

Console.WriteLine($"parsed {entries.Count()} entities");
