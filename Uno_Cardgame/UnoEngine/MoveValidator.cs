using Domain;

namespace UnoEngine;

public abstract class MoveValidator
{
    public static bool IsLegalMove(GameState state, GameCard card)
    {
        var topCardOnTable = state.CardsOnTable.Peek();
        
        return card.CardColour == topCardOnTable.CardColour
               || card.CardValue == topCardOnTable.CardValue
               || card.CardColour == ECardColour.Special;
    }
}