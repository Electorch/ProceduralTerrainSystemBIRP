using UnityEngine;
using UnityEditor;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
	public class UpdatableData : ScriptableObject
	{
		public event System.Action OnValuesUpdated;
		public bool autoUpdate;

		protected virtual void OnValidate()
		{
#if UNITY_EDITOR

			if (autoUpdate)
			{
				EditorApplication.update += NotifyOfUpdatedValues;
			}
#endif
		}

		public void NotifyOfUpdatedValues()
		{
#if UNITY_EDITOR
			EditorApplication.update -= NotifyOfUpdatedValues;
			if (OnValuesUpdated != null)
			{
				OnValuesUpdated();
			}
#endif
		}
	}
}