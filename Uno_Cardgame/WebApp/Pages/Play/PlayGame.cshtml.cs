using System.Globalization;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnoEngine;

namespace WebApp.Pages.Play;

public class PlayGame : PageModel
{
    private readonly IGameRepository _repo;
    
    public PlayGame(IGameRepository repo)
    {
        _repo = repo;
    }
    public GameEngine? Engine { get; set; }
    public Player? Player { get; set; }
    
    [BindProperty (SupportsGet = true)] 
    public Guid GameId { get; set; }
    
    [BindProperty (SupportsGet = true)] 
    public Guid PlayerId { get; set; }
    
    [BindProperty] 
    public bool? MakeAiMove { get; set; }
    
    [BindProperty] 
    public bool? CallUno { get; set; }
    
    [BindProperty] 
    public bool? TakeACard { get; set; }
    
    [BindProperty] 
    public bool? EndTurn { get; set; }
    
    [BindProperty] 
    public string? Card { get; set; }
    
    [BindProperty] 
    public string? ColorSelected { get; set; }
    
    public IActionResult OnGet()
    {
        CreateEngine();
        if (Engine!.State.ShowCurrentPlayer().HasMadeAMove)
        {
            Engine.State.NextPlayer();
            Engine.SaveGame();
            
            return RedirectToPage("./PlayGame", new
            {
                GameId,
                PlayerId
            });
        }

        if (Engine!.State.ShowCurrentPlayer().NeedsToDrawCards > 0 &&
            Engine.State.ShowCurrentPlayer().PlayerType != EPlayerType.Ai)
        {
            Engine.DrawCardsFromDeck(Engine!.State.ShowCurrentPlayer(),
                Engine!.State.ShowCurrentPlayer().NeedsToDrawCards);
            Engine.State.NextPlayer();
            Engine.SaveGame();
            
            return RedirectToPage("./PlayGame", new
            {
                GameId,
                PlayerId
            });
        }
        
        if (!PlayerId.Equals(Guid.Empty))
        {
            Player = Engine!.State.Players.Find(p => p.PlayerId == PlayerId);
        }
        return Page();
    }
    
    public IActionResult OnPost()
    {
        CreateEngine();
        if (MakeAiMove == true)
        {
            Engine!.MakeAiMove(Engine.State.ShowCurrentPlayer());
            Engine.SaveGame();
        }

        else if (CallUno == true)
        {
            Engine!.State.ShowCurrentPlayer().HasCalledUno = true;
            Engine.SaveGame();
        }
        
        else if (TakeACard == true)
        {
            var player = Engine!.State.ShowCurrentPlayer();
            player.NeedsToDrawCards++;
            Engine!.DrawCardsFromDeck(player, player.NeedsToDrawCards);
            Engine.SaveGame();
        }
        
        else if (EndTurn == true)
        {
            Engine!.State.NextPlayer();
            Engine.SaveGame();
        }
        
        else if (Card != null)
        {
            return CardValidation(Engine!.State.ShowCurrentPlayer()
                .PlayerHand.Find(c => c.ToString() == Card)!);
        }
        
        else if(ColorSelected != null)
        {
            return ColorPicker();
        }
        
        
        return RedirectToPage("./PlayGame", new
        {
            GameId,
            PlayerId
        });
    }

    public IActionResult CardValidation(GameCard card)
    {
        Player = Engine!.State.Players.Find(p => p.PlayerId == PlayerId);
        if (MoveValidator.IsLegalMove(Engine!.State, card))
        {
            Engine.PlayTheCard(Player!, Player!.PlayerHand.Find(c => c.ToString() == Card)!);
            if (Player.NeedsToPickAColour)
            {
                Engine.SaveGame();
                return RedirectToPage("./PlayGame", new
                {
                    GameId,
                    PlayerId
                });
            }
            Engine!.State.NextPlayer();
            Engine.SaveGame();
        }
        else
        {
            return Page();
        }

        return RedirectToPage("./PlayGame", new
        {
            GameId,
            PlayerId
        });
    }
    
    public IActionResult ColorPicker()
    {
        Player = Engine!.State.Players.Find(p => p.PlayerId == PlayerId);
        var card = Engine.State.CardsOnTable.Pop();
        switch (ColorSelected)
        {
            case "Red":
                card.CardColour = ECardColour.Red;
                Engine.State.CardsOnTable.Push(card);
                break;
            case "Yellow":
                card.CardColour = ECardColour.Yellow;
                break;
            case "Blue":
                card.CardColour = ECardColour.Blue;
                break;
            case "Green":
                card.CardColour = ECardColour.Green;
                break;
        }
        Engine.State.CardsOnTable.Push(card);
        Player!.NeedsToPickAColour = false;
        Engine.State.NextPlayer();
        Engine.SaveGame();
        
        return RedirectToPage("./PlayGame", new
        {
            GameId,
            PlayerId
        });
    }

    private void CreateEngine()
    {
        var gameState = _repo.LoadGame(GameId);
        Engine = new GameEngine(_repo, gameState);
    }
    
    public string GetLastHistoryDate(Guid gameId)
    {
        var info = _repo.GetSaveGames()
            .Where(g => g.id == gameId).MaxBy(g => g.dt);

        return info.dt.ToString("dd-MM-yyyy--HH-mm-ss");
    }
}