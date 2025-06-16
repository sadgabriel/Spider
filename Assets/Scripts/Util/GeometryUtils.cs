using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class GeometryUtils
{
    public static Vector2 ProjectOntoPlane2D(Vector3 point, Vector3 origin, Vector3 xAxis, Vector3 yAxis)
    {
        Vector3 relative = point - origin;
        float x = Vector3.Dot(relative, xAxis);
        float y = Vector3.Dot(relative, yAxis);
        return new Vector2(x, y);
    }

    public static bool DoIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        float ccw(Vector2 p, Vector2 q, Vector2 r) =>
            (q.x - p.x) * (r.y - p.y) - (q.y - p.y) * (r.x - p.x);

        return ccw(a, b, c) * ccw(a, b, d) < 0 &&
               ccw(c, d, a) * ccw(c, d, b) < 0;
    }
}