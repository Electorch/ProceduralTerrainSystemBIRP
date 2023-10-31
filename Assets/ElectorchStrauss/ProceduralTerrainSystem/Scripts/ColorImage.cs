using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
    public class ColorImage : MonoBehaviour
    {
        private Button _button;
        private ColorPicker[] _colorPickers;
        private ColorPicker _colorPicker;
        private ColorData colorData;
        private Image _image;
        private MapGenerator terrainGenerator;

        // Start is called before the first frame update
        void Start()
        {
            terrainGenerator = GameObject.Find("TerrainGenerator").GetComponent<MapGenerator>();
            colorData = terrainGenerator.colorData;
            _colorPicker = GetComponent<ColorPicker>();
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
            _button.onClick.AddListener(delegate { LockInput(_button); });
        }
        ColorPicker[] getColorPicker()
        {
            ColorPicker[] scripts = FindObjectsOfType<ColorPicker>();
            ColorPicker[] colorPicker = new ColorPicker[scripts.Length];
            for (int i = 0; i < colorPicker.Length; i++)
                colorPicker[i] = scripts[i];
            return colorPicker;
        }

        // Checks if there is anything entered into the input field.
        void LockInput(Button input)
        {
            if (getColorPicker().Length > 0)
            {
                for (int i = 0; i < getColorPicker().Length; i++)
                {
                    getColorPicker()[i].enabled = false;
                }
            }

            //check if any _colorPicker are enabled and disable them before enabling another one
            _colorPicker.enabled = true;
        }
        private void Update()
        {
            string resultString = Regex.Match(transform.name, @"\d+").Value;
            int value = int.Parse(resultString);
            Color tempColour = colorData.regions[value].colour;
            tempColour.a = 255f;
            _image.color = tempColour;
        }
    }
}