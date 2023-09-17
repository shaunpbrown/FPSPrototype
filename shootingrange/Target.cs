using Godot;

public class Target : StaticBody, IShootable
{
    public void Shot(Vector3 hitPoint)
    {
        var spawner = GetTree().Root.FindNode("TargetSpawner", true, false) as TargetSpawner;
        spawner.TargetHit();
        RemoveTarget();

        var player = GetTree().Root.FindNode("Player", true, false) as Player;
        player.HitMarker();
    }

    public void RemoveTarget()
    {
        BulletHole.RemoveBulletHoles(this);
        CallDeferred("queue_free");
    }
}
