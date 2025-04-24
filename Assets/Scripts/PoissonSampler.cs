using UnityEngine;
using System.Collections.Generic;

public static class PoissonSampler
{
    public static List<Vector2> GeneratePoissonPoints(int count, float radius, (float, float) xRange, (float, float) yRange, int maxAttempts, List<Vector2> existingPoints = null)
    {
        if (existingPoints == null) existingPoints = new List<Vector2>();

        List<Vector2> points = new(existingPoints);

        int attempts = 0;
        while (points.Count < count && attempts < maxAttempts)
        {
            Vector2 candidate = new Vector2(
                Random.Range(xRange.Item1, xRange.Item2),
                Random.Range(yRange.Item1, yRange.Item2)
            );

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
}