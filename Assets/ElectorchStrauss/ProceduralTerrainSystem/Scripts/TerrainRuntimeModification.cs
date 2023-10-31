using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TerrainData = ElectorchStrauss.ProceduralTerrainSystem.Scripts.Data.TerrainData;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
    public class TerrainRuntimeModification : MonoBehaviour
    {
        public MapGenerator terrainGenerator;

        public Slider seedSlider,
            scaleSlider,
            lacunaritySlider,
            persistanceSlider,
            octavesSlider,
            heightMeshMultiplierSlider;

        public Toggle flatShadingToggle, useFallOffToggle, useFallOnToggle;
        public RectTransform regionsPanel;
        public GameObject seedLock, scaleLock, lacunarityLock, persistanceLock, octavesLock;
        public GameObject heightLock, fallOffLock, fallOnLock, flatShadingLock;
        public RectTransform heightCurvePanel;
        public GameObject heightCurveEdit;
        public List<TerrainData> terrainDatas;

        public Transform electurch, playerSpawner;

        //Display this separated
        [Space(10)] [Header("MeshHeightCurve Runtime Variables")]
        public float[] layersHeight;

        [SerializeField] private List<Image> curveDots = new List<Image>();
        [SerializeField] private List<RectTransform> curveLines = new List<RectTransform>();

        [SerializeField] private List<RectTransform> handleDots = new List<RectTransform>();
        [SerializeField] private List<RectTransform> handleLines = new List<RectTransform>();
        public Vector3 diffMouse;
        [SerializeField] private List<Vector3> curveLinesPoints = new List<Vector3>();
        private void Start()
        {
            if (terrainGenerator.noiseData != null)
            {
                seedSlider.value = terrainGenerator.noiseData.seed;
                scaleSlider.value = terrainGenerator.noiseData.scale;
                lacunaritySlider.value = terrainGenerator.noiseData.lacunarity;
                persistanceSlider.value = terrainGenerator.noiseData.persistance;
                octavesSlider.value = terrainGenerator.noiseData.octaves;
            }

            if (terrainGenerator.terrainData != null)
            {
                heightMeshMultiplierSlider.value = terrainGenerator.terrainData.meshHeightMultiplier;
                useFallOffToggle.isOn = terrainGenerator.terrainData.useFalloff;
                useFallOnToggle.isOn = terrainGenerator.terrainData.useFallon;
                flatShadingToggle.isOn = terrainGenerator.terrainData.useFlatShading;
                CreateHeightCurve();
            }

            if (terrainGenerator.colorData != null)
            {
                layersHeight = new float[terrainGenerator.colorData.regions.Length];
                for (int i = 0; i < layersHeight.Length; i++)
                {
                    if (terrainGenerator.terrainData != null)
                    {
                        layersHeight[i] = terrainGenerator.colorData.regions[i].height *
                                          terrainGenerator.terrainData.meshHeightMultiplier;
                    }
                }
                Vector3 electurchPos = new Vector3(0, layersHeight[^1], 0);
                Vector3 spawnerPos = new Vector3(0, layersHeight[^1] - 1, 0);
                electurch.position = electurchPos;
                playerSpawner.position = spawnerPos;
            }
            CreateRegions();
        }

        void CreateHeightCurve()
        {
            for (int i = 0; i < terrainGenerator.terrainData.meshHeightCurve.length; i++)
            {
                #region CurveDots

                //create curveDots
                Image curveDot = Instantiate(Resources.Load<Image>("Image"), heightCurvePanel.transform);
                curveDot.rectTransform.localPosition = new Vector2(
                    terrainGenerator.terrainData.meshHeightCurve[i].time * heightCurvePanel.sizeDelta.x,
                    terrainGenerator.terrainData.meshHeightCurve[i].value * heightCurvePanel.sizeDelta.y);
                curveDot.name = "CurveDots" + i;
                curveDots.Add(curveDot);

                #endregion
            }
            int nbrCurve = terrainGenerator.terrainData.meshHeightCurve.length - 1;
            for (int i = 0; i < nbrCurve; i++)
            {
                //find p1 and p2
                Vector2 p1 = new Vector2(
                    terrainGenerator.terrainData.meshHeightCurve[i].time * heightCurvePanel.sizeDelta.x,
                    terrainGenerator.terrainData.meshHeightCurve[i].value * heightCurvePanel.sizeDelta.y);
                Vector2 p2 = new Vector2(
                    terrainGenerator.terrainData.meshHeightCurve[i + 1].time * heightCurvePanel.sizeDelta.x,
                    terrainGenerator.terrainData.meshHeightCurve[i + 1].value * heightCurvePanel.sizeDelta.y);

                //instantiate curveLine
                RectTransform curveLine =
                    Instantiate(Resources.Load<RectTransform>("Line"), heightCurvePanel.transform);
                curveLine.name = "Line" + i;
                LineRenderer lineRenderer = curveLine.GetComponent<LineRenderer>();
                lineRenderer.positionCount = 2;
                curveLinesPoints.Add(p1);
                curveLinesPoints.Add(p2);
                
                Vector3[] linearLine = new Vector3[2];
                for (int j = 0; j < 2; j++)
                {
                    linearLine[j] = curveLinesPoints[j + 2 * i];
                }
                lineRenderer.SetPositions(linearLine);
                curveLines.Add(curveLine);
            }
        }

        public void ModifyMeshHeightCurve()
        {
            int nbrCurve = terrainGenerator.terrainData.meshHeightCurve.length - 1;
            //make curvelines follow curvepoints
            for (int i = 0; i < nbrCurve; i++)
            {
                Vector2 p1 = new Vector2(curveDots[i].rectTransform.localPosition.x,
                    curveDots[i].rectTransform.localPosition.y);

                Vector2 p2 = new Vector2(curveDots[i + 1].rectTransform.localPosition.x,
                    curveDots[i + 1].rectTransform.localPosition.y);

                curveLinesPoints[0 + i * 2] = p1;
                curveLinesPoints[1 + i * 2] = p2;
                LineRenderer lineRenderer = curveLines[i].GetComponent<LineRenderer>();
                
                Vector3[] linearLine = new Vector3[2];
                for (int j = 0; j < 2; j++)
                {
                    linearLine[j] = curveLinesPoints[j + 2 * i];
                }
                lineRenderer.SetPositions(linearLine);
            }
            //edit height animationcurve
            for (int i = 0; i < terrainGenerator.terrainData.meshHeightCurve.length; i++)
            {
                Keyframe[] keyframes = terrainGenerator.terrainData.meshHeightCurve.keys;
                keyframes[i].time = curveDots[i].rectTransform.localPosition.x / heightCurvePanel.sizeDelta.x;
                keyframes[i].value = curveDots[i].rectTransform.localPosition.y / heightCurvePanel.sizeDelta.y;
                terrainGenerator.terrainData.meshHeightCurve.keys = keyframes;
            }
            terrainGenerator.GenerateMap();
        }

        void DestroyHeightCurve()
        {
            foreach (Transform child in heightCurvePanel.transform) {
                Destroy(child.gameObject);
            }
            curveDots.Clear();
            curveLines.Clear();
            handleDots.Clear();
            handleLines.Clear();
            curveLinesPoints.Clear();
        }
        void ChangedHeightCurve()
        {
            DestroyHeightCurve();
            CreateHeightCurve();
            terrainGenerator.GenerateMap();
        }
        public void SetLinearHeightCurve()
        {
            if(terrainGenerator.terrainData != terrainDatas[0])
            {
                terrainGenerator.terrainData = terrainDatas[0];
                ChangedHeightCurve();
            }
        }
        public void SetLinear1HeightCurve()
        {
            if (terrainGenerator.terrainData != terrainDatas[1])
            {
                terrainGenerator.terrainData = terrainDatas[1];
                ChangedHeightCurve();
            }
        }
        public void SetLinear2HeightCurve()
        {
            if (terrainGenerator.terrainData != terrainDatas[2])
            {
                terrainGenerator.terrainData = terrainDatas[2];
                ChangedHeightCurve();
            }
        }
        public void EnableDisableMeshHeightCurveEdit()
        {
            heightCurveEdit.SetActive(!heightCurveEdit.activeInHierarchy);
        }
        public void RandomizeColor()
        {
            for (int i = 0; i < terrainGenerator.colorData.regions.Length; i++)
            {
                terrainGenerator.colorData.regions[i].colour = Random.ColorHSV();
            }

            terrainGenerator.GenerateMap();
        }

        public void RandomizeNoise()
        {
            //check for lockins
            if(seedLock.activeInHierarchy)
                seedSlider.value = Random.Range(-100000, 100000);
            if(scaleLock.activeInHierarchy)
                scaleSlider.value = Random.Range(1, 501);
            if(lacunarityLock.activeInHierarchy)
                lacunaritySlider.value = Random.Range(1f, 2.6f);
            if(persistanceLock.activeInHierarchy)
                persistanceSlider.value = Random.Range(0f, 1.01f);
            if(octavesLock.activeInHierarchy)
                octavesSlider.value = Random.Range(1, 7);
        }

        public void RandomizeTerrain()
        {
            //check for lockins
            if(heightLock.activeInHierarchy)
                heightMeshMultiplierSlider.value = Random.Range(0, 350);
            if(fallOffLock.activeInHierarchy)
                useFallOffToggle.isOn = (Random.value > 0.5f);
            if(fallOnLock.activeInHierarchy)
                useFallOnToggle.isOn = (Random.value > 0.5f);
            if(flatShadingLock.activeInHierarchy)
                flatShadingToggle.isOn = (Random.value > 0.5f);
        }

        void RemoveRegions()
        {
            if (regionsPanel.childCount > 0)
            {
                for (int i = 0; i < regionsPanel.childCount; i++)
                {
                    Destroy(regionsPanel.GetChild(i).gameObject);
                }
            }
        }

        public void RegionsRemoveButton()
        {
            RemoveRegions();
            if (terrainGenerator.colorData.regions.Length > 0)
            {
                //remember the regions
                int colorDataRegionCount = terrainGenerator.colorData.regions.Length;
                string[] colorDataName = new string[colorDataRegionCount];
                float[] colorDataHeight = new float[colorDataRegionCount];
                Color[] colorDataColour = new Color[colorDataRegionCount];
                for (int i = 0; i < colorDataRegionCount; i++)
                {
                    colorDataName[i] = terrainGenerator.colorData.regions[i].name;
                    colorDataHeight[i] = terrainGenerator.colorData.regions[i].height;
                    colorDataColour[i] = terrainGenerator.colorData.regions[i].colour;
                }

                terrainGenerator.colorData.regions = new ColorData.TerrainType[colorDataRegionCount - 1];
                for (int i = 0; i < colorDataRegionCount - 1; i++)
                {
                    terrainGenerator.colorData.regions[i].name = colorDataName[i];
                    terrainGenerator.colorData.regions[i].height = colorDataHeight[i];
                    terrainGenerator.colorData.regions[i].colour = colorDataColour[i];
                }
            }

            CreateRegions();
        }

        public void RegionsAddButton()
        {
            RemoveRegions();
            if (terrainGenerator.colorData.regions.Length > 0)
            {
                //remember the regions
                int colorDataRegionCount = terrainGenerator.colorData.regions.Length;
                string[] colorDataName = new string[colorDataRegionCount];
                float[] colorDataHeight = new float[colorDataRegionCount];
                Color[] colorDataColour = new Color[colorDataRegionCount];
                for (int i = 0; i < colorDataRegionCount; i++)
                {
                    colorDataName[i] = terrainGenerator.colorData.regions[i].name;
                    colorDataHeight[i] = terrainGenerator.colorData.regions[i].height;
                    colorDataColour[i] = terrainGenerator.colorData.regions[i].colour;
                }

                terrainGenerator.colorData.regions = new ColorData.TerrainType[colorDataRegionCount + 1];
                for (int i = 0; i < colorDataRegionCount; i++)
                {
                    terrainGenerator.colorData.regions[i].name = colorDataName[i];
                    terrainGenerator.colorData.regions[i].height = colorDataHeight[i];
                    terrainGenerator.colorData.regions[i].colour = colorDataColour[i];
                }
            }

            CreateRegions();
        }

        void CreateRegions()
        {
            for (int i = 0; i < terrainGenerator.colorData.regions.Length; i++)
            {
                GameObject panel = new GameObject("Panel");
                panel.AddComponent<CanvasRenderer>();
                Image imagePanel = panel.AddComponent<Image>();
                Color tempColor = imagePanel.color;
                tempColor.a = 0.2f;
                imagePanel.color = tempColor;
                panel.transform.SetParent(regionsPanel.transform, false);
                RectTransform panelRect = panel.transform.GetComponent<RectTransform>();
                panelRect.anchorMin = new Vector2(0.5f, 1);
                panelRect.anchorMax = new Vector2(0.5f, 1);
                panelRect.pivot = new Vector2(0.5f, 1);
                panelRect.anchoredPosition = new Vector2(0, 0 - (i * (regionsPanel.sizeDelta.y / terrainGenerator.colorData.regions.Length)));
                panelRect.sizeDelta = new Vector2(regionsPanel.sizeDelta.x, regionsPanel.sizeDelta.y / terrainGenerator.colorData.regions.Length);

                //first one is the name of the regions so a input field again
                GameObject regionName = new GameObject("RegionNameInputField" + i);
                regionName.AddComponent<CanvasRenderer>();
                Image regionImage = regionName.AddComponent<Image>();
                regionImage.type = Image.Type.Sliced;
                regionImage.fillCenter = true;
#if UNITY_EDITOR
                regionImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
#endif
                TMP_InputField regionInputField = regionName.AddComponent<TMP_InputField>();
                regionInputField.text = terrainGenerator.colorData.regions[i].name;
                regionName.transform.SetParent(panel.transform, false);
                RectTransform regionNameRect = regionName.transform.GetComponent<RectTransform>();
                regionNameRect.anchorMin = new Vector2(0, 0.5f);
                regionNameRect.anchorMax = new Vector2(0, 0.5f);
                regionNameRect.pivot = new Vector2(0, 0.5f);
                regionNameRect.anchoredPosition = new Vector2(5, 0);
                regionNameRect.sizeDelta = new Vector2(110, regionsPanel.sizeDelta.y / terrainGenerator.colorData.regions.Length);
                regionName.AddComponent<NameInputField>();

                GameObject textAreaRegionName = new GameObject("RegionNameTextArea");
                textAreaRegionName.AddComponent<RectMask2D>();
                RectTransform textAreaRectTransform = textAreaRegionName.GetComponent<RectTransform>();
                textAreaRegionName.transform.SetParent(regionName.transform, false);
                textAreaRectTransform.pivot = new Vector2(0.5f, 0.5f);
                textAreaRectTransform.anchorMin = Vector2.zero;
                textAreaRectTransform.anchorMax = Vector2.one;
                textAreaRectTransform.offsetMin = new Vector2(0, textAreaRectTransform.offsetMin.y);
                textAreaRectTransform.offsetMax = new Vector2(0, textAreaRectTransform.offsetMax.y);
                textAreaRectTransform.offsetMax = new Vector2(textAreaRectTransform.offsetMax.x, 0);
                textAreaRectTransform.offsetMin = new Vector2(textAreaRectTransform.offsetMin.x, 0);

                GameObject placeholderRegionName = new GameObject("RegionNamePlaceholder");
                placeholderRegionName.AddComponent<CanvasRenderer>();
                TextMeshProUGUI textMesProPlaceholder = placeholderRegionName.AddComponent<TextMeshProUGUI>();
                RectTransform placeholderRectTransform = placeholderRegionName.GetComponent<RectTransform>();
                placeholderRegionName.transform.SetParent(textAreaRegionName.transform, false);
                placeholderRectTransform.pivot = new Vector2(0.5f, 0.5f);
                placeholderRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                placeholderRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                placeholderRectTransform.sizeDelta = new Vector2(110, regionsPanel.sizeDelta.y / terrainGenerator.colorData.regions.Length);
                textMesProPlaceholder.alignment = TextAlignmentOptions.Center;
                textMesProPlaceholder.color = Color.gray;
                textMesProPlaceholder.fontSize = 12;
                textMesProPlaceholder.text = terrainGenerator.colorData.regions[i].name;

                GameObject textRegionName = new GameObject("RegionNameText");
                textRegionName.AddComponent<CanvasRenderer>();
                TextMeshProUGUI textMeshoProRegionName = textRegionName.AddComponent<TextMeshProUGUI>();
                RectTransform textRectTransform = textRegionName.GetComponent<RectTransform>();
                textRegionName.transform.SetParent(textAreaRegionName.transform, false);
                textRectTransform.pivot = new Vector2(0.5f, 0.5f);
                textRectTransform.anchorMin = Vector2.zero;
                textRectTransform.anchorMax = Vector2.one;
                textRectTransform.offsetMin = new Vector2(0, textRectTransform.offsetMin.y);
                textRectTransform.offsetMax = new Vector2(0, textRectTransform.offsetMax.y);
                textRectTransform.offsetMax = new Vector2(textRectTransform.offsetMax.x, 0);
                textRectTransform.offsetMin = new Vector2(textRectTransform.offsetMin.x, 0);
                textMeshoProRegionName.alignment = TextAlignmentOptions.Center;
                textMeshoProRegionName.color = Color.black;
                textMeshoProRegionName.fontSize = 12;

                regionInputField.textViewport = textAreaRectTransform;
                regionInputField.textComponent = textMeshoProRegionName;
                regionInputField.placeholder = textMesProPlaceholder;

                //second one is the height of the regions so a input field also
                GameObject regionHeight = new GameObject("RegionHeightInputField" + i);
                regionHeight.AddComponent<CanvasRenderer>();
                Image regionHeightImage = regionHeight.AddComponent<Image>();
                regionHeightImage.type = Image.Type.Sliced;
                regionHeightImage.fillCenter = true;
#if UNITY_EDITOR
                regionHeightImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
#endif
                TMP_InputField regionHeightInputField = regionHeight.AddComponent<TMP_InputField>();
                regionHeightInputField.text = terrainGenerator.colorData.regions[i].height.ToString();
                regionHeight.transform.SetParent(panel.transform, false);
                RectTransform regionHeightNameRect = regionHeight.transform.GetComponent<RectTransform>();
                regionHeightNameRect.anchorMin = new Vector2(0.5f, 0.5f);
                regionHeightNameRect.anchorMax = new Vector2(0.5f, 0.5f);
                regionHeightNameRect.pivot = new Vector2(0.5f, 0.5f);
                regionHeightNameRect.anchoredPosition = new Vector2(-5, 0);
                regionHeightNameRect.sizeDelta = new Vector2(90, regionsPanel.sizeDelta.y / terrainGenerator.colorData.regions.Length);
                regionHeight.AddComponent<HeightInputField>();

                GameObject textAreaRegionHeight = new GameObject("RegionHeightTextArea");
                textAreaRegionHeight.AddComponent<RectMask2D>();
                RectTransform textAreaHeightRectTransform = textAreaRegionHeight.GetComponent<RectTransform>();
                textAreaRegionHeight.transform.SetParent(regionHeight.transform, false);
                textAreaHeightRectTransform.pivot = new Vector2(0.5f, 0.5f);
                textAreaHeightRectTransform.anchorMin = Vector2.zero;
                textAreaHeightRectTransform.anchorMax = Vector2.one;
                textAreaHeightRectTransform.offsetMin = new Vector2(0, textAreaHeightRectTransform.offsetMin.y);
                textAreaHeightRectTransform.offsetMax = new Vector2(0, textAreaHeightRectTransform.offsetMax.y);
                textAreaHeightRectTransform.offsetMax = new Vector2(textAreaHeightRectTransform.offsetMax.x, 0);
                textAreaHeightRectTransform.offsetMin = new Vector2(textAreaHeightRectTransform.offsetMin.x, 0);

                GameObject placeholderRegionHeight = new GameObject("RegionHeightPlaceholder");
                placeholderRegionHeight.AddComponent<CanvasRenderer>();
                TextMeshProUGUI textMesProPlaceholderHeight = placeholderRegionHeight.AddComponent<TextMeshProUGUI>();
                RectTransform placeholderRectTransformHeight = placeholderRegionHeight.GetComponent<RectTransform>();
                placeholderRegionHeight.transform.SetParent(textAreaRegionHeight.transform, false);
                placeholderRectTransformHeight.pivot = new Vector2(0.5f, 0.5f);
                placeholderRectTransformHeight.anchorMin = new Vector2(0.5f, 0.5f);
                placeholderRectTransformHeight.anchorMax = new Vector2(0.5f, 0.5f);
                placeholderRectTransformHeight.sizeDelta = new Vector2(110, regionsPanel.sizeDelta.y / terrainGenerator.colorData.regions.Length);
                textMesProPlaceholderHeight.alignment = TextAlignmentOptions.Center;
                textMesProPlaceholderHeight.color = Color.gray;
                textMesProPlaceholderHeight.fontSize = 12;
                textMesProPlaceholderHeight.text = terrainGenerator.colorData.regions[i].height.ToString();

                GameObject textRegionHeight = new GameObject("RegionHeightText");
                textRegionHeight.AddComponent<CanvasRenderer>();
                TextMeshProUGUI textMeshoProRegionHeight = textRegionHeight.AddComponent<TextMeshProUGUI>();
                RectTransform textRectTransformHeight = textRegionHeight.GetComponent<RectTransform>();
                textRegionHeight.transform.SetParent(textAreaRegionHeight.transform, false);
                textRectTransformHeight.pivot = new Vector2(0.5f, 0.5f);
                textRectTransformHeight.anchorMin = Vector2.zero;
                textRectTransformHeight.anchorMax = Vector2.one;
                textRectTransformHeight.offsetMin = new Vector2(0, textRectTransformHeight.offsetMin.y);
                textRectTransformHeight.offsetMax = new Vector2(0, textRectTransformHeight.offsetMax.y);
                textRectTransformHeight.offsetMax = new Vector2(textRectTransformHeight.offsetMax.x, 0);
                textRectTransformHeight.offsetMin = new Vector2(textRectTransformHeight.offsetMin.x, 0);
                textMeshoProRegionHeight.alignment = TextAlignmentOptions.Center;
                textMeshoProRegionHeight.color = Color.black;
                textMeshoProRegionHeight.fontSize = 12;

                regionHeightInputField.textViewport = textAreaHeightRectTransform;
                regionHeightInputField.textComponent = textMeshoProRegionHeight;
                regionHeightInputField.placeholder = textMesProPlaceholderHeight;

                GameObject regionColor = new GameObject("RegionColor" + i);
                regionColor.AddComponent<ColorImage>();
                ColorPicker regionColorPicker = regionColor.AddComponent<ColorPicker>();
                regionColor.AddComponent<Image>();
                regionColor.AddComponent<Button>();
                RectTransform regionColorRectTransform = regionColor.GetComponent<RectTransform>();
                regionColor.transform.SetParent(panel.transform, false);
                regionColorRectTransform.pivot = new Vector2(1f, 0.5f);
                regionColorRectTransform.anchorMin = new Vector2(1f, 0.5f);
                regionColorRectTransform.anchorMax = new Vector2(1f, 0.5f);
                regionColorRectTransform.anchoredPosition = new Vector2(-5, 0);
                regionColorRectTransform.sizeDelta = new Vector2(110, (regionsPanel.sizeDelta.y / terrainGenerator.colorData.regions.Length)-3);
                regionColorPicker.enabled = false;
                regionColorPicker.region = regionInputField;
            }
        }

        public void FallOnChanged()
        {
            terrainGenerator.terrainData.useFallon = useFallOnToggle.isOn;
            terrainGenerator.GenerateMap();
        }

        public void FallOffChanged()
        {
            terrainGenerator.terrainData.useFalloff = useFallOffToggle.isOn;
            terrainGenerator.GenerateMap();
        }

        public void FlatShadingChanged()
        {
            terrainGenerator.terrainData.useFlatShading = flatShadingToggle.isOn;
            terrainGenerator.GenerateMap();
        }

        public void HeightMeshMultiplierChanged()
        {
            terrainGenerator.terrainData.meshHeightMultiplier = heightMeshMultiplierSlider.value;
            terrainGenerator.GenerateMap();
        }

        public void OctavesChanged()
        {
            terrainGenerator.noiseData.octaves = (int) octavesSlider.value;
            terrainGenerator.GenerateMap();
        }

        public void PersistanceChanged()
        {
            terrainGenerator.noiseData.persistance = persistanceSlider.value;
            terrainGenerator.GenerateMap();
        }

        public void LacunarityChanged()
        {
            terrainGenerator.noiseData.lacunarity = lacunaritySlider.value;
            terrainGenerator.GenerateMap();
        }

        public void ScaleChanged()
        {
            terrainGenerator.noiseData.scale = (int) scaleSlider.value;
            terrainGenerator.GenerateMap();
        }

        public void SeedChanged()
        {
            terrainGenerator.noiseData.seed = (int) seedSlider.value;
            terrainGenerator.GenerateMap();
        }
    }
}