namespace UI.ConsoleUI.ConsoleMenuSystem;

public class Menu
{
    private string? Title { get; set; }
    private List<MenuItem> MenuItems { get; set; }
    private int SelectedOptionIndex { get; set; }
    private const string Separator = "=======================";
    public bool ExitMenu;
    
    public void Run()
    {
        while (!ExitMenu)
        {
            DisplayMenu();
            ProcessUserInput();
        }
    }

    public Menu(string? title, List<MenuItem> menuItems)
    {
        Title = title;
        MenuItems = menuItems;
        menuItems[SelectedOptionIndex].IsSelected = true;
    }
    
    private void DisplayMenu()
    {
        Console.Clear();
        Console.WriteLine(">>UNO<<");
        Console.WriteLine(Separator);
        
        if (!string.IsNullOrWhiteSpace(Title))
        {
            Console.WriteLine(Title);
            Console.WriteLine(Separator);
        }
        
        foreach (var menuItem in MenuItems)
        {
            if (menuItem.IsSelected)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            Console.WriteLine(menuItem.MenuLabel);
            Console.ResetColor();
        }
        
        Console.WriteLine(Separator);
        Console.WriteLine("Hint: Use arrow keys to move.");
    }

    private void ProcessUserInput()
    {
        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.UpArrow:
                MenuItems[SelectedOptionIndex].IsSelected = false;
                SelectedOptionIndex = (SelectedOptionIndex - 1 + MenuItems.Count) % MenuItems.Count;
                MenuItems[SelectedOptionIndex].IsSelected = true;
                break;

            case ConsoleKey.DownArrow:
                MenuItems[SelectedOptionIndex].IsSelected = false;
                SelectedOptionIndex = (SelectedOptionIndex + 1) % MenuItems.Count;
                MenuItems[SelectedOptionIndex].IsSelected = true;
                break;

            case ConsoleKey.Enter:
                MenuItems[SelectedOptionIndex].Execute();
                break;
        }
    }
}