using Godot;

public class Drone : KinematicBody, IShootable
{
    private AnimationPlayer _animationPlayer;
    private Spatial _gunBarrel;
    private float _targetingTimer;
    private float _fireCooldownTimer;
    private float _firingTimer;
    private Spatial _target;
    private float _speed = 6f;
    private int _health = 5;
    private Area _liftArea;
    private enum DroneState
    {
        Idle,
        Falling,
        Hit,
        Firing,
        MovingForward,
        MovingBackward,
        Dead,
    }
    private DroneState _state;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayerEvents");
        _animationPlayer.Connect("animation_finished", this, nameof(AnimationEnded));

        _gunBarrel = GetNode<Spatial>("GunBarrel");
        _liftArea = GetNode<Area>("LiftArea");

        var player = GetTree().Root.FindNode("Player", true, false) as Spatial;
        _target = player.GetNode<Spatial>("Head");

        _state = DroneState.Falling;
        _animationPlayer.Play("Falling");
    }

    public override void _Process(float delta)
    {
        if (_target != null)
            LookAt(_target.GlobalTransform.origin, Vector3.Up);

        switch (_state)
        {
            case DroneState.Idle:
                _targetingTimer += delta;
                if (_targetingTimer >= 2)
                {
                    _targetingTimer = 0;
                    _state = DroneState.Firing;
                }

                if (_target != null && _animationPlayer.CurrentAnimation != "FinishFalling")
                {
                    if (GlobalTranslation.DistanceTo(_target.GlobalTranslation) > 12)
                    {
                        _state = DroneState.MovingForward;
                        _animationPlayer.Play("Forward");
                    }
                    else if (GlobalTranslation.DistanceTo(_target.GlobalTranslation) < 5)
                    {
                        _state = DroneState.MovingBackward;
                        _animationPlayer.Play("Backward");
                    }
                }
                break;
            case DroneState.Falling:
                if (GlobalTranslation.y <= 4.5f)
                {
                    _state = DroneState.Idle;
                    _animationPlayer.Stop();
                    _animationPlayer.Play("Idle");
                }
                break;
            case DroneState.Hit:
                _targetingTimer = 0;
                break;
            case DroneState.Firing:
                _firingTimer += delta;
                if (_firingTimer >= 3)
                {
                    _firingTimer = 0;
                    _state = DroneState.Idle;
                    _animationPlayer.Play("Idle");
                }

                _fireCooldownTimer += delta;
                if (_fireCooldownTimer >= .15f)
                {
                    _fireCooldownTimer = 0;
                    _animationPlayer.Stop();
                    _animationPlayer.Play("Fire");
                    FireBullet();
                }

                break;
            case DroneState.MovingForward:
                if (_target != null)
                {
                    if (GlobalTranslation.DistanceTo(_target.GlobalTranslation) < 6)
                    {
                        _state = DroneState.Idle;
                        _animationPlayer.Play("Idle");
                    }
                }
                else
                {
                    _state = DroneState.Idle;
                    _animationPlayer.Play("Idle");
                }
                break;
            case DroneState.MovingBackward:
                if (_target != null)
                {
                    if (GlobalTranslation.DistanceTo(_target.GlobalTranslation) > 10)
                    {
                        _state = DroneState.Idle;
                        _animationPlayer.Play("Idle");
                    }
                }
                else
                {
                    _state = DroneState.Idle;
                    _animationPlayer.Play("Idle");
                }
                break;
            default:
                break;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        switch (_state)
        {
            case DroneState.Idle:
                break;
            case DroneState.Falling:
                MoveAndSlide(Vector3.Down * 8, Vector3.Up);
                break;
            case DroneState.Hit:
                break;
            case DroneState.Firing:
                break;
            case DroneState.MovingForward:
                if (_target != null)
                {
                    var forward = GlobalTranslation.DirectionTo(new Vector3(_target.GlobalTranslation.x, GlobalTranslation.y, _target.GlobalTranslation.z));
                    MoveAndSlide(forward * _speed, Vector3.Up);

                    if (_liftArea.GetOverlappingBodies().Count > 1 && GlobalTranslation.y < 3)
                    {
                        MoveAndSlide(Vector3.Up * _speed * .5f, Vector3.Up);
                    }
                    else if (_liftArea.GetOverlappingBodies().Count == 1 && GlobalTranslation.y > 2)
                    {
                        MoveAndSlide(Vector3.Down * _speed * .2f, Vector3.Up);
                    }
                }
                break;
            case DroneState.MovingBackward:
                if (_target != null)
                {
                    var back = new Vector3(_target.GlobalTranslation.x, GlobalTranslation.y, _target.GlobalTranslation.z).DirectionTo(GlobalTranslation);
                    MoveAndSlide(back * _speed, Vector3.Up);

                    if (_liftArea.GetOverlappingBodies().Count > 1 && GlobalTranslation.y < 3)
                    {
                        MoveAndSlide(Vector3.Up * _speed * .5f, Vector3.Up);
                    }
                    else if (_liftArea.GetOverlappingBodies().Count == 1 && GlobalTranslation.y > 2)
                    {
                        MoveAndSlide(Vector3.Down * _speed * .2f, Vector3.Up);
                    }
                }
                break;
            default:
                break;
        }
    }

    public void AnimationEnded(string animationName)
    {
        switch (_state)
        {
            case DroneState.Falling:
                _animationPlayer.Play("Falling");
                break;
            case DroneState.MovingForward:
                _animationPlayer.Play("Forward");
                break;
            case DroneState.MovingBackward:
                _animationPlayer.Play("Backward");
                break;
            case DroneState.Hit:
                _state = DroneState.Idle;
                _animationPlayer.Play("Idle");
                break;
            case DroneState.Firing:
                _animationPlayer.Play("Fire");
                break;
            default:
                _animationPlayer.Play("Idle");
                break;
        }
    }

    public void FireBullet()
    {
        var randomDegrees = Mathf.Deg2Rad(GD.Randf() * 6);
        var bullet = DroneBullet.GetBullet();
        bullet.GlobalTransform = _gunBarrel.GlobalTransform;
        bullet.RotateY((GD.Randf() * 2 - 1) * randomDegrees);
        bullet.RotateX((GD.Randf() * 2 - 1) * randomDegrees);
        bullet.Show();
        _animationPlayer.Stop();
        _animationPlayer.Play("Fire");
    }

    public bool CanFireBullet()
    {
        return _animationPlayer.CurrentAnimation != "Hit" && GlobalTranslation.y <= 3;
    }

    public void Shot(Vector3 hitPoint)
    {
        if (_health <= 0)
            return;

        _animationPlayer.Stop();
        _animationPlayer.Play("Hit");
        _state = DroneState.Hit;
        _health--;
        if (_health <= 0)
        {
            Destroy();
        }

        var player = GetTree().Root.FindNode("Player", true, false) as Player;
        player.HitMarker();
    }

    public void Destroy()
    {
        var droneSpawner = GetTree().Root.FindNode("DroneSpawner", true, false) as DroneSpawner;
        droneSpawner.DroneDestroyed(this);
        _state = DroneState.Dead;
        BulletHole.RemoveBulletHoles(this);
        CallDeferred("queue_free");
    }
}