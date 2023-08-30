using Godot;
using System;

public class DroneSpawner : Spatial
{
    [Export]
    PackedScene DroneScene;

    public override void _Ready()
    {
        SpawnDrone();
    }

    public void SpawnDrone()
    {
        var drone = DroneScene.Instance() as Drone;
        var mainScene = GetTree().Root.FindNode("Main", true, false);
        mainScene.CallDeferred("add_child", drone);
        drone.CallDeferred("set_translation", GlobalTransform.origin + Vector3.Up * 20);
    }
}
