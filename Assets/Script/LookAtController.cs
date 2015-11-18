using UnityEngine;
using System.Collections;

public class LookAtController : MonoBehaviour
{
	public Transform follow;

	void Update ()
	{
		transform.LookAt (follow.position);
	}
}