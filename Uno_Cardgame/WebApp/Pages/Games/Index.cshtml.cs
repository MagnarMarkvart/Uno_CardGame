using DAL;
using Domain.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Games
{
    public class IndexModel : PageModel
    {
        private readonly IGameRepository _repo;

        public IndexModel(IGameRepository repo)
        {
            _repo = repo;
        }

        public IList<Game> Game { get;set; } = default!;

        public IActionResult OnGet()
        {
            var games = _repo.GetSaveGames();
            
            Game = new List<Game>();

            foreach (var g in games)
            {
                var gameState = _repo.LoadGame(g.id);
                Game.Add(new Game()
                {
                    GameId = g.id,
                    UpdatedAt = g.dt,
                    Players = gameState.Players.Select(p => new Player()
                    {
                        PlayerId = p.PlayerId,
                        NickName = p.NickName!,
                        PlayerType = p.PlayerType,
                        GameId = g.id
                    }).ToList()
                });
            }

            return Page();
        }
    }
}
