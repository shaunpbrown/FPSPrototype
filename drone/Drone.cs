using Godot;

public class Drone : KinematicBody, IShootable
{
    private AnimationPlayer _animationPlayer;
    private Spatial _gunBarrel;
    private float _fireCooldown = 2f;
    private Spatial _target;
    private float _speed = 5f;
    private int _health = 3;
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

        _target = GetTree().Root.FindNode("Player", true, false) as Spatial;

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
                if (_target != null)
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
                if (GlobalTranslation.y <= 2)
                {
                    _state = DroneState.Idle;
                    _animationPlayer.Play("FinishFalling");
                }
                break;
            case DroneState.Hit:
                break;
            case DroneState.Firing:
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
                MoveAndSlide(Vector3.Down * 15, Vector3.Up);
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
            default:
                _animationPlayer.Play("Idle");
                break;
        }
    }

    public void FireBullet()
    {
        var bullet = Bullet.GetBullet();
        bullet.GlobalTransform = _gunBarrel.GlobalTransform;
        bullet.Speed = 100;
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
        _animationPlayer.Stop();
        _animationPlayer.Play("Hit");
        _state = DroneState.Hit;
        _health--;
        if (_health <= 0)
        {
            _state = DroneState.Dead;
            BulletHole.RemoveBulletHoles(this);
            CallDeferred("queue_free");
        }
    }
}