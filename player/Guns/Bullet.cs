using Godot;

public class Bullet : Spatial
{
    public Vector3 Target;
    public float Speed = 300f;
    public float MaxDistance = 500f;

    private float _currentDistance;

    public override void _Process(float delta)
    {
        if (Visible)
        {
            Translate(Vector3.Forward * Speed * delta);
            _currentDistance += Speed * delta;
            if (_currentDistance >= MaxDistance)
                QueueFree();
        }
    }
}
