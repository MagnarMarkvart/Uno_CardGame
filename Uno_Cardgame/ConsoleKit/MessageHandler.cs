namespace ConsoleKit;

public abstract class MessageHandler
{
    public static void WriteMessageWithTimeout(string message, int timeoutMilliseconds)
    {
        Console.WriteLine(message);
        Thread.Sleep(timeoutMilliseconds);
        ConsoleCleaner.ClearConsoleLine(1);
    }
}