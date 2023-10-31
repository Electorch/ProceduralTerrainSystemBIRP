using UnityEngine;
using System.Collections;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
	public class MapDisplay : MonoBehaviour
	{

		[SerializeField] MeshRenderer mapRenderer;
		[SerializeField] public MeshFilter meshFilter;
		[SerializeField] public MeshRenderer meshRenderer;
		[SerializeField] public MeshCollider meshCollider;

		void Start()
		{
			GameObject generatedMesh = GameObject.Find("GeneratedMesh");
			mapRenderer = GameObject.Find("MiniMap").GetComponent<MeshRenderer>();
			meshFilter = generatedMesh.GetComponent<MeshFilter>();
			meshRenderer = generatedMesh.GetComponent<MeshRenderer>();
			meshCollider = generatedMesh.GetComponent<MeshCollider>();
		}

		public void DrawTexture(Texture2D texture)
		{
			mapRenderer.sharedMaterial.mainTexture = texture;
			mapRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
		}

		public void DrawMesh(MeshData meshData, Texture2D texture)
		{
			meshFilter.sharedMesh = meshData.CreateMesh();
			meshCollider.sharedMesh = meshData.CreateMesh();
			meshRenderer.sharedMaterial.mainTexture = texture;
		}
	}
}