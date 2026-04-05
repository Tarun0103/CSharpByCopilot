// ============================================================
// PHASE 16: GRAPHS — Complete C# Implementation
// Topics: Graph basics, Adjacency Matrix/List, BFS, DFS,
//         Dijkstra, Bellman-Ford, Floyd-Warshall (concept),
//         Prim's, Kruskal's, Cycle Detection, Topological Sort
// Run this file in a Console Application project
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;

// ============================================================
// SECTION 1: GRAPH BASICS
//
// A Graph G = (V, E) consists of:
//   V = set of VERTICES (nodes)
//   E = set of EDGES (connections between nodes)
//
// Types of Graphs:
//   - UNDIRECTED: edges have no direction (A-B = B-A)
//   - DIRECTED (Digraph): edges have direction (A→B ≠ B→A)
//   - WEIGHTED: edges have a cost/weight
//   - UNWEIGHTED: all edges have equal cost
//   - CYCLIC: contains at least one cycle
//   - ACYCLIC (DAG): Directed Acyclic Graph — no cycles
//
// Real-world graphs:
//   - Social networks (people = nodes, friendships = edges)
//   - Road maps (cities = nodes, roads = edges with distance)
//   - Internet (servers = nodes, connections = edges)
//   - Dependencies (tasks = nodes, "must complete before" = directed edges)
// ============================================================

// ============================================================
// SECTION 2: GRAPH REPRESENTATIONS
// ============================================================

// --- ADJACENCY MATRIX ---
// 2D array where matrix[i][j] = 1 (or weight) if edge exists between i and j
// 
// Graph:  0-1, 0-2, 1-2, 1-3
//         0  1  2  3
//    0  [ 0  1  1  0 ]
//    1  [ 1  0  1  1 ]
//    2  [ 1  1  0  0 ]
//    3  [ 0  1  0  0 ]
//
// Space: O(V²) — wastes space for sparse graphs
// Edge lookup: O(1)
// Finding all neighbors: O(V)

class AdjacencyMatrixGraph
{
    private int[,] _matrix;   // [from, to] = weight (0 = no edge)
    private int _vertices;
    public bool IsDirected { get; }

    public AdjacencyMatrixGraph(int vertices, bool directed = false)
    {
        _vertices = vertices;
        IsDirected = directed;
        _matrix = new int[vertices, vertices];
    }

    // Add unweighted edge (weight = 1)
    public void AddEdge(int from, int to, int weight = 1)
    {
        _matrix[from, to] = weight;
        if (!IsDirected)
            _matrix[to, from] = weight;  // undirected: add both directions
    }

    public bool HasEdge(int from, int to) => _matrix[from, to] != 0;
    public int GetWeight(int from, int to) => _matrix[from, to];

    public List<int> GetNeighbors(int vertex)
    {
        var neighbors = new List<int>();
        for (int i = 0; i < _vertices; i++)
            if (_matrix[vertex, i] != 0)
                neighbors.Add(i);
        return neighbors;
    }

    public void Print()
    {
        Console.Write("  Adjacency Matrix:     ");
        for (int i = 0; i < _vertices; i++) Console.Write($"  {i}");
        Console.WriteLine();
        for (int i = 0; i < _vertices; i++)
        {
            Console.Write($"    {i}:  [");
            for (int j = 0; j < _vertices; j++)
                Console.Write($" {_matrix[i, j]}");
            Console.WriteLine(" ]");
        }
    }
}

// --- ADJACENCY LIST ---
// Each vertex stores a list of its neighbors.
// Dictionary<int, List<(int neighbor, int weight)>>
//
// Same graph:  0: [1,2]  1: [0,2,3]  2: [0,1]  3: [1]
//
// Space: O(V + E) — much better for sparse graphs
// Edge lookup: O(degree)
// Finding all neighbors: O(degree)

class WeightedGraph
{
    // Adjacency list: vertex → list of (neighbor, weight)
    private Dictionary<int, List<(int to, int weight)>> _adj;
    private int _vertices;
    public bool IsDirected { get; }

    public WeightedGraph(int vertices, bool directed = false)
    {
        _vertices = vertices;
        IsDirected = directed;
        _adj = new Dictionary<int, List<(int, int)>>();

        // Initialize empty adjacency lists for all vertices
        for (int i = 0; i < vertices; i++)
            _adj[i] = new List<(int, int)>();
    }

    public void AddEdge(int from, int to, int weight = 1)
    {
        _adj[from].Add((to, weight));
        if (!IsDirected)
            _adj[to].Add((from, weight));  // undirected: add reverse edge
    }

    public IEnumerable<(int to, int weight)> GetNeighbors(int vertex) => _adj[vertex];
    public int VertexCount => _vertices;

    public void Print()
    {
        Console.WriteLine("  Adjacency List:");
        foreach (var kv in _adj)
        {
            var neighbors = kv.Value.Select(e => $"{e.to}(w:{e.weight})");
            Console.WriteLine($"    {kv.Key}: [{string.Join(", ", neighbors)}]");
        }
    }
}

// ============================================================
// SECTION 3: GRAPH TRAVERSALS
// ============================================================

class GraphTraversals
{
    // --- BREADTH-FIRST SEARCH (BFS) --- O(V + E)
    // Explores nodes level by level (shortest path in unweighted graph).
    // Uses a QUEUE.
    //
    // Applications:
    //   - Shortest path in unweighted graph
    //   - Level-order traversal
    //   - Finding connected components
    //   - Bipartite graph check
    public static List<int> BFS(WeightedGraph graph, int start)
    {
        List<int> visited_order = new List<int>();
        HashSet<int> visited = new HashSet<int>();
        Queue<int> queue = new Queue<int>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            int node = queue.Dequeue();
            visited_order.Add(node);

            foreach (var (neighbor, _) in graph.GetNeighbors(node))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return visited_order;
    }

    // BFS Shortest Path — returns distances from source to all vertices
    public static Dictionary<int, int> BFSShortestPath(WeightedGraph graph, int source, int vertexCount)
    {
        var distance = new Dictionary<int, int>();
        for (int i = 0; i < vertexCount; i++) distance[i] = int.MaxValue;

        distance[source] = 0;
        Queue<int> queue = new Queue<int>();
        queue.Enqueue(source);

        while (queue.Count > 0)
        {
            int node = queue.Dequeue();

            foreach (var (neighbor, _) in graph.GetNeighbors(node))
            {
                if (distance[neighbor] == int.MaxValue)
                {
                    distance[neighbor] = distance[node] + 1;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return distance;
    }

    // --- DEPTH-FIRST SEARCH (DFS) --- O(V + E)
    // Explores as deep as possible before backtracking.
    // Uses recursion (implicit stack) or explicit Stack.
    //
    // Applications:
    //   - Cycle detection
    //   - Topological sort
    //   - Path finding
    //   - Connected components
    //   - Maze solving
    public static List<int> DFS(WeightedGraph graph, int start)
    {
        List<int> visited_order = new List<int>();
        HashSet<int> visited = new HashSet<int>();
        DFSHelper(graph, start, visited, visited_order);
        return visited_order;
    }

    private static void DFSHelper(WeightedGraph graph, int node, HashSet<int> visited, List<int> order)
    {
        visited.Add(node);
        order.Add(node);

        foreach (var (neighbor, _) in graph.GetNeighbors(node))
        {
            if (!visited.Contains(neighbor))
                DFSHelper(graph, neighbor, visited, order);
        }
    }

    // DFS Iterative (using explicit stack — avoids recursion stack overflow for large graphs)
    public static List<int> DFSIterative(WeightedGraph graph, int start)
    {
        List<int> order = new List<int>();
        HashSet<int> visited = new HashSet<int>();
        Stack<int> stack = new Stack<int>();

        stack.Push(start);

        while (stack.Count > 0)
        {
            int node = stack.Pop();

            if (visited.Contains(node)) continue;

            visited.Add(node);
            order.Add(node);

            // Push neighbors (in reverse order to maintain left-to-right visit order)
            foreach (var (neighbor, _) in graph.GetNeighbors(node).Reverse())
            {
                if (!visited.Contains(neighbor))
                    stack.Push(neighbor);
            }
        }

        return order;
    }
}

// ============================================================
// SECTION 4: SHORTEST PATH ALGORITHMS
// ============================================================

class ShortestPathAlgorithms
{
    // --- DIJKSTRA'S ALGORITHM --- O((V + E) log V) with priority queue
    // Finds shortest path from source to ALL other vertices.
    // Works on WEIGHTED graphs with NON-NEGATIVE weights.
    //
    // Algorithm:
    // 1. Set all distances to infinity, source to 0.
    // 2. Use min-heap (PriorityQueue). Start with (0, source).
    // 3. For each extracted min node, relax all its edges.
    // 4. If new distance < known distance → update and enqueue.
    //
    // "Relaxing an edge": check if going through current node gives shorter path
    //   if dist[u] + weight(u,v) < dist[v] → update dist[v]
    public static int[] Dijkstra(WeightedGraph graph, int source)
    {
        int n = graph.VertexCount;
        int[] dist = new int[n];
        Array.Fill(dist, int.MaxValue);
        dist[source] = 0;

        // PriorityQueue<vertex, distance> — min-heap by distance
        var pq = new PriorityQueue<int, int>();
        pq.Enqueue(source, 0);

        while (pq.Count > 0)
        {
            pq.TryDequeue(out int u, out int currentDist);

            // Skip if we've already found a shorter path to u
            if (currentDist > dist[u]) continue;

            foreach (var (v, weight) in graph.GetNeighbors(u))
            {
                int newDist = dist[u] + weight;

                // RELAXATION: update if new path is shorter
                if (newDist < dist[v])
                {
                    dist[v] = newDist;
                    pq.Enqueue(v, newDist);
                }
            }
        }

        return dist;  // dist[i] = shortest distance from source to i
    }

    // --- BELLMAN-FORD ALGORITHM --- O(V * E)
    // Finds shortest path even with NEGATIVE weight edges.
    // Also detects NEGATIVE CYCLES (a cycle whose total weight is negative).
    // Cannot use Dijkstra when negative weights exist.
    //
    // Algorithm:
    // 1. Set all distances to infinity, source to 0.
    // 2. Repeat V-1 times: relax ALL edges.
    //    (shortest path has at most V-1 edges in a graph with V vertices)
    // 3. Run one more pass: if any distance still decreases → negative cycle!
    public static (int[] distances, bool hasNegativeCycle) BellmanFord(
        int vertices, List<(int from, int to, int weight)> edges, int source)
    {
        int[] dist = new int[vertices];
        Array.Fill(dist, int.MaxValue);
        dist[source] = 0;

        // Relax all edges V-1 times
        for (int iteration = 0; iteration < vertices - 1; iteration++)
        {
            bool anyUpdate = false;

            foreach (var (from, to, weight) in edges)
            {
                if (dist[from] != int.MaxValue && dist[from] + weight < dist[to])
                {
                    dist[to] = dist[from] + weight;
                    anyUpdate = true;
                }
            }

            if (!anyUpdate) break;  // early termination if no updates
        }

        // Check for negative cycle (one more pass — if distance still decreases, negative cycle exists)
        bool negativeCycle = false;
        foreach (var (from, to, weight) in edges)
        {
            if (dist[from] != int.MaxValue && dist[from] + weight < dist[to])
            {
                negativeCycle = true;
                break;
            }
        }

        return (dist, negativeCycle);
    }
}

// ============================================================
// SECTION 5: MINIMUM SPANNING TREE (MST)
// An MST connects ALL vertices with minimum total edge weight
// and NO cycles. (n vertices → n-1 edges in MST)
//
// Common algorithms: Prim's, Kruskal's
// ============================================================

class MSTAlgorithms
{
    // --- PRIM'S ALGORITHM --- O((V + E) log V)
    // Grows the MST one VERTEX at a time.
    // Always pick the MINIMUM WEIGHT EDGE that connects a new vertex to the current MST.
    //
    // Think of it as: start from one vertex, greedily expand to nearest neighbors.
    public static int Prims(WeightedGraph graph, int start)
    {
        int n = graph.VertexCount;
        int[] minEdge = new int[n];        // minimum edge weight to connect vertex to MST
        bool[] inMST = new bool[n];        // is vertex already in MST?

        Array.Fill(minEdge, int.MaxValue);
        minEdge[start] = 0;

        // PriorityQueue<vertex, edge_weight>
        var pq = new PriorityQueue<int, int>();
        pq.Enqueue(start, 0);

        int mstCost = 0;
        int mstEdges = 0;

        while (pq.Count > 0 && mstEdges < n)
        {
            pq.TryDequeue(out int u, out int edgeWeight);

            if (inMST[u]) continue;  // already in MST, skip

            inMST[u] = true;
            mstCost += edgeWeight;
            mstEdges++;

            Console.WriteLine($"    Added vertex {u} to MST with edge weight {edgeWeight}");

            // Update neighbors' minimum edge weights
            foreach (var (v, weight) in graph.GetNeighbors(u))
            {
                if (!inMST[v] && weight < minEdge[v])
                {
                    minEdge[v] = weight;
                    pq.Enqueue(v, weight);
                }
            }
        }

        return mstCost;
    }

    // --- KRUSKAL'S ALGORITHM --- O(E log E) for sorting edges + Union-Find
    // Grows MST one EDGE at a time.
    // Sort all edges by weight, then greedily add edges that don't create a cycle.
    // Uses UNION-FIND (Disjoint Set Union) to detect cycles.
    public static int Kruskals(int vertices, List<(int from, int to, int weight)> edges)
    {
        // Sort edges by weight
        var sortedEdges = edges.OrderBy(e => e.weight).ToList();

        // Union-Find data structure
        int[] parent = new int[vertices];
        int[] rank   = new int[vertices];
        for (int i = 0; i < vertices; i++) parent[i] = i;  // each vertex is its own component

        int mstCost = 0;
        int edgeCount = 0;

        Console.WriteLine("  Kruskal's MST construction:");

        foreach (var (from, to, weight) in sortedEdges)
        {
            int rootFrom = Find(parent, from);
            int rootTo   = Find(parent, to);

            // If they have DIFFERENT roots → adding this edge won't create a cycle
            if (rootFrom != rootTo)
            {
                Union(parent, rank, rootFrom, rootTo);
                mstCost += weight;
                edgeCount++;
                Console.WriteLine($"    Added edge {from}-{to} (weight={weight})");

                if (edgeCount == vertices - 1) break;  // MST complete
            }
        }

        return mstCost;
    }

    // Union-Find: Find the root of a component (with path compression)
    private static int Find(int[] parent, int x)
    {
        if (parent[x] != x)
            parent[x] = Find(parent, parent[x]);  // path compression
        return parent[x];
    }

    // Union-Find: Merge two components (by rank)
    private static void Union(int[] parent, int[] rank, int x, int y)
    {
        if (rank[x] < rank[y])       (x, y) = (y, x);  // ensure x has higher rank
        parent[y] = x;                // y's root becomes x
        if (rank[x] == rank[y]) rank[x]++;
    }
}

// ============================================================
// SECTION 6: TOPOLOGICAL SORT (for Directed Acyclic Graphs)
// Order vertices such that every directed edge u→v comes BEFORE v.
// Used for: build systems, task scheduling, course prerequisites
//
// Example: Tasks with dependencies
//   A → C → E
//   A → B → D → E
// Valid topological order: A, B, C, D, E  (or A, C, B, D, E)
// ============================================================

class TopologicalSort
{
    // DFS-based: finish time determines order (later finish = earlier in result)
    public static List<int> Sort(int vertices, Dictionary<int, List<int>> adj)
    {
        Stack<int> stack = new Stack<int>();     // stores result
        HashSet<int> visited = new HashSet<int>();

        // Run DFS from all unvisited vertices
        for (int i = 0; i < vertices; i++)
        {
            if (!visited.Contains(i))
                DFS(i, adj, visited, stack);
        }

        return stack.ToList();  // top of stack = first in topological order
    }

    private static void DFS(int node, Dictionary<int, List<int>> adj, HashSet<int> visited, Stack<int> stack)
    {
        visited.Add(node);

        if (adj.ContainsKey(node))
        {
            foreach (int neighbor in adj[node])
            {
                if (!visited.Contains(neighbor))
                    DFS(neighbor, adj, visited, stack);
            }
        }

        stack.Push(node);  // push AFTER all descendants are visited
    }
}

// ============================================================
// MAIN PROGRAM
// ============================================================
class GraphsProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 16: GRAPHS");
        Console.WriteLine("======================================================");

        // -------------------------------------------------------
        // DEMO 1: Graph Representations
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 1: Graph Representations ===");

        // Build this undirected graph:
        //   0 --- 1 --- 3
        //   |   / |
        //   |  /  |
        //   2-----+   (0-2, 1-2, 1-3, 0-1)

        var matrixGraph = new AdjacencyMatrixGraph(4);
        matrixGraph.AddEdge(0, 1);
        matrixGraph.AddEdge(0, 2);
        matrixGraph.AddEdge(1, 2);
        matrixGraph.AddEdge(1, 3);
        matrixGraph.Print();

        var listGraph = new WeightedGraph(4);
        listGraph.AddEdge(0, 1, 1);
        listGraph.AddEdge(0, 2, 1);
        listGraph.AddEdge(1, 2, 1);
        listGraph.AddEdge(1, 3, 1);
        listGraph.Print();

        // -------------------------------------------------------
        // DEMO 2: BFS and DFS
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 2: BFS and DFS Traversals ===");

        var bfsDfsGraph = new WeightedGraph(6);
        bfsDfsGraph.AddEdge(0, 1);
        bfsDfsGraph.AddEdge(0, 2);
        bfsDfsGraph.AddEdge(1, 3);
        bfsDfsGraph.AddEdge(1, 4);
        bfsDfsGraph.AddEdge(2, 5);

        var bfsResult = GraphTraversals.BFS(bfsDfsGraph, 0);
        Console.WriteLine($"  BFS from 0: {string.Join(" → ", bfsResult)}");
        // 0 → 1 → 2 → 3 → 4 → 5 (level by level)

        var dfsResult = GraphTraversals.DFS(bfsDfsGraph, 0);
        Console.WriteLine($"  DFS from 0: {string.Join(" → ", dfsResult)}");
        // 0 → 1 → 3 → 4 → 2 → 5 (goes deep first)

        // BFS Shortest Distances
        var distances = GraphTraversals.BFSShortestPath(bfsDfsGraph, 0, 6);
        Console.Write("  Distances from 0: ");
        for (int i = 0; i < 6; i++) Console.Write($"{i}:{distances[i]} ");
        Console.WriteLine();

        // -------------------------------------------------------
        // DEMO 3: Dijkstra's Algorithm
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 3: Dijkstra's Shortest Path ===");

        // Weighted graph:
        //   0 --(4)-- 1 --(1)-- 3
        //   |         |
        //  (2)       (2)
        //   |         |
        //   2 --(3)-- 4
        var dijkGraph = new WeightedGraph(5, directed: true);
        dijkGraph.AddEdge(0, 1, 4);
        dijkGraph.AddEdge(0, 2, 2);
        dijkGraph.AddEdge(2, 4, 3);
        dijkGraph.AddEdge(4, 1, 1);
        dijkGraph.AddEdge(1, 3, 1);
        dijkGraph.AddEdge(4, 3, 5);

        int[] dijkDist = ShortestPathAlgorithms.Dijkstra(dijkGraph, 0);
        Console.WriteLine("  Dijkstra from vertex 0:");
        for (int i = 0; i < dijkDist.Length; i++)
            Console.WriteLine($"    → vertex {i}: {(dijkDist[i] == int.MaxValue ? "∞" : dijkDist[i].ToString())}");

        // -------------------------------------------------------
        // DEMO 4: Bellman-Ford
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 4: Bellman-Ford (handles negative weights) ===");

        var bfEdges = new List<(int, int, int)>
        {
            (0, 1, 4), (0, 2, 2), (2, 4, 3), (4, 1, 1),
            (1, 3, 1), (4, 3, 5), (1, 2, -2)  // negative edge!
        };

        var (bfDist, hasNegCycle) = ShortestPathAlgorithms.BellmanFord(5, bfEdges, 0);
        Console.WriteLine($"  Has negative cycle: {hasNegCycle}");
        Console.WriteLine("  Bellman-Ford distances from vertex 0:");
        for (int i = 0; i < bfDist.Length; i++)
            Console.WriteLine($"    → vertex {i}: {(bfDist[i] == int.MaxValue ? "∞" : bfDist[i].ToString())}");

        // -------------------------------------------------------
        // DEMO 5: Minimum Spanning Tree
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 5: Minimum Spanning Tree ===");

        var mstGraph = new WeightedGraph(5);
        mstGraph.AddEdge(0, 1, 2);
        mstGraph.AddEdge(0, 3, 6);
        mstGraph.AddEdge(1, 2, 3);
        mstGraph.AddEdge(1, 3, 8);
        mstGraph.AddEdge(1, 4, 5);
        mstGraph.AddEdge(2, 4, 7);
        mstGraph.AddEdge(3, 4, 9);

        Console.WriteLine("  Prim's Algorithm:");
        int primsCost = MSTAlgorithms.Prims(mstGraph, 0);
        Console.WriteLine($"  Prim's MST total cost: {primsCost}");

        Console.WriteLine();
        var kruskEdges = new List<(int, int, int)>
        {
            (0, 1, 2), (0, 3, 6), (1, 2, 3), (1, 3, 8),
            (1, 4, 5), (2, 4, 7), (3, 4, 9)
        };
        int kruskCost = MSTAlgorithms.Kruskals(5, kruskEdges);
        Console.WriteLine($"  Kruskal's MST total cost: {kruskCost}");

        // -------------------------------------------------------
        // DEMO 6: Topological Sort
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 6: Topological Sort (Course Prerequisites) ===");

        // Courses: 0=Math, 1=Physics, 2=CS, 3=Algorithms, 4=ML
        // Prerequisites: Math→Physics, Math→CS, CS→Algorithms, Algorithms→ML, Physics→ML
        var dag = new Dictionary<int, List<int>>
        {
            [0] = new List<int> { 1, 2 },  // Math → Physics, CS
            [1] = new List<int> { 4 },      // Physics → ML
            [2] = new List<int> { 3 },      // CS → Algorithms
            [3] = new List<int> { 4 },      // Algorithms → ML
            [4] = new List<int>()           // ML (no outgoing)
        };

        string[] courseNames = { "Math", "Physics", "CS", "Algorithms", "ML" };
        var topoOrder = TopologicalSort.Sort(5, dag);
        Console.Write("  Course order: ");
        Console.WriteLine(string.Join(" → ", topoOrder.Select(i => courseNames[i])));

        // -------------------------------------------------------
        // SUMMARY
        // -------------------------------------------------------
        Console.WriteLine("\n======================================================");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  1. Adjacency List O(V+E) space — use for sparse graphs");
        Console.WriteLine("  2. Adjacency Matrix O(V²) space — use for dense graphs");
        Console.WriteLine("  3. BFS: O(V+E), shortest path (unweighted), level-order");
        Console.WriteLine("  4. DFS: O(V+E), cycle detection, topological sort, paths");
        Console.WriteLine("  5. Dijkstra: O((V+E) log V), shortest path with +ve weights");
        Console.WriteLine("  6. Bellman-Ford: O(V*E), handles -ve weights, detects -ve cycles");
        Console.WriteLine("  7. Prim's/Kruskal's: O((V+E) log V), Minimum Spanning Tree");
        Console.WriteLine("  8. Topological Sort: only on DAGs, used for dependency ordering");
        Console.WriteLine("======================================================");
    }
}
