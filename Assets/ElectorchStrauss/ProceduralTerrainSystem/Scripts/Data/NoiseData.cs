using UnityEngine;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts.Data
{
	[CreateAssetMenu()]
	public class NoiseData : UpdatableData
	{
		public Noise.NormalizeMode normalizeMode;

		[Range(1, 7)] public int octaves;
		[Range(0, 1)] public float persistance;
		[Range(1, 2.5f)] public float lacunarity;
		[Range(1, 500)] public float scale;
		[Range(-100000, 100000)] public int seed;
		public Vector2 offset;

		public void ValidateValues()
		{
			scale = Mathf.Max(scale, 0.01f);
			octaves = Mathf.Max(octaves, 1);
			lacunarity = Mathf.Max(lacunarity, 1);
			persistance = Mathf.Clamp01(persistance);
		}
	}
}