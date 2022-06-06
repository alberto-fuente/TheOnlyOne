using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;
    public Gradient gradient;
    public Gradient[] gradientOptions;
    private float minTerrainHeight;
    private float maxTerrainHeight;
    public int xSize;//ancho
    public int zSize;//largo
    public float scale;
    public float noise1Scale = 2f;
    public float noise1Frec = 2f;
    public float noise2Scale = 4f;
    public float noise2Frec = 4f;
    public float noise3Scale = 6f;
    public float noise3Frec = 6f;
    public float noiseStrength;
    public float xOffset;
    public float zOffset;
    public int seed;

    void Awake()
    {
        gradient = gradientOptions[Random.Range(0, gradientOptions.Length)];
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateMap(xSize, xSize);
        UpdateMesh();
        GetComponent<MeshCollider>().sharedMesh = mesh;

    }
    /*private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            CreateMap(xSize, xSize);
            UpdateMesh();
        }
    }*/
    private void CreateMap(int xSize, int zSize)
    {
        GenerateRandomValues(Random.Range(0,100));
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = GetPerlin(x, z);

                vertices[i] = new Vector3(x, y, z);

                if (y > maxTerrainHeight)
                {
                    maxTerrainHeight = y;
                }
                if (y < minTerrainHeight)
                {
                    minTerrainHeight = y;
                }
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);//altura del vertice normalizada(entre 0 y 1)
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    private void GenerateRandomValues(int seed)
    {
        System.Random prng = new System.Random(seed);
        xOffset = prng.Next(-100000, 100000);
        zOffset = prng.Next(-100000, 100000);
        noise1Scale = Random.Range(0f, 15f);
        noise1Frec = Random.Range(0f, 0.03f);

        noise2Scale = Random.Range(0f, 5f);
        noise2Frec = Random.Range(0f, 0.07f);

        noise3Scale = Random.Range(0f, 0.1f);
        noise3Frec = Random.Range(0f, 0.02f);

        noiseStrength = Random.Range(1f, 3f);

    }

    // Update is called once per frame
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }
    private float GetPerlin(float x, float z)
    {
        float noise = noise1Scale * Mathf.PerlinNoise(x * noise1Frec + xOffset, z * noise1Frec + zOffset)
                    + noise2Scale * Mathf.PerlinNoise(x * noise2Frec + xOffset, z * noise2Frec + zOffset)
                    + noise3Scale * Mathf.PerlinNoise(x * noise3Frec + xOffset, z * noise3Frec + zOffset)
                    * noiseStrength;
        return noise;
    }

    /*private void OnDrawGizmos()
    {
        if (vertices != null)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], .1f);
            }
        }

    }*/

}
