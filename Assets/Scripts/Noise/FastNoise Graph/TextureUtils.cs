using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class TextureUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetIndexFrom2d(Vector2Int coords, Vector2Int size)
    {
        return GetIndexFrom2d(coords.x, coords.y, size.x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetIndexFrom2d(int x, int y, int sizeX)
    {
        return y * sizeX + x;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Normalize(float value)
    {
        return (value + 1f) * 0.5f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Denormalize(float value)
    {
        return (value * 2f) - 1f;
    }
}
