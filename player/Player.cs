using Godot;

public class Player : KinematicBody
{
	public float MouseSensitivity = 0.005f;
	public float MovementSpeed = 5.0f;
	public float JumpStrength = 5.0f;
	public float Gravity = -9.8f;

	private Vector3 _velocity = new Vector3();
	private Spatial _head;

	public override void _Ready()
	{
		_head = GetNode<Spatial>("Head");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			RotateY(-eventMouseMotion.Relative.x * MouseSensitivity);
			_head.RotateX(-eventMouseMotion.Relative.y * MouseSensitivity);
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (Input.IsActionPressed("app_exit"))
			GetTree().Quit();

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

		if (Input.IsActionJustPressed("shoot"))
		{
			var node = PlayerRayCast();
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

	public Node PlayerRayCast()
	{
		Vector3 rayOrigin = _head.GlobalTranslation;
		Vector3 rayEnd = -_head.GlobalTransform.basis.z * 1000 + rayOrigin;

		PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
		Godot.Collections.Dictionary hit = spaceState.IntersectRay(rayOrigin, rayEnd);

		if (hit.Count > 0)
		{
			Node node = (Node)hit["collider"];
			GD.Print(node.Name);
			if (node is IShootable shootable)
			{
				shootable.Shot((Vector3)hit["position"]);
			}
			return node;
		}

		return null;
	}
}
