using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class GeometryUtils
{
    // Convex Hull 생성: Andrew's Monotone Chain 알고리즘 (O(n log n))
    public static List<Vector2> ComputeConvexHull(List<Vector2> points)
    {
        points = points.Distinct(new Vector2Comparer()).ToList();
        if (points.Count <= 3) return new List<Vector2>(points);

        points.Sort((a, b) =>
            a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));

        List<Vector2> lower = new();
        foreach (var p in points)
        {
            while (lower.Count >= 2 &&
                   Cross(lower[^2], lower[^1], p) <= 0)
                lower.RemoveAt(lower.Count - 1);
            lower.Add(p);
        }

        List<Vector2> upper = new();
        for (int i = points.Count - 1; i >= 0; i--)
        {
            var p = points[i];
            while (upper.Count >= 2 &&
                   Cross(upper[^2], upper[^1], p) <= 0)
                upper.RemoveAt(upper.Count - 1);
            upper.Add(p);
        }

        lower.RemoveAt(lower.Count - 1);
        upper.RemoveAt(upper.Count - 1);
        lower.AddRange(upper);

        return lower;
    }

    private static float Cross(Vector2 a, Vector2 b, Vector2 c)
    {
        return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
    }

    // Convex Hull을 Segment constraint 목록으로 변환
    public static List<(Vector2, Vector2)> GetHullEdges(List<Vector2> convexHull)
    {
        List<(Vector2, Vector2)> edges = new();
        for (int i = 0; i < convexHull.Count; i++)
        {
            var a = convexHull[i];
            var b = convexHull[(i + 1) % convexHull.Count];
            edges.Add((a, b));
        }
        return edges;
    }
}