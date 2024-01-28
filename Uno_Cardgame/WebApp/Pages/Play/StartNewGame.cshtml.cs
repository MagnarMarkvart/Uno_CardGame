using DAL;
using Domain;
using Domain.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UI.ConsoleUI.ConsoleMenuSystem;
using UnoEngine;

namespace WebApp.Pages.Play;

public class StartNewGame : PageModel
{
    private readonly IGameRepository _repo;

    public StartNewGame(IGameRepository repo)
    {
        _repo = repo;
    }
    
    

    public void OnGet()
    {
        
    }
    
    [BindProperty]
    public int? AiCount { get; set; }
    
    [BindProperty (SupportsGet = true)]
    public int? HumanCount { get; set; }

    [BindProperty] 
    public List<string>? NickNames { get; set; } = new List<string>();

    
    public IActionResult OnPost()
    {
        if (NickNames!.Count == 0 && AiCount == null ||
            NickNames.Count + AiCount < 2) return Page();
        
        
        var players = PlayerConfig.SetGamePlayers(NickNames, AiCount.GetValueOrDefault());
        var gameState = new GameState
        {
            Players = players
        };
        var gameEngine = new GameEngine(_repo, gameState);
        gameEngine.Run();
        gameEngine.SaveGame();
        var gameId = gameEngine.State.GameStateId;
        
        return RedirectToPage("./Index", new
        {
            GameId = gameId,
            HistoryDate = _repo.GetSaveGames().First(sg => sg.id == gameId).dt
                .ToString("dd-MM-yyyy--HH-mm-ss")
        });
    }
}