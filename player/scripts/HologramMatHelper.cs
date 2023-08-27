using Godot;

public static class HologramMaterialHelper
{
    public static void ConvertToGreenHologram(Node node)
    {
        ShaderMaterial greenHologramMaterial = CreateGreenHologramMaterial();

        ApplyMaterialToNode(node, greenHologramMaterial);

        foreach (Node child in node.GetChildren())
        {
            ConvertToGreenHologram(child);
        }
    }

    private static ShaderMaterial CreateGreenHologramMaterial()
    {
        ShaderMaterial material = new ShaderMaterial();
        Shader shader = new Shader();

        shader.Code = @"
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

        material.Shader = shader;
        return material;
    }

    private static void ApplyMaterialToNode(Node node, ShaderMaterial material)
    {
        if (node is MeshInstance meshInstance)
        {
            meshInstance.SetSurfaceMaterial(0, material);
        }
    }
}