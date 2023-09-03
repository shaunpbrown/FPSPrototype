using Godot;

public class Player : KinematicBody
{
	public float MouseSensitivity = 0.005f;
	public float MovementSpeed = 6.0f;
	public float JumpStrength = 5.0f;
	public float Gravity = -9.8f;
	public bool IsUsingPrinter;

	private Vector3 _velocity = new Vector3();
	private Spatial _head;
	private Gun _gun;

	public override void _Ready()
	{
		_head = GetNode<Spatial>("Head");
		_gun = GetNode<Gun>("Head/GunHolder/Gun");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
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
		if (IsUsingPrinter)
			return;

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
			var gun = GetNode<Gun>("Head/GunHolder/Gun");
			gun.PullTrigger();
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

	public void SetInteractText(string text)
	{
		var interactText = GetNode<Label>("CanvasLayer/Panel/InteractText");
		interactText.Text = text;
	}
}
