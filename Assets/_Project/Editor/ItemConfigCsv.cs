using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

// Editor-only configuration rows. They describe assets, not inventory quantities.
public sealed class ItemConfigRow
{
    public string ItemId;
    public string DisplayName;
    public ItemType ItemType;
    public int MaxStack;
    public float EffectAmount;
}

public static class ItemConfigCsv
{
    public static List<ItemConfigRow> Parse(string text)
    {
        var rows = new List<ItemConfigRow>();
        var ids = new HashSet<string>(StringComparer.Ordinal);
        using (var reader = new StringReader(text.TrimStart('\uFEFF')))
        {
            string header = reader.ReadLine();
            if (header != "itemId,displayName,itemType,maxStack,effectAmount")
                throw new FormatException("Expected header: itemId,displayName,itemType,maxStack,effectAmount");

            int lineNumber = 1;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] cells = SplitLine(line);
                if (cells.Length != 5)
                    throw new FormatException($"Line {lineNumber}: expected 5 columns.");
                if (string.IsNullOrWhiteSpace(cells[0]) || string.IsNullOrWhiteSpace(cells[1]))
                    throw new FormatException($"Line {lineNumber}: itemId and displayName are required.");
                if (!ids.Add(cells[0]))
                    throw new FormatException($"Line {lineNumber}: duplicate itemId '{cells[0]}'.");
                if (!Enum.TryParse(cells[2], out ItemType type) || type.ToString() != cells[2])
                    throw new FormatException($"Line {lineNumber}: use Water, Medical, Quest or Other.");
                if (!int.TryParse(cells[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out int stack) || stack < 1)
                    throw new FormatException($"Line {lineNumber}: maxStack must be an integer >= 1.");
                if (!float.TryParse(cells[4], NumberStyles.Float, CultureInfo.InvariantCulture, out float effect)
                    || float.IsNaN(effect) || float.IsInfinity(effect) || effect < 0f)
                    throw new FormatException($"Line {lineNumber}: effectAmount must be a finite number >= 0.");

                rows.Add(new ItemConfigRow
                {
                    ItemId = cells[0], DisplayName = cells[1], ItemType = type,
                    MaxStack = stack, EffectAmount = effect
                });
            }
        }
        if (rows.Count == 0) throw new FormatException("CSV has no item rows.");
        return rows;
    }

    // Supports quoted commas and escaped quotes; cells must stay on one line.
    private static string[] SplitLine(string line)
    {
        var cells = new List<string>();
        var value = new StringBuilder();
        bool quoted = false;
        bool closedQuote = false;
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (quoted)
            {
                if (c != '"') value.Append(c);
                else if (i + 1 < line.Length && line[i + 1] == '"') { value.Append('"'); i++; }
                else { quoted = false; closedQuote = true; }
            }
            else if (c == ',')
            {
                cells.Add(value.ToString().Trim());
                value.Clear();
                closedQuote = false;
            }
            else if (c == '"' && value.Length == 0 && !closedQuote) quoted = true;
            else if (c == '"' || (closedQuote && !char.IsWhiteSpace(c)))
                throw new FormatException("Invalid CSV quotes. Keep each cell on one line.");
            else value.Append(c);
        }
        if (quoted) throw new FormatException("Unclosed CSV quote. Multiline cells are not supported.");
        cells.Add(value.ToString().Trim());
        return cells.ToArray();
    }
}
