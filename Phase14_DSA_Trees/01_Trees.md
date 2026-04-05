# Phase 14: Trees

## What is a Tree?

A **Tree** is a hierarchical, non-linear data structure with nodes connected by edges, with one special **root** node and no cycles.

```
         10        ← root
        /  \
       5    15     ← internal nodes
      / \     \
     3   7    20   ← leaves (no children)
```

**Properties:**
- N nodes → exactly N-1 edges
- One path between any two nodes
- Hierarchical parent-child relationships

---

## Key Terminology

| Term | Meaning |
|------|---------|
| **Root** | Top node (no parent) |
| **Leaf** | Node with no children |
| **Height** | Longest path from root to leaf |
| **Depth** | Distance from root to a specific node |
| **Level** | All nodes at the same depth |
| **Subtree** | A tree rooted at any node |
| **Ancestor** | Any node on path from root to a node |
| **Descendant** | Any node in a node's subtree |

---

## Types of Trees

| Type | Description |
|------|-------------|
| **Binary Tree** | Each node has at most 2 children |
| **Binary Search Tree (BST)** | Left subtree < node < right subtree |
| **Balanced BST** | Height = O(log n); e.g., AVL Tree, Red-Black Tree |
| **Complete Binary Tree** | All levels fully filled except possibly last, filled left to right |
| **Perfect Binary Tree** | All leaves at same level, all internal nodes have 2 children |
| **Full Binary Tree** | Every node has 0 or 2 children |
| **N-ary Tree** | Each node can have up to N children |
| **Trie** | Prefix tree for strings |
| **Segment Tree** | For range queries (sum, min, max) |

---

## Binary Search Tree (BST)

**Invariant:** For every node N:
- ALL values in left subtree < N.value
- ALL values in right subtree > N.value

```
Insert: 10, 5, 15, 3, 7, 20

        10
       /  \
      5   15
     / \    \
    3   7   20
```

**BST Operations:**

| Operation | Average (Balanced) | Worst (Skewed) |
|-----------|--------------------|----------------|
| Search | O(log n) | O(n) |
| Insert | O(log n) | O(n) |
| Delete | O(log n) | O(n) |

**Worst case = sorted input** → inserts create a linear chain (like a linked list).

---

## Tree Traversals

### Depth-First (3 orderings)

```
        A
       / \
      B   C
     / \
    D   E
```

| Order | Pattern | Result | Use |
|-------|---------|--------|-----|
| **In-order** | Left → Node → Right | D B E A C | BST sorted output |
| **Pre-order** | Node → Left → Right | A B D E C | Copy/serialize tree |
| **Post-order** | Left → Right → Node | D E B C A | Delete tree, evaluate expression |

### Breadth-First (Level-Order)
Uses a **Queue**. Visit all nodes at depth k before depth k+1.
Result: `A B C D E` (by levels)

---

## Common Tree Problems & Solutions

| Problem | Approach | Time |
|---------|----------|------|
| Max depth | DFS: 1 + max(left, right) | O(n) |
| Is balanced | DFS: check height && balanced recursively | O(n) |
| Lowest Common Ancestor | DFS: if both in different subtrees → LCA found | O(n) |
| Same tree | DFS: compare structure and values | O(n) |
| Symmetric | BFS or DFS: compare mirror images | O(n) |
| IsValidBST | DFS with min/max bounds | O(n) |
| Level order | BFS with Queue | O(n) |
| Path sum | DFS: subtract value, check at leaves | O(n) |

---

## Trie (Prefix Tree)

A trie stores strings where each path from root to a marked node spells a word. All strings sharing a prefix share the same path.

```
Insert: "cat", "car", "cab"

root → c → a → t (word)
            ↘ r (word)
            ↘ b (word)
```

**Operations:** Insert O(L), Search O(L), StartsWith O(L) where L = word length.

**Uses:** Autocomplete, spell checking, IP routing, dictionary, word games

---

## AVL Tree and Red-Black Tree (concept)

Both are **self-balancing BSTs** that maintain height O(log n) after every insertion/deletion.

| | AVL Tree | Red-Black Tree |
|-|----------|---------------|
| Balance condition | Height difference ≤ 1 | No red-red parent-child |
| Insert/Delete speed | Slightly slower (more rotations) | Slightly faster |
| Lookup speed | Slightly faster (stricter balance) | Slightly slower |
| Used in | SortedSet, SortedDictionary (.NET internals) | `std::map` in C++, TreeMap in Java |

---

## Interview Questions & Answers

**Q1: What is the difference between BFS and DFS tree traversal? Which uses more memory?**

**A:** BFS (Breadth-First) uses a Queue to visit nodes level by level — finds the shortest path in trees/graphs. DFS (Depth-First) uses recursion/Stack to go deep before backtracking — three orderings: in/pre/post-order.

Memory: BFS uses O(w) where w = max width of tree (worst case: O(n) for complete tree at last level). DFS uses O(h) where h = height. For a balanced tree, DFS uses O(log n) — less than BFS. For a skewed tree, DFS uses O(n).

---

**Q2: How would you validate if a binary tree is a valid BST?**

**A:** Don't just compare a node with its immediate children — that misses the global constraint. Use a "range validation" approach: pass `(min, max)` bounds down the tree. For each node, check `min < node.value < max`. For the left child, update upper bound to `node.value`. For the right child, update lower bound to `node.value`. If any node violates its bounds → not a valid BST. Time: O(n), Space: O(h).

---

**Q3: What is the Lowest Common Ancestor (LCA) of two nodes?**

**A:** The LCA of nodes p and q is the deepest node that has both p and q as descendants (or is p or q itself). Algorithm: traverse the tree. If current node is p or q → return it. Recurse left and right. If both return non-null → current node is the LCA. If only one returns non-null → that subtree contains both p and q. Time: O(n), Space: O(h).

---

**Q4: How do you serialize and deserialize a binary tree?**

**A:** Serialize using **pre-order traversal** with a null marker:
`serialize(root) = root.val + "," + serialize(left) + "," + serialize(right)`
Null nodes → "null" in output.

Deserialize: split string into tokens, use a queue of tokens, recursively rebuild using same pre-order traversal. Non-null tokens create nodes; "null" tokens create null pointers.

---

**Q5: What is the height of a balanced BST with n nodes?**

**A:** Height = ⌊log₂(n)⌋. For n=7 → height=2 (0-indexed), For n=15 → height=3. This is why balanced BST operations are O(log n) — we only traverse from root to leaf = O(height).

A perfectly balanced BST with height h has 2^(h+1) - 1 nodes.

---

**Q6: Explain in-order traversal of a BST and why it produces sorted output.**

**A:** In-order traversal visits: Left → Node → Right. In a BST, the left subtree contains all smaller values and the right subtree contains all larger values. By visiting left first, then the current node, then right, we naturally visit nodes in ascending order of value. This property is used to flatten a BST to a sorted array or to find the k-th smallest element.

---

## Scenario-Based Questions

**Scenario 1:** A file system stores directories and files in a tree structure. Design an algorithm to find all files larger than a given size.

**Answer:** Use DFS (pre-order or post-order) on the directory tree. For each node: if it's a file and size > threshold → add to results. If it's a directory → recursively search all children. DFS naturally handles arbitrary nesting depth. Use `yield return` in C# for lazy enumeration (avoids storing all results in memory). Time: O(total nodes), Space: O(max depth of directory tree).

---

**Scenario 2:** A decision-making system uses a binary decision tree for recommendations. Given a user's answers to questions, traverse the tree to reach a recommendation leaf.

**Answer:** Model the decision tree so each internal node stores a "question" and each edge stores a "yes/no answer". Traversal: start at root, evaluate the question for the user, follow the yes/no branch, repeat until reaching a leaf (recommendation). Implementation: `if question(user) → go left; else → go right`. This is how decision tree classifiers (like CART) and expert systems work.

---

**Scenario 3:** You need to find the most recently used file in a code editor (like IntelliJ's "Recent Files"). Files are opened and closed frequently. Support O(log n) "most recently opened" queries.

**Answer:** Use an **Order Statistics Tree** (augmented BST storing subtree sizes) or simply maintain an `OrderedDictionary` keyed on timestamp. For "most recently opened", use a max-heap keyed on timestamp. With a Red-Black Tree (SortedDictionary), you can do O(log n) insert, O(log n) delete, and O(1) min/max access. Also consider: simple `LinkedList<string>` with `Dictionary<string, LinkedListNode>` for O(1) move-to-front = LRU pattern.

---

## Common Mistakes

1. **Not handling null nodes** → NullReferenceException in tree traversal
2. **BST validation with wrong approach** → checking only immediate parent-child instead of full range constraints
3. **Confusing height and depth** → height = root-to-leaf, depth = root-to-specific-node
4. **Recursion without base case** → `MaxDepth(null)` must return 0 or -1
5. **Level-order traversal without Queue** → using recursion for BFS doesn't give level-by-level control
6. **Modifying the tree during iteration** → changing structure while traversing causes undefined behavior
7. **Skewed BST performance** → inserting sorted data into a plain BST gives O(n) performance; use self-balancing trees
