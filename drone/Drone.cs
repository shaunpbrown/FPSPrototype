using Godot;
using System;

public class Drone : KinematicBody, IShootable
{
    private AnimationPlayer _animationPlayer;
    private Spatial _gunBarrel;
    private float _fireCooldown;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayerEvents");
        _animationPlayer.Connect("animation_finished", this, nameof(AnimationEnded));

        _gunBarrel = GetNode<Spatial>("GunBarrel");
    }

    public override void _Process(float delta)
    {
        if (_fireCooldown > 0 && _animationPlayer.CurrentAnimation != "Hit")
            _fireCooldown -= delta;

        if (_fireCooldown <= 0)
        {
            _fireCooldown = 1.5f;
            FireBullet();
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

    public void Shot(Vector3 hitPoint)
    {
        _animationPlayer.Stop();
        _animationPlayer.Play("Hit");
    }

    public void AnimationEnded(string animationName)
    {
        if (animationName == "Hit" || animationName == "Fire")
        {
            _animationPlayer.Play("Idle");
        }
    }

}
