using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json;
using Domain;
using Helpers;

namespace DAL;

public class GameRepositoryFileSystem : IGameRepository
{
    private static readonly string SaveLocation = GetPathForSaving();

    public void Save(Guid id, GameState state)
    {
        var content = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);
        var fileName = Path.ChangeExtension(id.ToString(), ".json");
        
        if (!Path.Exists(SaveLocation))
        {
            Directory.CreateDirectory(SaveLocation);
        }

        File.WriteAllText(Path.Combine(SaveLocation, fileName), content);
    }

    public IEnumerable<(Guid id, DateTime dt)> GetSaveGames()
    {
        var data = Directory.EnumerateFiles(SaveLocation);
        var res = data
            .Select(
                path => (
                    Guid.Parse(Path.GetFileNameWithoutExtension(path)),
                    File.GetLastWriteTime(path)
                )
            ).ToList();
        
        return res;
    }

    public GameState LoadGame(Guid id)
    {
        var fileName = Path.ChangeExtension(id.ToString(), ".json");
        var jsonStr = File.ReadAllText(Path.Combine(SaveLocation, fileName));
        var gameState = JsonSerializer.Deserialize<GameState>(jsonStr, JsonHelpers.JsonSerializerOptions);
        var correctedStack = new Stack<GameCard>(gameState!.CardsOnTable);
        gameState.CardsOnTable = correctedStack;
        
        if (gameState == null) throw new SerializationException($"Cannot deserialize {jsonStr}");

        return gameState;
    }

    private static string GetPathForSaving()
    {
        // var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        // var saveDirectory = Path.Combine(baseDir, "Save_Games");
        //
        // if (!Directory.Exists(saveDirectory))
        // {
        //     Directory.CreateDirectory(saveDirectory);
        // }
        //
        // return saveDirectory;
        var path = "C:/Users/ELuck/Documents/TalTech/CSharp/icd0008-23f/Uno_Cardgame/ConsoleApp/bin/Debug/net7.0/Save_Games";
        return path;
    }
}
