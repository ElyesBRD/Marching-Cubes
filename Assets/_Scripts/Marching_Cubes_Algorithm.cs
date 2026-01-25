using System.Collections.Generic;
using UnityEngine;

public class Marching_Cubes_Algorithm
{
    public Vector3 pivot;
    public float voxelSize;
    Dictionary<Vector3, int> vertexIndices = new Dictionary<Vector3, int>();
    List<int> triangles = new List<int>();
    public void meshGeneration(float[,,] voxels, Vector3 pivot, float voxelSize, float isoLevel, List<Vector3> vertices, List<int> triangles)
    {
        this.pivot = pivot;
        this.voxelSize = voxelSize;
        for (int x = 0; x < voxels.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < voxels.GetLength(1) - 1; y++)
            {
                for (int z = 0; z < voxels.GetLength(2) - 1; z++)
                {
                    MarchCube(new Vector3Int(x, y, z), voxels, isoLevel);
                }
            }
        }
        vertices.AddRange(vertexIndices.Keys);
        triangles.AddRange(this.triangles);
    }
    void MarchCube(Vector3Int pos, float[,,] voxels, float isoLevel)
    {
        //summary
        // for each selected corner from 0 to 7, is essentially 76543210, now for each selected edge, we mark it as 1, otherwise 0,
        // wich leaves us with a binary number that is in base 2, that means, for example we select number 3, wich converts it into, 00001000, and that converts to 8 in base 10
        // that is why for each corner we do cubeIndex |= 1 << cornerIndex, to set the bit at cornerIndex to 1,
        //summary
        int cubeIndex = 0;
        if (voxels[pos.x, pos.y, pos.z] < isoLevel) cubeIndex |= 1;
        if (voxels[pos.x + 1, pos.y, pos.z] < isoLevel) cubeIndex |= 2;
        if (voxels[pos.x + 1, pos.y + 1, pos.z] < isoLevel) cubeIndex |= 4;
        if (voxels[pos.x, pos.y + 1, pos.z] < isoLevel) cubeIndex |= 8;
        if (voxels[pos.x, pos.y, pos.z + 1] < isoLevel) cubeIndex |= 16;
        if (voxels[pos.x + 1, pos.y, pos.z + 1] < isoLevel) cubeIndex |= 32;
        if (voxels[pos.x + 1, pos.y + 1, pos.z + 1] < isoLevel) cubeIndex |= 64;
        if (voxels[pos.x, pos.y + 1, pos.z + 1] < isoLevel) cubeIndex |= 128;

        //summary
        // create the vertices using the selected edges,
        // for each edge that is selected, we get the vertices of that edge and interpolate the between the two positions using the isolevel of each one,
        // again for each selected edge, we mark it as 1, otherwise 0, and we got 12 bits to represent the 12 edges of the cube, wich meants to convert it to base 10 again,
        // we will get a number between 1 and 2048, that will tell us which edges are selected, if its selected we create a vertex between the two corners of that edge.
        //summary
        int edges = Tables.edgeTable[cubeIndex];
        if (edges == 0)
            return;

        Vector3[] vertList = new Vector3[12];

        // all cases for edges 0-11 => 1-2048
        if ((edges & 1) != 0) vertList[0]=(VertexInterp(isoLevel, new Vector3(pos.x, pos.y, pos.z), new Vector3(pos.x + 1, pos.y, pos.z), voxels[pos.x, pos.y, pos.z], voxels[pos.x + 1, pos.y, pos.z]));
        if ((edges & 2) != 0) vertList[1]=(VertexInterp(isoLevel, new Vector3(pos.x + 1, pos.y, pos.z), new Vector3(pos.x + 1, pos.y + 1, pos.z), voxels[pos.x + 1, pos.y, pos.z], voxels[pos.x + 1, pos.y + 1, pos.z]));
        if ((edges & 4) != 0) vertList[2]=(VertexInterp(isoLevel, new Vector3(pos.x + 1, pos.y + 1, pos.z), new Vector3(pos.x, pos.y + 1, pos.z), voxels[pos.x + 1, pos.y + 1, pos.z], voxels[pos.x, pos.y + 1, pos.z]));
        if ((edges & 8) != 0) vertList[3]=(VertexInterp(isoLevel, new Vector3(pos.x, pos.y + 1, pos.z), new Vector3(pos.x, pos.y, pos.z), voxels[pos.x, pos.y + 1, pos.z], voxels[pos.x, pos.y, pos.z]));
        if ((edges & 16) != 0) vertList[4]=(VertexInterp(isoLevel, new Vector3(pos.x, pos.y, pos.z + 1), new Vector3(pos.x + 1, pos.y, pos.z + 1), voxels[pos.x, pos.y, pos.z + 1], voxels[pos.x + 1, pos.y, pos.z + 1]));
        if ((edges & 32) != 0) vertList[5]=(VertexInterp(isoLevel, new Vector3(pos.x + 1, pos.y, pos.z + 1), new Vector3(pos.x + 1, pos.y + 1, pos.z + 1), voxels[pos.x + 1, pos.y, pos.z + 1], voxels[pos.x + 1, pos.y + 1, pos.z + 1]));
        if ((edges & 64) != 0) vertList[6]=(VertexInterp(isoLevel, new Vector3(pos.x + 1, pos.y + 1, pos.z + 1), new Vector3(pos.x, pos.y + 1, pos.z + 1), voxels[pos.x + 1, pos.y + 1, pos.z + 1], voxels[pos.x, pos.y + 1, pos.z + 1]));
        if ((edges & 128) != 0) vertList[7]=(VertexInterp(isoLevel, new Vector3(pos.x, pos.y + 1, pos.z + 1), new Vector3(pos.x, pos.y, pos.z + 1), voxels[pos.x, pos.y + 1, pos.z + 1], voxels[pos.x, pos.y, pos.z + 1]));
        if ((edges & 256) != 0) vertList[8]=(VertexInterp(isoLevel, new Vector3(pos.x, pos.y, pos.z), new Vector3(pos.x, pos.y, pos.z + 1), voxels[pos.x, pos.y, pos.z], voxels[pos.x, pos.y, pos.z + 1]));
        if ((edges & 512) != 0) vertList[9]=(VertexInterp(isoLevel, new Vector3(pos.x + 1, pos.y, pos.z), new Vector3(pos.x + 1, pos.y, pos.z + 1), voxels[pos.x + 1, pos.y, pos.z], voxels[pos.x + 1, pos.y, pos.z + 1]));
        if ((edges & 1024) != 0) vertList[10]=(VertexInterp(isoLevel, new Vector3(pos.x + 1, pos.y + 1, pos.z), new Vector3(pos.x + 1, pos.y + 1, pos.z + 1), voxels[pos.x + 1, pos.y + 1, pos.z], voxels[pos.x + 1, pos.y + 1, pos.z + 1]));
        if ((edges & 2048) != 0) vertList[11]=(VertexInterp(isoLevel, new Vector3(pos.x, pos.y + 1, pos.z), new Vector3(pos.x, pos.y + 1, pos.z + 1), voxels[pos.x, pos.y + 1, pos.z], voxels[pos.x, pos.y + 1, pos.z + 1]));

        // now after creating the vertices, we must create the triangles using the triangle table, since each 3 vertices make up a triangle

        //summary
        // we got the cubeIndex that tells us wich edges are selected, which means now we do know wich vertices each traingle uses up in the vertList,
        // Tables.triangleTable[cubeIndex] gives us the necessary indecies that we must use from the vertList to create a triangle,
        // Tables.triangleTable[cubeIndex,i] eterate throughout the indecies until it encounters -1, wich means the end of the triangle list for that cubeIndex,
        // for each 3 indecies we create a triangle that we store for the mesh,
        // and we are essentially sorting the vertList into the trianglesList, so it can play both the vertices and the traingels for the mesh at the same time.
        //summary
        int currentIndex = 0;
        for (int i = 0; Tables.triangleTable[cubeIndex, i] != -1; i++)
        {
            currentIndex = Tables.triangleTable[cubeIndex, i];

            if (vertexIndices.TryGetValue(vertList[currentIndex], out int index))
            {
                // Vertex already exists, use its index
                triangles.Add(index);
            }
            else
            {
                // New vertex, add it to the dictionary
                vertexIndices[vertList[currentIndex]] = vertexIndices.Count;
                triangles.Add(vertexIndices.Count - 1);
            }
        }
    }
    Vector3 VertexInterp(float isoLevel, Vector3 p1, Vector3 p2, float valp1, float valp2)
    {
        p1 += pivot;
        p1 *= voxelSize;
        p2 += pivot;
        p2 *= voxelSize;
        const float EPS = 1e-6f;
        if (Mathf.Abs(isoLevel - valp1) < EPS)
            return p1;
        if (Mathf.Abs(isoLevel - valp2) < EPS)
            return p2;
        if (Mathf.Abs(valp1 - valp2) < EPS)
            return p1;
        float mu = (isoLevel - valp1) / (valp2 - valp1);
        return p1 + mu * (p2 - p1);
    }
}