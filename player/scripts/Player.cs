using Godot;

public class Player : KinematicBody
{
	public float MouseSensitivity = 0.005f;
	public float MovementSpeed = 6.0f;
	public float JumpStrength = 5.0f;
	public float Gravity = -9.8f;
	public bool IsUsingPrinter;
	public RoundInformation RoundInformation;

	private Vector3 _velocity = new Vector3();
	private Spatial _head;
	private Spatial _gunCamera;
	private Spatial _headCamera;
	private Gun _gun;
	private Health _health;
	private TextureRect _crosshair;
	private Texture _hitMarkerTexture;
	private Texture _crosshairTexture;
	private float _hitMarkerTimer;
	private bool _isTitleSreenOpen = true;
	private AudioStreamPlayer _hitAudioStreamPlayer;
	private ViewportContainer _gunViewport;

	public Player() : base()
	{
		RoundInformation = new RoundInformation(this);
	}

	public override void _Ready()
	{
		_head = GetNode<Spatial>("Head");
		_gun = GetNode<Gun>("Head/GunHolder/Gun");
		_health = new Health(this);
		_gunCamera = GetNode<Spatial>("CanvasLayer/ViewportContainer/Viewport/GunCamera");
		_headCamera = GetNode<Spatial>("Head/Camera");
		_crosshair = GetNode<TextureRect>("CanvasLayer/Panel/Crosshair");
		_crosshairTexture = _crosshair.Texture;
		_hitMarkerTexture = GD.Load<Texture>("res://player/crosshair_simple_hitconfirm.png");
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_hitAudioStreamPlayer = GetNode<AudioStreamPlayer>("HitAudioStream");
		_gunViewport = GetNode<ViewportContainer>("CanvasLayer/ViewportContainer");
	}

	public override void _Input(InputEvent @event)
	{
		if (_isTitleSreenOpen || _health.IsDead())
			return;

		if (IsUsingPrinter || Input.MouseMode != Input.MouseModeEnum.Captured)
			return;

		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			RotateY(-eventMouseMotion.Relative.x * MouseSensitivity);
			_head.RotateX(-eventMouseMotion.Relative.y * MouseSensitivity);
			_head.RotationDegrees = new Vector3(Mathf.Clamp(_head.RotationDegrees.x, -90, 90), 0, 0);
		}
	}

	public override void _Process(float delta)
	{
		_gunViewport.RectSize = GetViewport().Size;
		_gunViewport.GetNode<Viewport>("Viewport").Size = GetViewport().Size;

		if (_isTitleSreenOpen || _health.IsDead())
		{
			_health.DeathProcess(delta);
			return;
		}
		_gunCamera.GlobalTransform = _headCamera.GlobalTransform;

		if (_hitMarkerTimer > 0)
			_hitMarkerTimer -= delta;
		else if (_crosshair.Texture != null)
			_crosshair.Texture = _crosshairTexture;

		if (IsUsingPrinter)
			return;

		_health.HealthProcess(delta);

		var rayHit = InteractRayCast();
		if (rayHit.Count > 0)
		{
			var node = rayHit["collider"] as Node;
			if (node is IInteractable interactable)
			{
				SetInteractText(interactable.GetInteractText());
				if (Input.IsActionJustPressed("interact"))
					interactable.Interact(this);
			}
		}
		else
		{
			SetInteractText("");
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (_isTitleSreenOpen || _health.IsDead())
			return;

		if (Input.IsActionPressed("app_exit"))
			GetTree().Quit();

		if (IsUsingPrinter)
			return;

		if (Input.IsActionJustPressed("mouse_capture"))
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Visible ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible; Vector3 direction = new Vector3();

		if (Input.IsActionPressed("move_forward"))
			direction -= Transform.basis.z;
		if (Input.IsActionPressed("move_backward"))
			direction += Transform.basis.z;
		if (Input.IsActionPressed("move_left"))
			direction -= Transform.basis.x;
		if (Input.IsActionPressed("move_right"))
			direction += Transform.basis.x;

		if (Input.IsActionJustPressed("shoot")
		|| (_gun.GunMods.IsModEquipped("Rate of fire") && Input.IsActionPressed("shoot")))
		{
			_gun.PullTrigger();
		}

		if (_gun.GunMods.IsModEquipped("Rocket Launcher") && Input.IsActionJustPressed("rocket") && _gun.CanFireRocket())
		{
			_gun.FireRockets();
		}

		_velocity.y += Gravity * delta;

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			_velocity.y = JumpStrength;
		}

		direction.y = 0;
		direction = direction.Normalized();
		_velocity.x = direction.x * MovementSpeed;
		_velocity.z = direction.z * MovementSpeed;

		MoveAndSlide(_velocity, new Vector3(0, 1, 0));
	}

	public Godot.Collections.Dictionary InteractRayCast()
	{
		var origin = _head.GlobalTransform.origin;
		var direction = -_head.GlobalTransform.basis.z.Normalized();

		Vector3 rayOrigin = origin;
		Vector3 rayEnd = direction * 6 + rayOrigin;

		PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
		uint collisionMask = 2; // 0100
		Godot.Collections.Dictionary hit = spaceState.IntersectRay(rayOrigin, rayEnd, null, collisionMask);

		return hit;
	}

	public void HitMarker()
	{
		_crosshair.Texture = _hitMarkerTexture;
		_hitMarkerTimer = 0.1f;
		_hitAudioStreamPlayer.PitchScale = (float)GD.RandRange(0.8f, 1.2f);
		_hitAudioStreamPlayer.Play();
	}

	public void SetInteractText(string text)
	{
		var interactText = GetNode<Label>("CanvasLayer/Panel/InteractText");
		interactText.Text = text;
	}

	public void TakeDamage(Vector3 hitPoint)
	{
		_health.TakeDamage(1);
	}

	public void CloseTitleScreen()
	{
		_isTitleSreenOpen = false;
	}
}
