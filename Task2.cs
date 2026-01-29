using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolNTTasks
{
    public static class EllipseAnalyzer
    {
        public static void AnalyzePoints(string ellipsePath, string pointsPath)
        {
            var (center, radiusX, radiusY) = ReadEllipse(ellipsePath);
            var points = ReadPoints(pointsPath);
            
            foreach (var point in points)
            {
                Console.WriteLine(ClassifyPoint(point, center, radiusX, radiusY));
            }
        }

        private static ((double X, double Y), double, double) ReadEllipse(string path)
        {
            var lines = File.ReadAllLines(path);
            if (lines.Length < 2) throw new FormatException("Invalid ellipse file format");
            
            var centerParts = lines[0].Split();
            var radiusParts = lines[1].Split();
            
            return (
                (double.Parse(centerParts[0]), double.Parse(centerParts[1])),
                double.Parse(radiusParts[0]),
                double.Parse(radiusParts[1])
            );
        }

        private static IEnumerable<(double X, double Y)> ReadPoints(string path)
        {
            foreach (var line in File.ReadAllLines(path))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                var parts = line.Split();
                if (parts.Length < 2) continue;
                
                yield return (double.Parse(parts[0]), double.Parse(parts[1]));
            }
        }

        private static int ClassifyPoint((double X, double Y) point, 
            (double X, double Y) center, double rx, double ry)
        {
            double dx = point.X - center.X;
            double dy = point.Y - center.Y;
            
            double value = (dx * dx) / (rx * rx) + (dy * dy) / (ry * ry);
            const double epsilon = 1e-12;
            
            if (Math.Abs(value - 1.0) < epsilon) return 0;
            return value < 1.0 ? 1 : 2;
        }
    }
}
