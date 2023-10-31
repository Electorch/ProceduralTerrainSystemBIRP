using UnityEngine;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
    public class PathConnectVertices : MonoBehaviour
    {
        private GameObject nextSibling;
        private Vector3[] closestVertices = new Vector3[3];

        public BoxCollider _objCollider;
        [HideInInspector] public Vector3 bottomLeft;
        [HideInInspector] public Vector3 topLeft;
        [HideInInspector] public Vector3 bottomRight;
        [HideInInspector] public Vector3 topRight;
        public Vector3[] pointList;
        public PathConnectVertices nextSiblingPathConnectVertices;
        public MeshFilter meshFilter, nextMeshFilter;

        void Update()
        {

            if (_objCollider == null)
            {
                _objCollider = GetComponent<BoxCollider>();
                return;
            }

            Vector3 min = _objCollider.bounds.min;
            Vector3 max = _objCollider.bounds.max;

            bottomLeft = new Vector3(min.x, max.y, min.z);
            topLeft = new Vector3(min.x, max.y, max.z);

            bottomRight = new Vector3(max.x, max.y, min.z);
            topRight = new Vector3(max.x, max.y, max.z);
            pointList = new Vector3[4]
            {
                bottomLeft,
                topLeft,
                bottomRight,
                topRight
            };
            if (meshFilter == null)
                meshFilter = transform.GetComponent<MeshFilter>();
            //find next sibling 
            int index = transform.GetSiblingIndex();
            if (index == transform.parent.childCount - 1)
            {
                nextSibling = transform.parent.GetChild(index).gameObject;
            }
            else
            {
                nextSibling = transform.parent.GetChild(index + 1).gameObject;
            }

            if (nextSibling)
            {
                if (nextMeshFilter == null)
                    nextMeshFilter = nextSibling.GetComponent<MeshFilter>();
                nextSiblingPathConnectVertices = nextSibling.transform.GetComponent<PathConnectVertices>();
            }

            foreach (var t in nextMeshFilter.mesh.vertices)
            {
                closestVertices = NearestVertexTo(t);
            }
        }

        public Vector3[] NearestVertexTo(Vector3 point)
        {
            float distance = Mathf.Infinity;
            Vector3[] nearestVertex = new Vector3[3];
            // scan all vertices to find nearest
            foreach (Vector3 vertex in pointList)
            {
                Vector3 diff = point - vertex;
                float distSqr = diff.sqrMagnitude;
                if (distSqr <= distance)
                {
                    nearestVertex[2] = nearestVertex[0];
                    nearestVertex[1] = nearestVertex[2];
                    nearestVertex[0] = vertex;
                    distance = distSqr;
                }
            }

            // convert nearest vertex back to world space
            return nearestVertex;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(bottomLeft, Vector3.one);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(topLeft, Vector3.one);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(bottomRight, Vector3.one);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(topRight, Vector3.one);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(closestVertices[0], Vector3.one / 2);
            Gizmos.DrawWireCube(closestVertices[1], Vector3.one / 2);
            Gizmos.DrawWireCube(closestVertices[2], Vector3.one / 2);
        }
    }
}