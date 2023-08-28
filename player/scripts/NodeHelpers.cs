using Godot;

public static class NodeHelper
{
    public static Node GetChildNode(string nodeName, Node node)
    {
        if (node.Name == nodeName)
        {
            return node;
        }

        foreach (Node child in node.GetChildren())
        {
            Node foundNode = GetChildNode(nodeName, child);
            if (foundNode != null)
            {
                return foundNode;
            }
        }

        return null;
    }

    public static void ReparentNode(Spatial node, Spatial parent)
    {
        var globalTransform = node.GlobalTransform;
        node.GetParent().RemoveChild(node);
        parent.AddChild(node);
        node.GlobalTransform = globalTransform;
    }
}