// ============================================================
// PHASE 24 — PROJECT 3: CONTACT MANAGER
// Demonstrates: Dictionary<K,V> for fast CRUD operations,
//               LINQ for querying, sorting, filtering
// Features: Add, Update, Delete, Search, Group, Sort contacts
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;

record Contact(
    string Id,        // unique ID (phone number or UUID)
    string FirstName,
    string LastName,
    string Phone,
    string Email,
    string Group     // "Family", "Work", "Friends"
)
{
    public string FullName => $"{FirstName} {LastName}";
    public override string ToString() =>
        $"  [{Group,8}] {FullName,-20} | {Phone,-15} | {Email}";
}

class ContactManager
{
    // Dictionary<phone, Contact> → O(1) add/update/delete/lookup by phone
    private Dictionary<string, Contact> _contacts = new Dictionary<string, Contact>();

    // ADD or UPDATE contact
    public bool AddOrUpdate(Contact contact)
    {
        bool isUpdate = _contacts.ContainsKey(contact.Id);
        _contacts[contact.Id] = contact;
        Console.WriteLine(isUpdate ? $"  Updated: {contact.FullName}" : $"  Added: {contact.FullName}");
        return !isUpdate;
    }

    // FIND by exact phone (O(1) dictionary lookup)
    public Contact? FindById(string id)
    {
        _contacts.TryGetValue(id, out var contact);
        return contact;
    }

    // SEARCH by name (partial, case-insensitive) — O(n) scan
    public List<Contact> SearchByName(string query)
    {
        return _contacts.Values
            .Where(c => c.FullName.Contains(query, StringComparison.OrdinalIgnoreCase)
                     || c.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase)
                     || c.LastName.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.LastName)
            .ToList();
    }

    // DELETE contact
    public bool Delete(string id)
    {
        if (_contacts.Remove(id, out var removed))
        {
            Console.WriteLine($"  Deleted: {removed.FullName}");
            return true;
        }
        Console.WriteLine($"  Not found: {id}");
        return false;
    }

    // LIST ALL sorted by last name
    public void ListAll()
    {
        Console.WriteLine($"\n  All Contacts ({_contacts.Count} total):");
        var sorted = _contacts.Values.OrderBy(c => c.LastName).ThenBy(c => c.FirstName);
        foreach (var c in sorted) Console.WriteLine(c);
    }

    // GROUP by category — Dictionary<group, List<Contact>>
    public void ShowByGroup()
    {
        Console.WriteLine("\n  Contacts by Group:");
        var groups = _contacts.Values
            .GroupBy(c => c.Group)
            .OrderBy(g => g.Key);

        foreach (var group in groups)
        {
            Console.WriteLine($"\n  [{group.Key}] ({group.Count()})");
            foreach (var c in group.OrderBy(c => c.LastName))
                Console.WriteLine($"    {c.FullName,-20} | {c.Phone}");
        }
    }

    // STATISTICS
    public void ShowStats()
    {
        Console.WriteLine("\n  Contact Statistics:");
        Console.WriteLine($"    Total contacts:  {_contacts.Count}");

        var byGroup = _contacts.Values.GroupBy(c => c.Group)
                                      .Select(g => $"{g.Key}: {g.Count()}")
                                      .OrderBy(s => s);
        Console.WriteLine($"    By group:        {string.Join(", ", byGroup)}");

        var topDomain = _contacts.Values
            .GroupBy(c => c.Email.Split('@').LastOrDefault() ?? "unknown")
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();
        if (topDomain != null)
            Console.WriteLine($"    Top email domain: @{topDomain.Key} ({topDomain.Count()} contacts)");
    }
}

class ContactManagerProject
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PROJECT 3: CONTACT MANAGER (Dictionary + LINQ)");
        Console.WriteLine("======================================================\n");

        var manager = new ContactManager();

        // Seed data
        var contacts = new[]
        {
            new Contact("111-0001", "Alice",   "Smith",   "555-001", "alice@gmail.com",    "Work"),
            new Contact("111-0002", "Bob",     "Johnson", "555-002", "bob@company.com",    "Work"),
            new Contact("111-0003", "Carol",   "Williams","555-003", "carol@gmail.com",    "Friends"),
            new Contact("111-0004", "Dave",    "Brown",   "555-004", "dave@family.net",    "Family"),
            new Contact("111-0005", "Eve",     "Jones",   "555-005", "eve@gmail.com",      "Friends"),
            new Contact("111-0006", "Frank",   "Smith",   "555-006", "frank@company.com",  "Work"),
            new Contact("111-0007", "Grace",   "Davis",   "555-007", "grace@family.net",   "Family"),
        };

        foreach (var c in contacts) manager.AddOrUpdate(c);

        manager.ListAll();

        Console.WriteLine("\n--- Search for 'Smith' ---");
        var results = manager.SearchByName("Smith");
        foreach (var c in results) Console.WriteLine(c);

        Console.WriteLine("\n--- Find by ID '111-0003' ---");
        var found = manager.FindById("111-0003");
        Console.WriteLine(found is not null ? found.ToString() : "  Not found");

        Console.WriteLine("\n--- Update Bob's email ---");
        var bob = manager.FindById("111-0002")!;
        manager.AddOrUpdate(bob with { Email = "bob@newcompany.com" });

        Console.WriteLine("\n--- Delete Frank ---");
        manager.Delete("111-0006");

        manager.ShowByGroup();
        manager.ShowStats();

        Console.WriteLine("\n--- Why Dictionary? ---");
        Console.WriteLine("  O(1) add/update/delete/lookup by key (phone/ID)");
        Console.WriteLine("  O(n) for search-by-name (full scan required without secondary index)");
        Console.WriteLine("  LINQ adds powerful in-memory querying, grouping, and sorting");
    }
}
