using ConsoleKit;
using Domain;

namespace UI.ConsoleUI.ConsoleMenuSystem;

public abstract class PlayerConfig
{
    private static readonly List<string> AiPlayerNames = new()
    {
        "CyberByte",
        "TechnoWiz",
        "NebulaNova",
        "QuantumPulse",
        "RoboStrategist",
        "SynthMind",
        "PixelMaster"
    };
    
    public static List<Player> SetNicknames(int humanPlayers, int aiPlayers)
    {
        var players = new List<Player>();

        for (var player = 0; player < humanPlayers; player++)
        {
            Console.Write($"Human Player {player + 1} Nickname: ");
            var nickName = TextInput.ProcessToString();
            ConsoleCleaner.ClearConsoleLine(1);
            Console.WriteLine($"Human Player {player + 1} Nickname: {nickName}");
            var newPlayer = new Player(nickName, new List<GameCard>())
            {
                PlayerType = EPlayerType.Human
            };
            players.Add(newPlayer);
        }
        
        for (var player = 0; player < aiPlayers; player++)
        {
            var newPlayer = SetAiPlayer(player);
            Console.WriteLine($"AI Player {player + 1} Nickname: {newPlayer.NickName}");
            players.Add(newPlayer);
            Thread.Sleep(1000);
        }
        
        return players;
    }

    public static List<Player> SetGamePlayers(List<string> humanNickNames, int aiCount)
    {
        var playerList = new List<Player>();
        
        foreach (var humanNickName in humanNickNames)
        {
            var player = new Player(humanNickName, new List<GameCard>());
            playerList.Add(player);
        }

        for (var ai = 0; ai < aiCount; ai++)
        {
            playerList.Add(SetAiPlayer(ai));
        }

        return playerList;
    }

    private static Player SetAiPlayer(int aiPlayer)
    {
        var nickName = AiPlayerNames[aiPlayer];
        var newPlayer = new Player(nickName, new List<GameCard>())
        {
            PlayerType = EPlayerType.Ai
        };
        
        return newPlayer;
    }
}