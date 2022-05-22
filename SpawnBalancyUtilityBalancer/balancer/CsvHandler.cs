
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace balancer;

public static class CsvHandler
{

    public static List<Entry> ParseCsv(string filename)
    {
        using var reader = new StreamReader(filename);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
        };
        using var csv = new CsvReader(reader, config);

        return csv.GetRecords<Entry>().ToList();
    }
}