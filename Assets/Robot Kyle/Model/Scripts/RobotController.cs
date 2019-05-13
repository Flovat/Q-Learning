using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour {


    private Animator animator;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float h = Input.GetAxis("Horizontal");
        animator.SetFloat("Direction", h);

        float v = Input.GetAxis("Vertical");
        animator.SetFloat("Speed", v);
    }
}
