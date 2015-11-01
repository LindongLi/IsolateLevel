using UnityEngine;
using System.Collections;

public class ElasticityController : MonoBehaviour
{
	public float bouncy = 1.2f;
	public float maxspeed = 90f;
	public Material bouncyEffect;
	/***************************/
	private Renderer render;
	private int bouncyOccur = 0;
	private Material oldMaterial;
	private AudioSource bouncySound;

	void Awake ()
	{
		render = GetComponent<Renderer> ();
		oldMaterial = render.material;
		bouncySound = GetComponent<AudioSource> ();
	}

	void Update ()
	{
		if (bouncyOccur > 0) {
			--bouncyOccur;
			if (bouncyOccur == 0) {
				render.material = oldMaterial;
			}
		}
	}

	void OnCollisionEnter (Collision other)
	{
		bouncySound.Play ();
		render.material = bouncyEffect;
		Vector3 normal = Vector3.zero;
		foreach (ContactPoint contact in other.contacts) {
			normal += contact.point;
		}
		normal = Vector3.Normalize (other.transform.position * other.contacts.Length - normal);
		float speedtogo = Vector3.Dot (normal, other.relativeVelocity) * (-bouncy);
		if (speedtogo > 0f) {
			other.rigidbody.velocity = Vector3.ClampMagnitude (other.rigidbody.velocity + normal * speedtogo, maxspeed);
		}
		bouncyOccur = 5;
	}
}