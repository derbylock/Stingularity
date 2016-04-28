using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour
{
	public string team;
	public int number;
	public Vector3 initialPos;
	public Vector3 initialHeading;	
	public Vector2 lastInputVelocity;
	public InputControl inputCtrl;

	public Flag flag;
	public SpawnPad spawnpoint;

	const float runSpeed = 6.5f;
	const float tackleAveSpeed = 7f;
	const float tackleDuration = 0.55f;
	const float tackleCooldown = 0.85f;
	const float respawnTime = 1f;

	public Vector2 heading = Vector2.up;
	public float currentBoost = 1f;
	public float tackleStartTime;
	public Vector2 tackleDirection;
	public bool tackling = false;
	public bool hasKnockback = false;

	public bool dead;
	public bool invincible;
	public bool frozen = false;
	public bool canGrabFlag = true;
	public bool canRespawn = true;

	public GameObject itemIcon;
	public int itemNo=-1;

 	static private List<string> dropItems =
	    new List<string> {
	   	"Turret",
	   	"Decoy",
	   	"Shockwave",
	   	"Boost"
	};

	public bool carrying {
		get {
			return flag != null;
		}
	}

	public string otherTeam {
		get {
			return Manager.S.teams.Where(t => t != team).Single();
		}
	}

	public SpriteRenderer spriteRenderer {
		get {
			return GetComponent<SpriteRenderer>();
		}
	}

	void Awake() {
		inputCtrl = InputControl.S;
	}

	void Update () {
		if (dead)
			return;
		CheckDrop();
		CheckTackle();
		MoveStep();
		UpdateRotation();
		CheckInvincible();
	}

	void MoveStep()
	{
		if (frozen)
			return;

		if (hasKnockback) {
			if (GetComponent<Rigidbody2D>().velocity.magnitude < 0.1f) {
				//unset hasKnockback after we've stopped moving
				hasKnockback = false;
			}
		}
		else {
			float tackleProg = (Time.time - tackleStartTime)/tackleDuration;
			float curve = (float)Math.Cos(tackleProg * Math.PI);
			float speed = runSpeed;
			if (tackleProg > 1f) {
				tackling = false;
				if (tackleProg < 1.5f) {
					//ease up normal running speed as "standing up" after tackle
					speed = runSpeed + (runSpeed * curve);
				}
			}
			if (tackling) {
				float tackleCurSpeed = tackleAveSpeed + (tackleAveSpeed * curve);
				GetComponent<Rigidbody2D>().velocity = tackleDirection * tackleCurSpeed * currentBoost;
				Turn(tackleDirection);
			} 
			else {
				Vector2 velocity = inputCtrl.RunVelocity(number);
				speed *= (carrying ? 0.9f : 1f);
				bool moving = velocity.magnitude > 0.2f;
				bool stopping = !moving && lastInputVelocity.magnitude > 0.1f;
				if (moving || stopping) {
					GetComponent<Rigidbody2D>().velocity = velocity.normalized * speed * currentBoost;
					if (moving)
						Turn(velocity);
				}
				lastInputVelocity = velocity;
			}
		}
	}

	void Turn(Vector2 newHeading) {
		heading = Vector2.Lerp(heading.normalized, newHeading.normalized, 0.7f).normalized;
	}

    void UpdateRotation()
    {
        transform.rotation = (heading * -1).ToQuaternion();
    }

	void CheckDrop() {
		if (!Manager.S.itemPickups) {
			dropItems.Each ((item, indexFromZero) => {
				int index = indexFromZero + 1;
				if (inputCtrl.ItemButtonDown (number, index)) {
						DropNewItem (item);
				}
			});
		} else {
			for (int i=0; i < 4; ++i) {
				if (itemNo != -1 && inputCtrl.ItemButtonDown(number, i+1)) {
					DropNewItem (dropItems[itemNo-1]);
					itemNo = -1;
				}
			}
		}
	}

	public void DropNewItem(string itemName)
	{
		GameObject go = (GameObject)Instantiate(Resources.Load(itemName));
		Vector3 newPos = transform.position;
		newPos.z = go.transform.position.z;
		go.transform.position = newPos;

		DropItem dropItem = go.GetComponent<DropItem>();
		if (!Manager.S.itemPickups) {
			if (Manager.S.teamManas [team] >= 1 && dropItem.TryDrop (this)) {
				Manager.S.teamManas [team] -= 1;
			} else { 
				Destroy (go);
			}
		} else if(dropItem.TryDrop(this))
			Destroy (itemIcon);
	}

	void CheckTackle() {
		float timeSinceTackle = Time.time - tackleStartTime;
		if (tackling || timeSinceTackle < tackleCooldown)
			return;
		Vector2 tackleForce = inputCtrl.TackleVelocity(number);
		if (tackleForce.magnitude >= 1f) {
			Tackle(tackleForce);
		}
	}

	void Tackle(Vector2 force) {
		tackleDirection = force.normalized;
		tackleStartTime = Time.time;
		tackling = true;
		hasKnockback = false;
	}

	void CheckInvincible() {
		Color c = spriteRenderer.material.color;
		c.a = invincible ? 0.5f : 1;
		spriteRenderer.material.color = c;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		Player p = coll.gameObject.GetComponent<Player>();
		if (p && p.team != team && p.tackling) {
			if (!tackling) {
				Die();
			}
			else AudioManager.Main.PlayNewSound("bounce");
		    if (tackling && carrying) {
				flag.Drop();
			}
		}
		if (tackling) {
			hasKnockback = true;
			if(!p) AudioManager.Main.PlayNewSound("bounce");				
		}
	}
	
	public void Die()
	{
		if (!invincible)
			StartCoroutine(DieCoroutine());
	}

	public IEnumerator DieCoroutine()
	{
		dead = true;
		spriteRenderer.enabled = false;
		GetComponent<Collider2D>().enabled = false;
		if (carrying) {
			flag.Drop();
		}
		
		GetComponent<ParticleSystem>().startColor = Manager.S.teamColors[team];
		GetComponent<ParticleSystem>().Play();
		AudioManager.Main.PlayNewSound ("YouDiedLol");
		yield return new WaitForSeconds(respawnTime);
		if (!canRespawn) {
			Destroy(gameObject);
			yield break;
		}
		Reset();
		invincible = true;
		yield return new WaitForSeconds(1);
		invincible = false;
	}

	public void Reset()
	{
		spriteRenderer.enabled = true;
		GetComponent<Collider2D>().enabled = true;
		dead = false;
		if (spawnpoint) {
			transform.position = spawnpoint.transform.position;
			Destroy(spawnpoint.gameObject);
			spawnpoint = null;
		}
		else {
			heading = initialHeading;
			transform.position = initialPos;
		}
		flag = null;
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		itemNo = -1;
		if (itemIcon)
			Destroy (itemIcon);
	}

	public void InvalidateSpawn()
	{
		spawnpoint = null;
	}
}
