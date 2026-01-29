using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace SchoolNTTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  For Task 1: program.exe n1 m1 n2 m2");
                Console.WriteLine("  For Task 2: program.exe ellipse_file.txt points_file.txt");
                Console.WriteLine("  For Task 3: program.exe values.json tests.json report.json");
                Console.WriteLine("  For Task 4: program.exe numbers_file.txt");
                return;
            }

            try
            {
                switch (args.Length)
                {
                    case 4:
                        Task1(args);
                        break;
                    case 2 when args[0].EndsWith(".txt") && args[1].EndsWith(".txt"):
                        Task2(args[0], args[1]);
                        break;
                    case 3 when args[0].EndsWith(".json"):
                        Task3(args[0], args[1], args[2]);
                        break;
                    case 1 when args[0].EndsWith(".txt"):
                        Task4(args[0]);
                        break;
                    default:
                        Console.WriteLine("Invalid arguments");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Задание 1:
        static void Task1(string[] args)
        {
            if (args.Length < 4) throw new ArgumentException("Need 4 arguments for Task 1");
            
            var params1 = (int.Parse(args[0]), int.Parse(args[1]));
            var params2 = (int.Parse(args[2]), int.Parse(args[3]));
            
            var path1 = GenerateCircularPath(params1.Item1, params1.Item2);
            var path2 = GenerateCircularPath(params2.Item1, params2.Item2);
            
            Console.WriteLine($"{string.Join("", path1)}{string.Join("", path2)}");
        }

        private static List<int> GenerateCircularPath(int n, int m)
        {
            var result = new List<int>();
            var current = 1;
            
            do
            {
                result.Add(current);
                current = (current + m - 1) % n;
                if (current == 0) current = n;
            } while (result[0] != current);
            
            result.RemoveAt(result.Count - 1);
            return result;
        }

        // Задание 2: 
        static void Task2(string ellipseFile, string pointsFile)
        {
            var ellipseData = File.ReadAllLines(ellipseFile);
            var pointsData = File.ReadAllLines(pointsFile);
            
            if (ellipseData.Length < 2) throw new InvalidDataException("Invalid ellipse file");
            
            var center = ellipseData[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var radii = ellipseData[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            var (cx, cy) = (double.Parse(center[0]), double.Parse(center[1]));
            var (rx, ry) = (double.Parse(radii[0]), double.Parse(radii[1]));
            
            var points = pointsData
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Select(parts => (x: double.Parse(parts[0]), y: double.Parse(parts[1])))
                .ToList();

            if (points.Count > 100) throw new ArgumentException("Too many points");
            
            foreach (var (x, y) in points)
            {
                var normalized = Math.Pow((x - cx) / rx, 2) + Math.Pow((y - cy) / ry, 2);
                var epsilon = 1e-10;
                
                if (Math.Abs(normalized - 1.0) < epsilon)
                    Console.WriteLine(0);
                else if (normalized < 1.0)
                    Console.WriteLine(1);
                else
                    Console.WriteLine(2);
            }
        }

        // Задание 3: 
        static void Task3(string valuesFile, string testsFile, string reportFile)
        {
            var valuesJson = File.ReadAllText(valuesFile);
            var testsJson = File.ReadAllText(testsFile);
            
            var valuesDoc = JsonDocument.Parse(valuesJson);
            var testsDoc = JsonDocument.Parse(testsJson);
            
            var valueDict = valuesDoc.RootElement
                .GetProperty("values")
                .EnumerateArray()
                .ToDictionary(
                    e => e.GetProperty("id").GetInt32(),
                    e => e.GetProperty("value").GetString()
                );

            var report = FillValues(testsDoc.RootElement, valueDict);
            
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(report, options);
            
            File.WriteAllText(reportFile, json);
        }

        private static Dictionary<string, object> FillValues(JsonElement element, 
            Dictionary<int, string> valueDict)
        {
            var result = new Dictionary<string, object>();
            
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.Name == "id" && prop.Value.ValueKind == JsonValueKind.Number)
                {
                    result[prop.Name] = prop.Value.GetInt32();
                    
                    if (valueDict.TryGetValue(prop.Value.GetInt32(), out var value))
                    {
                        result["value"] = value;
                    }
                }
                else if (prop.Name == "values" && prop.Value.ValueKind == JsonValueKind.Array)
                {
                    var filledValues = prop.Value.EnumerateArray()
                        .Select(e => FillValues(e, valueDict))
                        .ToList();
                    result[prop.Name] = filledValues;
                }
                else if (prop.Value.ValueKind == JsonValueKind.String)
                {
                    result[prop.Name] = prop.Value.GetString();
                }
                else if (prop.Value.ValueKind == JsonValueKind.Number)
                {
                    result[prop.Name] = prop.Value.GetInt32();
                }
                else
                {
                    result[prop.Name] = prop.Value.ToString();
                }
            }
            
            return result;
        }

        // Задание 4: 
        static void Task4(string filePath)
        {
            var numbers = File.ReadAllLines(filePath)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(int.Parse)
                .ToList();
            
            if (numbers.Count == 0) throw new InvalidDataException("File is empty");
            
            var sorted = numbers.OrderBy(x => x).ToList();
            var median = sorted[sorted.Count / 2];
            
            var moves = sorted.Sum(x => Math.Abs(x - median));
            
            if (moves > 20)
            {
                Console.WriteLine("20 ходов недостаточно для приведения всех элементов массива к одному числу");
            }
            else
            {
                Console.WriteLine(moves);
            }
        }
    }
}
