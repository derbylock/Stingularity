using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class StingExtensions
{
    private static System.Random random;
    public static float Rand {
        get { 
            if (random == null)
                random = new System.Random(new System.DateTime().Millisecond);
            return (float)(random.NextDouble());
        }
    }

    public static float ToAngle(this Vector2 v) {
        float angle = Vector2.Angle(Vector2.up, v);
        if (v.x < 0) {
            angle = 360 - angle;
        }
        return angle;
    }
    public static Quaternion ToQuaternion(this Vector2 v) {
        Vector3 euler = new Vector3(0, 0, -v.ToAngle());
        return Quaternion.Euler(euler);
    }

    public static Vector2 Abs(this Vector2 v) {
        return new Vector2(Math.Abs(v.x), Math.Abs(v.y));
    }

    public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action) {
        var i = 0;
        foreach ( var e in ie ) action( e, i++ );
    }

    public static Vector3 convertXYToXZ(this Vector3 v) {
        return new Vector3(v.x, 0, v.y);
    }

    public static Vector3 convertXZToXY(this Vector3 v) {
        return new Vector3(v.x, v.z, 0);
    }
}
