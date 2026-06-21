using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// "Build the Route": wire the Start node to the Destination by clicking nodes.
// Cables have a max length, so long hauls must route through routers. Every
// completed route scores +10 and randomizes a new Start + Destination. The
// session starts at 20 points and each connection you make costs 1.
// Place ONE of these in the Build-the-Route scene.
public class RouteManager : MonoBehaviour
{
    [Header("Scoring")]
    public int startingPoints = 20;
    public int connectionCost = 1;     // points lost per cable you create
    public int completionReward = 10;  // points gained per completed route

    [Header("Rules")]
    public float maxCableLength = 8f;  // cables longer than this are rejected
    public float nextRoundDelay = 0.8f;

    [Header("Node colors")]
    public Color startColor = new Color(0.2f, 0.6f, 1f);  // blue
    public Color destinationColor = new Color(1f, 0.3f, 0.3f); // red

    [Header("Link visuals")]
    public Material linkMaterial;
    public Color linkColor = Color.cyan;
    public Color completeColor = Color.green;
    public float linkWidth = 0.15f;

    [Header("Highlight")]
    public float selectedScale = 1.2f;

    [Header("Aiming")]
    public bool aimFromScreenCenter = true; // first-person: aim with the crosshair
    public float reach = 100f;

    [Header("HUD")]
    [SerializeField] GameObject hudPanel; // your shared HUD panel; keep it INACTIVE in the scene

    // shown in the shared HUD's how-to box
    const string HowTo = "Connect the <color=#3399FF>BLUE start</color> to the <color=#FF4D4D>RED destination</color> using the fewest connections.";

    class Link { public RouteNode a, b; public LineRenderer line; }

    readonly List<Link> links = new List<Link>();
    readonly Dictionary<RouteNode, Color> originalColors = new Dictionary<RouteNode, Color>();
    List<RouteNode> nodes = new List<RouteNode>();

    RouteNode source, destination;
    RouteNode selected;
    Vector3 selectedBaseScale;
    bool busy;          // blocks input during the round transition
    bool started;       // true once the player actually starts playing
    bool warnedNoCamera;

    void Start()
    {
        nodes = new List<RouteNode>(FindObjectsOfType<RouteNode>());
        foreach (RouteNode n in nodes)
        {
            Renderer r = n.GetComponentInChildren<Renderer>();
            if (r != null) originalColors[n] = r.material.color;
        }

        if (nodes.Count < 2)
        {
            Debug.LogWarning("[RouteManager] Need at least 2 RouteNodes in the scene.");
            enabled = false;
            return;
        }

        // set up the board so it looks ready, but don't score or show the HUD yet
        StartNewRound();
    }

    // Called the first time the player interacts (or call it yourself from a
    // trigger/button). Turns on the HUD and grants the starting points - once.
    public void BeginGame()
    {
        if (started) return;
        started = true;

        if (hudPanel != null) hudPanel.SetActive(true);
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(startingPoints);
        if (MiniGameHUD.Instance != null) MiniGameHUD.Instance.SetHowTo(HowTo);
    }

    void Update()
    {
        if (busy) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Camera cam = Camera.main;
        if (cam == null)
        {
            if (!warnedNoCamera)
            {
                Debug.LogWarning("[RouteManager] No camera tagged 'MainCamera' - can't aim.");
                warnedNoCamera = true;
            }
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Vector3 aim = aimFromScreenCenter
            ? new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f)
            : Input.mousePosition;

        if (Physics.Raycast(cam.ScreenPointToRay(aim), out RaycastHit hit, reach))
        {
            RouteNode node = hit.collider.GetComponentInParent<RouteNode>();
            if (node != null) HandleNodeClick(node);
            else ClearSelection();
        }
        else ClearSelection();
    }

    void HandleNodeClick(RouteNode node)
    {
        if (!started) BeginGame(); // first node click starts the game

        if (selected == null) { Select(node); return; }
        if (node == selected) { ClearSelection(); return; }

        // remove an existing cable (free), or add a new one if it's short enough
        if (selected.IsConnectedTo(node))
        {
            selected.Disconnect(node);
            RemoveLink(selected, node);
        }
        else
        {
            float dist = Vector3.Distance(selected.transform.position, node.transform.position);
            if (dist > maxCableLength)
            {
                if (MiniGameHUD.Instance != null)
                    MiniGameHUD.Instance.ShowResponse("<color=#FFD24D>Too long - route through a closer node</color>");
                ClearSelection();
                return;
            }

            selected.Connect(node);
            AddLink(selected, node);
            if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(-connectionCost);
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
        if (source == null || destination == null) return;
        if (ShortestHops(source, destination) < 0) return; // not connected yet

        busy = true;
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(completionReward);
        foreach (Link l in links) l.line.startColor = l.line.endColor = completeColor;
        if (MiniGameHUD.Instance != null)
            MiniGameHUD.Instance.ShowResponse($"<color=#5CFF5C>Route complete!  +{completionReward}</color>", nextRoundDelay + 1.5f);
        StartCoroutine(NextRoundAfter(nextRoundDelay));
    }

    IEnumerator NextRoundAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartNewRound();
        busy = false;
    }

    void StartNewRound()
    {
        // clear cables + connections
        foreach (Link l in links) if (l.line != null) Destroy(l.line.gameObject);
        links.Clear();
        foreach (RouteNode n in nodes) n.connections.Clear();
        ClearSelection();

        // reset every node to a plain router look
        foreach (RouteNode n in nodes) { n.type = RouteNode.NodeType.Router; RestoreColor(n); }

        // pick a fresh start + destination
        source = nodes[Random.Range(0, nodes.Count)];
        do { destination = nodes[Random.Range(0, nodes.Count)]; }
        while (destination == source);

        source.type = RouteNode.NodeType.Source;
        destination.type = RouteNode.NodeType.Destination;
        SetColor(source, startColor);
        SetColor(destination, destinationColor);
    }

    // breadth-first shortest hop count, or -1 if unconnected
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
                if (n != null && !dist.ContainsKey(n)) { dist[n] = dist[cur] + 1; queue.Enqueue(n); }
        }
        return -1;
    }

    void SetColor(RouteNode node, Color color)
    {
        Renderer r = node.GetComponentInChildren<Renderer>();
        if (r != null) r.material.color = color;
    }

    void RestoreColor(RouteNode node)
    {
        Renderer r = node.GetComponentInChildren<Renderer>();
        if (r != null && originalColors.TryGetValue(node, out Color c)) r.material.color = c;
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
