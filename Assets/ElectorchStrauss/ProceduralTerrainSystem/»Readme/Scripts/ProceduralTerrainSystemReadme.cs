using System;
using UnityEngine;

public class ProceduralTerrainSystemReadme : ScriptableObject {
	public Texture2D icon;
	public float iconMaxWidth = 128f;
	public string title;
	public Section[] sections;
	
	[Serializable]
	public class Section {
		public string heading, text, linkText, url;
	}
}
