namespace Domain;

public class GameCard
{
    public ECardColour CardColour { get; set; }
    public ECardValue CardValue { get; set; }
    public int DrawCard { get; set; }   
    public bool IsSelected { get; set; }
    
    public override string ToString()
    {
        return (int) CardValue < 10 ? $"{CardColour} {(int) CardValue}" : $"{CardColour} {CardValue}";
    }
}