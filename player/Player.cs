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
			_head.RotationDegrees = new Vector3(Mathf.Clamp(_head.RotationDegrees.x, -90, 90), 0, 0);
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
			var pistol = GetNode<Pistol>("Head/GunHolder/Pistol");
			pistol.FireBullet(_head.GlobalTranslation, -_head.GlobalTransform.basis.z);
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
}
