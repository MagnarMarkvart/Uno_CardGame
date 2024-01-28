using System.Text;

namespace ConsoleKit;

public abstract class TextFormatter
{
    private static string FormatTextLineToFitLineLength(string text, int lineLength)
    {
        var result = new StringBuilder();
        var words = text.Split(' ');
        var currentLineLength = 0;

        foreach (var word in words)
        {
            if (currentLineLength + word.Length + 1 <= lineLength)
            {
                // Add the word to the current line
                if (currentLineLength > 0)
                {
                    result.Append(' ');
                    currentLineLength++;
                }
                result.Append(word);
                currentLineLength += word.Length;
            }
            else
            {
                // Start a new line
                result.AppendLine();
                result.Append(word);
                currentLineLength = word.Length;
            }
        }

        return result.ToString();
    }

    public static string FormatTextFromFile(string filePath)
    {
        var rulesText = File.ReadAllText(filePath);

        // Split the text into lines
        var lines = rulesText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Initialize a StringBuilder to build the formatted text
        var formattedRules = new StringBuilder();

        foreach (var line in lines)
        {
            // Break long lines into shorter lines of the specified length
            var formattedLine = FormatTextLineToFitLineLength(line, 60);
            formattedRules.AppendLine(formattedLine);
        }

        return formattedRules.ToString();
    }
    
    public static string CenterString(string str, int width)
    {
        var padding = (width - str.Length) / 2;
        return str.PadLeft(padding + str.Length).PadRight(width);
    }
}