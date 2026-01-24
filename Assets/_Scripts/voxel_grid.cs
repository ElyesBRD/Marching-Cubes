using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class voxel_grid : MonoBehaviour
{
    public static voxel_grid instance;

    public int gridSizeX = 10;
    public int gridSizeY = 10;
    public int gridSizeZ = 10;
    public float voxelSize = 1.0f;
    public float isoLevel = 0.5f;
    private float[,,] voxels;
    public Transform pivotT;
    public Transform[] centerOfSphereT;

    MeshFilter meshFilter;
    [SerializeField] Mesh mesh;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        voxels = new float[gridSizeX, gridSizeY, gridSizeZ];
        updateVoxel();

        meshFilter = GetComponent<MeshFilter>();
        UpdateMesh();
    }
    void UpdateMesh()
    {
        Marching_Cubes_Algorithm mc = new Marching_Cubes_Algorithm();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        mc.meshGeneration(voxels, pivotT.position, voxelSize, isoLevel, vertices, triangles);

        mesh = new Mesh();

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }
    private void Update()
    {
        updateVoxel();
        UpdateMesh();
    }
    void updateVoxel()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    voxels[x, y, z] = voxelValue((new Vector3(x, y, z) + pivotT.position) * voxelSize);
                }
            }
        }
    }
    public float radius = 3.0f;
    float voxelValue(Vector3 voxelPos)
    {
        for (int i = 0; i < centerOfSphereT.Length; i++)
        {
            if (inSphere(voxelPos, centerOfSphereT[i].position, radius)) return 1;
        }
        return 0;
    }
    bool inSphere(Vector3 p, Vector3 center, float radius)
    {
        return (p - center).magnitude < radius;
    }
    float sphereR = 0.1f;
    public bool canDraw = true;
    void OnDrawGizmos()
    {
        if (!canDraw) return;
        if (voxels == null) return;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 pos = new Vector3(x * voxelSize, y * voxelSize, z * voxelSize);

                    Gizmos.color = voxels[x, y, z] < isoLevel ? Color.black : Color.white;

                    Gizmos.DrawSphere(pos, sphereR);
                }
            }
        }
    }
}