using Godot;

public class Rocket : Spatial
{
    private CPUParticles _smokeParticles;

    public Drone Target;

    private enum State
    {
        Idle,
        Up,
        Down,
        Exploding
    }
    private State _currentState;
    private float _timer;

    public override void _Ready()
    {
        _smokeParticles = GetNode<CPUParticles>("Rocket Smoke");
        _smokeParticles.Emitting = false;
    }

    public override void _Process(float delta)
    {
        if (_currentState == State.Up)
        {
            LookAt(GlobalTranslation + Vector3.Up, Vector3.Right);
            GlobalTranslation += Vector3.Up * delta * 8;
            _timer += delta;
            if (_timer > .5f)
            {
                _currentState = State.Down;
                _timer = 0;
            }
        }
        else if (_currentState == State.Down)
        {
            try
            {
                if (Target == null)
                {
                    if (DroneSpawner.Drones.Count > 0)
                        Target = DroneSpawner.Drones[0];
                    else
                    {
                        QueueFree();
                        return;
                    }
                }

                LookAt(Target.GlobalTranslation, Vector3.Up);
                Translate(Vector3.Forward * delta * 55);
                if (GlobalTranslation.DistanceTo(Target.GlobalTranslation) < 0.5f)
                {
                    _currentState = State.Exploding;
                    _timer = 0;
                    var particles1 = GetNode<CPUParticles>("Explosion");
                    particles1.Emitting = true;
                    var particles2 = GetNode<CPUParticles>("Explosion/CPUParticles");
                    particles2.Emitting = true;
                    Target.Destroy();
                    var explosionAudio = GetNode<AudioStreamPlayer3D>("ExplosionStream");
                    explosionAudio.Play();
                }
            }
            catch
            {
                QueueFree();
            }
        }
        else if (_currentState == State.Exploding)
        {
            _timer += delta;
            if (_timer > 3)
            {
                QueueFree();
            }
        }
    }

    public void Fire()
    {
        var main = GetTree().Root.FindNode("Main", true, false) as Spatial;
        NodeHelper.ReparentNode(this, main);
        _smokeParticles.Emitting = true;
        _currentState = State.Up;
        var audioStreamPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
        audioStreamPlayer.Play();
    }
}