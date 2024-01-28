namespace ConsoleKit;

public abstract class ConsoleCleaner
{
    public static void ClearConsoleLine(int linesBack)
    {
        var currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, currentLineCursor - linesBack); // Move the cursor to the desired line
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor - linesBack); // Move the cursor back to the bottom line
    }

}