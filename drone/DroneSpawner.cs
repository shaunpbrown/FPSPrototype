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
    private CSGBox _spawnArea;

    public override void _Ready()
    {
        var droneBullet = GetNode<DroneBullet>("DroneBullet");
        DroneBullet.InitializePool(droneBullet);
        _player = GetTree().Root.FindNode("Player", true, false) as Player;
        _spawnArea = GetNode<CSGBox>("SpawnArea");
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
        var spawnPosition = _spawnArea.GlobalTransform.origin + new Vector3(
            (float)GD.RandRange(-_spawnArea.Scale.x / 2, _spawnArea.Scale.x / 2),
            0,
            (float)GD.RandRange(-_spawnArea.Scale.z / 2, _spawnArea.Scale.z / 2)
        );
        var drone = DroneScene.Instance() as Drone;
        Drones.Add(drone);
        var mainScene = GetTree().Root.FindNode("Main", true, false);
        mainScene.CallDeferred("add_child", drone);
        drone.CallDeferred("set_translation", spawnPosition + Vector3.Up * 12);
    }

    public void DroneDestroyed(Drone drone)
    {
        Drones.Remove(drone);
        _killedDrones++;
        if (_player.RoundInformation.RoundNumber <= 10)
            _player.RoundInformation.SetObjective($"DESTROY DRONES\n {_killedDrones}/{MaxSpawnCount}");
        else
        {
            _player.RoundInformation.DronesDestroyed++;
            _player.RoundInformation.SetObjective($"SURVIVE AS LONG AS POSSIBLE\nDRONES DESTROYED: {_player.RoundInformation.DronesDestroyed}");
        }

        if (_killedDrones >= MaxSpawnCount)
        {
            _killedDrones = 0;
            MaxAliveCount = 0;
            MaxSpawnCount = 0;
            _player.RoundInformation.FinishRound();
        }
    }
}