using Domain;

namespace UI.WebUI;

public abstract class GameBoard
{
    public static string GetImagePath(GameCard gameCard)
    {
        string path;
        const string postFix = ".png";
        switch ((int) gameCard.CardValue)
        {
            case < 10:
                path = $"~/Images/{gameCard.ToString().Replace(" ", "").ToLower()}";
                return path + postFix;
            case < 13:
                path = $"~/Images/{gameCard.CardColour.ToString().ToLower()}{(int) gameCard.CardValue}";
                return path + postFix;
            default:
                path = $"~/Images/{gameCard.CardValue.ToString().ToLower()}{(int) gameCard.CardValue}";
                return path + postFix;
        }
    }
}