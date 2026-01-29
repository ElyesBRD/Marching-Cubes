using UnityEngine;

public class TerrainGeneration
{
    public static float GetTerrainHeight(Vector3 pos, float noiseStrenght,float yoffset)
    {
        var height = Mathf.PerlinNoise(pos.x * noiseStrenght, pos.z * noiseStrenght) * yoffset;
        return Mathf.Floor(height) == pos.y ? height : -1;
    }
}