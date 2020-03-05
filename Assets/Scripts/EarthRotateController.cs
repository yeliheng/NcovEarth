using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * 2020-03-03
 * 控制旋转
 * Written By Yeliheng
 */
public class EarthRotateController : MonoBehaviour {
	private Rigidbody rb;
	public Vector3 angleVelocity = new Vector3(0, 5, 0);
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		Quaternion deltaRotation = Quaternion.Euler(angleVelocity * Time.deltaTime);
		rb.MoveRotation(rb.rotation * deltaRotation);
	}
}
