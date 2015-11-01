using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShootBolt : MonoBehaviour
{
	public Transform SpawnPos;
	public float boltSpeed = 50.0f;
	public float reloadTime = 1.0f;
	public float minfirePrepare = 2.0f;
	public float maxfirePrepare = 4.0f;
	public List<GameObject> bolts;
	/*********************/
	private GameObject reload = null;
	private bool auto = false;
	private float nextReload;
	private float nextFire;
	private AudioSource fireSound;

	void Awake ()
	{
		fireSound = GetComponent<AudioSource> ();
	}

	void Update ()
	{
		if (reload) {
			if (Time.time >= nextFire) {
				reload.GetComponent<Rigidbody> ().isKinematic = false;
				reload.GetComponent<Rigidbody> ().velocity = SpawnPos.forward * boltSpeed;
				nextReload = Time.time + reloadTime;
				reload = null;
				fireSound.Play ();
			}
		} else if (auto) {
			if (Time.time >= nextReload) {
				int index = Random.Range (0, bolts.Count);
				reload = Instantiate (bolts [index], SpawnPos.position, SpawnPos.rotation) as GameObject;
				reload.transform.SetParent (SpawnPos);
				nextFire = Time.time + Random.Range (minfirePrepare, maxfirePrepare);
			}
		}
	}

	void OnPause ()
	{
		auto = false;
	}

	void OnRestart ()
	{
		auto = true;
	}
}