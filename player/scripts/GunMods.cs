using System.Collections.Generic;
using Godot;
using System.Linq;

public class GunMods
{
    public List<string> ModNames = new List<string>{
        "Rocket Launcher",
//        "RocketLauncher2",
        //"RocketLauncher3",
        "Shotgun",
        "Rate of fire",
        "Red Dot",
        "CoolingBlock",
        "BarrelExtension",
        "MagExtension",
        "Stock",
        "ElectricRounds",
        "Flashlight",
        "GripPads",
        "RoundLoader",
    };

    public bool[] _isModEquiped;

    private Gun _gun;

    public GunMods(Gun gun)
    {
        _gun = gun;
        HideAllGunMods();
        _isModEquiped = new bool[ModNames.Count];
    }

    public void HideAllGunMods()
    {
        foreach (string modName in ModNames)
        {
            Node modNode = NodeHelper.GetChildNode(modName, _gun);
            if (modNode != null && modNode is Spatial modSpatial)
            {
                var holoMod = modNode.Duplicate() as Spatial;
                modNode.GetParent().AddChild(holoMod);
                holoMod.Visible = false;
                holoMod.Name = modName + "HOLO";
                ConvertToGreenHologram(holoMod);
                modSpatial.Visible = false;
            }
        }
    }

    public void EquipMod(string name)
    {
        int index = ModNames.IndexOf(name);
        if (index == -1)
            return;

        _isModEquiped[index] = true;

        switch (name)
        {
            case "Rocket Launcher":
                break;
            case "RocketLauncher2":
                break;
            case "RocketLauncher3":
                break;
            case "Shotgun":
                _gun.GunStats.Projectiles += 5;
                _gun.GunStats.Spread += 60;
                _gun.GunStats.Recoil += 5;
                break;
            case "Rate of fire":
                _gun.GunStats.FireCooldown -= .2f;
                _gun.GunStats.Recoil += 1;
                _gun.GunStats.Spread += 30;
                break;
            case "Red Dot":
                _gun.GunStats.Spread -= 10;
                break;
            case "CoolingBlock":
                _gun.GunStats.FireCooldown -= .2f;
                break;
            case "BarrelExtension":
                _gun.GunStats.Recoil -= 5;
                break;
            case "MagExtension":
                _gun.GunStats.Projectiles += 2;
                _gun.GunStats.Spread += 10;
                _gun.GunStats.Recoil += 3;
                break;
            case "Stock":
                _gun.GunStats.Recoil -= 5;
                _gun.GunStats.Spread -= 5;
                break;
            case "ElectricRounds":
                break;
            case "Flashlight":
                _gun.GunStats.Spread -= 5;
                break;
            case "GripPads":
                _gun.GunStats.FireCooldown -= .1f;
                _gun.GunStats.Recoil -= 5;
                break;
            case "RoundLoader":
                _gun.GunStats.FireCooldown -= .1f;
                break;
            default:
                break;
        }
    }

    public bool IsModEquipped(string name)
    {
        int index = ModNames.IndexOf(name);
        if (index == -1)
            return false;

        return _isModEquiped[index];
    }

    public static void ConvertToGreenHologram(Node node)
    {
        ShaderMaterial greenHologramMaterial = new ShaderMaterial();
        Shader shader = new Shader();
        shader.Code = _greenHologramShaderCode;
        greenHologramMaterial.Shader = shader;

        if (node is MeshInstance meshInstance)
        {
            meshInstance.SetSurfaceMaterial(0, greenHologramMaterial);
        }

        foreach (Node child in node.GetChildren())
        {
            ConvertToGreenHologram(child);
        }
    }

    public string GetRandomModName(List<string> filter = null)
    {
        if (filter == null)
            filter = new List<string>();

        var filtered = ModNames.Where((name, index) => !IsModEquipped(name) && !filter.Contains(name)).ToList();
        var val = filtered[(int)(GD.Randf() * filtered.Count)];
        return val;
    }

    private static readonly string _greenHologramShaderCode = @"
        shader_type spatial;

        void vertex() {
            UV.x += TIME * 0.1;
        }

        void fragment() {
            vec3 color = vec3(0.0, 0.8, 0.0);
            float wave = 0.45 * sin(UV.x * 10.0 + TIME) + 0.55;
            ALBEDO = color + vec3(0.0, 0.0, 1.0) * wave;
            EMISSION = color * wave * 0.5;
        }
    ";
}