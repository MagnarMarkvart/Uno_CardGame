using Domain;

namespace DAL;

public interface IGameRepository
{
    void Save(Guid id, GameState state);
    IEnumerable<(Guid id, DateTime dt)> GetSaveGames();
    GameState LoadGame(Guid id);
}

