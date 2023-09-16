using Godot;

public class GunStats
{
    public int Spread;
    public float FireCooldown = .4f;
    public int Projectiles = 1;
    public int Recoil;

    public float GetGunRecoilInDegrees()
    {
        return Recoil / 10f;
    }
}