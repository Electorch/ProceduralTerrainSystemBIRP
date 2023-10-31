using UnityEngine;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
	[CreateAssetMenu()]
	public class ColorData : UpdatableData
	{
		public TerrainType[] regions;

		[System.Serializable]
		public struct TerrainType
		{
			public string name;
			public float height;
			public Color colour;
		}
	}
}