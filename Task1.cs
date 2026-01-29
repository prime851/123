using System;
using System.Collections.Generic;

namespace SchoolNTTasks
{
    public static class CircularArrayProcessor
    {
        public static string ProcessCircularArrays(int n1, int m1, int n2, int m2)
        {
            var path1 = GeneratePath(n1, m1);
            var path2 = GeneratePath(n2, m2);
            
            return $"{string.Join("", path1)}{string.Join("", path2)}";
        }

        private static List<int> GeneratePath(int n, int step)
        {
            var visited = new HashSet<int>();
            var result = new List<int>();
            int current = 1;
            
            while (true)
            {
                result.Add(current);
                visited.Add(current);
                
                int next = current + step - 1;
                while (next > n) next -= n;
                
                if (next == 1) break;
                if (visited.Contains(next) && next != 1) 
                {
                    result.Add(next);
                    break;
                }
                
                current = next;
            }
            
            return result;
        }
    }
}
