using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class lld_MainController : MonoBehaviour
{
	public Transform world;
	public Transform player;
	public float tiltRatio = 0.04f;
	public float cameraMaxLen = 20f;
	/**************************/
	const float maxzoom = 0.9f;
	private float zoomval = 0.3f;
	private float gravityTheta = 0f;
	private Vector3 currentup;
	private Vector3 currentlook;
	private Vector3 currentpos;
	private Vector3 accFilter;
	/************************/
	private Vector2 screenMid;
	private bool draging = false;
	private float lastGravityTheta;
	private Vector2 lastDragOffset;
	private bool zooming = false;
	private float lastZoomval;
	private float lastZoomOffset;

	void Awake ()
	{
		Input.gyro.enabled = true;
		Input.gyro.updateInterval = 1f / 30f;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		currentup = world.forward;
		currentlook = world.position;
		currentpos = currentlook + (1f - zoomval) * cameraMaxLen * world.up;
		/***************************/
		screenMid = new Vector2 (Screen.width >> 1, Screen.height >> 1);
	}

	void FixedUpdate ()
	{
		Vector3 desireUp = world.forward * Mathf.Cos (gravityTheta) + world.right * Mathf.Sin (gravityTheta);
		if (!desireUp.Equals (currentup)) {
			currentup = Vector3.RotateTowards (currentup, desireUp, 0.07f, 0f);
		}
		Vector3 desirelook = world.position + (zoomval / maxzoom) * (player.position - world.position);
		if (!desirelook.Equals (currentlook)) {
			currentlook += Vector3.ClampMagnitude (desirelook - currentlook, 0.5f);
		}
		Vector3 desirePos = desirelook + (1f - zoomval) * cameraMaxLen * world.up;
		if (!desirePos.Equals (currentpos)) {
			currentpos += Vector3.ClampMagnitude (desirePos - currentpos, 0.5f);
		}
	}

	void LateUpdate ()
	{
		switch (Input.touchCount) {
		case 1:
			Vector2 dragOffset = Input.GetTouch (0).position;
			dragOffset.Set (dragOffset.x + screenMid.x, dragOffset.y - screenMid.y);
			if (draging) {
				gravityTheta = lastGravityTheta + Vector2.Angle (lastDragOffset, dragOffset) / 60f;
			} else {
				lastGravityTheta = gravityTheta;
				lastDragOffset = dragOffset;
				draging = true;
			}
			zooming = false;
			break;
		case 2:
			float zoomOffset = Vector2.Distance (Input.GetTouch (0).position, Input.GetTouch (1).position);
			if (zooming) {
				zoomval = Mathf.Max (0f, Mathf.Min (maxzoom, lastZoomval + (zoomOffset - lastZoomOffset) * 0.001f));
			} else {
				lastZoomval = zoomval;
				lastZoomOffset = zoomOffset;
				zooming = true;
			}
			draging = false;
			break;
		default:
			draging = false;
			zooming = false;
			break;
		}

		Vector3 realgravity, realacc;
#if UNITY_EDITOR
		realgravity = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f) * 9.8f;
		realacc = Vector3.zero;
#else
		realgravity = Input.gyro.gravity * 19.6f;
		realacc = Input.gyro.userAcceleration * 9.8f;
#endif
		Vector3 currentright = Vector3.Cross (world.up, currentup);
		realgravity = Vector3.ClampMagnitude (currentright * realgravity.x + currentup * realgravity.y, 9.8f);
		realacc -= Vector3.ClampMagnitude (realacc, 5f);
		accFilter = accFilter * 0.6f + (currentright * realacc.x + currentup * realacc.y) * 25f;

		transform.position = currentpos - (1f - zoomval) * cameraMaxLen * tiltRatio * realgravity;
		transform.LookAt (currentlook, currentup);
		Physics.gravity = realgravity + accFilter;
	}
}