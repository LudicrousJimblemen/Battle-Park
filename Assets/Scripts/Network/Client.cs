﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour {
	private NetworkManager networkManager;
	
	public NetworkClient NetworkClient;

	void Start() {
		networkManager = FindObjectOfType<NetworkManager>();
		StartClient();
	}
	
	void Update() {
		if (Input.GetKeyDown(KeyCode.Q)) {
			SummonGridObject("Sculpture");
		}
		if (Input.GetKeyDown(KeyCode.W)) {
			SummonGridObject("Tree");
		}
	}
	
	public void SummonGridObject(string name) {
		GameObject newGridObject = (GameObject)Instantiate(Resources.Load("Prefabs/" + name));
		
		GridPlaceholder component = newGridObject.AddComponent<GridPlaceholder>();
		component.Type = name;
	}

	public void StartClient() {
		NetworkClient = new NetworkClient();
		NetworkClient.RegisterHandler(ChatNetMessage.Code, OnChatNetMessage);
		NetworkClient.RegisterHandler(GridObjectPlacedNetMessage.Code, OnGridObjectPlacedNetMessage);
		NetworkClient.Connect(networkManager.Ip, networkManager.Port);
	}
	
	public void OnChatNetMessage(NetworkMessage incoming) {
		 ChatNetMessage message = incoming.ReadMessage<ChatNetMessage>();
		 print(message.Message);
	}
	
	public void OnGridObjectPlacedNetMessage(NetworkMessage incoming) {
		GridObjectPlacedNetMessage message = incoming.ReadMessage<GridObjectPlacedNetMessage>();
		
		GameObject newGridObject = (GameObject)Instantiate(Resources.Load("Prefabs/" + message.Type));
		GridObject component = newGridObject.GetComponent<GridObject>();
		
		component.Direction = message.Direction;
		component.Position = message.Position;
		
		component.IsScenery = message.IsScenery;
		component.IsNice = message.IsNice;
	}
}
