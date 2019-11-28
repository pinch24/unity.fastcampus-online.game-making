using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float Speed = 15.0f;
    
    private Transform myTransform = null;

    public GameObject BulletPrefab = null;

    void Start()
    {
        myTransform = GetComponent<Transform>();
    }

    void Update()
    {
		float axis = Input.GetAxis("Horizontal");
		// Debug.Log("AXID: " + axis);

		Vector3 moveAmount = axis * Speed * -Vector3.right * Time.deltaTime;
		myTransform.Translate(moveAmount);

        if (Input.GetButtonDown("Fire1")) // Input.GetKeyDown(KeyCode.Space) == true
        {
            Instantiate(BulletPrefab, myTransform.position, Quaternion.identity);
        }
    }
}
