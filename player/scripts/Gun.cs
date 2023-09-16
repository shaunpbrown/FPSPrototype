using Godot;

public class Gun : Spatial
{
	public GunStats GunStats = new GunStats();
	public GunMods GunMods;

	private float _muzzleTimer;
	private Spatial _muzzleFlash;
	private float _fireCooldown;
	private Player _player;
	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_muzzleFlash = GetNode<Spatial>("Gun_v3/Armature/Skeleton/BoneAttachment9/MuzzleFlash");
		_muzzleFlash.Hide();

		var bullet = GetNode<Spatial>("Bullet") as Bullet;
		Bullet.InitializePool(bullet);
		bullet.Hide();

		var bulletSplash = GetNode<CPUParticles>("BulletSplash") as BulletSplash;
		BulletSplash.InitializePool(bulletSplash);

		var bulletHole = GetNode<Spatial>("BulletHole") as BulletHole;
		BulletHole.InitializePool(bulletHole);
		bulletHole.Hide();

		_player = GetTree().Root.FindNode("Player", true, false) as Player;
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayerEvents");

		GunMods = new GunMods(this);
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
			_animationPlayer.Stop();
			_animationPlayer.Play("Fire");
		}
	}

	public void FireBullet()
	{
		for (int i = 0; i < GunStats.Projectiles; i++)
		{
			var playerHead = _player.GetNode<Spatial>("Head");
			var origin = playerHead.GlobalTransform.origin;
			var direction = -playerHead.GlobalTransform.basis.z.Normalized();

			var maxSpreadAngle = GunStats.Spread / 20;
			float yaw = (float)GD.RandRange(-maxSpreadAngle, maxSpreadAngle);
			direction = direction.Rotated(Vector3.Up, Mathf.Deg2Rad(yaw));
			float pitch = (float)GD.RandRange(-maxSpreadAngle, maxSpreadAngle);
			direction = direction.Rotated(Vector3.Right, Mathf.Deg2Rad(pitch));

			_fireCooldown = GunStats.FireCooldown;
			_muzzleFlash.Show();
			_muzzleTimer = 0.1f;

			var clonedBullet = default(Bullet);
			if (i < 5)
			{
				clonedBullet = Bullet.GetBullet();
				clonedBullet.GlobalTranslation = origin;
				clonedBullet.LookAt(origin + direction, Vector3.Up);
				clonedBullet.Translate(Vector3.Right * 0.5f + Vector3.Down * 0.2f);
				clonedBullet.Show();
			}

			var hit = BulletRayCast(origin, direction);
			if (hit.Count > 0)
			{
				var hitPoint = (Vector3)hit["position"];

				var clonedBulletSplash = BulletSplash.GetBulletSplash();
				clonedBulletSplash.GlobalTranslation = hitPoint;
				clonedBulletSplash.Emitting = true;

				var hitEntity = hit["collider"] as Spatial;
				var hitNormal = (Vector3)hit["normal"];

				//				var clonedBulletHole = BulletHole.GetBulletHole();
				//				NodeHelper.ReparentNode(clonedBulletHole, hitEntity);
				//				clonedBulletHole.GlobalTransform = new Transform(new Quat(), hitPoint + hitNormal * .01f).LookingAt(hitPoint + hitNormal, (Vector3.Up + Vector3.Forward).Normalized());
				//				clonedBulletHole.Show();

				if (hitEntity is IShootable shootable)
				{
					if (clonedBullet != null)
						clonedBullet.MaxDistance = (hitPoint - origin).Length();
					shootable.Shot(hitPoint);
				}
			}
		}

		var player = GetTree().Root.FindNode("Player", true, false) as Player;
		if (player != null)
		{
			var gun = player.GetNode<Gun>("Head/GunHolder/Gun");
			if (gun != null)
			{
				var head = player.GetNode<Spatial>("Head");
				head.RotateX(Mathf.Deg2Rad(gun.GunStats.Recoil));
			}
		}
	}

	public Godot.Collections.Dictionary BulletRayCast(Vector3 origin, Vector3 direction)
	{
		Vector3 rayOrigin = origin;
		Vector3 rayEnd = direction * 1000 + rayOrigin;

		PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
		uint collisionMask = 1; // 1000
		Godot.Collections.Dictionary hit = spaceState.IntersectRay(rayOrigin, rayEnd, null, collisionMask);

		return hit;
	}
}
