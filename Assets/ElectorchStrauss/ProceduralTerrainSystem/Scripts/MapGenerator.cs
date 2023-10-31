using ElectorchStrauss.ProceduralTerrainSystem.Scripts.Data;
using UnityEngine;
using UnityEngine.Windows;
using TerrainData = ElectorchStrauss.ProceduralTerrainSystem.Scripts.Data.TerrainData;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
	[ExecuteInEditMode]
	public class MapGenerator : MonoBehaviour
	{

		public enum DrawMode
		{
			PreviewNoise,
			PreviewFalloff,
			MeshAndMap
		};

		public DrawMode drawMode;

		public TerrainData terrainData;
		public NoiseData noiseData;
		public ColorData colorData;

		public bool autoUpdate;
		bool falloffMapGenerated;
		private float[,] fallOffMap;
		public int mapSize = 96;

		private void Awake()
		{
			GenerateFalloffMap();
		}

		void OnValuesUpdated()
		{
			if (!Application.isPlaying)
				GenerateMap();
		}

		void GenerateFalloffMap()
		{
			if (terrainData.useFalloff && !falloffMapGenerated)
			{
				int flatShadingValue = terrainData.useFlatShading ? 2 : 0;
				fallOffMap = FalloffGenerator.GenerateFalloffMap(mapSize + flatShadingValue);
				falloffMapGenerated = true;
			}
		}

		public void GenerateMap()
		{
			float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, noiseData.seed, noiseData.scale, noiseData.octaves,
				noiseData.persistance, noiseData.lacunarity, noiseData.offset, noiseData.normalizeMode);

			Color[] colourMap = new Color[mapSize * mapSize];

			if (fallOffMap == null)
			{
				fallOffMap = FalloffGenerator.GenerateFalloffMap(mapSize);
			}

			for (int y = 0; y < mapSize; y++)
			{
				for (int x = 0; x < mapSize; x++)
				{
					if (terrainData.useFalloff)
					{
						noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - fallOffMap[x, y]);
					}

					if (terrainData.useFallon)
					{
						noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] + fallOffMap[x, y]);
					}

					float currentHeight = noiseMap[x, y];
					for (int i = 0; i < colorData.regions.Length; i++)
					{
						if (currentHeight <= colorData.regions[i].height)
						{
							colourMap[y * mapSize + x] = colorData.regions[i].colour;
							break;
						}
					}
				}
			}

			MapDisplay display = FindObjectOfType<MapDisplay>();
			if (drawMode == DrawMode.PreviewNoise)
			{
				display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
			}
			else if (drawMode == DrawMode.PreviewFalloff)
			{
				display.DrawTexture(
					TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapSize)));
			}
			else if (drawMode == DrawMode.MeshAndMap)
			{
				display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapSize, mapSize));
				display.DrawMesh(
					MeshGenerator.GenerateTerrainMesh(noiseMap, terrainData.meshHeightMultiplier,
						terrainData.meshHeightCurve, terrainData.useFlatShading),
					TextureGenerator.TextureFromColourMap(colourMap, mapSize, mapSize));
#if UNITY_EDITOR
				var dirPath = Application.dataPath + "/ElectorchStrauss/ProceduralTerrainSystem/Art/Textures/";
				
				if(!File.Exists(dirPath + "MeshTexture" + ".png"))
				{
					Texture2D textureFromColourMap = TextureGenerator.TextureFromColourMap(colourMap, mapSize, mapSize);
					byte[] bytes = textureFromColourMap.EncodeToPNG();
					File.WriteAllBytes(dirPath + "MeshTexture" + ".png", bytes);
				}
#endif
			}
		}

		private void OnValidate()
		{
			if (terrainData != null)
			{
				terrainData.OnValuesUpdated -= OnValuesUpdated;
				terrainData.OnValuesUpdated += OnValuesUpdated;
			}

			if (noiseData != null)
			{
				noiseData.OnValuesUpdated -= OnValuesUpdated;
				noiseData.OnValuesUpdated += OnValuesUpdated;
			}

			if (colorData != null)
			{
				colorData.OnValuesUpdated -= OnValuesUpdated;
				colorData.OnValuesUpdated += OnValuesUpdated;
			}

			falloffMapGenerated = false;
		}
	}
}