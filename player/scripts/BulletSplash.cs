using Godot;

public class BulletSplash : CPUParticles
{
    private static BulletSplash[] _pool = new BulletSplash[50];
    private static int _poolIndex = 0;

    public static void InitializePool(Spatial gun, PackedScene bulletSplash)
    {
        var mainScene = gun.GetTree().Root.FindNode("Main", true, false);
        for (int i = 0; i < _pool.Length; i++)
        {
            var newBulletSplash = bulletSplash.Instance() as BulletSplash;
            mainScene.CallDeferred("add_child", newBulletSplash);
            _pool[i] = newBulletSplash;
        }
    }

    public static BulletSplash GetBulletSplash()
    {
        var bulletSplash = _pool[_poolIndex];
        _poolIndex++;
        if (_poolIndex >= _pool.Length)
            _poolIndex = 0;

        var audioStreamPlayer = bulletSplash.GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
        audioStreamPlayer.Play();

        return bulletSplash;
    }
}
