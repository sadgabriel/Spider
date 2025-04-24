using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Topology;
using UnityEngine;

public static class TriangleNetWrapper
{
    public static List<(Vector2, Vector2)> GenerateDelaunayEdges(List<Vector2> points)
    {
        var mesh = Triangulate(points, constraints: null);
        return ExtractEdgesFromMesh(mesh);
    }

    public static List<(Vector2, Vector2)> GenerateConstrainedDelaunayEdges(List<Vector2> points, List<(Vector2, Vector2)> constraints)
    {
        var mesh = Triangulate(points, constraints);
        return ExtractEdgesFromMesh(mesh);
    }

    private static IMesh Triangulate(List<Vector2> points, List<(Vector2, Vector2)> constraints)
    {
        var polygon = new Polygon();
        var pointIndexMap = new Dictionary<Vector2, int>(new Vector2Comparer());

        for (int i = 0; i < points.Count; i++)
        {
            var p = points[i];
            polygon.Add(new Vertex(p.x, p.y));
            pointIndexMap[p] = i;
        }

        if (constraints != null)
        {
            foreach (var (a, b) in constraints)
            {
                if (!pointIndexMap.ContainsKey(a) || !pointIndexMap.ContainsKey(b))
                {
                    Debug.LogWarning($"Constraint endpoints not in point list: {a} -> {b}");
                    continue;
                }

                polygon.Add(new Segment(
                    polygon.Points[pointIndexMap[a]],
                    polygon.Points[pointIndexMap[b]]
                ));
            }
        }

        return polygon.Triangulate();
    }

    private static List<(Vector2, Vector2)> ExtractEdgesFromMesh(IMesh mesh)
    {
        var edges = new List<(Vector2, Vector2)>();
        var seenEdges = new HashSet<(int, int)>();

        foreach (var triangle in mesh.Triangles)
        {
            var vertices = new[]
            {
                triangle.GetVertex(0),
                triangle.GetVertex(1),
                triangle.GetVertex(2)
            };

            for (int i = 0; i < 3; i++)
            {
                var a = vertices[i];
                var b = vertices[(i + 1) % 3];

                int id1 = Mathf.Min(a.ID, b.ID);
                int id2 = Mathf.Max(a.ID, b.ID);

                if (seenEdges.Add((id1, id2)))
                {
                    var pointA = new Vector2((float)a.X, (float)a.Y);
                    var pointB = new Vector2((float)b.X, (float)b.Y);
                    edges.Add((pointA, pointB));
                }
            }
        }

        return edges;
    }
}
