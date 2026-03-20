using TaskTracker.Services;

namespace TaskTracker.Tests;

public class ExportCsvTests
{
    [Fact]
    public void ExportCsv_EmptyTaskList_ReturnsHeaderOnly()
    {
        var service = new TaskService();

        var csv = service.ExportCsv();

        Assert.Equal("Id,Title,IsCompleted,Priority,CreatedAt", csv);
    }

    [Fact]
    public void ExportCsv_PlainTitle_ExportsCorrectly()
    {
        var service = new TaskService();
        service.Add("Buy milk");

        var csv = service.ExportCsv();
        var lines = csv.Split(Environment.NewLine);

        Assert.Equal(2, lines.Length);
        // Title should be quoted
        Assert.Contains("\"Buy milk\"", lines[1]);
        // Should have exactly 5 fields
        var fields = ParseCsvLine(lines[1]);
        Assert.Equal(5, fields.Length);
        Assert.Equal("1", fields[0]);
        Assert.Equal("Buy milk", fields[1]);
        Assert.Equal("False", fields[2]);
    }

    [Fact]
    public void ExportCsv_TitleWithCommas_IsProperlyQuoted()
    {
        var service = new TaskService();
        service.Add("Buy milk, eggs, bread");

        var csv = service.ExportCsv();
        var lines = csv.Split(Environment.NewLine);

        Assert.Equal(2, lines.Length);
        Assert.Contains("\"Buy milk, eggs, bread\"", lines[1]);
        // Even with commas in title, CSV should parse to exactly 5 fields
        var fields = ParseCsvLine(lines[1]);
        Assert.Equal(5, fields.Length);
        Assert.Equal("Buy milk, eggs, bread", fields[1]);
    }

    [Fact]
    public void ExportCsv_TitleWithDoubleQuotes_IsEscapedAndQuoted()
    {
        var service = new TaskService();
        service.Add("Fix \"critical\" bug");

        var csv = service.ExportCsv();
        var lines = csv.Split(Environment.NewLine);

        Assert.Equal(2, lines.Length);
        // Double quotes inside should be escaped as ""
        Assert.Contains("\"Fix \"\"critical\"\" bug\"", lines[1]);
        var fields = ParseCsvLine(lines[1]);
        Assert.Equal(5, fields.Length);
        Assert.Equal("Fix \"critical\" bug", fields[1]);
    }

    /// <summary>
    /// Simple RFC 4180 CSV line parser for test assertions.
    /// </summary>
    private static string[] ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var current = "";
        var inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            if (inQuotes)
            {
                if (line[i] == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current += '"';
                        i++; // skip escaped quote
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    current += line[i];
                }
            }
            else
            {
                if (line[i] == '"')
                {
                    inQuotes = true;
                }
                else if (line[i] == ',')
                {
                    fields.Add(current);
                    current = "";
                }
                else
                {
                    current += line[i];
                }
            }
        }
        fields.Add(current);
        return fields.ToArray();
    }
}
