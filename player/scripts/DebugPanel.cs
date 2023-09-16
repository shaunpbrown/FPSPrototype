using Godot;

public class DebugPanel : Panel
{
    private HSlider _spreadSlider;
    private Label _spreadLabel;
    private HSlider _recoilSlider;
    private Label _recoilLabel;
    private HSlider _fireRateSlider;
    private Label _fireRateLabel;
    private HSlider _projectilesSlider;
    private Label _projectilesLabel;
    private Player _player;

    public override void _Ready()
    {
        _player = GetTree().Root.FindNode("Player", true, false) as Player;
        var gun = _player.GetNode<Gun>("Head/GunHolder/Gun");

        _spreadSlider = GetNode<HSlider>("Spread/HSlider");
        _spreadSlider.Value = gun.GunStats.Spread;
        _spreadLabel = GetNode<Label>("Spread/Label");
        _spreadLabel.Text = $"{(int)_spreadSlider.Value}";
        _spreadSlider.Connect("value_changed", this, nameof(SetPlayerGunStats));

        _recoilSlider = GetNode<HSlider>("Recoil/HSlider");
        _recoilSlider.Value = gun.GunStats.Recoil;
        _recoilLabel = GetNode<Label>("Recoil/Label");
        _recoilLabel.Text = $"{gun.GunStats.GetGunRecoilInDegrees():F2}\u00B0";
        _recoilSlider.Connect("value_changed", this, nameof(SetPlayerGunStats));

        _fireRateSlider = GetNode<HSlider>("FireRate/HSlider");
        _fireRateSlider.Value = gun.GunStats.FireCooldown;
        _fireRateLabel = GetNode<Label>("FireRate/Label");
        _fireRateLabel.Text = $"{gun.GunStats.FireCooldown:F2}s";
        _fireRateSlider.Connect("value_changed", this, nameof(SetPlayerGunStats));

        _projectilesSlider = GetNode<HSlider>("Projectiles/HSlider");
        _projectilesSlider.Value = gun.GunStats.Projectiles;
        _projectilesLabel = GetNode<Label>("Projectiles/Label");
        _projectilesLabel.Text = $"{(int)_projectilesSlider.Value}";
        _projectilesSlider.Connect("value_changed", this, nameof(SetPlayerGunStats));
    }

    public void SetPlayerGunStats(float _)
    {
        var gun = _player.GetNode<Gun>("Head/GunHolder/Gun");
        if (gun != null)
        {
            gun.GunStats = new GunStats
            {
                Spread = (int)_spreadSlider.Value,
                Recoil = (int)_recoilSlider.Value,
                FireCooldown = (int)_fireRateSlider.Value,
                Projectiles = (int)_projectilesSlider.Value
            };


            _spreadLabel.Text = $"{(int)_spreadSlider.Value}";
            _projectilesLabel.Text = $"{(int)_projectilesSlider.Value}";
            _fireRateLabel.Text = $"{gun.GunStats.FireCooldown:F2}s";
            _recoilLabel.Text = $"{gun.GunStats.GetGunRecoilInDegrees():F2}\u00B0";
        }
    }
}