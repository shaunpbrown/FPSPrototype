using Godot;
using System;

public class TargetSpawner : Spatial
{
    [Export]
    public PackedScene Target;

    private CSGBox _spawnArea;

    public override void _Ready()
    {
        _spawnArea = GetNode<CSGBox>("SpawnArea");
        SpawnTarget();
    }

    public void SpawnTarget()
    {
        var point = new Vector3
        {
            x = (float)GD.RandRange(-_spawnArea.Width, _spawnArea.Width),
            y = (float)GD.RandRange(-_spawnArea.Height, _spawnArea.Height),
            z = 0
        };

        var target = (Target)Target.Instance();
        AddChild(target);
        target.Translation = point;
    }
}
