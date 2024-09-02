
using System;
using UnityEngine;

[Serializable]
public class SkillFlags {

	[SerializeField]
	private bool doubleDash;
	
	[SerializeField]
	private bool chargeAttack;

	public bool DoubleDash => doubleDash;

	public bool ChargeAttack => chargeAttack;
}
