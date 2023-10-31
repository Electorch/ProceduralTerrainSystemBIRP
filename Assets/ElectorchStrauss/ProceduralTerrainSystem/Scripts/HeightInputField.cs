using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
    public class HeightInputField : MonoBehaviour
    {
        private MapGenerator terrainGenerator;
        private TMP_InputField _inputField;

        private ColorData colorData;

        // Checks if there is anything entered into the input field.
        void LockInput(TMP_InputField input)
        {
            if (input.text.Length > 0)
            {
                string resultString = Regex.Match(transform.name, @"\d+").Value;
                int value = int.Parse(resultString);
                colorData.regions[value].height = float.Parse(_inputField.text);
                terrainGenerator.GenerateMap();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            terrainGenerator = GameObject.Find("TerrainGenerator").GetComponent<MapGenerator>();
            colorData = terrainGenerator.colorData;
            _inputField = GetComponent<TMP_InputField>();
            _inputField.onValueChanged.AddListener(delegate { LockInput(_inputField); });
        }
    }
}