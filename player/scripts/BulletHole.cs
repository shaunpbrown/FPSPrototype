using Godot;

public class BulletHole : Spatial
{
    private static BulletHole[] _pool = new BulletHole[100];
    private static int _poolIndex = 0;

    public static void InitializePool(BulletHole bulletHole)
    {
        var mainScene = bulletHole.GetTree().Root.FindNode("Main", true, false);
        for (int i = 0; i < _pool.Length; i++)
        {
            var newBulletHole = bulletHole.Duplicate() as BulletHole;
            mainScene.CallDeferred("add_child", newBulletHole);
            _pool[i] = newBulletHole;
        }
    }

    public static BulletHole GetBulletHole()
    {
        var bulletHole = _pool[_poolIndex];
        _poolIndex++;
        if (_poolIndex >= _pool.Length)
            _poolIndex = 0;

        if (bulletHole == null)
        {
            var mainScene = bulletHole.GetTree().Root.FindNode("Main", true, false);
            var newBulletHole = bulletHole.Duplicate() as BulletHole;
            mainScene.AddChild(newBulletHole);
            _pool[_poolIndex] = newBulletHole;
            bulletHole = newBulletHole;
        }

        return bulletHole;
    }

    public static void RemoveBulletHoles(Node node)
    {
        var mainScene = node.GetTree().Root.FindNode("Main", true, false) as Spatial;
        foreach (var bulletHole in NodeHelper.GetChildren<BulletHole>(node))
        {
            NodeHelper.ReparentNode(bulletHole, mainScene);
            bulletHole.Visible = false;
        }
    }
}