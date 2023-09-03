using Godot;

public class Bullet : Spatial
{
    private static Bullet[] _pool = new Bullet[50];
    private static int _poolIndex = 0;

    public Vector3 Target;
    public float Speed;
    public float MaxDistance;

    private float _currentDistance;

    public override void _Process(float delta)
    {
        if (Visible)
        {
            Translate(Vector3.Forward * Speed * delta);
            _currentDistance += Speed * delta;
            if (_currentDistance >= MaxDistance)
                Visible = false;
        }
    }

    public static void InitializePool(Bullet bullet)
    {
        var mainScene = bullet.GetTree().Root.FindNode("Main", true, false);
        for (int i = 0; i < _pool.Length; i++)
        {
            var newBullet = bullet.Duplicate() as Bullet;
            mainScene.CallDeferred("add_child", newBullet);
            _pool[i] = newBullet;
        }
    }

    public static Bullet GetBullet()
    {
        var bullet = _pool[_poolIndex];
        _poolIndex++;
        if (_poolIndex >= _pool.Length)
            _poolIndex = 0;

        bullet.Target = Vector3.Zero;
        bullet.MaxDistance = 500f;
        bullet.Visible = true;
        bullet._currentDistance = 0f;
        bullet.Speed = 300f;

        return bullet;
    }
}
