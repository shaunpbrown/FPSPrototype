using Godot;
using System;

public class Drone : KinematicBody, IShootable
{
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayerEvents");
        _animationPlayer.Connect("animation_finished", this, nameof(AnimationEnded));
    }


    public void Shot(Vector3 hitPoint)
    {
        _animationPlayer.Stop();
        _animationPlayer.Play("Hit");
    }

    public void AnimationEnded(string animationName)
    {
        if (animationName == "Hit")
        {
            _animationPlayer.Play("Idle");
        }
    }

}
