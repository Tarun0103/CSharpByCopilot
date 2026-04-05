# 6. SOLID Principles

SOLID is a set of design principles that help build maintainable, testable, and extensible systems.

## S - Single Responsibility Principle (SRP)
A class should have only one reason to change.

**Example**: Separate the responsibility of managing orders from persistence.

## O - Open/Closed Principle (OCP)
Software entities should be open for extension but closed for modification.

**Example**: Use abstractions (interfaces) so you can add new behavior without modifying existing code.

## L - Liskov Substitution Principle (LSP)
Derived classes should be substitutable for their base classes.

**Example**: If `Square` inherits `Rectangle`, it should not break expectations of `Rectangle`.

## I - Interface Segregation Principle (ISP)
Clients should not be forced to depend on interfaces they do not use.

**Example**: Break a large interface into smaller, focused interfaces.

## D - Dependency Inversion Principle (DIP)
High-level modules should not depend on low-level modules; both should depend on abstractions.

**Example**: Depends on `IRepository<T>` instead of `SqlRepository<T>`.

---

## ✅ Interview Prep (SOLID)
- Give real-world examples of each principle (e.g., order processing, payment providers, logging).
- Explain how DIP enables dependency injection.
- Talk through a refactor where an interface is introduced to comply with OCP.
