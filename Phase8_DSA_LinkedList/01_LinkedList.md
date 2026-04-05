# Phase 8: Linked List — Complete Guide

## What is a Linked List?

A **Linked List** is a linear data structure where elements (called **nodes**) are stored in memory **non-contiguously**. Each node contains:
1. **Data** — the actual value stored
2. **Pointer(s)** — reference(s) to the next (and/or previous) node

Unlike arrays, linked list elements are NOT stored in adjacent memory locations. They are connected via pointers.

```
Singly Linked List:
[10|*] --> [20|*] --> [30|*] --> [40|null]
  Head                             Tail

Doubly Linked List:
null<--[10|*] <--> [*|20|*] <--> [*|30|*] <--> [*|40]-->null
         Head                                     Tail
```

---

## Types of Linked Lists

### 1. Singly Linked List
- Each node has ONE pointer pointing to the **next** node
- Can only traverse **forward**
- Less memory (one pointer per node)

### 2. Doubly Linked List
- Each node has TWO pointers: **previous** and **next**
- Can traverse **both directions**
- More memory (two pointers per node)
- C#'s built-in `LinkedList<T>` is a **doubly linked list**

### 3. Circular Linked List
- Last node points back to the **first** node
- Used in round-robin scheduling, circular buffers

---

## Node Structure

```csharp
// Custom node for singly linked list
public class SinglyNode<T>
{
    public T Data;           // Value stored in node
    public SinglyNode<T> Next; // Pointer to next node (null if last)

    public SinglyNode(T data)
    {
        Data = data;
        Next = null;
    }
}

// C# built-in LinkedListNode<T>
LinkedListNode<int> node = new LinkedListNode<int>(42);
// node.Value  = 42
// node.Next   = next node
// node.Previous = previous node
// node.List   = the LinkedList<T> this node belongs to
```

---

## C# Built-in `LinkedList<T>`

C# provides `System.Collections.Generic.LinkedList<T>` — a **doubly linked list**.

```csharp
LinkedList<int> list = new LinkedList<int>();

// Add to end
list.AddLast(10);
list.AddLast(20);
list.AddLast(30);

// Add to front
list.AddFirst(5);

// Add after a specific node
LinkedListNode<int> node = list.Find(20); // find node with value 20
list.AddAfter(node, 25);                  // insert 25 after 20
list.AddBefore(node, 15);                 // insert 15 before 20

// Remove
list.Remove(10);        // remove by value
list.RemoveFirst();     // remove head
list.RemoveLast();      // remove tail

// Properties
int count = list.Count;
LinkedListNode<int> first = list.First; // head node
LinkedListNode<int> last  = list.Last;  // tail node

// Contains check
bool has20 = list.Contains(20);
```

---

## Time Complexity

| Operation              | LinkedList | Array/List |
|------------------------|-----------|------------|
| Access by index        | O(n)      | O(1)       |
| Search by value        | O(n)      | O(n)       |
| Insert at head         | **O(1)**  | O(n)       |
| Insert at tail (known) | **O(1)**  | O(1) amort |
| Insert at middle       | O(n)      | O(n)       |
| Delete at head         | **O(1)**  | O(n)       |
| Delete by reference    | **O(1)**  | O(n)       |
| Memory (per element)   | Higher    | Lower      |

> **Key insight**: LinkedList wins when you do many **insertions/deletions at known positions**. Arrays/List win for **random access by index**.

---

## Advantages of Linked List

1. **Dynamic size** — grows/shrinks at runtime without reallocation
2. **Efficient insert/delete** — O(1) at head/tail or given a node reference
3. **No wasted memory** — allocates exactly what it needs (unlike arrays which over-allocate)
4. **Building block** — foundation for stacks, queues, hash table chaining

## Disadvantages of Linked List

1. **No random access** — must traverse from head to reach index N → O(n)
2. **Extra memory** — each node carries pointer overhead (8 bytes per pointer on 64-bit)
3. **Cache unfriendly** — nodes scattered in memory → cache misses → slower in practice
4. **No backward traversal** in singly linked list
5. **Complex code** — pointer manipulation is error-prone

---

## Implementing a Singly Linked List from Scratch

```csharp
public class SinglyLinkedList<T>
{
    private Node<T> head;  // first node
    public int Count { get; private set; }

    // Insert at beginning — O(1)
    public void InsertAtHead(T data)
    {
        Node<T> newNode = new Node<T>(data);
        newNode.Next = head;  // new node points to old head
        head = newNode;       // head is now the new node
        Count++;
    }

    // Insert at end — O(n) without tail pointer
    public void InsertAtTail(T data)
    {
        Node<T> newNode = new Node<T>(data);
        if (head == null) { head = newNode; Count++; return; }

        Node<T> current = head;
        while (current.Next != null)  // walk to last node
            current = current.Next;

        current.Next = newNode;  // last node now points to new node
        Count++;
    }

    // Delete by value — O(n)
    public bool Delete(T data)
    {
        if (head == null) return false;

        // Special case: deleting head
        if (EqualityComparer<T>.Default.Equals(head.Data, data))
        {
            head = head.Next;  // skip old head
            Count--;
            return true;
        }

        // Walk list looking for node BEFORE the one to delete
        Node<T> current = head;
        while (current.Next != null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Next.Data, data))
            {
                current.Next = current.Next.Next;  // skip the target node
                Count--;
                return true;
            }
            current = current.Next;
        }
        return false;
    }

    // Reverse the list — O(n)
    public void Reverse()
    {
        Node<T> prev = null;
        Node<T> current = head;
        while (current != null)
        {
            Node<T> next = current.Next;  // save next
            current.Next = prev;           // reverse pointer
            prev = current;                // advance prev
            current = next;                // advance current
        }
        head = prev;  // head is now the old tail
    }
}
```

---

## Real-World Use Cases

| Use Case | Why LinkedList? |
|---|---|
| Browser back/forward history | Doubly linked: prev/next page |
| Music playlist | Insert/remove songs at any position |
| Undo/Redo operations | Each action is a node |
| Hash table chaining | Collision resolution at each bucket |
| OS process scheduling | Processes added/removed dynamically |
| LRU Cache implementation | O(1) move-to-front |

---

## Interview Questions & Answers

### Q1: What is the difference between a singly and doubly linked list?

**Answer:**
- **Singly**: Each node has only a `Next` pointer. Traversal is one-directional (head to tail). Uses less memory.
- **Doubly**: Each node has both `Next` and `Previous` pointers. Can traverse in both directions. C#'s `LinkedList<T>` is doubly linked. Uses more memory but enables O(1) `Remove` when you already have a node reference.

---

### Q2: What is the time complexity of inserting at the beginning of a linked list vs an array?

**Answer:**
- **LinkedList**: O(1) — just change the head pointer
- **Array/List**: O(n) — must shift all existing elements right by one position

---

### Q3: How would you detect a cycle in a linked list?

**Answer:** Use **Floyd's Tortoise and Hare algorithm**:
```csharp
bool HasCycle<T>(Node<T> head)
{
    Node<T> slow = head;  // moves 1 step at a time
    Node<T> fast = head;  // moves 2 steps at a time

    while (fast != null && fast.Next != null)
    {
        slow = slow.Next;        // 1 step
        fast = fast.Next.Next;   // 2 steps

        if (slow == fast) return true;  // they met = cycle exists
    }
    return false;  // fast reached null = no cycle
}
```
Time: O(n), Space: O(1)

---

### Q4: How do you reverse a linked list?

**Answer:** Iterate through the list and reverse each pointer:
```
Original: 1 -> 2 -> 3 -> null
After:    3 -> 2 -> 1 -> null
```
Use three pointers: `prev = null`, `current = head`, `next`. On each step:
1. Save `next = current.Next`
2. Reverse `current.Next = prev`
3. Advance `prev = current`, `current = next`

Time: O(n), Space: O(1)

---

### Q5: When would you use a LinkedList over a List<T> in C#?

**Answer:** Use `LinkedList<T>` when:
- You frequently **insert/delete at the front** (O(1) vs O(n) for List)
- You have a reference to a node and need O(1) insertion before/after it
- Implementing a **deque** (double-ended queue)
- Implementing an **LRU cache** (most recently used moves to front)

Use `List<T>` when:
- You need **random access by index** (O(1))
- Cache performance matters (contiguous memory)
- Memory efficiency matters (no pointer overhead)

---

### Q6: How is C#'s LinkedList<T> different from a custom implementation?

**Answer:**
- C#'s `LinkedList<T>` is a **doubly linked list** — each node has `Previous` and `Next`
- It maintains both `First` (head) and `Last` (tail) references → O(1) add to either end
- `LinkedListNode<T>` exposes `List`, `Value`, `Next`, `Previous`
- `Remove(node)` is O(1) because it's doubly linked — it can update both neighbors
- A custom singly linked list `Remove` would be O(n) without a node reference

---

## Scenario-Based Questions

### Scenario 1: LRU Cache
> "Design an LRU (Least Recently Used) cache with O(1) get and put."

**Answer:** Combine a `Dictionary<K, LinkedListNode<V>>` with a `LinkedList<V>`:
- Dictionary gives O(1) lookup by key
- LinkedList front = most recently used; back = least recently used
- On `get`: find node via dictionary, move it to front — O(1)
- On `put`: add new node to front; if capacity exceeded, remove `Last` — O(1)
- This is the classic LRU cache — used in CPU caches, web CDN, browser history

---

### Scenario 2: Browser History
> "Model a browser's back and forward navigation."

**Answer:** Use a `LinkedList` where each node is a URL:
- `current` pointer tracks the current page
- **Back**: move `current = current.Previous`
- **Forward**: move `current = current.Next`
- **Visiting new page**: set `current.Next = newNode`, clear everything after it, advance current

---

### Scenario 3: Music Playlist
> "Implement a music player that supports: play next, play previous, add song, remove current song."

**Answer:** `LinkedList<Song>` where:
- `AddLast(song)` adds to end of playlist
- `RemoveCurrent()` removes current node — O(1) since you hold the node reference
- `current = current.Next` for next song
- `current = current.Previous` for previous song

---

## Common Mistakes to Avoid

1. **NullReferenceException**: Always check `if (node == null)` before accessing `.Next` or `.Data`
2. **Losing the head**: When deleting or reversing, ensure you don't lose your reference to the head before updating it
3. **Off-by-one in counting**: Count changes on every insert AND delete — easy to forget
4. **Infinite loop**: In circular lists, ensure your traversal has a termination condition
5. **Using LinkedList<T> for indexed access**: `list.ElementAt(5)` is O(n) — don't do it in a loop!

---

## Summary Table

| Feature | Singly Linked | Doubly Linked | Array | List<T> |
|---|---|---|---|---|
| Random access | O(n) | O(n) | O(1) | O(1) |
| Insert at front | O(1) | O(1) | O(n) | O(n) |
| Insert at back | O(n)* | O(1) | O(1) | O(1) |
| Delete at front | O(1) | O(1) | O(n) | O(n) |
| Delete (by ref) | O(n)** | O(1) | O(n) | O(n) |
| Memory per node | Data + 1 ptr | Data + 2 ptrs | Data only | Data only |
| Cache friendly | No | No | Yes | Yes |

*O(1) with tail pointer | **O(1) with doubly linked
