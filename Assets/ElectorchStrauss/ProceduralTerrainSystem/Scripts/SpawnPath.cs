using UnityEngine;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
    public class SpawnPath : MonoBehaviour
    {
        public float terrainRange = 500f;
        private Vector3 pathStart, pathEnd;
        public bool first, second, third;
        public Vector3[] pathPoints;

        private Vector3 skyStart;
        private GameObject pathParent;
        public int minimumPathLength, smoothness;
        float width = 4;
        float height = 8;
        private float depth = 4;
        public Material pathMat;
        public bool resetPath, addPath,parkourPath,curvePath;
        public bool spawnIt;
        public float t = 4f;
        private void Start()
        {
            pathParent = new GameObject("PathParent");
        }

        public void SpawnItPath()
        {
            spawnIt = true;
        }

        public void ResetPath()
        {
            parkourPath = false;
            resetPath = true;
        }

        public void AddPath()
        {
            addPath = true;
        }

        public void ParkourPath()
        {
            parkourPath = true;
            addPath = true;
        }

        public void CurvePath()
        {
            curvePath = true;
            addPath = true;
        }
        
        public void DeletePath()
        {
            int childs = pathParent.transform.childCount;
            for (int i = childs - 1; i >= 0; i--) {
                DestroyImmediate( pathParent.transform.GetChild(i).gameObject );
            }
            pathPoints = null;
            spawnIt = false;
            first = false;
            second = false;
            third = false;
            parkourPath = false;
        }
        void Update()
        {
            if (!spawnIt)
            {
                return;
            }
            if (resetPath)
            {
                int childs = pathParent.transform.childCount;
                for (int i = childs - 1; i >= 0; i--) {
                    DestroyImmediate( pathParent.transform.GetChild(i).gameObject );
                }
                pathPoints = null;
                third = false;
            }

            if (addPath)
            {
                third = false;
            }

            if (first && second && !third)
            {
                int directionLength = (int) Vector3.Distance(pathEnd, pathStart) / smoothness;
                if (directionLength < minimumPathLength || addPath || resetPath)
                {
                    first = false;
                    second = false;
                    addPath = false;
                    resetPath = false;
                    return;
                }

                pathPoints = new Vector3[directionLength];
                Debug.DrawLine(pathStart, pathEnd, Color.black);

                for (int i = 0; i < pathPoints.Length; i++)
                {
                    skyStart = Vector3.LerpUnclamped(pathStart, pathEnd, (1f / directionLength) * i);
                    if (Physics.Raycast(new Vector3(skyStart.x, skyStart.y + 100, skyStart.z),
                        new Vector3(skyStart.x, skyStart.y - 100, skyStart.z) -
                        new Vector3(skyStart.x, skyStart.y + 100, skyStart.z), out RaycastHit hit))
                    {
                        if (parkourPath)
                        {
                            pathPoints[i] = new Vector3(hit.point.x+Random.Range(-20f,20f), hit.point.y + height / Random.Range(1f,2f), hit.point.z+Random.Range(-20f,20f));
                        }
                        else if(curvePath)
                        {
                            pathPoints[i] = new Vector3(hit.point.x+Random.Range(0f,i), hit.point.y + height / 2, hit.point.z+Random.Range(0,i));
                        }
                        else
                        {
                            pathPoints[i] = new Vector3(hit.point.x, hit.point.y + height / 2, hit.point.z);
                        }

                        GameObject pathObject = new GameObject("PathObject" + i);
                        pathObject.transform.SetParent(pathParent.transform);
                        MeshRenderer meshRenderer = pathObject.AddComponent<MeshRenderer>();
                        meshRenderer.sharedMaterial = pathMat;
                        MeshFilter meshFilter = pathObject.AddComponent<MeshFilter>();
                        BoxCollider boxCollider = pathObject.AddComponent<BoxCollider>();
                        boxCollider.center = new Vector3(0, height/2, 0);
                        boxCollider.size = new Vector3(width * 2, height, depth * 2);
                        pathObject.AddComponent<PathConnectVertices>();
                        pathObject.layer = 3;
                        Mesh mesh = new Mesh();
                        Vector3 pathDir = pathEnd - pathStart;
                        float angle = Vector3.Angle(pathObject.transform.forward, pathDir);
                        Vector3[] vertices = new Vector3[36]
                        {
                            new Vector3(pathObject.transform.position.x - width,
                                pathObject.transform.position.y + height, pathObject.transform.position.z - depth),
                            new Vector3(pathObject.transform.position.x + width,
                                pathObject.transform.position.y + height, pathObject.transform.position.z - depth),
                            new Vector3(pathObject.transform.position.x - width,
                                pathObject.transform.position.y + height, pathObject.transform.position.z + depth),
                            new Vector3(pathObject.transform.position.x + width,
                                pathObject.transform.position.y + height, pathObject.transform.position.z + depth),
                            new Vector3(pathObject.transform.position.x - width,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z - depth),
                            new Vector3(pathObject.transform.position.x + width,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z - depth),
                            new Vector3(pathObject.transform.position.x - width,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z + depth),
                            new Vector3(pathObject.transform.position.x + width,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z + depth),
                            new Vector3(pathObject.transform.position.x - width,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z - depth),
                            new Vector3(pathObject.transform.position.x - width,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z + depth),
                            new Vector3(pathObject.transform.position.x + width,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z - depth),
                            new Vector3(pathObject.transform.position.x + width,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z + depth),
                            
                            new Vector3(pathObject.transform.position.x - width,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z - depth),
                            new Vector3(pathObject.transform.position.x-width/2,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z - depth),
                            new Vector3(pathObject.transform.position.x-width/2,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z - depth),
                            new Vector3(pathObject.transform.position.x + width/2,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z - depth),
                            new Vector3(pathObject.transform.position.x + width,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z - depth),
                            new Vector3(pathObject.transform.position.x + width/2,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z - depth),
                            
                            new Vector3(pathObject.transform.position.x - width/2,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z + depth),
                            new Vector3(pathObject.transform.position.x-width,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z + depth),
                            new Vector3(pathObject.transform.position.x-width/2,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z +depth),
                            new Vector3(pathObject.transform.position.x + width/2,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z + depth),
                            new Vector3(pathObject.transform.position.x + width/2,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z + depth),
                            new Vector3(pathObject.transform.position.x + width,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z + depth),
                            
                            new Vector3(pathObject.transform.position.x - width,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z - depth),
                            new Vector3(pathObject.transform.position.x-width,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z - depth/2),
                            new Vector3(pathObject.transform.position.x-width,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z - depth/2),
                            new Vector3(pathObject.transform.position.x - width,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z + depth/2),
                            new Vector3(pathObject.transform.position.x - width,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z + depth),
                            new Vector3(pathObject.transform.position.x - width,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z + depth/2),
                            
                            new Vector3(pathObject.transform.position.x + width,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z - depth/2),
                            new Vector3(pathObject.transform.position.x + width,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z - depth),
                            new Vector3(pathObject.transform.position.x + width,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z -depth/2),
                            new Vector3(pathObject.transform.position.x + width,
                                pathObject.transform.position.y + height/2, pathObject.transform.position.z + depth/2),
                            new Vector3(pathObject.transform.position.x + width,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z + depth/2),
                            new Vector3(pathObject.transform.position.x + width,
                                pathObject.transform.position.y - height*6, pathObject.transform.position.z + depth)
                            
                        };
                        mesh.vertices = vertices;

                        int[] tris = new int[78]
                        {
                            0, 2, 1,
                            2, 3, 1,
                            4, 0, 5,
                            0, 1, 5,
                            2, 6, 3,
                            6, 7, 3,
                            8, 9, 0,
                            9, 2, 0,
                            1, 3, 10,
                            3, 11, 10,
                            12,4,13,
                            4,14,13,
                            15,17,16,
                            17,5,16,
                            6,19,18,
                            19,20,18,
                            21,22,7,
                            22,23,7,
                            24,25,8,
                            25,26,8,
                            27,28,29,
                            28,9,29,
                            31,10,30,
                            31,30,32,
                            33,11,34,
                            11,35,34
                        };
                        mesh.triangles = tris;

                        Vector3[] normals = new Vector3[36]
                        {
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward,
                            Vector3.forward
                        };

                        mesh.normals = normals;

                        Vector2[] uv = new Vector2[36]
                        {
                            new Vector2(0, 0),
                            new Vector2(1, 0),
                            new Vector2(0, 1),
                            new Vector2(1, 1),
                            new Vector2(0, 0),
                            new Vector2(1, 0),
                            new Vector2(0, 1),
                            new Vector2(1, 1),
                            new Vector2(0, 0),
                            new Vector2(0, 1),
                            new Vector2(1, 0),
                            new Vector2(1, 1),
                            new Vector2(0, 0),
                            new Vector2(0, 0),
                            new Vector2(1, 0),
                            new Vector2(1, 1),
                            new Vector2(1, 0),
                            new Vector2(1, 1),
                            new Vector2(0, 0),
                            new Vector2(0, 0),
                            new Vector2(1, 0),
                            new Vector2(1, 1),
                            new Vector2(1, 0),
                            new Vector2(1, 1),
                            new Vector2(0, 0),
                            new Vector2(0, 0),
                            new Vector2(1, 0),
                            new Vector2(1, 1),
                            new Vector2(1, 0),
                            new Vector2(1, 1),
                            new Vector2(0, 0),
                            new Vector2(0, 0),
                            new Vector2(1, 0),
                            new Vector2(1, 1),
                            new Vector2(1, 0),
                            new Vector2(1, 1)
                            
                        };
                        mesh.uv = uv;
                        meshFilter.mesh = mesh;
                        pathObject.transform.position = pathPoints[i];
                        pathObject.transform.Rotate(new Vector3(0, angle, 0), Space.Self);
                        
                        third = true;
                    }
                }
            }

            if (second) return;
            float randomX = Random.Range(-terrainRange, terrainRange);
            float randomZ = Random.Range(-terrainRange, terrainRange);
            if (Physics.Raycast(new Vector3(randomX, terrainRange, randomZ),
                new Vector3(randomX, -terrainRange, randomZ), out RaycastHit hit1))
            {
                if (Vector3.Dot(Vector3.up, hit1.normal) >= Mathf.Cos(35f * Mathf.Deg2Rad))
                {
                    if (!first)
                    {
                        pathStart = hit1.point;
                        first = true;
                        return;
                    }

                    if (!second && first)
                    {
                        pathEnd = hit1.point;
                        second = true;
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(pathStart, 5f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pathEnd, 5f);
            if(pathPoints != null)
            {
                for (int i = 0; i < pathPoints.Length; i++)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(pathPoints[i], 0.3f);
                }
            }
        }
    }

}