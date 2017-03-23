﻿using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {
	public float health = 211f; //max is 255
	public float radiated = 0f;
	public float resetAfterDeathTime = 0.5f;
	public float timer;
	public static bool playerDead = false;
	public bool mediPatchActive = false;
	public bool detoxPatchActive = false;
	public AudioSource PainSFX;
	public AudioClip PainSFXClip;
	public GameObject cameraObject;
	public GameObject hardwareShield;
	private bool shieldOn = false;
	public bool radiationArea = false;
	private float radiationBleedOffFinished = 0f;
	public float radiationBleedOffTime = 1f;
	public float radiationReductionAmount = 1f;
	public float radiationHealthDamageRatio = 0.2f;
	public GameObject mainPlayerParent;
	public int radiationAmountWarningID = 323;
	public int radiationAreaWarningID = 322;
	
	void Update (){
		if (health <= 0f) {
			if (!playerDead) {
				PlayerDying();
			} else {
				PlayerDead();
			}
		}

		if (radiated > 0) {
			TextWarningsManager twm = mainPlayerParent.GetComponent<PlayerReferenceManager>().playerTextWarningManager.GetComponent<TextWarningsManager>();
			if (radiationArea) twm.SendWarning(("Radiation Area"),0.1f,-2,TextWarningsManager.warningTextColor.white,radiationAreaWarningID);
			twm.SendWarning(("Radiation poisoning "+radiated.ToString()+" LBP"),0.1f,-2,TextWarningsManager.warningTextColor.red,radiationAmountWarningID);
		}

		if (radiated < 1) {
			radiationArea = false;
		}

		if (radiationBleedOffFinished < Time.time) {
			if (radiated > 0) {
				health -= radiated*radiationHealthDamageRatio*radiationBleedOffTime; // apply health at rate of bleedoff time
				if (!radiationArea) radiated -= radiationReductionAmount;  // bleed off the radiation over time
				radiationBleedOffFinished = Time.time + radiationBleedOffTime;
			}
		}
	}
	
	void PlayerDying (){
		timer += Time.deltaTime;
		
		if (timer >= resetAfterDeathTime) {
			health = 0f;
			playerDead = true;
		}
	}
	
	void PlayerDead (){
		//gameObject.GetComponent<PlayerMovement>().enabled = false;
		//cameraObject.SetActive(false);
		cameraObject.GetComponent<Camera>().enabled = false;
		Cursor.lockState = CursorLockMode.None;
	}
	
	public void TakeDamage ( float take  ){
		float shieldBlock = 0f;
		if (shieldOn) {
			//shieldBlock = hardwareShield.GetComponent<Shield>().GetShieldBlock();
		}
		take = Const.a.GetDamageTakeAmount(take,0,shieldBlock,Const.AttackType.None,false,0,0);
		health -= take;
		PainSFX.PlayOneShot(PainSFXClip);
		//print("Player Health: " + health.ToString());
	}

	public void GiveRadiation (float rad) {
		if (radiated < rad)
			radiated = rad;

		//radiated -= suitReduction;
	}
}