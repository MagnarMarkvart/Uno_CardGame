using ConsoleKit;
using Domain;
using UnoEngine;

namespace UI.ConsoleUI.ConsoleGameLoader;

public class GameLoader
{
    private static GameEngine Engine { get; set; } = null!;
    private int SelectedCardIndex { get; set; }
    private const string Separator = "=======================";

    public GameLoader(GameEngine engine)
    {
        Engine = engine;
    }

    public void Run()
    {
        Engine.Run();
        var state = Engine.State;
        
        while (state.GameWinner == null)
        {
            DrawGameTable(state);
            var player = state.ShowCurrentPlayer();
            if (player.PlayerType == EPlayerType.Ai)
            {
                if (player.HasMadeAMove)
                {
                    state.NextPlayer();
                    continue;
                }
                Engine.MakeAiMove(state.ShowCurrentPlayer());
                continue;
            }
            ProcessUserInput();
        }

        DesignGameOver(state.GameWinner);
    }
    
    private void ProcessUserInput()
    {
        var state = Engine.State;
        var activePlayer = state.ShowCurrentPlayer();

        if (activePlayer.NeedsToPickAColour)
        {
            state.CardsOnTable.Push(CardColourPicker(state.CardsOnTable.Pop(), activePlayer));
            activePlayer.NeedsToPickAColour = false;
        }

        if (activePlayer.HasMadeAMove) 
        {
            if (GameEngine.ClassicRules)
            {
                state.NextPlayer();
                return;
            }
        }
        
        while (activePlayer.HasDrawnACard != null)
        {
            var userInput = PromptValidator.YesNoPrompt("Play the card? [Y/N]");
            var card = activePlayer.HasDrawnACard;
            if (userInput)
            {
                if (!MoveValidator.IsLegalMove(state, card))
                {
                    MessageHandler.WriteMessageWithTimeout("Illegal Move!", 1000);
                    ConsoleCleaner.ClearConsoleLine(1);
                    continue;
                }

                SelectedCardIndex = 0;
                Engine.PlayTheCard(activePlayer, card);
                return;
            }
            state.NextPlayer();
            return;
        }

        if (state.CardsOnTable.Peek().CardValue == ECardValue.WildDrawFour &&
            activePlayer.NeedsToDrawCards != 0)
        {
            ChallengeCard(state, activePlayer);
        }
        
        var activePlayerHand = activePlayer.PlayerHand;
        while (true)
        {
            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.Escape:
                    if (!PromptValidator.YesNoPrompt("Are you sure you want to exit game? [Y/N]")) return;
                    
                    Console.Clear();
                    Console.WriteLine("Thank You for Playing UNO!");
                    MessageHandler.WriteMessageWithTimeout("See You Soon! :)", 2000);
                    ConsoleCleaner.ClearConsoleLine(1);
                    Environment.Exit(0);

                    return;

                case ConsoleKey.S:
                    SaveMenu();
                    return;

                case ConsoleKey.L:
                    ShowGameState(state);
                    return;

                case ConsoleKey.D:
                    activePlayer.PlayerHand[SelectedCardIndex].IsSelected = false;
                    SelectedCardIndex = 0;
                    Console.Clear();
                    MessageHandler.WriteMessageWithTimeout($"{activePlayer.NickName} draws a card...", 2000);
                    Engine.DrawCardsFromDeck(activePlayer, 1);
                    return;

                case ConsoleKey.E:
                    if (GameEngine.ClassicRules)
                    {
                        return;
                    }
                    if (activePlayer.HasMadeAMove)
                    {
                        state.NextPlayer();
                    }
                    return;

                case ConsoleKey.UpArrow:
                    activePlayerHand[SelectedCardIndex].IsSelected = false;
                    SelectedCardIndex = (SelectedCardIndex - 1 + activePlayerHand.Count) % activePlayerHand.Count;
                    activePlayerHand[SelectedCardIndex].IsSelected = true;
                    return;

                case ConsoleKey.DownArrow:
                    activePlayerHand[SelectedCardIndex].IsSelected = false;
                    SelectedCardIndex = (SelectedCardIndex + 1) % activePlayerHand.Count;
                    activePlayerHand[SelectedCardIndex].IsSelected = true;
                    return;

                case ConsoleKey.Enter:
                    if (!MoveValidator.IsLegalMove(state, activePlayerHand[SelectedCardIndex]))
                    {
                        MessageHandler.WriteMessageWithTimeout
                            ("Illegal move. Try something else or draw a card [D].", 1500);
                        continue;
                    }

                    Engine.PlayTheCard(activePlayer, activePlayerHand[SelectedCardIndex]);
                    SelectedCardIndex = 0;
                    
                    
                    return;
            }
        }
    }

    private static void ChallengeCard(GameState state, Player activePlayer)
    {
            Console.Clear();
            DesignCardToConsole(state.CardsOnTable.Peek());
            Console.WriteLine("A Wild Draw Four card was played by: " + state.ShowLastPlayer().NickName);
            Console.WriteLine("Can be challenged!");
            Console.WriteLine("Win a challenge: Previous player will draw 4, you will play next.");
            Console.WriteLine("Lose a challenge: Draw 6 and lose your turn.");
            Console.WriteLine("No challenge: Draw 4 and lose your turn.");
            Console.WriteLine($"Previous player ({state.ShowLastPlayer().NickName}) has " +
                              $"{state.ShowLastPlayer().PlayerHand.Count} cards remaining..");
            var userInput = PromptValidator.YesNoPrompt("Challenge? [Y/N]");
            Console.Clear();
            if (userInput)
            {
                if (Engine.ChallengeWildDrawFour())
                {
                    MessageHandler.WriteMessageWithTimeout("Challenge Successful!", 2000);
                    MessageHandler.WriteMessageWithTimeout
                        ($"{state.ShowLastPlayer().NickName} draws 4 cards from the deck..", 2500);
                    return;
                }
                
                MessageHandler.WriteMessageWithTimeout("Challenge Lost!", 2000);
                MessageHandler.WriteMessageWithTimeout
                    ($"{activePlayer.NickName} draws 6 cards from the deck..", 2500);
                return;
            }

            DrawCardsIfNeeded(state);
            state.NextPlayer();
            MessageHandler.WriteMessageWithTimeout
                ($"{activePlayer.NickName} draws 4 cards from the deck..", 2500);
    }

    private static void ShowGameState(GameState state)
    {
        Console.Clear();
        Console.WriteLine($"Next Player: {state.ShowNextPlayer().NickName} - Cards On Hand: " +
                      state.ShowNextPlayer().PlayerHand.Count);
        if (state.Players.Count < 3)
        {
            Console.WriteLine("Press [L] to go back to the game.");
            KeyStrokeInput.DesiredKeyHit(ConsoleKey.L);
            return;
        }
        Console.WriteLine();
        Console.WriteLine("Other Players: ");
        foreach (var player in state.Players.Where(player => player != state.ShowNextPlayer() &&
                                                               player != state.ShowCurrentPlayer()))
        {
            Console.WriteLine(player.NickName + " - Cards On Hand: " + player.PlayerHand.Count);
        }
        Console.WriteLine();
        Console.WriteLine("Press [L] to go back to the game.");
        KeyStrokeInput.DesiredKeyHit(ConsoleKey.L);
    }


    private static ConsoleColor GetConsoleColour(string colourName)
    {
        var colorMappings = new Dictionary<string, ConsoleColor>
        {
            { "Red", ConsoleColor.Red },
            { "Blue", ConsoleColor.Blue },
            { "Green", ConsoleColor.Green },
            { "Yellow", ConsoleColor.Yellow },
            { "Special", ConsoleColor.Magenta }
        };

        return colorMappings.TryGetValue(colourName, out var colour) ? colour : ConsoleColor.White;
    }

    private static void DrawGameTable(GameState state)
    {
        var activePlayer = state.ShowCurrentPlayer();
        
        Console.Clear();
        
        IntroducePlayer(state);

        DrawCardsIfNeeded(state);
        
        Console.WriteLine(Separator);
        
        if (activePlayer.PlayerType == EPlayerType.Human)
        {
            Console.WriteLine(activePlayer.HasMadeAMove
                ? "Cards in Play: Draw a Card [D]  End Turn [E]"
                : "Cards in Play: Draw a Card [D]");
        }
        else
        {
            Console.WriteLine("Cards in Play:");
        }
        DesignCardToConsole(state.CardsOnTable.Peek());
        Console.WriteLine(Separator);
        Console.WriteLine($"Player: {activePlayer.NickName}");
        Console.WriteLine(Separator);
        
        CallUno(activePlayer);

        if (!state.ContainsHumanPlayers() ||
            (activePlayer.PlayerType != EPlayerType.Ai &&
             activePlayer.HasDrawnACard == null))
        {
            DrawPlayerHand(activePlayer);
            if (state.ContainsHumanPlayers())
            {
                Console.WriteLine("Save Game [S], ShowGameState [L], Exit Game [ESC]");
            }
            return;
        }
        
        switch (activePlayer.PlayerType)
        {
            case EPlayerType.Ai:
            {
                if (activePlayer.HasMadeAMove)
                {
                    return;
                }
                MessageHandler.WriteMessageWithTimeout
                    ($"{activePlayer.NickName} is making a move...", 2000);
                break;
            }
            case EPlayerType.Human:
            {
                DesignCardToConsole(activePlayer.HasDrawnACard!);
                break;
            }
        }
    }

    private static void DrawCardsIfNeeded(GameState state)
    {
        var activePlayer = state.ShowCurrentPlayer();
        switch (activePlayer)
        {
            case { PlayerType: EPlayerType.Human, NeedsToDrawCards: 0 or 4 }:
                return;
            case { PlayerType: EPlayerType.Ai, NeedsToDrawCards: 0}:
                return;
        }

        MessageHandler.WriteMessageWithTimeout
            ($"{activePlayer.NickName} Draws {activePlayer.NeedsToDrawCards} cards..", 2500);
        Engine.DrawCardsFromDeck(activePlayer, activePlayer.NeedsToDrawCards);
        state.ShowCurrentPlayer().HasMadeAMove = true;
    }

    private static void CallUno(Player activePlayer)
    {
        if (activePlayer.PlayerHand.Count != 1 || activePlayer.HasCalledUno) return;
        
        Console.WriteLine("UNO");
        Console.WriteLine("!UNO!");
        MessageHandler.WriteMessageWithTimeout("!!UNO!!", 2000);
        ConsoleCleaner.ClearConsoleLine(1);
        ConsoleCleaner.ClearConsoleLine(1);
        activePlayer.HasCalledUno = true;
    }

    private static void IntroducePlayer(GameState state)
    {
        var activePlayer = state.ShowCurrentPlayer();

        if (!state.ContainsHumanPlayers()) return;
        if (activePlayer is not { PlayerType: EPlayerType.Human, HasBeenIntroduced: false }) return;
        
        MessageHandler.WriteMessageWithTimeout
            ($"Next Player: {activePlayer.NickName}", 3000);
        activePlayer.HasBeenIntroduced = true;
    }

    private static void DrawPlayerHand(Player player)
    {
        foreach (var card in player.PlayerHand)
        {
            if (card.IsSelected)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
            }
            DesignCardToConsole(card);
            Console.ResetColor();
        }
    }

    private static void DesignCardToConsole(GameCard card)
    {
        var cardText = (int) card.CardValue < 10 ? card.CardValue.ToString()[5..] : card.CardValue.ToString();
        var centeredCardText = TextFormatter.CenterString(cardText, 12);
        const string cardTop =    "┌─────────────┐";
        var cardMiddle =         $"│ {centeredCardText}│";
        const string cardBottom = "└─────────────┘";

        var colour = GetConsoleColour(card.CardColour.ToString());
        Console.ForegroundColor = colour;

        Console.WriteLine(cardTop);
        Console.WriteLine(cardMiddle);
        Console.WriteLine(cardBottom);

        Console.ResetColor();
    }

    private static void SaveMenu()
    {
        Console.Clear();
        var prompt = PromptValidator.YesNoPrompt("Do you want to save the game? (Y/N): ");
        switch (prompt)
        {
            case true:
                Engine.SaveGame();
                Console.Clear();
                MessageHandler.WriteMessageWithTimeout
                    ("Game saved successfully!", 3000);
                return;
            case false:
                MessageHandler.WriteMessageWithTimeout
                    ("Game not saved!", 3000);
                return;
        }
    }

    private static void DesignGameOver(Player winner)
    {
        Console.Clear();
        Console.WriteLine("Game Over");
        Console.WriteLine($"Winner: {winner.NickName}");
        PromptValidator.OpenMenuPrompt("Main Menu [M]");
    }

    private static GameCard CardColourPicker(GameCard card, Player player)
    {
        var newCard = new GameCard();

        if (player.PlayerType == EPlayerType.Ai)
        {
            newCard.CardColour = ECardColour.Red;
            newCard.CardValue = card.CardValue;
            return newCard;
        }

        Console.Clear();
        Console.WriteLine($"What colour would you like to pick, {player.NickName}?");
        Console.WriteLine("Red [r], Green [g], Blue [b], Yellow [y]");
        Console.WriteLine("Your cards:");
        DrawPlayerHand(player);
        while (true)
        {
            var userInput = Console.ReadKey(true).Key;
            switch (userInput)
            {
                case ConsoleKey.R:
                    newCard.CardValue = card.CardValue;
                    newCard.CardColour = ECardColour.Red;
                    return newCard;

                case ConsoleKey.G:
                    newCard.CardValue = card.CardValue;
                    newCard.CardColour = ECardColour.Green;
                    return newCard;

                case ConsoleKey.B:
                    newCard.CardValue = card.CardValue;
                    newCard.CardColour = ECardColour.Blue;
                    return newCard;

                case ConsoleKey.Y:
                    newCard.CardValue = card.CardValue;
                    newCard.CardColour = ECardColour.Yellow;
                    return newCard;
                default:
                    continue;
            }
        }
    }
}