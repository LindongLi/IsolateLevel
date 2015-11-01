using UnityEngine;
using System.Collections;

public class ConveyorSpeed : MonoBehaviour
{
	public float speed = 5f;
	public float friction = 0.8f;
	/***************************/
	private float oldspeed = 0f;

	private void BroadCast ()
	{
		BroadcastMessage ("OnUpdateVariable");
	}

	void OnPause ()
	{
		if (speed != 0f) {
			oldspeed = speed;
			speed = 0f;
			BroadCast ();
		}
	}

	void OnRestart ()
	{
		if (speed != oldspeed) {
			speed = oldspeed;
			BroadCast ();
		}
	}

	void OnReverse ()
	{
		if (speed != 0f) {
			speed = -speed;
			BroadCast ();
		}
	}
}