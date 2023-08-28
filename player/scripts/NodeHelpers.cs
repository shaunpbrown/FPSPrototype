using System.Collections.Generic;
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

    public static T GetChild<T>(Node node) where T : Node
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is T)
            {
                return child as T;
            }
        }
        return null;
    }

    public static List<T> GetChildren<T>(Node node, List<T> list = default) where T : Node
    {
        if (list == default)
            list = new List<T>();

        foreach (Node child in node.GetChildren())
        {
            GetChildren(child, list);

            if (child is T)
            {
                list.Add(child as T);
            }
        }

        return list;
    }

    public static void ReparentNode(Spatial node, Spatial parent)
    {
        var globalTransform = node.GlobalTransform;
        node.GetParent().RemoveChild(node);
        parent.AddChild(node);
        node.GlobalTransform = globalTransform;
    }
}