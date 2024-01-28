namespace ConsoleKit;

public abstract class TextInput
{
    public static string ProcessToString()
    {
        while (true)
        {
            var userInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(userInput))
            {
                return userInput.Trim();
            }
            
            MessageHandler.WriteMessageWithTimeout("Input can't be null!", 2000);
        }
    }
}