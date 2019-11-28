using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
	public float BulletSpeed = 100.0f;

	private Transform myTransform = null;
    
    public GameObject bulletPrefab = null;

    void Start()
    {
        myTransform = GetComponent<Transform>();
    }

    void Update()
    {
        Vector3 moveAmount = BulletSpeed * Vector3.up * Time.deltaTime;
		myTransform.Translate(moveAmount);

		if (myTransform.position.y > 60.0f)
        {
			Destroy(gameObject);
		}
    }
}
