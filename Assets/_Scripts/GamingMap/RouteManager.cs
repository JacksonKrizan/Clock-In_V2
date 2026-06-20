using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// "Build the Route": click two RouteNodes to wire them together; connect the
// Source to the Destination and the shortest path scores the most. Place ONE of
// these in the Build-the-Route scene (it uses left-click, so don't mix it with
// the packet-carry scene where IPdraggable also uses left-click).
public class RouteManager : MonoBehaviour
{
    [Header("Scoring")]
    public int baseDelivery = 100;   // points for a 1-hop route
    public int hopPenalty = 10;      // points lost per extra hop
    public int minPoints = 10;

    [Header("Link visuals")]
    public Material linkMaterial;          // optional; falls back to a default line shader
    public Color linkColor = Color.cyan;
    public Color completeColor = Color.green;
    public float linkWidth = 0.15f;

    [Header("Highlight")]
    public float selectedScale = 1.2f;

    [Header("Aiming")]
    public bool aimFromScreenCenter = true; // first-person: aim with the crosshair, not the mouse
    public float reach = 100f;              // how far you can click a node

    class Link { public RouteNode a, b; public LineRenderer line; }

    readonly List<Link> links = new List<Link>();
    RouteNode selected;
    Vector3 selectedBaseScale;
    bool complete;
    bool warnedNoCamera;

    void Update()
    {
        if (complete) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Camera cam = Camera.main;
        if (cam == null)
        {
            if (!warnedNoCamera)
            {
                Debug.LogWarning("[RouteManager] No camera tagged 'MainCamera' - can't aim. Tag your player camera MainCamera.");
                warnedNoCamera = true;
            }
            return;
        }

        // ignore clicks that land on UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Vector3 aim = aimFromScreenCenter
            ? new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f)
            : Input.mousePosition;

        Ray ray = cam.ScreenPointToRay(aim);
        if (Physics.Raycast(ray, out RaycastHit hit, reach))
        {
            RouteNode node = hit.collider.GetComponentInParent<RouteNode>();
            if (node != null) HandleNodeClick(node);
            else ClearSelection();
        }
        else ClearSelection();
    }

    void HandleNodeClick(RouteNode node)
    {
        if (selected == null) { Select(node); return; }
        if (node == selected) { ClearSelection(); return; }

        // clicking a second node toggles the link between them
        if (selected.IsConnectedTo(node))
        {
            selected.Disconnect(node);
            RemoveLink(selected, node);
        }
        else
        {
            selected.Connect(node);
            AddLink(selected, node);
        }

        ClearSelection();
        CheckRoute();
    }

    void Select(RouteNode node)
    {
        selected = node;
        selectedBaseScale = node.transform.localScale;
        node.transform.localScale = selectedBaseScale * selectedScale;
    }

    void ClearSelection()
    {
        if (selected != null) selected.transform.localScale = selectedBaseScale;
        selected = null;
    }

    void CheckRoute()
    {
        RouteNode src = FindNode(RouteNode.NodeType.Source);
        RouteNode dst = FindNode(RouteNode.NodeType.Destination);
        if (src == null || dst == null) return;

        int hops = ShortestHops(src, dst);
        if (hops < 0) return; // not connected yet

        complete = true;
        int points = Mathf.Max(minPoints, baseDelivery - (hops - 1) * hopPenalty);
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(points);

        foreach (Link l in links) { l.line.startColor = l.line.endColor = completeColor; }
        Debug.Log($"Route complete in {hops} hop(s)! +{points}");
    }

    // breadth-first shortest hop count between two nodes, or -1 if unconnected
    int ShortestHops(RouteNode start, RouteNode goal)
    {
        var dist = new Dictionary<RouteNode, int> { { start, 0 } };
        var queue = new Queue<RouteNode>();
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            RouteNode cur = queue.Dequeue();
            if (cur == goal) return dist[cur];
            foreach (RouteNode n in cur.connections)
            {
                if (n != null && !dist.ContainsKey(n))
                {
                    dist[n] = dist[cur] + 1;
                    queue.Enqueue(n);
                }
            }
        }
        return -1;
    }

    RouteNode FindNode(RouteNode.NodeType type)
    {
        foreach (RouteNode n in FindObjectsOfType<RouteNode>())
            if (n.type == type) return n;
        return null;
    }

    void AddLink(RouteNode a, RouteNode b)
    {
        var go = new GameObject($"Link_{a.name}_{b.name}");
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.material = linkMaterial != null ? linkMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = linkColor;
        lr.startWidth = lr.endWidth = linkWidth;
        lr.positionCount = 2;
        lr.SetPosition(0, a.transform.position);
        lr.SetPosition(1, b.transform.position);
        links.Add(new Link { a = a, b = b, line = lr });
    }

    void RemoveLink(RouteNode a, RouteNode b)
    {
        for (int i = links.Count - 1; i >= 0; i--)
        {
            Link l = links[i];
            if ((l.a == a && l.b == b) || (l.a == b && l.b == a))
            {
                if (l.line != null) Destroy(l.line.gameObject);
                links.RemoveAt(i);
            }
        }
    }
}
