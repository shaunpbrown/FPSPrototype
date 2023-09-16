using System.Collections.Generic;
using Godot;

public class DroneSpawner : Spatial
{
    [Export]
    PackedScene DroneScene;

    public static List<Drone> Drones = new List<Drone>();

    public float SpawnInterval = 3;
    public int MaxSpawnCount;
    public int MaxAliveCount;

    private float _timer;
    private int _killedDrones;
    private Player _player;

    public override void _Ready()
    {
        var droneBullet = GetNode<DroneBullet>("DroneBullet");
        DroneBullet.InitializePool(droneBullet);
        _player = GetTree().Root.FindNode("Player", true, false) as Player;
    }

    public override void _Process(float delta)
    {
        if (_timer < SpawnInterval)
            _timer += delta;

        if (Drones.Count < MaxAliveCount && _timer >= SpawnInterval && _killedDrones + Drones.Count < MaxSpawnCount)
        {
            _timer = 0;
            SpawnDrone();
        }
    }

    public void SpawnDrone()
    {
        var drone = DroneScene.Instance() as Drone;
        Drones.Add(drone);
        var mainScene = GetTree().Root.FindNode("Main", true, false);
        mainScene.CallDeferred("add_child", drone);
        drone.CallDeferred("set_translation", GlobalTransform.origin + Vector3.Up * 20);
    }

    public void DroneDestroyed(Drone drone)
    {
        Drones.Remove(drone);
        _killedDrones++;
        _player.RoundInformation.SetObjective($"DESTROY DRONES\n {_killedDrones}/{MaxSpawnCount}");

        if (_killedDrones >= MaxSpawnCount)
        {
            _killedDrones = 0;
            MaxAliveCount = 0;
            MaxSpawnCount = 0;
            _player.RoundInformation.FinishRound();
        }
    }
}