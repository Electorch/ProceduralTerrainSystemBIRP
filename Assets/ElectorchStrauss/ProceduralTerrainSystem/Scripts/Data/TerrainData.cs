using UnityEngine;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts.Data
{
	[CreateAssetMenu()]
	public class TerrainData : UpdatableData
	{
		public bool useFlatShading;
		public bool useFalloff;
		public bool useFallon;
		public float meshHeightMultiplier;
		public AnimationCurve meshHeightCurve;
		public NoiseData noiseData;

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			noiseData.ValidateValues();
			base.OnValidate();
		}
#endif
	}
}