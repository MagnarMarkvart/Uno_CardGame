using System.Globalization;
using ConsoleKit;
using DAL;
using Domain;
using UI.ConsoleUI.ConsoleGameLoader;
using UnoEngine;

namespace UI.ConsoleUI.ConsoleMenuSystem;

public class MenuLoader
{
    private IGameRepository GameRepository { get; }
    private GameLoader? Loader { get; set; }
    private readonly Stack<Menu> _menuStack = new();
    private GameState State { get; set; } = new();
    private const string RulesFilePath = "../../../../Assets/ClassicRules.txt";
    
    public MenuLoader(IGameRepository gameRepository)
    {
        GameRepository = gameRepository;
    }
    
    public void Run()
    {
        LoadMenu("Main Menu");
    }
    
    private void PauseMenu()
    {
        if (_menuStack.Count <= 0) return;
        
        _menuStack.Peek().ExitMenu = true;
        Console.Clear();
    }
    
    private void ResumeMenu()
    {
        if (_menuStack.Count <= 0) return;
        
        _menuStack.Peek().ExitMenu = false;
        _menuStack.Peek().Run();
    }
    
    private void CloseMenuSystem()
    {
        while (_menuStack.Count != 0)
        {
            _menuStack.Pop().ExitMenu = true;
        }
        Console.Clear();
    }

    private void NavigateBack()
    {
        _menuStack.Pop().ExitMenu = true;
        _menuStack.Peek().ExitMenu = false;
        _menuStack.Peek().Run();
    }
    
    private void LoadGame(Guid id, DateTime dt)
    {
        CloseMenuSystem();
        State = GameRepository.LoadGame(id);
        Loader = new GameLoader(new GameEngine(GameRepository, State));
        Loader.Run();
    }
    
    private void DisplayRules()
    {
        PauseMenu();
        Console.WriteLine("Close Rules (B)");
        Console.WriteLine(TextFormatter.FormatTextFromFile(RulesFilePath));
        Console.WriteLine("Close Rules (B)");
        if (KeyStrokeInput.DesiredKeyHit(ConsoleKey.B))
        {
            ResumeMenu();
        }
    }

    private List<MenuItem> GetSavedGamesAsMenuItems()
    {
        var savedGames = GameRepository.GetSaveGames();
        
        var menuItems = savedGames.Select(i => 
            new MenuItem($"{i.id} - {i.dt.ToString(CultureInfo.CurrentCulture)}", () =>
                LoadGame(i.id, i.dt))).ToList();
        menuItems.Add(CreateBackMenuItem());
        
        return menuItems;
    }
    
    private void ConfigurePlayers()
    {
        CloseMenuSystem();
        Console.WriteLine("How many human players? (0-7)");
        var humanPlayers = KeyStrokeInput.ToInt(0, 7);
        Console.WriteLine($"You have selected {humanPlayers} human player(s).");
        
        var minAiPlayers = humanPlayers switch
        {
            0 => 2,
            1 => 1,
            _ => 0
        };

        Console.WriteLine($"How many AI players? ({minAiPlayers}-{7 - humanPlayers})");
        var aiPlayers = KeyStrokeInput.ToInt(minAiPlayers, 7 - humanPlayers);
        Console.WriteLine($"You have selected {aiPlayers} AI player(s).");
        Console.WriteLine("Set player names.");
        State.Players = PlayerConfig.SetNicknames(humanPlayers, aiPlayers);
        var gameLoader = new GameLoader(new GameEngine(GameRepository, State));
        gameLoader.Run();
    }

    private static void ExitGame()
    {
        Console.Clear();
        Console.WriteLine("Thank You for Playing UNO!");
        MessageHandler.WriteMessageWithTimeout("See You Soon! :)", 2000);
        ConsoleCleaner.ClearConsoleLine(1);
        Environment.Exit(0);
    }

    private MenuItem CreateBackMenuItem()
    {
        return new MenuItem("Back", NavigateBack);
    }
    
    private void LoadMenu(string? menuTitle)
    {
        PauseMenu();
        
        switch (menuTitle)
        {
            case "Main Menu":
                _menuStack.Push(new Menu("Main Menu", new List<MenuItem>()
                {
                    new("Play", () => LoadMenu("Game Menu")),
                    new("Options", () => LoadMenu("Options")),
                    new("Exit", ExitGame)
                }));
                break;

            case "Game Menu":
                _menuStack.Push(new Menu("Game Menu", new List<MenuItem>()
                {
                    new("New Game", () => LoadMenu("New Game")),
                    new("Load Game", () => LoadMenu("Load Game")),
                    CreateBackMenuItem()
                }));
                break;

            case "Options":
                _menuStack.Push(new Menu("Options", new List<MenuItem>()
                {
                    new("Classic Rules", DisplayRules),
                    new("House Rules", (() => LoadMenu("House Rules"))),
                    CreateBackMenuItem()
                }));
                break;

            case "New Game":
                _menuStack.Push(new Menu("New Game", new List<MenuItem>()
                {
                    new("One-Round Game", ConfigurePlayers),
                    new("500 Point Game", () => MessageHandler.WriteMessageWithTimeout
                        ("Coming Soon..", 2000)),
                    CreateBackMenuItem()
                }));
                break;
            
            case "Load Game":
                _menuStack.Push(new Menu("Load Game", GetSavedGamesAsMenuItems()));
                break;
            
            case "House Rules":
                _menuStack.Push(new Menu("House Rules", new List<MenuItem>()
                {
                    new("Stacking", ImplementHouseRules),
                    new("Wild Shuffle Hands Card", ImplementHouseRules),
                    CreateBackMenuItem()
                }));
                break;
        }
        
        ResumeMenu();
    }

    private static void ImplementHouseRules()
    {
        MessageHandler.WriteMessageWithTimeout
            ("Not Possible Yet...", 2000);
    }
}