using Godot;
using System;

public class BulletSplash : CPUParticles
{
    private float _emitionTimer;

    public override void _Process(float delta)
    {
        if (Emitting)
        {
            _emitionTimer += delta;
            if (_emitionTimer > Lifetime + 1)
            {
                QueueFree();
            }
        }
    }
}
