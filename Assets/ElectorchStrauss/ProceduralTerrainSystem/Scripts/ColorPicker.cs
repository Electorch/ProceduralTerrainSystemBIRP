using UnityEngine;
using System;
using System.Text.RegularExpressions;
using TMPro;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
    public class ColorPicker : MonoBehaviour
    {
        [SerializeField] private Texture2D colorPicker;
        [SerializeField]private int ImageWidth = 400;
        [SerializeField]private int ImageHeight = 400;
        private ColorData colorData;
        private MapGenerator terrainGenerator;
        private Rect rect;
        private Color col;
        private float xx=9;
        public TMP_InputField region;
        private string regionName;
        private void Start()
        {
            terrainGenerator = GameObject.Find("TerrainGenerator").GetComponent<MapGenerator>();
            colorData = terrainGenerator.colorData;
            colorPicker = Resources.Load<Texture2D>("ColorPicker");
        }

        private void Update()
        {
            regionName=region.text;
        }

        void OnGUI()
        {
            GUI.Label(new Rect(Screen.width - 395, Screen.height - 515, 150, 20), regionName);
            
            if (GUI.Button(new Rect(Screen.width - 45, Screen.height - 535, 40, 40), "X"))
            {
                enabled = false;
            }
            
            
            if (GUI.RepeatButton(new Rect(Screen.width-ImageWidth, Screen.height-ImageHeight-95, ImageWidth, ImageHeight), colorPicker))
            {
                Vector2 pickpos = Event.current.mousePosition;
                int aaa = Convert.ToInt32(pickpos.x-Screen.width-ImageWidth+xx);
                int bbb = Convert.ToInt32(pickpos.y-Screen.height-ImageHeight+95);
                
                Color col = colorPicker.GetPixel(aaa, -bbb);
                
                string resultString = Regex.Match(transform.name, @"\d+").Value;
                int value = int.Parse(resultString);
                colorData.regions[value].colour = col;
                terrainGenerator.GenerateMap();
            }
        }
    }
}