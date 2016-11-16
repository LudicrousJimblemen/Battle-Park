﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class GridPlaceholder : MonoBehaviour {
	private NetworkClient client;
	private new Camera camera;
	private Grid grid;
	
	public GridObject GridObject;
	
	public int Owner;
	
	void Start() {
		client = FindObjectOfType<Client>().NetworkClient;
		camera = FindObjectOfType<Camera>();
		grid = FindObjectsOfType<Grid>().First(x => x.PlayerId == Owner);
		GridObject = GetComponent<GridObject>();
	}
	
	public void Snap() {
		if (GridObject == null) {
			return;
		}
		
		transform.rotation = Quaternion.Euler(-90, 0, (int)GridObject.Direction * 90);
		
		transform.position = new Vector3 { //snap to grid
			x = Mathf.Round(transform.position.x / grid.GridXZ) * grid.GridXZ,
			z = Mathf.Round(transform.position.z / grid.GridXZ) * grid.GridXZ,
			y = Mathf.Round(Mathf.Clamp(transform.position.y / grid.GridY, 0, Mathf.Infinity)) * grid.GridY
		};
	}
	
	public void PlaceObject() {
		Vector3 SnappedPos = new Vector3(Mathf.RoundToInt(transform.position.x / grid.GridXZ), 
			                     Mathf.RoundToInt(transform.position.y / grid.GridY),
			                     Mathf.RoundToInt(transform.position.z / grid.GridXZ));
		//if the snapped position, ie the desired spot to place the object at, is null, it's o k to place it
		//otherwise, it's no good
		if (grid.Objects.OccupiedIn(SnappedPos)) {
			return;
		}
		GridObject.X = (int)SnappedPos.x;
		GridObject.Y = (int)SnappedPos.y;
		GridObject.Z = (int)SnappedPos.z;
		client.Send(GridObjectPlacedNetMessage.Code, new GridObjectPlacedNetMessage() {
			//N A M E ( C L O N E )
			//0 1 2 3 4 5 6 7 8 9 10
			Type = name.Substring(0, name.Length - 7),
			ObjectData = GridObject.Serialize()
		});
		//grid.Objects.Add (SnappedPos, GridObject);
		FindObjectOfType<Client>().CanSummon = true;
		Destroy(gameObject);
	}
	
	public void Rotate(int direction) {
		GridObject.Direction += direction;
		
		if (GridObject.Direction > (Direction)3) {
			GridObject.Direction = (Direction)0;
		}
		if (GridObject.Direction < (Direction)0) {
			GridObject.Direction = (Direction)3;
		}
	}
	
	public void Position(bool UseVerticalConstraint = false) {
		RaycastHit hit;
		bool hasHit;
		if (UseVerticalConstraint) {
			if (hasHit = Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, grid.VerticalConstrainRaycastLayerMask)) {
				//if (hit.collider.GetComponent<Grid> ().playerId == client.connection.connectionId)
				transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
			}
			gameObject.SetActive(hasHit);
		} else {
			if (grid == null)
				return;
			if (hasHit = Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, grid.RaycastLayerMask)) {
				if (hit.collider.GetComponent<Grid>().PlayerId == FindObjectOfType<Client>().PlayerId)
					transform.position = hit.point;
			}
			gameObject.SetActive(hasHit);
		}
	}
}