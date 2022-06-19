using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Terrain")]
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    [Header("Terrain Properties")]
    private float minTerrainHeight;
    private float maxTerrainHeight;
    public int xSize;//width
    public int zSize;//length
    public float scale;
    //noise scales of the perlin noise
    private float noise1Scale = 2f;
    private float noise1Frec = 2f;
    private float noise2Scale = 4f;
    private float noise2Frec = 4f;
    private float noise3Scale = 6f;
    private float noise3Frec = 6f;
    private float noiseStrength;
    private float xOffset;
    private float zOffset;
    public int seed;

    [Header("Color")]
    private Color[] colors;
    private Gradient gradient;
    public Gradient[] gradientOptions;

    [Header("Elements")]
    private int radius = 290;
    public Transform[] spawnPoints;
    public GameObject[] buildings;
    public Color[] buildingColors;
    public GameObject bouncepad;
    public GameObject[] trees;
    public GameObject[] rocks;
    public GameObject lightpost;
    public GameObject grass;
    public GameObject crate;
    public GameObject enemy;

    [Header("Number of elements")]
    public int grassCount = 1000;
    public int bouncepadCount = 10;
    public int treeCount = 200;
    public int rockCount = 25;
    public int lightpostCount = 50;
    public int enemyCount=49;
    public int crateCount = 13;

    void Awake()
    {
        StartCoroutine(GenerateScene());
        //move player to certain height
        Vector2 playerInitialPosition = GetRandomPosition(radius);
        FindObjectOfType<PlayerController>().gameObject.transform.position = new Vector3(playerInitialPosition.x, 250, playerInitialPosition.y);
    }
    IEnumerator GenerateScene()
    {
        GenerateTerrain();
        SpawnBuildings();
        StartCoroutine(Spawn(bouncepad, bouncepadCount, new Vector3(0, 1, 0)));
        StartCoroutine(Spawn(crate, crateCount, new Vector3(0, 1, 0)));
        StartCoroutine(SpawnRandom(trees, treeCount,new Vector3(0, 0, 0)));
        StartCoroutine(SpawnRandom(rocks, rockCount, new Vector3(0, 0, 0)));
        StartCoroutine(Spawn(lightpost, lightpostCount, new Vector3(0, 0, 0)));
        StartCoroutine(Spawn(grass, grassCount, Vector3.zero));
        StartCoroutine(Spawn(enemy, enemyCount, new Vector3(0, 1, 0)));
        yield return null;
    }
    private void GenerateTerrain()
    {
        gradient = gradientOptions[Random.Range(0, gradientOptions.Length)];
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateMap(xSize, xSize);
        UpdateMesh();
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void SpawnBuildings()
    {
        foreach(Transform point in spawnPoints)
        {
            GameObject building=Instantiate(buildings[Random.Range(0, buildings.Length)],point.position,Quaternion.Euler(new Vector3(-90,0,Random.Range(0,361))));
            building.GetComponentInChildren<MeshRenderer>().material.color = buildingColors[Random.Range(0, buildingColors.Length)];

        }
    }
    //Returns random position in a circle of a given radius
    Vector2 GetRandomPosition(float radius)
    {
        float randomAngle = Random.Range(0f, 2 * Mathf.PI - float.Epsilon);
        Vector2 position = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * Random.Range(0, radius);
        return position;
    }
    //spawns "count" number of items at a certain offset of the ground
    private IEnumerator Spawn(GameObject item, int count,Vector3 offset)
    {
        int placed = 0;
        while(placed < count)
        {
            Vector2 coordinates = GetRandomPosition(radius);
            Vector3 position = new Vector3(coordinates.x, 200, coordinates.y);
            RaycastHit hit;
            Physics.Raycast(position, -transform.up, out hit, 300);
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Instantiate(item, hit.point + offset, item.transform.rotation);
                placed++;
            }
        }
        yield return null;
        

    }
    private IEnumerator SpawnRandom(GameObject[] items, int count, Vector3 offset)
    {
        int placed = 0;
        while (placed < count)
        {
            Vector2 coordinates = GetRandomPosition(radius);
            Vector3 position = new Vector3(coordinates.x, 200, coordinates.y);
            //Vector3 position = new Vector3(Random.Range(-xLimit, xLimit), 200, Random.Range(-zLimit, zLimit));
            RaycastHit hit;
            Physics.Raycast(position, -transform.up, out hit, 300);
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                var item = items[Random.Range(0, items.Length)];
                GameObject inst=Instantiate(item, hit.point + offset, item.transform.rotation);
                inst.transform.localScale *= Random.Range(1f,2.15f);
                inst.transform.Rotate(new Vector3(0,0, Random.Range(0, 360)));
                placed++;
            }
        }
        yield return null;

    }
    
    private void CreateMap(int xSize, int zSize)
    {
        GenerateRandomValues(Random.Range(0, 100));
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = GetPerlinNoise(x, z);

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
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);//vertexaltura del vertice normalizada(entre 0 y 1)
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    private void GenerateRandomValues(int seed)
    {
        System.Random rand = new System.Random(seed);
        xOffset = rand.Next(-100000, 100000);
        zOffset = rand.Next(-100000, 100000);
        noise1Scale = Random.Range(0f, 15f);
        noise1Frec = Random.Range(0f, 0.03f);

        noise2Scale = Random.Range(0f, 5f);
        noise2Frec = Random.Range(0f, 0.07f);

        noise3Scale = Random.Range(0f, 0.1f);
        noise3Frec = Random.Range(0f, 0.02f);

        noiseStrength = Random.Range(1f, 3f);

    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }
    private float GetPerlinNoise(float x, float z)
    {
        //nosises overlay on top of each other to crate an interesting terrain
        float noise = noise1Scale * Mathf.PerlinNoise(x * noise1Frec + xOffset, z * noise1Frec + zOffset)
                    + noise2Scale * Mathf.PerlinNoise(x * noise2Frec + xOffset, z * noise2Frec + zOffset)
                    + noise3Scale * Mathf.PerlinNoise(x * noise3Frec + xOffset, z * noise3Frec + zOffset)
                    * noiseStrength;
        return noise;
    }


}
