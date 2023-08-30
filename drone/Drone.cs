using Godot;
using System;

public class Drone : KinematicBody, IShootable
{
    private AnimationPlayer _animationPlayer;
    private Spatial _gunBarrel;
    private float _fireCooldown = 2f;
    private Spatial _target;
    private bool _falling = true;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayerEvents");
        _animationPlayer.Connect("animation_finished", this, nameof(AnimationEnded));

        _gunBarrel = GetNode<Spatial>("GunBarrel");

        _target = GetTree().Root.FindNode("Player", true, false) as Spatial;
    }

    public override void _Process(float delta)
    {
        if (_target != null)
            LookAt(_target.GlobalTransform.origin, Vector3.Up);

        if (_fireCooldown > 0 && CanFireBullet())
            _fireCooldown -= delta;

        if (_fireCooldown <= 0)
        {
            _fireCooldown = 1.5f;
            FireBullet();
        }

        if (_falling && GlobalTranslation.y <= 2)
        {
            _falling = false;
            _animationPlayer.Play("FinishFalling");
        }
        else if (GlobalTranslation.y > 2)
        {
            _animationPlayer.Play("Falling");
            _falling = true;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_falling)
        {
            MoveAndSlide(Vector3.Down * 15, Vector3.Up);
            _falling = true;
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
    }

    public void AnimationEnded(string animationName)
    {
        if (_falling)
        {
            _animationPlayer.Play("Falling");
        }
        else
        {
            _animationPlayer.Play("Idle");
        }
    }

}
