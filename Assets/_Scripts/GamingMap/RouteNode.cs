using System.Collections.Generic;
using UnityEngine;

// A router/endpoint in the "Build the Route" mini-game.
// Put one on each node GameObject (needs a Collider). Set the type in the inspector.
public class RouteNode : MonoBehaviour
{
    public enum NodeType { Router, Source, Destination }
    public NodeType type = NodeType.Router;

    // neighbours this node is wired to (kept in sync on both ends)
    [HideInInspector] public List<RouteNode> connections = new List<RouteNode>();

    public bool IsConnectedTo(RouteNode other) => connections.Contains(other);

    public void Connect(RouteNode other)
    {
        if (other == null || other == this) return;
        if (!connections.Contains(other)) connections.Add(other);
        if (!other.connections.Contains(this)) other.connections.Add(this);
    }

    public void Disconnect(RouteNode other)
    {
        connections.Remove(other);
        if (other != null) other.connections.Remove(this);
    }
}
