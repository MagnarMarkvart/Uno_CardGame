namespace ConsoleKit;

public abstract class KeyStrokeInput
{
    public static string ToStringValue()
    {
        var keyInfo = Console.ReadKey(true).KeyChar.ToString().ToLower();
        
        return keyInfo;
    }

    private static ConsoleKey ToConsoleKey()
    {
        var keyInfo = Console.ReadKey(true).Key;
        
        return keyInfo;
    }

    public static int ToInt(int min, int max)
    {
        if (min == max)
        {
            return min;
        }
        
        while (true)
        {
            var keyInfo = ToInt();
            
            if (keyInfo > min - 1 && keyInfo < max + 1)
            {
                return keyInfo;
            }

            var errorMessage = $"Invalid input. Please press a number key from {min} to {max}.";
            MessageHandler.WriteMessageWithTimeout
                (errorMessage, 2000);
        }
    }

    private static int ToInt()
    {
        while (true)
        {
            var keyInfo = ToStringValue();

            if (char.IsDigit(char.Parse(keyInfo))) return int.Parse(keyInfo);
            
            MessageHandler.WriteMessageWithTimeout
                ("Invalid input. Please press a number key.", 2000);

        }
    }
    
    public static bool DesiredKeyHit(ConsoleKey desiredKey)
    {
        while (true)
        {
            if (!Console.KeyAvailable) continue;
            
            var keyInfo = ToConsoleKey();
            
            if (keyInfo == desiredKey)
            {
                return true;
            }

        }
    }
    
}