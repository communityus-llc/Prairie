﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

/// <summary>
/// Defines the Import Twine window and a few contextual menu actions to trigger import.
/// </summary>
public class TwineImporterUI : EditorWindow {

	public TextAsset targetFile;

	/// <summary>
	/// Defines the "Import Twine Data" menu item and its action.
	/// If triggered from a context menu on an HTML asset, the asset is automatically selected for import.
	/// </summary>
	[MenuItem("Assets/Import Twine Data...")]
	static void ShowTwineImportWindow () {

		// if triggered while a text asset is selected, populate it as the target file
		TextAsset selectedFile = null;
		if (Selection.activeObject != null) {
			var filePath = AssetDatabase.GetAssetPath (Selection.activeObject);
			if (Path.GetExtension (filePath) == "html") {
				selectedFile = (TextAsset) Selection.activeObject;
			}
		}

		// create and show window
		var window = EditorWindow.GetWindow<TwineImporterUI> ();
		window.targetFile = selectedFile;
	}

	/// <summary>
	/// Draws the GUI of the import window
	/// </summary>
	void OnGUI () {
		GUILayout.Label ("Import Twine Data", EditorStyles.boldLabel);

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Twine File:");

		// button which selects a target file
		var prompt = "Select File...";
		if (targetFile != null) {
			prompt = targetFile.name;
		}
		if (GUILayout.Button (prompt)) {
			var fullPath = EditorUtility.OpenFilePanel ("Select File", "Assets/", "html");

			// obnoxiously, the OpenFilePanel returns a full file path,
			// and Unity will only play nice with a relative one so we must convert
			var projectDirectory = Directory.GetParent (Application.dataPath).ToString ();
			var relativePath = GetRelativePath (fullPath, projectDirectory);

			// double check we'll have access to this file
			if (relativePath.StartsWith ("Assets/")) {
				this.targetFile = AssetDatabase.LoadAssetAtPath<TextAsset> (relativePath);
			} else {
				EditorUtility.DisplayDialog ("Can't Load Asset", "The file must be stored as part of your Unity project's assets.", "OK");
			}
		}

		GUILayout.EndHorizontal ();

		GUILayout.FlexibleSpace ();

		// button to send to importer
		GUI.enabled = (this.targetFile != null);
		if (GUILayout.Button ("Import")) {
			SendToImporter (this.targetFile);
			this.Close ();
		}
	}

	/// <summary>
	/// Converts a absolute path to a relative path from some other folder
	/// </summary>
	/// <returns>A relative path with the `folder` as the base</returns>
	/// <param name="filespec">The full path to a file inside of `folder`</param>
	/// <param name="folder">The folder to act as the root for the new path</param>
	private string GetRelativePath(string filespec, string folder) {
		Uri pathUri = new Uri(filespec);
		// Folders must end in a slash
		if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
		{
			folder += Path.DirectorySeparatorChar;
		}
		Uri folderUri = new Uri(folder);
		return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
	}

	/// <summary>
	/// Sends an html file to the Twine importer for input
	/// </summary>
	/// <param name="filePath">The file path of the Twine html to be imported</param>
	void SendToImporter (TextAsset file) {

		// =======    Link to Importer     ==========
		// 	TwineImporter.Import (file);
		// ==========================================

		Debug.Log ("Importing "+file.name+"...");

		// TODO: Read in the JSON string from a .json file loaded into the importer!
		// Hardcoded for now:
		var jsonString = "[{\"pid\":1,\"position\":{\"x\":505,\"y\":261},\"name\":\"Bedroom\",\"tags\":[\"warm\"],\"content\":\"You wake up in your bedroom.  [[Go to the kitchen]]  [[Go to the bathroom]] \",\"childrenNames\":[\"[[Go to the kitchen]]\",\"[[Go to the bathroom]]\"]},{\"pid\":2,\"position\":{\"x\":400,\"y\":450},\"name\":\"Go to the kitchen\",\"tags\":[\"yummy\"],\"content\":\"You make breakfast. It's very warm and nice.  [[Go to the bathroom]] [[Go to bed]] \",\"childrenNames\":[\"[[Go to the bathroom]]\",\"[[Go to bed]]\"]},{\"pid\":3,\"position\":{\"x\":700,\"y\":450},\"name\":\"Go to the bathroom\",\"tags\":[\"minty\"],\"content\":\"You brush your teeth. You have your favorite flavor of toothpaste.  [[Go to the CMC]] \",\"childrenNames\":[\"[[Go to the CMC]]\"]},{\"pid\":4,\"position\":{\"x\":400,\"y\":600},\"name\":\"Go to bed\",\"tags\":[\"wat\"],\"content\":\"Whelp, it's been a good morning. Time to go back to sleep I guess.\",\"childrenNames\":[]},{\"pid\":5,\"position\":{\"x\":700,\"y\":600},\"name\":\"Go to the CMC\",\"tags\":[\"fun\"],\"content\":\"COMPS MEETING TIME!!\",\"childrenNames\":[]}]\n";

		// TODO: (related to above todo) Change `jsonString` parameter to `file`:
		TwineJsonParser.ReadJson (jsonString);

		Debug.Log ("Done!");
	
	}

}