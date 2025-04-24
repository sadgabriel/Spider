using System.Collections.Generic;
using UnityEngine;

public class Vector2Comparer : IEqualityComparer<Vector2>
    {
        public bool Equals(Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b) < 1e-5f;
        }

        public int GetHashCode(Vector2 v)
        {
            return v.x.GetHashCode() ^ v.y.GetHashCode();
        }
    }