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
    public static float sphericalPlanetNoise(Vector3 voxelPos, Vector3 center, float radius, float noiseDensity, float noiseStrength, float minDistanceFromSurface)
    {
        var height = Vector3.Distance(voxelPos, center);
        if (height <= radius) return 1;
        if (height > radius + minDistanceFromSurface) return 0;
        var noiseValue = Perlin.Perlin3D((voxelPos) * noiseDensity) * noiseStrength;
        return noiseValue;
    }
}