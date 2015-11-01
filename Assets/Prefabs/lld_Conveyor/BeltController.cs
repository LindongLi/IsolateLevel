using UnityEngine;
using System.Collections;

public class BeltController : MonoBehaviour
{
	public GameObject father;
	/***********************/
	private float friction;
	private float speed;
	private AudioSource frictionSound;

	void Start ()
	{
		float distance = father.transform.localScale.x;
		float radian = Mathf.Min (distance, father.transform.localScale.z);
		transform.localScale = new Vector3 (2f - radian / distance, transform.localScale.y, 1f);
		OnUpdateVariable ();
		frictionSound = father.GetComponent<AudioSource> ();
		frictionSound.Pause ();
	}
	
	void OnUpdateVariable ()
	{
		friction = father.GetComponent<ConveyorSpeed> ().friction;
		speed = father.GetComponent<ConveyorSpeed> ().speed;
	}

	private void HandleCollision (Collision other)
	{
		Vector3 normal = Vector3.zero, direct;
		foreach (ContactPoint contact in other.contacts) {
			normal += contact.point;
		}
		normal = other.transform.position * other.contacts.Length - normal;
		if (Vector3.Dot (normal, transform.forward) > 0f) {
			normal = transform.forward;
			direct = transform.right;
		} else {
			normal = -transform.forward;
			direct = -transform.right;
		}
		float speedtogo = speed - Vector3.Dot (direct, other.relativeVelocity);
		if (speedtogo != 0f) {
			float impulse = Vector3.Dot (normal, other.impulse) * friction / (other.rigidbody.mass * Mathf.Abs (speedtogo));
			other.rigidbody.velocity += direct * speedtogo * Mathf.Min (1f, impulse);
		}
	}
	
	void OnCollisionEnter (Collision other)
	{
		HandleCollision (other);
	}
	
	void OnCollisionStay (Collision other)
	{
		HandleCollision (other);
		if ((speed != 0f) && !frictionSound.isPlaying) {
			frictionSound.UnPause ();
		}
	}

	void OnCollisionExit ()
	{
		frictionSound.Pause ();
	}
}