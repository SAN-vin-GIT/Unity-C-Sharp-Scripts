using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rightarm : MonoBehaviour {

	public void ChangeWeapon(Weapon weaponToEquip)
	{
		Destroy(GameObject.FindGameObjectWithTag("Weapon"));
		Instantiate(weaponToEquip, transform.position, transform.rotation, transform);
	}
}
