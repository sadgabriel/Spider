using UnityEngine;
using System.Collections.Generic;

public static class PoissonSampler
{
    public static List<Vector2> GeneratePoissonPoints(int targetCount, float radius, (float, float) xRange, (float, float) yRange, int maxAttempts, List<Vector2> existingPoints = null)
    {
        List<Vector2> points = existingPoints != null ? new List<Vector2>(existingPoints) : new List<Vector2>();

        int attempts = 0;
        while (points.Count < targetCount && attempts < maxAttempts)
        {
            Vector2 candidate = new Vector2(Random.Range(xRange.Item1, xRange.Item2), Random.Range(yRange.Item1, yRange.Item2));

            bool valid = true;
            foreach (Vector2 p in points)
            {
                if (Vector2.Distance(candidate, p) < radius)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                points.Add(candidate);
            }

            attempts++;
        }

        return points;
    }

    public static List<Vector3> GeneratePoissonSpherePoints(int targetCount, float minAngleDegrees, int maxAttempts, List<Vector3> existingPoints = null)
    {
        List<Vector3> points = existingPoints != null ? new List<Vector3>(existingPoints) : new List<Vector3>();

        int attempts = 0;
        float minDot = Mathf.Cos(minAngleDegrees * Mathf.Deg2Rad);

        while (points.Count < targetCount && attempts < maxAttempts)
        {
            Vector3 candidate = Random.onUnitSphere;

            bool valid = true;
            foreach (var p in points)
            {
                if (Vector3.Dot(candidate, p) > minDot)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                points.Add(candidate);
            }

            attempts++;
        }

        return points;
    }
}