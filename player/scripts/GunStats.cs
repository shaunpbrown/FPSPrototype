using Godot;

public class GunStats
{
    public int Spread;
    public int FireRate = 80;
    public int Projectiles = 1;
    public int Recoil;

    public float GetGunFireRateInSeconds()
    {
        return (100 - FireRate) / 50f;
    }

    public float GetGunRecoilInDegrees()
    {
        return Recoil / 10f;
    }
}