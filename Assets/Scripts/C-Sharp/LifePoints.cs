using UnityEngine;
using System.Collections;

public class LifePoints : MonoBehaviour {
	
	public C_PlayerMan playerType;
	public M_TankController controller;
	
	[SerializeField]
	private float energyPoints = 100f;
	public float maxRegEnergyPoints = 100f;
	public float maxEnergyPoints;
	public float moveEnergyConsumption = 2f;
	public float shootEnergyConsumption = 5f;
	
	[SerializeField]
	private float shieldPoints = 100f;
	public float maxRegShieldPoints = 100f;
	public float maxShieldPoints;
	
	public float energyRegenerationRate = 10f;
	public float shieldRegenerationRate = 5f;
	public bool inBase;
	public float otherBaseDivider = 2f;
	
	void Awake () {
	
		if (!playerType) playerType = gameObject.GetComponent <C_PlayerMan> ();
		if (!controller) controller = gameObject.GetComponent <M_TankController> ();
		
		if (!Network.isServer) {		
			enabled = false;	
		}
		
		maxEnergyPoints = maxRegEnergyPoints * (4f/3f);
		maxShieldPoints = maxRegShieldPoints * (4f/3f);
		
	}
	
	void Update () {
	
		energyPoints = Mathf.Clamp (energyPoints, 0f, maxEnergyPoints);
		shieldPoints = Mathf.Clamp (shieldPoints, 0f, maxShieldPoints);

		if (controller.isMoving && !inBase && energyPoints > 0) {
		
			ChangeEnergy (-moveEnergyConsumption);
		
		}
	
	}
	
	public void ChangeEnergy (float amount) {
	
		energyPoints += amount * Time.deltaTime;
	
	}
	
	public float GetEnergy () {
	
		return energyPoints;
	
	}
	
	public void ChangeShield (float amount) {
	
		shieldPoints += amount * Time.deltaTime;
	
	}
	
	public bool ApplyDamage (float amount) {
	
		shieldPoints -= amount;
		if (shieldPoints < 0) {
			return true;
		} else {
			return false;
		}
	
	}
	
	public float GetShield () {
	
		return shieldPoints;
	
	}
	
	void OnTriggerStay (Collider other) {
	
		if (playerType.IsMyBase (other.tag)) {		
			inBase = true;
			if (energyPoints < maxRegEnergyPoints) ChangeEnergy (energyRegenerationRate);
			if (shieldPoints < maxRegShieldPoints) ChangeShield (shieldRegenerationRate);	
		} else if (!playerType.IsMyBase (other.tag)) {		
			inBase = true;
			if (energyPoints < maxRegEnergyPoints) ChangeEnergy (energyRegenerationRate / otherBaseDivider);
			if (shieldPoints < maxRegShieldPoints) ChangeShield (shieldRegenerationRate / otherBaseDivider);	
		}
	
	}
	
	void OnTriggerExit (Collider other) {
	
		if (playerType.IsMyBase (other.tag)) {
			inBase = false;
		} else if (!playerType.IsMyBase (other.tag)) {
			inBase = false;
		}
	
	}
	
	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info) {
	
		if (stream.isWriting) {
		
			stream.Serialize (ref energyPoints);
			stream.Serialize (ref shieldPoints);
		
		}
	
	}
}