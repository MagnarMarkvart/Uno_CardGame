namespace UI.ConsoleUI.ConsoleMenuSystem;

public class MenuItem
{
    public string MenuLabel { get; set; }
    public bool IsSelected { get; set; }
    private Action? OnSelect { get; set; }
    
    public MenuItem(string label, Action? onSelect)
    {
        MenuLabel = label;
        OnSelect = onSelect;
    }
    
    public void Execute()
    {
        OnSelect?.Invoke();
    }

}