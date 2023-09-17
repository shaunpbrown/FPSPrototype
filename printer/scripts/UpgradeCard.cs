using System;
using Godot;

public class UpgradeCard : TextureButton
{
    public Action<UpgradeCard> SelectedAction;

    public string ModName { get; set; }

    private TextureRect _card;

    public override void _Ready()
    {
        Connect("pressed", this, nameof(UpgradeCardSelected));
        _card = GetNode<TextureRect>("Foreground/Card");
    }

    private void UpgradeCardSelected()
    {
        SelectedAction?.Invoke(this);
    }

    public void SetCard(string modName)
    {
        ModName = modName;
        var texture = GD.Load<Texture>($"res://printer/ui/{modName.Trim().Replace(" ", "").ToLower()}.png");
        _card.Texture = texture;
    }
}
