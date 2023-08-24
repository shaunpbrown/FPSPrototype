using Godot;

public class Pistol : Spatial
{
    [Export]
    public NodePath MuzzleFlashPath;
    [Export]
    public NodePath BulletPath;
    [Export]
    public NodePath BulletSplashPath;
    [Export]
    public NodePath BulletHolePath;

    private float _muzzleTimer;
    private Spatial _muzzleFlash;
    private Spatial _bullet;
    private Spatial _bulletHole;
    private CPUParticles _bulletSplash;
    private float _fireCooldown;
    private Player _player;
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _muzzleFlash = GetNode<Spatial>(MuzzleFlashPath);
        _muzzleFlash.Hide();

        _bullet = GetNode<Spatial>(BulletPath);
        _bullet.Hide();

        _bulletSplash = GetNode<CPUParticles>(BulletSplashPath);

        _bulletHole = GetNode<Spatial>(BulletHolePath);
        _bulletHole.Hide();

        _player = GetTree().Root.FindNode("Player", true, false) as Player;
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayerEvents");
    }

    public override void _Process(float delta)
    {
        if (_muzzleTimer > 0)
        {
            _muzzleTimer -= delta;
            if (_muzzleTimer < 0)
                _muzzleFlash.Hide();
        }

        if (_fireCooldown > 0)
            _fireCooldown -= delta;
    }

    public bool CanFireBullet()
    {
        return _fireCooldown <= 0;
    }

    public void PullTrigger()
    {
        if (CanFireBullet())
        {
            _animationPlayer.Play("Fire");
        }
    }

    public void FireBullet()
    {
        var playerHead = _player.GetNode<Spatial>("Head");
        var origin = playerHead.GlobalTransform.origin;
        var direction = -playerHead.GlobalTransform.basis.z.Normalized();

        _fireCooldown = 0.3f;
        _muzzleFlash.Show();
        _muzzleTimer = 0.1f;

        var clonedBullet = _bullet.Duplicate() as Bullet;
        var mainScene = GetTree().Root.FindNode("Main", true, false);
        mainScene.AddChild(clonedBullet);
        clonedBullet.GlobalTranslation = origin;
        clonedBullet.LookAt(origin + direction, Vector3.Up);
        clonedBullet.Translate(Vector3.Right * 0.5f + Vector3.Down * 0.2f);
        clonedBullet.Show();

        var hit = BulletRayCast(origin, direction);
        if (hit.Count > 0)
        {
            var hitPoint = (Vector3)hit["position"];

            var clonedBulletSplash = _bulletSplash.Duplicate() as CPUParticles;
            mainScene.AddChild(clonedBulletSplash);
            clonedBulletSplash.GlobalTranslation = hitPoint;
            clonedBulletSplash.Emitting = true;

            var hitEntity = hit["collider"] as Spatial;
            var hitNormal = (Vector3)hit["normal"];

            var clonedBulletHole = _bulletHole.Duplicate() as Spatial;
            hitEntity.AddChild(clonedBulletHole);
            clonedBulletHole.GlobalTransform = new Transform(new Quat(), hitPoint + hitNormal * .01f).LookingAt(hitPoint + hitNormal, (Vector3.Up + Vector3.Forward).Normalized());
            clonedBulletHole.Show();

            if (hitEntity is IShootable shootable)
            {
                clonedBullet.MaxDistance = (hitPoint - origin).Length();
                shootable.Shot(hitPoint);
            }
        }
    }

    public Godot.Collections.Dictionary BulletRayCast(Vector3 origin, Vector3 direction)
    {
        Vector3 rayOrigin = origin;
        Vector3 rayEnd = direction * 1000 + rayOrigin;

        PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
        Godot.Collections.Dictionary hit = spaceState.IntersectRay(rayOrigin, rayEnd);

        return hit;
    }
}