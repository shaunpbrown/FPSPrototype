using Godot;
using System;

public class TitleScreenScript : CanvasLayer
{
    public override void _PhysicsProcess(float delta)
    {
        foreach (KeyList key in Enum.GetValues(typeof(KeyList)))
        {
            if ((int)key != 0 && Input.IsKeyPressed((int)key))
            {
                var player = GetTree().Root.FindNode("Player", true, false) as Player;
                player.CloseTitleScreen();
                Input.MouseMode = Input.MouseModeEnum.Captured;
                QueueFree();
                break;
            }
        }
    }
}