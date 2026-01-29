using UnityEngine;

public class TerrainGeneration
{
    public static float GetTerrainHeight(Vector3 pos, float noiseStrenght,float yoffset)
    {
        var height = Mathf.PerlinNoise(pos.x * noiseStrenght, pos.z * noiseStrenght) * yoffset;
        return Mathf.Floor(height) == pos.y ? height : -1;
    }
    public static bool inSphere(Vector3 p, Vector3 center, float radius)
    {
        return (p - center).magnitude < radius;
    }
    public static float sphericalPlanetNoise(Vector3 voxelPos, Vector3 center, float radius, float noiseStrength, float noiseOffsetFromRadius)
    {
        var fr = radius - Vector3.Distance(voxelPos, center);
        return fr + Perlin.Perlin3D(voxelPos * noiseStrength) * noiseOffsetFromRadius;
    }
}