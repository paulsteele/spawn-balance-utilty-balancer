
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace balancer;

public static class CsvHandler
{
    private static CsvConfiguration Configuration => new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            ShouldQuote = (args => false),
        };

    public static List<Entry> ParseCsv(string fileName)
    {
        using var reader = new StreamReader(fileName);
        using var csv = new CsvReader(reader, Configuration);

        return csv.GetRecords<Entry>().ToList();
    }

    public static void WriteCsv(string fileName, List<Entry> entries)
    {
        using var writer = new StreamWriter(fileName);
        using var csv = new CsvWriter(writer, Configuration);

        csv.WriteRecords(entries);
    }
}