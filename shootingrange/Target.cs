using Godot;

public class Target : StaticBody, IShootable
{
    public void Shot(Vector3 hitPoint)
    {
        var spawner = GetTree().Root.FindNode("TargetSpawner", true, false) as TargetSpawner;
        spawner.SpawnTarget();
        QueueFree();
    }
}
