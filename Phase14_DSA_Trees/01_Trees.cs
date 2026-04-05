// ============================================================
// PHASE 14: TREES — Complete C# Implementation
// Topics: Tree terminology, Binary Tree, BST, Tree Traversals
//         (Inorder, Preorder, Postorder, Level Order),
//         BST operations, AVL concept, Trie
// Run this file in a Console Application project
// ============================================================

using System;
using System.Collections.Generic;

// ============================================================
// SECTION 1: TREE NODE DEFINITION
// A tree is a hierarchical data structure.
// Each node has a value and can have multiple children.
// Binary Tree: each node has AT MOST 2 children (left and right).
// ============================================================

public class TreeNode
{
    public int Val;
    public TreeNode Left;   // left child
    public TreeNode Right;  // right child

    public TreeNode(int val, TreeNode left = null, TreeNode right = null)
    {
        Val = val;
        Left = left;
        Right = right;
    }
}

// ============================================================
// TREE TERMINOLOGY:
//
//          1          ← ROOT (no parent)
//        /   \
//       2     3       ← INTERNAL NODES
//      / \   / \
//     4   5 6   7     ← LEAF NODES (no children)
//
// NODE: each element with a value and child pointers
// ROOT: topmost node (no parent)
// LEAF: node with no children
// EDGE: connection between parent and child
// HEIGHT: longest path from any given node to a leaf
//   height(4) = 0,  height(2) = 1,  height(1) = 2
// DEPTH: distance from root to a node  
//   depth(1) = 0,  depth(2) = 1,  depth(4) = 2
// LEVEL: all nodes at same depth form a level
// ============================================================

// ============================================================
// SECTION 2: TREE TRAVERSALS
//
// There are 4 standard traversal orders:
// 1. INORDER   (Left, Root, Right) → gives SORTED order in a BST!
// 2. PREORDER  (Root, Left, Right) → used to COPY/SERIALIZE a tree
// 3. POSTORDER (Left, Right, Root) → used to DELETE a tree
// 4. LEVEL ORDER (BFS)            → used for level-by-level processing
//
// Tree used in examples:
//          4
//        /   \
//       2     6
//      / \   / \
//     1   3 5   7
//
// Inorder:    1,2,3,4,5,6,7 (sorted!)
// Preorder:   4,2,1,3,6,5,7
// Postorder:  1,3,2,5,7,6,4
// Level Order: 4,2,6,1,3,5,7
// ============================================================

class TreeTraversals
{
    // --- INORDER: Left → Root → Right ---
    // Visits nodes in ASCENDING order for BST!
    // Use for: BST validation, sorted output, in-order operations
    public static void Inorder(TreeNode root, List<int> result)
    {
        if (root == null) return;   // base case

        Inorder(root.Left, result);   // 1. traverse left subtree
        result.Add(root.Val);          // 2. process root
        Inorder(root.Right, result);  // 3. traverse right subtree
    }

    // Inorder using explicit stack (iterative — avoids recursion stack overflow)
    public static List<int> InorderIterative(TreeNode root)
    {
        List<int> result = new List<int>();
        Stack<TreeNode> stack = new Stack<TreeNode>();
        TreeNode current = root;

        while (current != null || stack.Count > 0)
        {
            // Go as far LEFT as possible, pushing onto stack
            while (current != null)
            {
                stack.Push(current);
                current = current.Left;
            }

            // Pop and process
            current = stack.Pop();
            result.Add(current.Val);

            // Move to right subtree
            current = current.Right;
        }

        return result;
    }

    // --- PREORDER: Root → Left → Right ---
    // Root is processed BEFORE its children.
    // Use for: copy/clone a tree, serialize to file, prefix expression
    public static void Preorder(TreeNode root, List<int> result)
    {
        if (root == null) return;

        result.Add(root.Val);          // 1. process root
        Preorder(root.Left, result);   // 2. traverse left
        Preorder(root.Right, result);  // 3. traverse right
    }

    // --- POSTORDER: Left → Right → Root ---
    // Root is processed AFTER its children.
    // Use for: delete a tree (delete children before parent), evaluate expression tree
    public static void Postorder(TreeNode root, List<int> result)
    {
        if (root == null) return;

        Postorder(root.Left, result);  // 1. traverse left
        Postorder(root.Right, result); // 2. traverse right
        result.Add(root.Val);          // 3. process root (last!)
    }

    // --- LEVEL ORDER (BFS): Level by level ---
    // Uses a QUEUE — nodes are processed in FIFO order = level by level
    // Use for: find shortest path, connect nodes at same level, visualize tree
    public static List<List<int>> LevelOrder(TreeNode root)
    {
        List<List<int>> result = new List<List<int>>();
        if (root == null) return result;

        Queue<TreeNode> queue = new Queue<TreeNode>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            int levelSize = queue.Count;  // number of nodes at CURRENT level
            List<int> level = new List<int>();

            // Process ALL nodes at the current level
            for (int i = 0; i < levelSize; i++)
            {
                TreeNode node = queue.Dequeue();
                level.Add(node.Val);

                // Enqueue children → they'll be processed in NEXT level
                if (node.Left  != null) queue.Enqueue(node.Left);
                if (node.Right != null) queue.Enqueue(node.Right);
            }

            result.Add(level);
        }

        return result;
    }
}

// ============================================================
// SECTION 3: BINARY SEARCH TREE (BST)
// A Binary Tree where: Left < Root < Right (for every node)
//
// This ordering gives us O(log n) average time for search, insert, delete!
// (Worst case O(n) if tree becomes a linked list — unbalanced)
//
// BST Property: Inorder traversal gives sorted output!
// ============================================================

class BinarySearchTree
{
    public TreeNode Root { get; private set; }

    // --- INSERT — O(h) where h = height ---
    // h = log n for balanced tree, n for unbalanced (worst case)
    public void Insert(int val)
    {
        Root = InsertRecursive(Root, val);
    }

    private TreeNode InsertRecursive(TreeNode node, int val)
    {
        if (node == null)
            return new TreeNode(val);  // found the insertion point

        if (val < node.Val)
            node.Left = InsertRecursive(node.Left, val);   // go left
        else if (val > node.Val)
            node.Right = InsertRecursive(node.Right, val); // go right
        // Equal: duplicates ignored (or you could count them — depends on problem)

        return node;
    }

    // --- SEARCH — O(h) ---
    public bool Search(int val)
    {
        return SearchRecursive(Root, val);
    }

    private bool SearchRecursive(TreeNode node, int val)
    {
        if (node == null) return false;   // reached a leaf without finding it
        if (node.Val == val) return true; // found!

        // BST property: go left if val < current, right if val > current
        if (val < node.Val) return SearchRecursive(node.Left, val);
        else                return SearchRecursive(node.Right, val);
    }

    // --- DELETE — O(h) ---
    // 3 cases:
    // 1. Node is a leaf: just remove it
    // 2. Node has 1 child: replace node with its child
    // 3. Node has 2 children: replace with INORDER SUCCESSOR (smallest in right subtree)
    public void Delete(int val)
    {
        Root = DeleteRecursive(Root, val);
    }

    private TreeNode DeleteRecursive(TreeNode node, int val)
    {
        if (node == null) return null;

        if (val < node.Val)
            node.Left = DeleteRecursive(node.Left, val);   // go left
        else if (val > node.Val)
            node.Right = DeleteRecursive(node.Right, val); // go right
        else
        {
            // Found the node to delete!

            // Case 1: Leaf node (no children)
            if (node.Left == null && node.Right == null)
                return null;

            // Case 2a: Only right child
            if (node.Left == null)
                return node.Right;

            // Case 2b: Only left child
            if (node.Right == null)
                return node.Left;

            // Case 3: Two children
            // Replace current node's value with INORDER SUCCESSOR value (min of right subtree)
            // Then delete that inorder successor node from the right subtree
            TreeNode successor = FindMin(node.Right);
            node.Val = successor.Val;                           // replace value
            node.Right = DeleteRecursive(node.Right, successor.Val); // delete successor
        }

        return node;
    }

    // Find minimum node in a subtree (leftmost node)
    private TreeNode FindMin(TreeNode node)
    {
        while (node.Left != null)
            node = node.Left;
        return node;
    }

    // Height of tree — O(n)
    public int Height()
    {
        return HeightRecursive(Root);
    }

    private int HeightRecursive(TreeNode node)
    {
        if (node == null) return -1;  // -1 means null node

        int leftHeight  = HeightRecursive(node.Left);
        int rightHeight = HeightRecursive(node.Right);

        return 1 + Math.Max(leftHeight, rightHeight);
    }

    // Check if tree is a valid BST — O(n)
    public bool IsValidBST()
    {
        return ValidateBST(Root, long.MinValue, long.MaxValue);
    }

    private bool ValidateBST(TreeNode node, long min, long max)
    {
        if (node == null) return true;

        // Every node must be within its valid range
        if (node.Val <= min || node.Val >= max) return false;

        // Left subtree: values must be < node.Val (so max = node.Val)
        // Right subtree: values must be > node.Val (so min = node.Val)
        return ValidateBST(node.Left, min, node.Val) &&
               ValidateBST(node.Right, node.Val, max);
    }
}

// ============================================================
// SECTION 4: COMMON TREE PROBLEMS
// ============================================================

class TreeProblems
{
    // Maximum depth (height) — most common interview question
    public static int MaxDepth(TreeNode root)
    {
        if (root == null) return 0;
        return 1 + Math.Max(MaxDepth(root.Left), MaxDepth(root.Right));
    }

    // Check if tree is balanced (height difference ≤ 1 for every node)
    public static bool IsBalanced(TreeNode root)
    {
        return CheckBalance(root) != -1;
    }

    // Returns height if balanced, -1 if unbalanced
    private static int CheckBalance(TreeNode node)
    {
        if (node == null) return 0;

        int left = CheckBalance(node.Left);
        if (left == -1) return -1;  // left subtree unbalanced

        int right = CheckBalance(node.Right);
        if (right == -1) return -1;  // right subtree unbalanced

        if (Math.Abs(left - right) > 1) return -1;  // this node unbalanced

        return 1 + Math.Max(left, right);  // return height
    }

    // Lowest Common Ancestor (LCA) of two nodes
    // LCA(p,q) = deepest node that is an ancestor of both p and q
    public static TreeNode LCA(TreeNode root, TreeNode p, TreeNode q)
    {
        if (root == null || root == p || root == q)
            return root;  // found one of the targets (or null)

        TreeNode left  = LCA(root.Left, p, q);
        TreeNode right = LCA(root.Right, p, q);

        if (left != null && right != null)
            return root;  // p is in one subtree, q in the other → root is LCA

        return left ?? right;  // both in same subtree
    }

    // Check if two trees are identical (same structure AND values)
    public static bool IsSameTree(TreeNode p, TreeNode q)
    {
        if (p == null && q == null) return true;           // both null = same
        if (p == null || q == null) return false;          // one null, one not
        if (p.Val != q.Val) return false;                  // different values

        return IsSameTree(p.Left, q.Left) &&               // left subtrees match
               IsSameTree(p.Right, q.Right);               // right subtrees match
    }

    // Check if tree is symmetric (mirror image of itself)
    public static bool IsSymmetric(TreeNode root)
    {
        return IsMirror(root?.Left, root?.Right);
    }

    private static bool IsMirror(TreeNode left, TreeNode right)
    {
        if (left == null && right == null) return true;
        if (left == null || right == null) return false;
        if (left.Val != right.Val) return false;

        // Outer pair mirrors: left.Left ↔ right.Right
        // Inner pair mirrors: left.Right ↔ right.Left
        return IsMirror(left.Left, right.Right) &&
               IsMirror(left.Right, right.Left);
    }

    // Diameter of binary tree = longest path between any two nodes
    // Path does NOT need to pass through root
    public static int Diameter(TreeNode root)
    {
        int maxDiameter = 0;
        DiameterHelper(root, ref maxDiameter);
        return maxDiameter;
    }

    private static int DiameterHelper(TreeNode node, ref int maxDiameter)
    {
        if (node == null) return 0;

        int leftDepth  = DiameterHelper(node.Left, ref maxDiameter);
        int rightDepth = DiameterHelper(node.Right, ref maxDiameter);

        // Diameter through this node = leftDepth + rightDepth
        maxDiameter = Math.Max(maxDiameter, leftDepth + rightDepth);

        return 1 + Math.Max(leftDepth, rightDepth);  // return height
    }

    // Path Sum: does tree have a root-to-leaf path that sums to target?
    public static bool HasPathSum(TreeNode root, int target)
    {
        if (root == null) return false;

        target -= root.Val;  // subtract current node's value from remaining target

        // If leaf node and remaining target is 0 → found valid path!
        if (root.Left == null && root.Right == null)
            return target == 0;

        // Check in either subtree
        return HasPathSum(root.Left, target) || HasPathSum(root.Right, target);
    }
}

// ============================================================
// SECTION 5: TRIE (PREFIX TREE)
// A Trie is an N-ary tree where each path from root to a special
// node represents a stored string.
//
// Perfect for: autocomplete, spell-checking, IP routing tables
//
// Example storing ["apple", "app", "application", "apply", "apt"]:
//         root
//          |
//          a
//          |
//          p
//         / \
//        p   t      ← "apt" ends here
//        |   |
//        l   *      ← "apt" end marker
//       / \
//      e   y        ← "apply" ends here (y*)
//      |
//      *            ← "app" ends here
//      |
//      i
//      |
//     ... (application, apple ...)
// ============================================================

class TrieNode
{
    public Dictionary<char, TrieNode> Children = new Dictionary<char, TrieNode>();
    public bool IsEndOfWord = false;  // marks if a complete word ends here
}

class Trie
{
    private TrieNode _root = new TrieNode();

    // --- INSERT — O(m) where m = word length ---
    public void Insert(string word)
    {
        TrieNode current = _root;

        foreach (char c in word)
        {
            // If this character doesn't have a node, create one
            if (!current.Children.ContainsKey(c))
                current.Children[c] = new TrieNode();

            current = current.Children[c];  // move to next character node
        }

        current.IsEndOfWord = true;  // mark end of this word
    }

    // --- SEARCH — O(m) ---
    // Returns true if the EXACT word exists in the trie
    public bool Search(string word)
    {
        TrieNode node = FindNode(word);
        return node != null && node.IsEndOfWord;  // must be marked as end-of-word
    }

    // --- STARTS WITH (PREFIX SEARCH) — O(m) ---
    // Returns true if any word in trie starts with this prefix
    public bool StartsWith(string prefix)
    {
        return FindNode(prefix) != null;  // just need the path to exist
    }

    // Common helper: traverse trie following the characters
    private TrieNode FindNode(string prefix)
    {
        TrieNode current = _root;

        foreach (char c in prefix)
        {
            if (!current.Children.ContainsKey(c))
                return null;  // path doesn't exist

            current = current.Children[c];
        }

        return current;
    }

    // --- AUTOCOMPLETE — find all words with given prefix ---
    public List<string> AutoComplete(string prefix)
    {
        List<string> suggestions = new List<string>();

        TrieNode prefixEnd = FindNode(prefix);
        if (prefixEnd == null) return suggestions;  // prefix not found

        // DFS from the end of the prefix to find all complete words
        DfsCollectWords(prefixEnd, prefix, suggestions);
        return suggestions;
    }

    private void DfsCollectWords(TrieNode node, string currentWord, List<string> results)
    {
        if (node.IsEndOfWord)
            results.Add(currentWord);  // complete word found

        foreach (var (ch, childNode) in node.Children)
            DfsCollectWords(childNode, currentWord + ch, results);
    }
}

// ============================================================
// MAIN PROGRAM
// ============================================================
class TreesProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 14: TREES — Complete Demonstration");
        Console.WriteLine("======================================================");

        // Build the sample tree:
        //          4
        //        /   \
        //       2     6
        //      / \   / \
        //     1   3 5   7

        TreeNode root = new TreeNode(4,
            new TreeNode(2, new TreeNode(1), new TreeNode(3)),
            new TreeNode(6, new TreeNode(5), new TreeNode(7)));

        // -------------------------------------------------------
        // DEMO 1: Tree Traversals
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 1: Tree Traversals ===");
        Console.WriteLine("  Tree structure:");
        Console.WriteLine("       4");
        Console.WriteLine("      / \\");
        Console.WriteLine("     2   6");
        Console.WriteLine("    / \\ / \\");
        Console.WriteLine("   1  3 5  7");

        var inorder = new List<int>();
        TreeTraversals.Inorder(root, inorder);
        Console.WriteLine($"\n  Inorder   (L→Root→R): {string.Join(", ", inorder)}");
        // 1, 2, 3, 4, 5, 6, 7 ← sorted!

        var preorder = new List<int>();
        TreeTraversals.Preorder(root, preorder);
        Console.WriteLine($"  Preorder  (Root→L→R): {string.Join(", ", preorder)}");
        // 4, 2, 1, 3, 6, 5, 7

        var postorder = new List<int>();
        TreeTraversals.Postorder(root, postorder);
        Console.WriteLine($"  Postorder (L→R→Root): {string.Join(", ", postorder)}");
        // 1, 3, 2, 5, 7, 6, 4

        var levelOrder = TreeTraversals.LevelOrder(root);
        Console.WriteLine($"  Level Order (BFS):    {string.Join(" | ", levelOrder.Select(level => string.Join(",", level)))}");
        // Level 0: 4 | Level 1: 2,6 | Level 2: 1,3,5,7

        // Iterative inorder
        var iterativeInorder = TreeTraversals.InorderIterative(root);
        Console.WriteLine($"  Inorder (iterative):  {string.Join(", ", iterativeInorder)}");

        // -------------------------------------------------------
        // DEMO 2: Binary Search Tree Operations
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 2: Binary Search Tree (BST) ===");

        var bst = new BinarySearchTree();

        int[] values = { 5, 3, 7, 1, 4, 6, 8, 2 };
        Console.Write("  Inserting: ");
        foreach (int v in values) { Console.Write(v + " "); bst.Insert(v); }
        Console.WriteLine();

        // Inorder BST = sorted
        var bstInorder = new List<int>();
        TreeTraversals.Inorder(bst.Root, bstInorder);
        Console.WriteLine($"  BST Inorder (sorted): {string.Join(", ", bstInorder)}");

        Console.WriteLine($"  Search 4: {bst.Search(4)}");   // True
        Console.WriteLine($"  Search 9: {bst.Search(9)}");   // False
        Console.WriteLine($"  Height: {bst.Height()}");
        Console.WriteLine($"  Is valid BST: {bst.IsValidBST()}");

        bst.Delete(3);
        var afterDelete = new List<int>();
        TreeTraversals.Inorder(bst.Root, afterDelete);
        Console.WriteLine($"  After Delete(3): {string.Join(", ", afterDelete)}");

        // -------------------------------------------------------
        // DEMO 3: Classic Tree Problems
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 3: Classic Tree Problems ===");

        Console.WriteLine($"  Max depth of sample tree: {TreeProblems.MaxDepth(root)}");   // 3
        Console.WriteLine($"  Is balanced: {TreeProblems.IsBalanced(root)}");               // True
        Console.WriteLine($"  Is symmetric: {TreeProblems.IsSymmetric(root)}");             // True (perfect BST)
        Console.WriteLine($"  Diameter: {TreeProblems.Diameter(root)}");                    // 4 (path 1→2→4→6→7)
        Console.WriteLine($"  Has path sum 7 (1+2+4): {TreeProblems.HasPathSum(root, 7)}");// True
        Console.WriteLine($"  Has path sum 20: {TreeProblems.HasPathSum(root, 20)}");       // False

        // LCA
        var node2 = root.Left;        // node with value 2
        var node5 = root.Right.Left;  // node with value 5
        var lca = TreeProblems.LCA(root, node2, node5);
        Console.WriteLine($"  LCA(2, 5) = {lca?.Val}");  // 4 (root)

        // -------------------------------------------------------
        // DEMO 4: Trie
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 4: Trie (Prefix Tree) ===");

        Trie trie = new Trie();

        string[] words = { "apple", "app", "application", "apply", "apt", "banana", "band" };
        Console.Write("  Inserting words: ");
        Console.WriteLine(string.Join(", ", words));

        foreach (string w in words) trie.Insert(w);

        Console.WriteLine($"\n  Search 'apple': {trie.Search("apple")}");   // True
        Console.WriteLine($"  Search 'app': {trie.Search("app")}");         // True
        Console.WriteLine($"  Search 'ap': {trie.Search("ap")}");           // False (not a complete word)
        Console.WriteLine($"  StartsWith 'ap': {trie.StartsWith("ap")}");   // True
        Console.WriteLine($"  StartsWith 'xyz': {trie.StartsWith("xyz")}"); // False

        // Autocomplete
        var suggestions = trie.AutoComplete("app");
        Console.WriteLine($"\n  AutoComplete('app'): {string.Join(", ", suggestions)}");

        var suggestionsAp = trie.AutoComplete("ap");
        Console.WriteLine($"  AutoComplete('ap'): {string.Join(", ", suggestionsAp)}");

        // -------------------------------------------------------
        // SUMMARY
        // -------------------------------------------------------
        Console.WriteLine("\n======================================================");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  1. BST: Left < Root < Right, O(log n) avg search/insert/delete");
        Console.WriteLine("  2. Inorder traversal of BST = sorted output");
        Console.WriteLine("  3. Level Order uses Queue; Inorder/Pre/Post use recursion (or Stack)");
        Console.WriteLine("  4. Tree height = O(n) worst (unbalanced), O(log n) balanced");
        Console.WriteLine("  5. Trie: O(m) insert/search where m = word length");
        Console.WriteLine("  6. Trie prefix operations are O(m) regardless of word count");
        Console.WriteLine("  7. Most tree problems: base case when node == null");
        Console.WriteLine("======================================================");
    }
}
