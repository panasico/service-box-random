using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace ServiceBoxRandom;

public static class CsvExtensions
{
    public static T[] GetRecords<T>(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";"
        });
        return csv.GetRecords<T>().ToArray();
    }
    
    public static void WriteBoxes(ServiceBox[] output, string path)
    {
        using var writer = new StreamWriter(path);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        });
        foreach (var box in output)
        {
            csv.WriteRecord(box);
            csv.NextRecord();
            csv.WriteRecords(box.Services);
        }
    }
}