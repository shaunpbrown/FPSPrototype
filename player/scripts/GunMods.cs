using System.Collections.Generic;
using Godot;

public class GunMods
{
    public List<string> ModNames = new List<string>{
        "Rocket Launcher",
        "Accuracy",
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
            Node modNode = GetNode(modName, _gun);
            if (modNode != null && modNode is Spatial modSpatial)
            {
                var holoMod = modNode.Duplicate() as Spatial;
                modNode.GetParent().AddChild(holoMod);
                holoMod.Visible = false;
                holoMod.Name = modName + "HOLO";
                HologramMaterialHelper.ConvertToGreenHologram(holoMod);
                modSpatial.Visible = false;
            }
        }
    }

    public void ShowGunMod(string modName)
    {
        Node modNode = GetNode(modName, _gun);
        if (modNode != null && modNode is Spatial modSpatial)
        {
            modSpatial.Visible = true;
            HologramMaterialHelper.ConvertToGreenHologram(modSpatial);
        }
    }

    public void HideGunMod(string modName)
    {
        Node modNode = GetNode(modName, _gun);
        if (modNode != null && modNode is Spatial modSpatial)
        {
            modSpatial.Visible = false;
        }
    }

    public Node GetNode(string nodeName, Node node)
    {
        if (node.Name == nodeName)
        {
            return node;
        }

        foreach (Node child in node.GetChildren())
        {
            Node foundNode = GetNode(nodeName, child);
            if (foundNode != null)
            {
                return foundNode;
            }
        }

        return null;
    }
}