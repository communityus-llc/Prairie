using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ComponentToggleTransform))]
public class ComponentToggleTransformEditor : Editor {

	ComponentToggleTransform componentToggle;

	public void Awake()
	{
		this.componentToggle = (ComponentToggleTransform)target;
	}

	public override void OnInspectorGUI ()
	{
		// Configuration:
		bool _repeatable = EditorGUILayout.Toggle ("Repeatable?", componentToggle.repeatable);
		GameObject[] _targets = PrairieGUI.drawObjectList<GameObject> ("Objects To Transform:", componentToggle.targets);
		int _trX = EditorGUILayout.IntField("X-axis transl amount:", componentToggle.trX);
		int _trY = EditorGUILayout.IntField("Y-axis transl amount:", componentToggle.trY);
		int _trZ = EditorGUILayout.IntField("Z-axis transl amount:", componentToggle.trZ);

		// Save:
		if (GUI.changed) {
			Undo.RecordObject(componentToggle, "Modify Component Translation");
			componentToggle.repeatable = _repeatable;
			componentToggle.targets = _targets;
			componentToggle.trX = _trX;
			componentToggle.trY = _trY;
			componentToggle.trZ = _trZ;
		}

		// Warnings (after properties have been updated):
		this.DrawWarnings();
	}

	public void DrawWarnings()
	{
		foreach (GameObject obj in componentToggle.targets)
		{
			if (obj == null)
			{
				PrairieGUI.warningLabel ("You have one or more empty slots in your list of toggles.  Please fill these slots or remove them.");
				break;
			}
		}
	}
}
