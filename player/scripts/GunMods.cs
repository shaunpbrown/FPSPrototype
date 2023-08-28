using System.Collections.Generic;
using Godot;

public class GunMods
{
    public List<string> ModNames = new List<string>{
        "Rocket Launcher",
        "Red Dot",
    };

    private Gun _gun;

    public GunMods(Gun gun)
    {
        _gun = gun;
        HideAllGunMods();
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