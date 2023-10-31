using System.Collections;
using System.Collections.Generic;
using ElectorchStrauss.ProceduralTerrainSystem.Scripts.Data;
using UnityEngine;
using TerrainData = ElectorchStrauss.ProceduralTerrainSystem.Scripts.Data.TerrainData;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
    public class ApplyTextureOnly : MonoBehaviour
    {
        public TerrainData terrainData;
        public NoiseData noiseData;
        public ColorData colorData;
        public int mapSize = 96;
        public bool colorBlending;
        // Start is called before the first frame update
        void Start()
        {
            GenerateMap();
        }
        void GenerateMap()
        {
            float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, noiseData.seed, noiseData.scale, noiseData.octaves,
                noiseData.persistance, noiseData.lacunarity, noiseData.offset, noiseData.normalizeMode);

            Color[] colourMap = new Color[mapSize * mapSize];

            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    for (int i = 0; i < colorData.regions.Length; i++)
                    {
                        //if color blending enabled
                        if (colorBlending)
                        {
                            if (currentHeight <= colorData.regions[i].height)
                            {
                                colourMap[y * mapSize + x] = colorData.regions[i].colour;
                                break;
                            } 
                        }
                        else
                        {
                            if (currentHeight <= colorData.regions[i].height)
                            {
                                colourMap[y * mapSize + x] = colorData.regions[i].colour;
                                break;
                            } 
                        }
                    }
                }
            }
            MapDisplay display = FindObjectOfType<MapDisplay>();
            if(display)display.DrawMesh(
                MeshGenerator.GenerateTerrainMesh(noiseMap, terrainData.meshHeightMultiplier,
                    terrainData.meshHeightCurve, terrainData.useFlatShading),
                TextureGenerator.TextureFromColourMap(colourMap, mapSize, mapSize));
        }
    }
}