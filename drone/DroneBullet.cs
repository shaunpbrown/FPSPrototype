using Godot;

public class DroneBullet : Spatial
{
    private static DroneBullet[] _pool = new DroneBullet[50];
    private static int _poolIndex = 0;

    public Vector3 Target;
    public float Speed;
    public float MaxDistance;

    private float _currentDistance;

    public override void _Process(float delta)
    {
        if (Visible)
        {
            var hit = BulletRayCast(Speed * delta);
            if (hit.Count > 0)
            {
                var hitPoint = (Vector3)hit["position"];

                var clonedBulletSplash = BulletSplash.GetBulletSplash();
                clonedBulletSplash.GlobalTranslation = hitPoint;
                clonedBulletSplash.Emitting = true;
                clonedBulletSplash.GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D").Stop();

                var hitEntity = hit["collider"] as Spatial;
                var hitNormal = (Vector3)hit["normal"];

                var clonedBulletHole = BulletHole.GetBulletHole();
                NodeHelper.ReparentNode(clonedBulletHole, hitEntity);
                clonedBulletHole.GlobalTransform = new Transform(new Quat(), hitPoint + hitNormal * .01f).LookingAt(hitPoint + hitNormal, (Vector3.Up + Vector3.Forward).Normalized());
                clonedBulletHole.Show();

                if (hitEntity is Player player)
                {
                    player.TakeDamage(hitPoint);
                }

                Visible = false;
            }

            Translate(Vector3.Forward * Speed * delta);
            _currentDistance += Speed * delta;
            if (_currentDistance >= MaxDistance)
                Visible = false;
        }
    }

    public static void InitializePool(DroneBullet bullet)
    {
        var mainScene = bullet.GetTree().Root.FindNode("Main", true, false);
        for (int i = 0; i < _pool.Length; i++)
        {
            var newBullet = bullet.Duplicate() as DroneBullet;
            mainScene.CallDeferred("add_child", newBullet);
            _pool[i] = newBullet;
        }
    }

    public static DroneBullet GetBullet()
    {
        var bullet = _pool[_poolIndex];
        _poolIndex++;
        if (_poolIndex >= _pool.Length)
            _poolIndex = 0;

        bullet.Target = Vector3.Zero;
        bullet.MaxDistance = 500f;
        bullet.Visible = true;
        bullet._currentDistance = 0f;
        bullet.Speed = 20f;

        return bullet;
    }

    public Godot.Collections.Dictionary BulletRayCast(float distance)
    {
        Vector3 rayOrigin = GlobalTransform.origin;
        Vector3 rayEnd = -GlobalTransform.basis.z.Normalized() * distance + rayOrigin;

        PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
        uint collisionMask = 1; // 1000
        Godot.Collections.Dictionary hit = spaceState.IntersectRay(rayOrigin, rayEnd, null, collisionMask);

        return hit;
    }
}