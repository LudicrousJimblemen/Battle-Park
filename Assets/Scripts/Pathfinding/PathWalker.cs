﻿using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Pathfinding {
	[RequireComponent(typeof(CharacterController))]
	public class PathWalker : MonoBehaviour {
		public Action DestinationReachedCallback;
		public bool DestinationReached;
		
		public NodeGraph graph;
		public float Speed = 5;
		public float TurnSpeed = 10;
		public float TurnDistance = 1;
		public float StopDistance = 0.1f;
		public float GiveUpTime = 5f;
		public float RepathRate = 1f;
		
		public Transform Target;
		public int PathPriority; // the priority to be used in pathfinding
		
		[HideInInspector]
		public bool Wandering;

		public bool Influenceable; // can the walker be influenced by other walkers?
		private const float Mass = 50; // mass of the walker in kg, arbitrary
		private Vector3 ExtInf = Vector3.zero; // external influence on the walker, internal

		private CharacterController controller;
		private Animator anim;

		public void Awake() {
			controller = GetComponent<CharacterController>();
			anim = GetComponent<Animator>();
			Wandering = true;
		}
		public void Start() {
			if (graph == null) {
				graph = FindObjectsOfType<NodeGraph>().First();
			}
		}
		public void OnEnable() {
			StartCoroutine("Repath");
		}
		public void OnDisable() {
			StopCoroutine("Repath");
		}
		public void SetDestination(Vector3 destination) {
			graph.RequestPath(transform, destination, FollowPath);
		}
		public void SetDestination(Transform destination) {
			graph.RequestPath(transform, destination.position, FollowPath);
		}
		public void Wander() {
			Wandering = true;
			if (followPathRoutine != null) {
				StopCoroutine(followPathRoutine);
			}
		}
		// RUN F O R E V E R
		IEnumerator Repath() {
			Transform lastTarget = null;
			while (true) {
				if(Target != null) {
					SetDestination(Target);
					lastTarget = Target;
				}
				yield return new WaitForSeconds(RepathRate);
			}
		}
		public void Influence (Vector3 force) {
			if (Influenceable && controller != null && controller.enabled) {
				ExtInf += force / Mass;
			}
		}
		public void Update () {
			if(Influenceable && controller != null && controller.enabled) {
				controller.SimpleMove(ExtInf);
				ExtInf = Vector3.Lerp(ExtInf,Vector3.zero,50*Time.deltaTime);
			}
		}
		public IEnumerator followPathRoutine;
		public void FollowPath(Path path, bool success) {
			if (!success)
				return;
			if (followPathRoutine != null) {
				StopCoroutine(followPathRoutine);
			}
			followPathRoutine = _followPath(path);
			StartCoroutine(followPathRoutine);
		}
		IEnumerator _followPath(Path path) {
			Wandering = false;
			DestinationReached = false;
			int count = path.Count;
			int waypointIndex = 0; // index of next node to go towards
			bool following = true;
			float speedPercent = 1f;
			float giveUpTimer = 0;
			Vector3 lastPos = transform.position;
			while (following) {
				float frameSpeedPercent = speedPercent;
				Vector3 adjustedPos = transform.position;
				if (Vector3.SqrMagnitude(transform.position - path[waypointIndex].Position) < TurnDistance * TurnDistance) {
					if (waypointIndex != path.Count - 1) {
						waypointIndex++;
					}
				}
				if (waypointIndex == path.Count - 1) {
					speedPercent = Mathf.Clamp01(Vector3.Distance(transform.position, path[waypointIndex].Position) / StopDistance);
					frameSpeedPercent = speedPercent;
					if (speedPercent < 0.1f) {
						following = false;
						break;
					}
				}
				int mask = 1 << 12; // person mask
				Collider[] overlap = Physics.OverlapSphere(transform.position + controller.center,controller.radius,mask);
				for(int p = 0; p < overlap.Length; p++) {
					//print(overlap[p].name);
					overlap[p].GetComponent<PathWalker> ().Influence ((overlap[p].transform.position - transform.position).Flat().normalized * 10);
				}
				Vector3 flatLook = path[waypointIndex].Position - transform.position;
				flatLook.y = 0;
				Quaternion targetRot = Quaternion.LookRotation(flatLook.normalized);
				transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, TurnSpeed * Time.deltaTime);
				if (controller != null) {
					controller.SimpleMove(transform.forward * Speed * frameSpeedPercent);
					if (anim != null) {
						anim.SetFloat("Speed", frameSpeedPercent * 0.9f);
					}
					if (Vector3.SqrMagnitude(transform.position - lastPos) < 0.01f * 0.01f) {
						giveUpTimer += Time.deltaTime;
						if (giveUpTimer >= GiveUpTime) {
							following = false;
							print("Unit gave up");
							break;
						}
					} else {
						giveUpTimer = 0;
					}
				} else {
					transform.Translate(transform.forward * Time.deltaTime * Speed * frameSpeedPercent, Space.World);
				}
				lastPos = transform.position;
				yield return null;
			}
			DestinationReached = true;
			StopInternal();
		}
		public void Stop() {
			if (followPathRoutine != null) {
				StopCoroutine(followPathRoutine);
			}
			StopInternal();
		}
		private void StopInternal() {
			Target = null;
			if (anim != null) {
				anim.SetFloat("Speed", 0);
			}
			Wandering = false;
			followPathRoutine = null;
		}
	}
}