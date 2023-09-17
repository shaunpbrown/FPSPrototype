using Godot;
public class RoundInformation
{
    public int RoundNumber = 1;

    private Player _player;
    private Label _objectiveLabel;
    private Printer _printer;
    private DroneSpawner _droneSpawner;

    public RoundInformation(Player player)
    {
        _player = player;
    }

    public void StartNextRound()
    {
        if (_printer == null)
            _printer = _player.GetTree().Root.FindNode("Printer", true, false) as Printer;

        if (_droneSpawner == null)
            _droneSpawner = _player.GetTree().Root.FindNode("DroneSpawner", true, false) as DroneSpawner;

        RoundNumber++;
        _printer.IsLocked = true;

        switch (RoundNumber)
        {
            case 2:
                _droneSpawner.MaxSpawnCount = 2;
                _droneSpawner.MaxAliveCount = 1;
                SetObjective($"DESTROY DRONES\n 0/{_droneSpawner.MaxSpawnCount}");
                break;
            case 3:
                _droneSpawner.MaxSpawnCount = 4;
                _droneSpawner.MaxAliveCount = 2;
                SetObjective($"DESTROY DRONES\n 0/{_droneSpawner.MaxSpawnCount}");
                break;
            case 4:
                _droneSpawner.MaxSpawnCount = 6;
                _droneSpawner.MaxAliveCount = 4;
                SetObjective($"DESTROY DRONES\n 0/{_droneSpawner.MaxSpawnCount}");
                break;
            case 5:
                _droneSpawner.MaxSpawnCount = 8;
                _droneSpawner.MaxAliveCount = 6;
                SetObjective($"DESTROY DRONES\n 0/{_droneSpawner.MaxSpawnCount}");
                break;
            case 6:
                _droneSpawner.MaxSpawnCount = 10;
                _droneSpawner.MaxAliveCount = 8;
                SetObjective($"DESTROY DRONES\n 0/{_droneSpawner.MaxSpawnCount}");
                break;
            case 7:
                _droneSpawner.MaxSpawnCount = 12;
                _droneSpawner.MaxAliveCount = 10;
                SetObjective($"DESTROY DRONES\n 0/{_droneSpawner.MaxSpawnCount}");
                break;
            case 8:
                _droneSpawner.MaxSpawnCount = 14;
                _droneSpawner.MaxAliveCount = 12;
                SetObjective($"DESTROY DRONES\n 0/{_droneSpawner.MaxSpawnCount}");
                break;
            case 9:
                _droneSpawner.MaxSpawnCount = 16;
                _droneSpawner.MaxAliveCount = 14;
                SetObjective($"DESTROY DRONES\n 0/{_droneSpawner.MaxSpawnCount}");
                break;
            case 10:
                _droneSpawner.MaxSpawnCount = 18;
                _droneSpawner.MaxAliveCount = 16;
                SetObjective($"DESTROY DRONES\n 0/{_droneSpawner.MaxSpawnCount}");
                break;
            default:
                _droneSpawner.MaxSpawnCount = 20;
                _droneSpawner.MaxAliveCount = 5;
                _droneSpawner.SpawnInterval = .5f;
                SetObjective($"DESTROY DRONES\n 0/{_droneSpawner.MaxSpawnCount}");
                break;
        }
    }

    public void FinishRound()
    {
        if (_printer == null)
            _printer = _player.GetTree().Root.FindNode("Printer", true, false) as Printer;

        if (RoundNumber != 1)
        {
            _printer.ShuffleCards();
        }

        SetObjective("UPGRADE GUN USING PRINTER");
        _printer.IsLocked = false;

    }

    public void SetObjective(string objective)
    {
        if (_objectiveLabel == null)
            _objectiveLabel = _player.GetNode<Label>("CanvasLayer/Panel/Objective");

        _objectiveLabel.Text = objective;
    }
}