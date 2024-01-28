using DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IGameRepository _repo;

    public IndexModel(ILogger<IndexModel> logger, IGameRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    public string DatabaseInfo { get; set; } = default!;
    
    public void OnGet()
    {
        var games = new List<Guid>();
        var countOfStates = _repo.GetSaveGames().Count();
        foreach (var saveGame in _repo.GetSaveGames())
        {
            if (!games.Contains(saveGame.id))
            {
                games.Add(saveGame.id);
            }
        }

        DatabaseInfo = $"There are {games.Count} Game(s) saved in the Database!";
    }
}