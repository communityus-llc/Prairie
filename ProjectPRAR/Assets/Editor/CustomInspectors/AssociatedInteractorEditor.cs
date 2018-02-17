using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AssociatedInteractor))]
public class AssociatedInteractorEditor : Editor {

	AssociatedInteractor associatedInteractor;

	public void Awake()
	{
		this.associatedInteractor = (AssociatedInteractor)target;
	}

	public override void OnInspectorGUI ()
	{
		// Configuration:
		bool _repeatable = EditorGUILayout.Toggle ("Repeatable?", associatedInteractor.repeatable);
		GameObject[] _associatedColliders = PrairieGUI.drawObjectList<GameObject> ("Colliders to Trigger:", associatedInteractor.associatedColliders);

		// Save:
		if (GUI.changed) {
			Undo.RecordObject(associatedInteractor, "Allow Object-to-Twine Interaction");
			associatedInteractor.repeatable = _repeatable;
			associatedInteractor.associatedColliders = _associatedColliders;
		}

		// Warnings (after properties have been updated):
		this.DrawWarnings();
	}

	public void DrawWarnings()
	{
		foreach (GameObject obj in associatedInteractor.associatedColliders)
		{
			if (obj == null)
			{
				PrairieGUI.warningLabel ("You have one or more empty slots in your list of toggles.  Please fill these slots or remove them.");
				break;
			}
		}
	}
}
