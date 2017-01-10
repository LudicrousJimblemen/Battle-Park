﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using BattlePark.Core;

namespace BattlePark.Menu {
	public class TitleScreenGUI : MonoBehaviour {
		public Image Fade;
		public Image FirstPanel;
		public Image Logo;
		public List<Text> Buttons;
	
		public Image CurrentPanel;
	
		public Image MainPanel;
		public Image ClientPanel;
	
		public Button StartClientButton;
		public Button ExitButton;
	
		public Button ClientBackButton;
		public InputField ClientIpInputField;
		public InputField ClientPortInputField;
		public InputField ClientUsernameInputField;
		public Button ClientJoinButton;
	
		private bool inAnimation;
	
		public void Awake() {
			StartCoroutine(FadeGraphic(Fade, 0, 60f, Color.white, new Color(1f, 1f, 1f, 0)));
			StartCoroutine(FadeGraphic(MainPanel, 70f, 40f, Color.clear, new Color(0f, 0f, 0f, 0.125f)));
			StartCoroutine(FadeGraphic(Logo, 100f, 40f, new Color(1f, 1f, 1f, 0), Color.white));
			for (int i = 0; i < Buttons.Count; i++) {
				StartCoroutine(FadeGraphic(Buttons[i], 100f + (10f * i), 60f, new Color(1f, 1f, 1f, 0), Color.white));
			}
		}
	
		private void Update() {
			Fade.raycastTarget = inAnimation;
		}
	
		public IEnumerator FadeGraphic(Graphic graphic, float delay, float duration, Color fromColor, Color toColor, bool disableRaycast = false, Action callback = null) {
			for (int i = 0; i < duration + delay; i++) {
				inAnimation = true;
				graphic.color = Color.Lerp(fromColor, toColor, Mathf.SmoothStep(0f, 1f, (i - delay) / duration));
				yield return null;
			}
			inAnimation = disableRaycast;
			
			if (callback != null) {
				callback();
			}
		}
	
		public IEnumerator AnimatePanel(Image panel, int fromDirection) {
			inAnimation = true;
			panel.gameObject.SetActive(true);
			foreach (var thing in CurrentPanel.transform.GetComponentsInChildren<Selectable>()) {
				thing.interactable = false;
			}
			foreach (var thing in panel.transform.GetComponentsInChildren<Selectable>()) {
				thing.interactable = true;
			}
		
			panel.rectTransform.position = new Vector3(644f * fromDirection, 0, 0);
			for (int i = 0; i < 70; i++) {
				panel.rectTransform.localPosition = Vector3.Lerp(
					new Vector3(panel.rectTransform.rect.width * fromDirection, 0, 0),
					Vector3.zero,
					Mathf.SmoothStep(0, 1f, Mathf.SmoothStep(0, 1f, i / 70f))
				);
				CurrentPanel.rectTransform.localPosition = Vector3.Lerp(
					Vector3.zero,
					new Vector3(CurrentPanel.rectTransform.rect.width * -fromDirection, 0, 0),
					Mathf.SmoothStep(0, 1f, Mathf.SmoothStep(0, 1f, i / 70f))
				);
				yield return null;
			}
		
			CurrentPanel.gameObject.SetActive(false);
			CurrentPanel = panel;
			inAnimation = false;
		}
	
		public void GenerateUsername() {
			string consonants = "bbbbbbbbbbbbbbbbbbbbbbbcdffgghjklmnppppppppppppppppppppppqrsssstvwxzzzz";
			string vowels = "aaeeiiiiiooooooooooouuuuuuuuuuuuuy";
			int type = Mathf.RoundToInt(UnityEngine.Random.Range(0, 1));

			string returnedName = String.Empty;
			for (int i = 0; i < 14; i++) {
				if (i != 7) {
					float chance = UnityEngine.Random.value;
					if (type == 0) {
						returnedName += consonants.ElementAt(UnityEngine.Random.Range(0, consonants.Length));
						if (chance <= 0.5) {
							type = 1;
						}
					} else {
						returnedName += vowels.ElementAt(UnityEngine.Random.Range(0, vowels.Length));
						if (chance <= 0.6) {
							type = 0;
						}
					}
				} else {
					returnedName += " ";
				}
			}
		
			ClientUsernameInputField.text = returnedName;
		}
	
		/*
		public void FindIP() {
			try {
				IPHostEntry host;
				host = Dns.GetHostEntry(Dns.GetHostName());
				foreach (IPAddress ip in host.AddressList) {
					if (ip.AddressFamily == AddressFamily.InterNetwork) {
						ClientIpInputField.text = ip.ToString();
					}
				}
				ClientIpInputField.text = "127.0.0.1";
			} catch {
				ClientIpInputField.text = "127.0.0.1";
			}
		}
		*/
	}
}