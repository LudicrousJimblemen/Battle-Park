﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;
	public Vector3[] ParkCenters;
	public Vector3[] ParkGates;
	public NodeGraph[] Graphs;

	public GameObject[] PlayerObjectParents = new GameObject[2];
	public GameObject PersonObj;

	public GridObject[] Objects;
	public List<Person> Guests;
	
	// Discord is bad software because they denied our request to use Discord GameBridge.
	public List<ChatMessage> Chat;

	private void Awake() {
		if(FindObjectsOfType<GameManager>().Length > 1) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	/*
	private void OnGUI() {
		string text = String.Empty;
		foreach (var person in Guests) {
			text += person.Name + ":\n";
			foreach (var thought in person.Thoughts) {
				//Debug.Log(thought.ThoughtString);
				text += "   \"" + String.Format(LanguageManager.GetString(thought.ThoughtString), thought.Parameters);
			}
		}
		UnityEngine.GUI.Label(new Rect(5, 5, 666, 666), text);
	}
	*/
}
