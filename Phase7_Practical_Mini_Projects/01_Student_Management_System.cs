using System;
using System.Collections.Generic;
using System.Linq;

namespace Phase7_Practical_Mini_Projects
{
    internal class StudentManagementSystem
    {
        private static readonly StudentRepository _repository = new();

        private static void Main()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Student Management");
                Console.WriteLine("1. Add student");
                Console.WriteLine("2. List students");
                Console.WriteLine("3. Search student");
                Console.WriteLine("4. Update student");
                Console.WriteLine("5. Delete student");
                Console.WriteLine("0. Exit");
                Console.Write("Select: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddStudent(); break;
                    case "2": ListStudents(); break;
                    case "3": SearchStudent(); break;
                    case "4": UpdateStudent(); break;
                    case "5": DeleteStudent(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        private static void AddStudent()
        {
            try
            {
                Console.Write("Name: ");
                string name = Console.ReadLine()?.Trim() ?? string.Empty;

                Console.Write("Age: ");
                int age = int.Parse(Console.ReadLine() ?? "0");

                var student = new Student(Guid.NewGuid(), name, age);
                _repository.Add(student);
                Console.WriteLine("Student added.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void ListStudents()
        {
            var students = _repository.GetAll().OrderBy(s => s.Name).ToList();
            Console.WriteLine($"{students.Count} students:");
            foreach (var student in students)
            {
                Console.WriteLine(student);
            }
        }

        private static void SearchStudent()
        {
            Console.Write("Search term (name or id): ");
            string term = Console.ReadLine()?.Trim() ?? string.Empty;

            var matches = _repository.Search(term).ToList();
            if (!matches.Any())
            {
                Console.WriteLine("No students found.");
                return;
            }

            Console.WriteLine($"Found {matches.Count} student(s):");
            foreach (var student in matches)
            {
                Console.WriteLine(student);
            }
        }

        private static void UpdateStudent()
        {
            try
            {
                Console.Write("Student Id: ");
                Guid id = Guid.Parse(Console.ReadLine() ?? string.Empty);

                var student = _repository.Get(id);
                if (student == null)
                {
                    Console.WriteLine("Student not found.");
                    return;
                }

                Console.Write("New name (leave empty to keep): ");
                string name = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(name))
                    student.Name = name.Trim();

                Console.Write("New age (leave empty to keep): ");
                string ageInput = Console.ReadLine() ?? string.Empty;
                if (int.TryParse(ageInput, out int age))
                    student.Age = age;

                _repository.Update(student);
                Console.WriteLine("Student updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void DeleteStudent()
        {
            try
            {
                Console.Write("Student Id: ");
                Guid id = Guid.Parse(Console.ReadLine() ?? string.Empty);
                _repository.Delete(id);
                Console.WriteLine("Student deleted (if existed).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private class Student
        {
            public Guid Id { get; }
            public string Name { get; set; }
            public int Age { get; set; }

            public Student(Guid id, string name, int age)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Name is required.", nameof(name));
                if (age <= 0)
                    throw new ArgumentOutOfRangeException(nameof(age), "Age must be positive.");

                Id = id;
                Name = name;
                Age = age;
            }

            public override string ToString() => $"{Id} - {Name} (Age {Age})";
        }

        private class StudentRepository
        {
            private readonly Dictionary<Guid, Student> _store = new();

            public void Add(Student student)
            {
                _store[student.Id] = student;
            }

            public Student? Get(Guid id) => _store.TryGetValue(id, out var student) ? student : null;

            public IEnumerable<Student> GetAll() => _store.Values;

            public IEnumerable<Student> Search(string term)
            {
                if (Guid.TryParse(term, out var id))
                {
                    var student = Get(id);
                    if (student != null)
                        yield return student;
                }

                foreach (var student in _store.Values)
                {
                    if (student.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                        yield return student;
                }
            }

            public void Update(Student student)
            {
                if (!_store.ContainsKey(student.Id))
                    throw new KeyNotFoundException("Student not found.");

                _store[student.Id] = student;
            }

            public void Delete(Guid id)
            {
                _store.Remove(id);
            }
        }
    }
}
