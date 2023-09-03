using Godot;
using System;

public class DroneSpawner : Spatial
{
    [Export]
    PackedScene DroneScene;

    private float _timer;
    private float _spawnInterval = 5;
    private int _spawnCount = 5;

    public override void _Ready()
    {
        var droneBullet = GetNode<DroneBullet>("DroneBullet");
        DroneBullet.InitializePool(droneBullet);
    }

    public override void _Process(float delta)
    {
        if (_timer < _spawnInterval)
            _timer += delta;

        if (_spawnCount > 0 && _timer > _spawnInterval)
        {
            _timer = 0;
            _spawnCount--;
            SpawnDrone();
        }
    }

    public void SpawnDrone()
    {
        var drone = DroneScene.Instance() as Drone;
        var mainScene = GetTree().Root.FindNode("Main", true, false);
        mainScene.CallDeferred("add_child", drone);
        drone.CallDeferred("set_translation", GlobalTransform.origin + Vector3.Up * 20);
    }

    public void SetSpawnCount(int count)
    {
        _spawnCount = count;
    }
}
