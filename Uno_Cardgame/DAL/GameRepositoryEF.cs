using System.Text.Json;
using Domain;
using Domain.Database;
using Helpers;

namespace DAL;

public class GameRepositoryEf : IGameRepository
{
    private readonly AppDbContext _ctx;

    public GameRepositoryEf(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public void Save(Guid id, GameState state)
    {
        // Is it already in DB?
        var game = _ctx.Games.FirstOrDefault(g => g.GameId == state.GameStateId);
        
        if (game == null)
        {
            game = new Game()
            {
                GameId = state.GameStateId,
                GameState = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Players = state.Players.Select(p => new Domain.Database.Player()
                {
                    PlayerId = p.PlayerId,
                    NickName = p.NickName!,
                    PlayerType = p.PlayerType
                }).ToList()
            };
            _ctx.Games.Add(game);
        }
        else
        {
            game.UpdatedAt = DateTime.Now;
            game.GameState = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);
        }

        var changeCount = _ctx.SaveChanges();
        Console.WriteLine("SaveChanges: " + changeCount);
    }

    public IEnumerable<(Guid id, DateTime dt)> GetSaveGames()
    {
        return _ctx.Games
            .OrderByDescending(g => g.UpdatedAt)
            .ToList()
            .Select(g => (g.GameId, g.UpdatedAt))
            .ToList();
    }


    public GameState LoadGame(Guid id)
    {
        var game = _ctx.Games.First(g => g.GameId == id);
        var gameState = JsonSerializer.Deserialize<GameState>(game.GameState, JsonHelpers.JsonSerializerOptions)!;
        var correctedStack = new Stack<GameCard>(gameState.CardsOnTable);
        gameState.CardsOnTable = correctedStack;
        return gameState;

    }
}
