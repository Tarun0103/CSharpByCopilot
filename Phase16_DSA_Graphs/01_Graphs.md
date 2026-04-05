# Phase 16: Graphs

## What is a Graph?

A **Graph** is a non-linear data structure consisting of **vertices (nodes)** connected by **edges**. It is the most versatile data structure and models real-world relationships naturally.

```
          0 ─── 1
          │   ╱ │
          │  ╱  │
          2     3
```

**Formal definition:** Graph G = (V, E) where V = vertices, E = edges

---

## Types of Graphs

| Type | Description | Example |
|------|-------------|---------|
| **Undirected** | Edges have no direction (A-B = B-A) | Social network friendships |
| **Directed (Digraph)** | Edges have direction (A→B ≠ B→A) | Web page links |
| **Weighted** | Edges have a cost/weight | Road maps with distances |
| **Unweighted** | All edges equal cost | Social connections |
| **Cyclic** | Contains at least one cycle | General road network |
| **Acyclic** | No cycles | Family tree |
| **DAG** | Directed Acyclic Graph | Build system dependencies |
| **Connected** | Every vertex reachable from every other | Single island of roads |
| **Disconnected** | Some vertices unreachable from others | Multiple islands |

---

## Graph Representations

### 1. Adjacency Matrix
2D array: `matrix[i][j] = weight` (0 if no edge)

```
Graph: 0-1, 0-2, 1-3

     0  1  2  3
0  [ 0  1  1  0 ]
1  [ 1  0  0  1 ]
2  [ 1  0  0  0 ]
3  [ 0  1  0  0 ]
```

- **Space:** O(V²)
- **Edge lookup:** O(1)
- **Find all neighbors:** O(V)
- **Best for:** Dense graphs (many edges)

### 2. Adjacency List
Each vertex stores a list of its neighbors.

```
0: [1, 2]
1: [0, 3]
2: [0]
3: [1]
```

- **Space:** O(V + E)
- **Edge lookup:** O(degree)
- **Find all neighbors:** O(degree)
- **Best for:** Sparse graphs (few edges) — most real-world graphs

---

## BFS vs DFS Comparison

| Feature | BFS | DFS |
|---------|-----|-----|
| Structure | Queue | Stack (recursive/iterative) |
| Time | O(V + E) | O(V + E) |
| Space | O(V) | O(V) |
| Shortest path (unweighted) | ✅ Yes | ❌ No |
| Cycle detection | ✅ Yes | ✅ Yes |
| Topological sort | ✅ (Kahn's) | ✅ (finish time) |
| Memory (dense graphs) | More | Less |
| Memory (sparse graphs) | Less | Less |

---

## Shortest Path Algorithms

| Algorithm | Weights | Complexity | Notes |
|-----------|---------|------------|-------|
| **BFS** | Unweighted | O(V + E) | Only for equal-weight edges |
| **Dijkstra** | Non-negative | O((V+E) log V) | Most common, uses min-heap |
| **Bellman-Ford** | Any (incl. negative) | O(V × E) | Detects negative cycles |
| **Floyd-Warshall** | Any | O(V³) | All-pairs shortest path |

### Dijkstra Key Idea
**Greedy:** Always pick the unvisited vertex with smallest known distance, then "relax" (update) its neighbors.

**Relaxation:** `if dist[u] + weight(u,v) < dist[v]` → update `dist[v]`

### Bellman-Ford Key Idea  
**DP approach:** Relax ALL edges V-1 times. A shortest path in a V-vertex graph uses at most V-1 edges.

---

## Minimum Spanning Tree (MST)

An MST is a subset of edges that:
- Connects all vertices
- Has no cycles
- Has minimum total weight

**For n vertices, MST has exactly n-1 edges.**

### Prim's Algorithm
- Grows MST one **vertex** at a time
- Use min-heap: always pick cheapest edge connecting new vertex to existing MST
- O((V + E) log V)

### Kruskal's Algorithm
- Grows MST one **edge** at a time
- Sort all edges by weight; greedily add edge if it doesn't create a cycle
- Uses **Union-Find** to detect cycles
- O(E log E)

---

## Union-Find (Disjoint Set Union)

Used to efficiently track connected components and detect cycles.

```
Operations:
  Find(x)  → root of x's component  (with path compression)
  Union(x,y) → merge x's and y's components  (by rank)

Initially: each vertex is its own component
  parent = [0, 1, 2, 3, 4]

After Union(0,1): parent[1] = 0
  parent = [0, 0, 2, 3, 4]

Adding edge 0-2: Find(0)=0, Find(2)=2 → different roots → safe to add!
Adding edge 0-1: Find(0)=0, Find(1)=0 → same root → CYCLE! Skip.
```

---

## Topological Sort

**Only valid for DAGs (Directed Acyclic Graphs)**

Orders vertices so every directed edge u→v appears with u before v.

```
Prerequisites: Math→Physics, Math→CS, CS→Algorithms, Algorithms→ML, Physics→ML

Valid order: Math → Physics → CS → Algorithms → ML
```

**Two approaches:**
1. **DFS with finish time**: push to stack AFTER visiting all descendants
2. **Kahn's Algorithm (BFS)**: repeatedly remove vertices with 0 in-degree

---

## Cycle Detection

### Undirected Graph
Use DFS: track parent. If neighbor is visited AND is not parent → cycle exists.

### Directed Graph
Use DFS: track 3 states:
- `WHITE` (0): unvisited
- `GRAY` (1): currently in recursion stack
- `BLACK` (2): fully processed

If we visit a `GRAY` node → cycle detected!

---

## Common Graph Problems

| Problem | Algorithm | Complexity |
|---------|-----------|------------|
| Shortest path (unweighted) | BFS | O(V+E) |
| Shortest path (weighted, +ve) | Dijkstra | O((V+E) log V) |
| Shortest path (negative weights) | Bellman-Ford | O(VE) |
| MST | Prim's / Kruskal's | O((V+E) log V) |
| Cycle detection | DFS | O(V+E) |
| Topological sort | DFS / Kahn's | O(V+E) |
| Connected components | DFS/BFS | O(V+E) |
| Bipartite check | BFS (2-color) | O(V+E) |

---

## Interview Questions & Answers

**Q1: What is the difference between BFS and DFS? When would you use each?**

**A:** BFS explores layer by layer using a Queue, DFS explores depth-first using recursion/Stack.
- Use **BFS** for: shortest path in unweighted graph, level-order traversal, finding nearest neighbor
- Use **DFS** for: cycle detection, topological sort, detecting connected components, exhaustive search (mazes, Sudoku)
- BFS uses **more memory** on wide graphs; DFS uses more memory on deep graphs and can overflow the call stack

---

**Q2: Why can't Dijkstra handle negative weight edges?**

**A:** Dijkstra is greedy — once a vertex is marked "done" (extracted from the priority queue), its distance is assumed final. With negative weights, a path discovered later could yield a shorter distance to an already-finalized vertex, violating this assumption. Use **Bellman-Ford** for negative weights.

---

**Q3: What is a negative cycle and why is it problematic?**

**A:** A negative cycle is a cycle whose total weight is less than zero. Following such a cycle indefinitely would keep reducing the path cost to −∞, making "shortest path" meaningless. **Bellman-Ford** detects negative cycles by running a (V)th relaxation pass — if any distance still decreases, a negative cycle exists.

---

**Q4: How do Prim's and Kruskal's algorithms differ?**

**A:**
- **Prim's** grows the MST by adding the cheapest edge connecting a **new vertex** to the existing MST. Works well with adjacency list and priority queue.
- **Kruskal's** sorts all edges and greedily adds the cheapest edge that doesn't form a cycle (uses Union-Find). Works well with edge list representation.
- Both produce the same MST for a given graph. Prim's is better for dense graphs; Kruskal's is better for sparse graphs.

---

**Q5: What is a topological sort? When is it applicable?**

**A:** Topological sort produces a linear ordering of vertices in a DAG such that every directed edge u→v places u before v. It's only valid on **Directed Acyclic Graphs**. Applications include build systems (compile order), task scheduling (dependencies), course prerequisites, and package management (npm/pip install order).

---

**Q6: What is the time/space complexity of BFS and DFS?**

**A:** Both BFS and DFS are **O(V + E)** time and **O(V)** space. The space is used by the visited set and the queue/stack. In the worst case (complete graph), E = V², making it O(V²) time.

---

**Q7: How do you detect a cycle in a directed graph?**

**A:** Use DFS with a "recursion stack" set (or 3-color marking: white/gray/black). When visiting a node, mark it gray (in progress). If we encounter a gray node again during DFS, a back edge exists → cycle detected. After fully processing a node, mark it black. This runs in O(V + E).

---

## Scenario-Based Questions

**Scenario 1:** You're building a GPS navigation app. Users need the fastest route between any two cities. How would you model and solve this?

**Answer:** Model the road network as a **weighted directed graph** where cities = vertices and roads = edges with travel time as weight. Use **Dijkstra's algorithm** to find the shortest path from source to destination. Store the graph as an adjacency list for memory efficiency. For real-time updates (traffic), re-run Dijkstra when edge weights change. For all-pairs precomputation (offline), use **Floyd-Warshall** and store a lookup table.

---

**Scenario 2:** A build system needs to compile files in the correct dependency order — a file can only be compiled after all its dependencies are compiled. Some configurations have circular dependencies (bugs). How do you handle this?

**Answer:** Model the dependency system as a **directed graph** where files = vertices and "A depends on B" = directed edge from A to B. First, **detect cycles** using DFS — if a cycle exists, report an error to the developer (circular dependency). If no cycle, apply **topological sort** to get the correct compilation order. This is exactly what `make`, Gradle, and webpack do internally.

---

**Scenario 3:** In a social network, you want to suggest "people you may know" (friends of friends). How would you implement this?

**Answer:** Model the social graph as an **undirected graph**. For a user U, run **BFS** from U (or find 2-hop neighbors). Level 1 neighbors are current friends; level 2 neighbors (friends of friends, not already friends with U) are candidates for suggestions. Rank them by number of mutual friends (how many paths connect them to U). BFS naturally gives this because it explores level by level.

---

## Common Mistakes

1. **Forgetting to mark visited nodes in BFS/DFS** → infinite loop on cyclic graphs
2. **Using Dijkstra with negative weights** → incorrect results; use Bellman-Ford instead
3. **Running topological sort on a cyclic graph** → undefined behavior; always check for cycles first
4. **Using Adjacency Matrix for sparse graphs** → wastes O(V²) memory
5. **Not handling disconnected graphs** in BFS/DFS → loop over all vertices and call BFS/DFS if unvisited
6. **Confusing MST edge count** → MST always has exactly V-1 edges for V vertices
7. **Off-by-one in Bellman-Ford** → must run exactly V-1 relaxation passes (not V)
