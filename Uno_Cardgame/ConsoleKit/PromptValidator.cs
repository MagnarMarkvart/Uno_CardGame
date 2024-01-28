namespace ConsoleKit;

public abstract class PromptValidator
{
    public static bool YesNoPrompt(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            var pressedKey = KeyStrokeInput.ToStringValue();
            
            switch (pressedKey)
            {
                case "y":
                    return true;
                case "n":
                    return false;
                default:
                    ConsoleCleaner.ClearConsoleLine(1);
                    break;
            }
            
        }
    }
    
    public static void OpenMenuPrompt(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            var pressedKey = KeyStrokeInput.ToStringValue();
            if (pressedKey == "m")
            {
                return;
            }
            
            ConsoleCleaner.ClearConsoleLine(1);
        }
    }
}