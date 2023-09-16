using Godot;

public class TargetSpawner : Spatial
{
    [Export]
    public PackedScene Target;

    private CSGBox _spawnArea;
    private int _hitTargets;
    private int _maxTargetCount = 10;

    public override void _Ready()
    {
        _spawnArea = GetNode<CSGBox>("SpawnArea");
        SpawnTarget();
    }

    public void TargetHit()
    {
        _hitTargets++;

        var player = GetTree().Root.FindNode("Player", true, false) as Player;
        player.RoundInformation.SetObjective($"SHOOT TARGETS \n {_hitTargets} / {_maxTargetCount}");

        if (_hitTargets < _maxTargetCount)
        {
            SpawnTarget();
        }
        else
        {
            player.RoundInformation.FinishRound();
        }
    }

    public void SpawnTarget()
    {
        var point = new Vector3
        {
            x = (float)GD.RandRange(-_spawnArea.Width, _spawnArea.Width) / 2f,
            y = (float)GD.RandRange(-_spawnArea.Height, _spawnArea.Height) / 2f,
            z = 0
        };

        var target = (Target)Target.Instance();
        AddChild(target);
        target.Translation = point;
    }
}
