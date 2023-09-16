using System.Collections.Generic;
using Godot;

public class GunMods
{
    public List<string> ModNames = new List<string>{
        "Rocket Launcher",
        "Red Dot",
        "Shotgun",
        "Rate of fire",
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
            case "Rate of fire":
                _gun.GunStats.FireCooldown += 10;
                break;
            case "Shotgun":
                _gun.GunStats.Projectiles += 5;
                _gun.GunStats.Spread += 60;
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