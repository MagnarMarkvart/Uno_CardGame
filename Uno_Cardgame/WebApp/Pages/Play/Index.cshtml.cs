using System.Globalization;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnoEngine;

namespace WebApp.Pages.Play;

public class Index : PageModel
{
    private readonly IGameRepository _repo;
    public GameEngine? Engine { get; set; }

    public Index(IGameRepository repo)
    {
        _repo = repo;
    }

    [BindProperty(SupportsGet = true)]
    public Guid? GameId { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public Guid? PlayerId { get; set; }
    
    public IActionResult OnGet()
    {
        if (GameId != null && PlayerId != null)
        {
            return RedirectToPage("./PlayGame", new
            {
                GameId,
                PlayerId
            });
        }
        
        if (GameId != null)
        {
            var gameId = GameId.GetValueOrDefault();
            var gameState = _repo.LoadGame(gameId);
            Engine = new GameEngine(_repo, gameState);
        }

        return Page();
    }
}