using UnityEngine;
using System.Collections;

public class WheelController : MonoBehaviour
{
	public GameObject father;
	public GameObject child;
	/**********************/
	private float friction;
	private float speed;
	private float radian;
	private float rotate;
	private AudioSource frictionSound;

	void Start ()
	{
		float distance = father.transform.localScale.x;
		radian = Mathf.Min (distance, father.transform.localScale.z);
		transform.Translate (transform.right * (distance - radian) * 0.5f, Space.World);
		transform.localScale = new Vector3 (radian / distance, transform.localScale.y, 1f);
		OnUpdateVariable ();
		frictionSound = father.GetComponent<AudioSource> ();
	}

	void OnUpdateVariable ()
	{
		friction = father.GetComponent<ConveyorSpeed> ().friction;
		speed = father.GetComponent<ConveyorSpeed> ().speed;
		rotate = speed * 60f / (Mathf.PI * radian);
	}

	void Update ()
	{
		child.transform.Rotate (0f, rotate * Time.deltaTime, 0f);
	}

	private void HandleCollision (Collision other)
	{
		Vector3 normal = Vector3.zero, direct;
		foreach (ContactPoint contact in other.contacts) {
			normal += contact.point;
		}
		normal = Vector3.Normalize (other.transform.position * other.contacts.Length - normal);
		direct = Vector3.Cross (gameObject.transform.up, normal);
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