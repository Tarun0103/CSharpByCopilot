using System;
using System.Collections.Generic;
using System.Linq;

namespace Phase7_Practical_Mini_Projects
{
    internal class EmployeeCrudApp
    {
        private static readonly EmployeeRepository _repository = new();

        private static void Main()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Employee CRUD");
                Console.WriteLine("1. Add employee");
                Console.WriteLine("2. List employees");
                Console.WriteLine("3. Search employee");
                Console.WriteLine("4. Update employee");
                Console.WriteLine("5. Delete employee");
                Console.WriteLine("0. Exit");
                Console.Write("Select: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddEmployee(); break;
                    case "2": ListEmployees(); break;
                    case "3": SearchEmployee(); break;
                    case "4": UpdateEmployee(); break;
                    case "5": DeleteEmployee(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private static void AddEmployee()
        {
            try
            {
                Console.Write("Name: ");
                string name = Console.ReadLine()?.Trim() ?? string.Empty;
                Console.Write("Department: ");
                string department = Console.ReadLine()?.Trim() ?? string.Empty;
                Console.Write("Salary: ");
                decimal salary = decimal.Parse(Console.ReadLine() ?? "0");

                var employee = new Employee(Guid.NewGuid(), name, department, salary);
                _repository.Add(employee);
                Console.WriteLine("Employee added.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void ListEmployees()
        {
            var employees = _repository.GetAll().OrderBy(e => e.Name).ToList();
            Console.WriteLine($"{employees.Count} employees:");
            foreach (var employee in employees)
            {
                Console.WriteLine(employee);
            }
        }

        private static void SearchEmployee()
        {
            Console.Write("Search term (name/department/id): ");
            string term = Console.ReadLine()?.Trim() ?? string.Empty;

            var matches = _repository.Search(term).ToList();
            if (!matches.Any())
            {
                Console.WriteLine("No employees found.");
                return;
            }

            Console.WriteLine($"Found {matches.Count} employee(s):");
            foreach (var employee in matches)
            {
                Console.WriteLine(employee);
            }
        }

        private static void UpdateEmployee()
        {
            try
            {
                Console.Write("Employee Id: ");
                Guid id = Guid.Parse(Console.ReadLine() ?? string.Empty);

                var employee = _repository.Get(id);
                if (employee == null)
                {
                    Console.WriteLine("Employee not found.");
                    return;
                }

                Console.Write("New name (leave empty to keep): ");
                string name = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(name))
                    employee.Name = name.Trim();

                Console.Write("New department (leave empty to keep): ");
                string dept = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(dept))
                    employee.Department = dept.Trim();

                Console.Write("New salary (leave empty to keep): ");
                string salaryInput = Console.ReadLine() ?? string.Empty;
                if (decimal.TryParse(salaryInput, out var salary))
                    employee.Salary = salary;

                _repository.Update(employee);
                Console.WriteLine("Employee updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void DeleteEmployee()
        {
            try
            {
                Console.Write("Employee Id: ");
                Guid id = Guid.Parse(Console.ReadLine() ?? string.Empty);
                _repository.Delete(id);
                Console.WriteLine("Employee deleted (if existed).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private class Employee
        {
            public Guid Id { get; }
            public string Name { get; set; }
            public string Department { get; set; }
            public decimal Salary { get; set; }

            public Employee(Guid id, string name, string department, decimal salary)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Name is required.", nameof(name));
                if (string.IsNullOrWhiteSpace(department))
                    throw new ArgumentException("Department is required.", nameof(department));
                if (salary < 0)
                    throw new ArgumentOutOfRangeException(nameof(salary), "Salary must be non-negative.");

                Id = id;
                Name = name;
                Department = department;
                Salary = salary;
            }

            public override string ToString() => $"{Id} - {Name} ({Department}) ${Salary:F2}";
        }

        private class EmployeeRepository
        {
            private readonly Dictionary<Guid, Employee> _store = new();

            public void Add(Employee employee)
            {
                _store[employee.Id] = employee;
            }

            public Employee? Get(Guid id) => _store.TryGetValue(id, out var employee) ? employee : null;

            public IReadOnlyCollection<Employee> GetAll() => _store.Values.ToList();

            public IEnumerable<Employee> Search(string term)
            {
                if (Guid.TryParse(term, out var id))
                {
                    var employee = Get(id);
                    if (employee != null)
                        yield return employee;
                }

                foreach (var employee in _store.Values)
                {
                    if (employee.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        employee.Department.Contains(term, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return employee;
                    }
                }
            }

            public void Update(Employee employee)
            {
                if (!_store.ContainsKey(employee.Id))
                    throw new KeyNotFoundException("Employee not found.");

                _store[employee.Id] = employee;
            }

            public void Delete(Guid id)
            {
                _store.Remove(id);
            }
        }
    }
}
