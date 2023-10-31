using System.Collections;
using System.Collections.Generic;
using ElectorchStrauss.ProceduralTerrainSystem.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrassRaycast : MonoBehaviour
{
    [Range(1, 600000)]
    public int grassLimit;
    public LayerMask hitMask = 3;
    public LayerMask noHitMask = 5;

    public float density;

    public float brushSize;
    public int i = 0;
    Vector3 hitPos;

    [SerializeField]
    List<Vector3> positions = new List<Vector3>();
    [SerializeField]
    List<Color> colors = new List<Color>();
    [SerializeField]
    List<int> indicies = new List<int>();
    [SerializeField]
    List<Vector3> normals = new List<Vector3>();
    [SerializeField]
    List<Vector2> length = new List<Vector2>();
    public float sizeWidth = 1f;
    public float sizeLength = 1f;
    public Color AdjustedColor;
    public float rangeR, rangeG, rangeB;
    private Vector3 lastPosition = Vector3.zero;
    public float terrainRange = 500f;
    public float grassHeight = 20f;
    public Mesh mesh;
    MeshFilter filter;
    int[] indi;
    private MapGenerator terrainGenerator;
    private GrassComputeScript _grassComputeScript;

    public Slider _slider;

    public TextMeshProUGUI _grassCount;
    // Start is called before the first frame update
    void Start()
    {
        terrainGenerator = FindObjectOfType<MapGenerator>();
        _slider.maxValue = grassLimit;
        filter = GetComponent<MeshFilter>();
        _grassComputeScript= GetComponent<GrassComputeScript>();
        _grassComputeScript.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        _grassCount.text = i.ToString();
        _slider.value = i;
        // place based on density
        for (int k = 0; k < density; k++)
        {
            // brushrange
            float t = 2f * Mathf.PI * Random.Range(0f, brushSize);
            float u = Random.Range(0f, brushSize) + Random.Range(0f, brushSize);
            float r = (u > 1 ? 2 - u : u);
            Vector3 origin = Vector3.zero;

            // place random in radius, except for first one
            if (k != 0)
            {
                origin.x += r * Mathf.Cos(t);
                origin.y += r * Mathf.Sin(t);
            }
            else
            {
                origin = Vector3.zero;
            }

            RaycastHit terrainHit;

            // add random range to ray
            float randomX = Random.Range(-terrainRange, terrainRange);
            float randomZ = Random.Range(-terrainRange, terrainRange);
            
            Ray ray = new Ray(new Vector3(randomX, terrainRange, randomZ),
                new Vector3(randomX, -terrainRange, randomZ));
            ray.origin += origin;

            if (Physics.Raycast(ray, out terrainHit, Mathf.Infinity, hitMask.value) && i < grassLimit)
            {
                if (Vector3.Dot(Vector3.up, terrainHit.normal) >= Mathf.Cos(45f * Mathf.Deg2Rad) && terrainHit.point.y > grassHeight)
                {
                    hitPos = terrainHit.point;
                    if (k != 0)
                    {
                        var grassPosition = hitPos;
                        grassPosition -= this.transform.position;
                        positions.Add((grassPosition));
                        indicies.Add(i);
                        length.Add(new Vector2(sizeWidth, sizeLength));
                        // add random color variations                          
                        colors.Add(new Color(AdjustedColor.r + (Random.Range(0, 1.0f) * rangeR),
                            AdjustedColor.g + (Random.Range(0, 1.0f) * rangeG),
                            AdjustedColor.b + (Random.Range(0, 1.0f) * rangeB), 1));
                        normals.Add(terrainHit.normal);
                        i++;
                    }
                    else
                    {
                        // to not place everything at once, check if the first placed point far enough away from the last placed first one
                        if (Vector3.Distance(terrainHit.point, lastPosition) > brushSize)
                        {
                            var grassPosition = hitPos;
                            grassPosition -= this.transform.position;
                            positions.Add((grassPosition));
                            indicies.Add(i);
                            length.Add(new Vector2(sizeWidth, sizeLength));
                            colors.Add(new Color(AdjustedColor.r + (Random.Range(0, 1.0f) * rangeR),
                                AdjustedColor.g + (Random.Range(0, 1.0f) * rangeG),
                                AdjustedColor.b + (Random.Range(0, 1.0f) * rangeB), 1));
                            normals.Add(terrainHit.normal);
                            i++;

                            if (origin == Vector3.zero)
                            {
                                lastPosition = hitPos;
                            }
                        }
                    }
                }
            }
        }
        // set all info to mesh
        mesh = new Mesh();
        mesh.SetVertices(positions);
        indi = indicies.ToArray();
        mesh.SetIndices(indi, MeshTopology.Points, 0);
        mesh.SetUVs(0, length);
        mesh.SetColors(colors);
        mesh.SetNormals(normals);
        filter.mesh = mesh;
        if (i >= grassLimit)
        {
            _grassComputeScript.enabled = true;
        }
    }
}
