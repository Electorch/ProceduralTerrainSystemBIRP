using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
    public class SpawnObjects : MonoBehaviour
    {
        private TerrainRuntimeModification _terrainRuntimeModification;
        public GameObject[] algueaPrefab = new GameObject[3];
        public GameObject[] grassPrefab = new GameObject[3];
        public GameObject[] rockPrefab = new GameObject[3];
        public GameObject[] treePrefab = new GameObject[2];
        public GameObject[] deadTreePrefab = new GameObject[1];
        
        [SerializeField] private int nbrOfAlguea;
        [SerializeField] private int nbrOfGrass;
        [SerializeField] private int nbrOfRock;
        [SerializeField] private int nbrOfTreeTotal;
        [SerializeField] private int nbrOfTree;
        [SerializeField] private int nbrOfDeadTree;
        [SerializeField] private float terrainRange; 
        private GameObject rockParent, treeParent, grassParent, algueaParent; 
        public GameObject grassSpawner; 
        public float brushSize=25f;
        public MeshCollider lookupCollider;
        private Vector3 randomPoint;
        [SerializeField] private float[] forestHeight= new float[3];
        [SerializeField] private int forestLayer;
        public bool getIt,spawnTree;
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(randomPoint,brushSize);
        }

        private void Start()
        {
            _terrainRuntimeModification = FindObjectOfType<TerrainRuntimeModification>();
            forestHeight[0] = _terrainRuntimeModification.layersHeight[forestLayer];
            forestHeight[1] = _terrainRuntimeModification.layersHeight[forestLayer+1];
            forestHeight[2] = _terrainRuntimeModification.layersHeight[forestLayer-1];
            rockParent = new GameObject("RockParent");
            treeParent = new GameObject("TreeParent");
            grassParent = new GameObject("GrassParent");
            algueaParent = new GameObject("AlgueaParent");
        }

        public void SpawnThem()
        {
            SpawnAlgueaPrefab();
            SpawnGrassPrefab();
            SpawnRockPrefab();
            SpawnDeadTreePrefab();
            spawnTree = true;
            grassSpawner.SetActive(true);
        }
        public void DeleteThem()
        {
            int childs = rockParent.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                DestroyImmediate(rockParent.transform.GetChild(i).gameObject);
            }

            int childs1 = treeParent.transform.childCount;
            for (int i = childs1 - 1; i >= 0; i--)
            {
                DestroyImmediate(treeParent.transform.GetChild(i).gameObject);
            }

            int childs2 = grassParent.transform.childCount;
            for (int i = childs2 - 1; i >= 0; i--)
            {
                DestroyImmediate(grassParent.transform.GetChild(i).gameObject);
            }

            int childs3 = algueaParent.transform.childCount;
            for (int i = childs3 - 1; i >= 0; i--)
            {
                DestroyImmediate(algueaParent.transform.GetChild(i).gameObject);
            }   
        }

        void SpawnAlgueaPrefab()
        {
            for (int i = 0; i < nbrOfAlguea; i++)
            {
                foreach (var t in algueaPrefab)
                {
                    float randomX = Random.Range(-terrainRange, terrainRange);
                    float randomZ = Random.Range(-terrainRange, terrainRange);
                    Ray ray = new Ray(new Vector3(randomX, terrainRange, randomZ),
                        new Vector3(randomX, -terrainRange, randomZ));
                    if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.gameObject.layer == 4)
                    {
                        Debug.DrawRay(new Vector3(randomX, terrainRange, randomZ),
                            new Vector3(randomX, -terrainRange, randomZ) * hit.distance, Color.blue);
                        if (Physics.Raycast(hit.point,
                            hit.point - new Vector3(randomX, terrainRange, randomZ), out RaycastHit hit1))
                        {
                            if (hit1.transform.gameObject.layer == 3 &&
                                Vector3.Dot(Vector3.up, hit.normal) >= Mathf.Cos(95f * Mathf.Deg2Rad))
                            {
                                Debug.DrawRay(new Vector3(randomX, terrainRange, randomZ),
                                    new Vector3(randomX, -terrainRange, randomZ) * hit1.distance, Color.red);
                                var alguea = Instantiate(t, hit1.point,
                                    Quaternion.LookRotation(t.transform.forward, hit1.normal));
                                alguea.transform.SetParent(algueaParent.transform);
                                alguea.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
                            }
                        }
                    }
                }
            }
        }

        void SpawnGrassPrefab()
        {
            for (int i = 0; i < nbrOfGrass; i++)
            {
                foreach (var t in grassPrefab)
                {
                    float randomX = Random.Range(-terrainRange, terrainRange);
                    float randomZ = Random.Range(-terrainRange, terrainRange);
                    if (Physics.Raycast(new Vector3(randomX, terrainRange, randomZ),
                            new Vector3(randomX, -terrainRange, randomZ), out RaycastHit hit) &&
                        hit.transform.gameObject.layer == 3)
                    {
                        if (Vector3.Dot(Vector3.up, hit.normal) >= Mathf.Cos(45f * Mathf.Deg2Rad))
                        {
                            Debug.DrawRay(new Vector3(randomX, terrainRange, randomZ),
                                new Vector3(randomX, -terrainRange, randomZ), Color.green);
                            var grass = Instantiate(t, hit.point,
                                Quaternion.LookRotation(t.transform.forward, hit.normal));
                            grass.transform.SetParent(grassParent.transform);
                            grass.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
                        }
                    }
                }
            }
        }

        void SpawnRockPrefab()
        {
            for (int i = 0; i < nbrOfRock; i++)
            {
                foreach (var t in rockPrefab)
                {
                    float randomX = Random.Range(-terrainRange, terrainRange);
                    float randomZ = Random.Range(-terrainRange, terrainRange);
                    if (Physics.Raycast(new Vector3(randomX, terrainRange, randomZ),
                            new Vector3(randomX, -terrainRange, randomZ), out RaycastHit hit) &&
                        hit.transform.gameObject.layer == 3)
                    {
                        if (Vector3.Dot(Vector3.up, hit.normal) >= Mathf.Cos(65f * Mathf.Deg2Rad))
                        {
                            Debug.DrawRay(new Vector3(randomX, terrainRange, randomZ),
                                new Vector3(randomX, -terrainRange, randomZ), Color.black);

                            var rock = Instantiate(t, hit.point,
                                Quaternion.LookRotation(t.transform.forward, hit.normal));
                            rock.transform.SetParent(rockParent.transform);
                            rock.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
                        }
                    }
                }
            }
        }

        void Update()
        {
            if (!spawnTree)
                return;
            if (!getIt)
            {
                randomPoint = GetRandomPointOnMesh(lookupCollider.sharedMesh);
                randomPoint += lookupCollider.transform.position;
                for (int i = 0; i < forestHeight.Length; i++)
                {
                    if (randomPoint.y >= forestHeight[i])
                    {
                        getIt = true;
                    }
                    else
                    {
                        getIt = false;
                    }
                }
            }
            else
            {

                for (int i = 0; i < Random.Range(nbrOfTree - nbrOfTree / 2, nbrOfTree); i++)
                {
                    foreach (var t in treePrefab)
                    {
                        // brushrange
                        float e = 2f * Mathf.PI * Random.Range(0f, brushSize);
                        float u = Random.Range(0f, brushSize) + Random.Range(0f, brushSize);
                        float r = (u > 1 ? 2 - u : u);
                        Vector3 origin = Vector3.zero;
                        // place random in radius, except for first one
                        if (i != 0)
                        {
                            origin.x += r * Mathf.Cos(e);
                            origin.z += r * Mathf.Sin(e);
                        }
                        else
                        {
                            origin = Vector3.zero;
                        }

                        Vector3 pointUpTree = new Vector3(randomPoint.x, terrainRange, randomPoint.z);
                        Vector3 pointDownTree = new Vector3(randomPoint.x, -terrainRange, randomPoint.z);

                        Ray rayTree = new Ray(pointUpTree, pointDownTree);
                        // add random range to ray
                        rayTree.origin += origin;

                        if (Physics.Raycast(rayTree, out RaycastHit hitTree))
                        {
                            if (hitTree.transform.gameObject.layer == 3)
                            {
                                if (hitTree.point.y is < 60 and > 20)
                                {
                                    if (Vector3.Dot(Vector3.up, hitTree.normal) >=
                                        Mathf.Cos(20f * Mathf.Deg2Rad))
                                    {
                                        Debug.DrawRay(new Vector3(pointUpTree.x, terrainRange, pointUpTree.z),
                                            new Vector3(pointDownTree.x, -terrainRange, pointDownTree.z),
                                            Color.green);
                                        
                                        var tree = Instantiate(t, hitTree.point,
                                            Quaternion.LookRotation(t.transform.forward, hitTree.normal));
                                        tree.transform.SetParent(treeParent.transform);
                                        tree.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
                                    }
                                }
                            }
                        }
                    }
                }
                getIt = false;
                if (treeParent.transform.childCount >= nbrOfTreeTotal)
                {
                    spawnTree = false;
                }
            }
        }

        void SpawnDeadTreePrefab()
        {
            for (int i = 0; i < nbrOfDeadTree; i++)
            {
                foreach (var t in deadTreePrefab)
                {
                    float randomX = Random.Range(-terrainRange, terrainRange);
                    float randomZ = Random.Range(-terrainRange, terrainRange);
                    if (Physics.Raycast(new Vector3(randomX, terrainRange, randomZ),
                            new Vector3(randomX, -terrainRange, randomZ), out RaycastHit hit) &&
                        hit.transform.gameObject.layer == 3)
                    {
                        if (Vector3.Dot(Vector3.up, hit.normal) >= Mathf.Cos(35f * Mathf.Deg2Rad))
                        {
                            Debug.DrawRay(new Vector3(randomX, terrainRange, randomZ),
                                new Vector3(randomX, -terrainRange, randomZ), Color.gray);

                            var deadtree = Instantiate(t, hit.point,
                                Quaternion.LookRotation(t.transform.forward, hit.normal));
                            deadtree.transform.SetParent(treeParent.transform);
                            deadtree.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
                        }
                    }
                }
            }
        }
        
        Vector3 GetRandomPointOnMesh(Mesh mesh)
        {
            //if you're repeatedly doing this on a single mesh, you'll likely want to cache cumulativeSizes and total
            float[] sizes = GetTriSizes(mesh.triangles, mesh.vertices);
            float[] cumulativeSizes = new float[sizes.Length];
            float total = 0;

            for (int i = 0; i < sizes.Length; i++)
            {
                total += sizes[i];
                cumulativeSizes[i] = total;
            }

            //so everything above this point wants to be factored out

            float randomsample = Random.value * total;

            int triIndex = -1;

            for (int i = 0; i < sizes.Length; i++)
            {
                if (randomsample <= cumulativeSizes[i])
                {
                    triIndex = i;
                    break;
                }
            }

            Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3]];
            Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
            Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

            //generate random barycentric coordinates

            float r = Random.value;
            float s = Random.value;

            if (r + s >= 1)
            {
                r = 1 - r;
                s = 1 - s;
            }

            //and then turn them back to a Vector3
            Vector3 pointOnMesh = a + r * (b - a) + s * (c - a);
            return pointOnMesh * lookupCollider.transform.localScale.x;
        }

        float[] GetTriSizes(int[] tris, Vector3[] verts)
        {
            int triCount = tris.Length / 3;
            float[] sizes = new float[triCount];
            for (int i = 0; i < triCount; i++)
            {
                sizes[i] = .5f * Vector3.Cross(verts[tris[i * 3 + 1]] - verts[tris[i * 3]],
                    verts[tris[i * 3 + 2]] - verts[tris[i * 3]]).magnitude;
            }

            return sizes;
        }
    }
}