﻿using System.Collections;
using System.Linq;
using UnityEngine;

public class GridAttractionFunRide : GridAttraction {
	public override Money Cost { get { return new Money(600, 00); } }
	
	// TODO Fix offsets
	public override Vector3[] OccupiedOffsets {
		get {
			return GenerateOffsets(3, 3, 2);
		}
	}
	
	public override int MaximumPassengers { get { return 9; } }

	public override Attraction Attraction { get { return Attraction.FunRide; } }
	
	protected override string languageId { get { return "attractionFunRide"; } }
	
	private void Update() {
		if (!InCycle) {
			if (TimeSinceLastCycle >= MaximumWaitTime) {
				// TODO: Check if anyone is waiting to enter the ride, let them in
				StartCoroutine(StartCycle());
			} else {
				if (Passengers.Count(passenger => passenger != null) >= MinimumPassengers && TimeSinceLastCycle >= MinimumWaitTime) {
					// TODO: Check if anyone is waiting to enter the ride, let them in
					StartCoroutine(StartCycle());
				}
			}
		}
	}
	
	public IEnumerator StartCycle() {
		InCycle = true;
		for (float i = 0; i < 1f; i+= Time.deltaTime) {
			foreach (var slot in PassengerSlots) {
				slot.transform.parent.localPosition = new Vector3(0, Random.value * Mathf.SmoothStep(0, 2.5f, i), 0);
			}
			yield return null;
		}
		for (float i = 0; i < 8; i+= Time.deltaTime) {
			foreach (var slot in PassengerSlots) {
				slot.transform.parent.localPosition = new Vector3(0, Random.value * 2.5f, 0);
			}
			yield return null;
		}
		for (float i = 0; i < 1f; i+= Time.deltaTime) {
			foreach (var slot in PassengerSlots) {
				slot.transform.parent.localPosition = new Vector3(0, Random.value * Mathf.SmoothStep(2.5f, 0, i), 0);
			}
			yield return null;
		}
		EndCycle();
	}
}
