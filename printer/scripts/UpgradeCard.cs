using System;
using Godot;

public class UpgradeCard : TextureButton
{
    public Action<UpgradeCard> SelectedAction;

    [Export]
    public string ModName { get; set; }

    public override void _Ready()
    {
        Connect("pressed", this, nameof(UpgradeCardSelected));
    }

    private void UpgradeCardSelected()
    {
        SelectedAction?.Invoke(this);
    }
}
