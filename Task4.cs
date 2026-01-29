using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SchoolNTTasks
{
    public static class ArrayMovesCalculator
    {
        public static string CalculateMinMoves(string filePath)
        {
            var numbers = LoadNumbers(filePath);
            
            // Используем медиану для минимального количества ходов
            int median = FindOptimalTarget(numbers);
            int moves = CalculateRequiredMoves(numbers, median);
            
            return moves <= 20 
                ? moves.ToString() 
                : "20 ходов недостаточно для приведения всех элементов массива к одному числу";
        }

        private static List<int> LoadNumbers(string path)
        {
            return File.ReadAllLines(path)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(int.Parse)
                .ToList();
        }

        private static int FindOptimalTarget(List<int> numbers)
        {
            var sorted = numbers.OrderBy(x => x).ToList();
            return sorted[sorted.Count / 2];
        }

        private static int CalculateRequiredMoves(List<int> numbers, int target)
        {
            return numbers.Sum(x => Math.Abs(x - target));
        }
    }
}
