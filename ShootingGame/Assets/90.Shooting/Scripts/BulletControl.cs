using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
	public float BulletSpeed = 100.0f;
	private Transform myTransform = null;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveAmount = BulletSpeed * Vector3.up * Time.deltaTime;
		myTransform.Translate(moveAmount);

		// 탄환이 화면 밖으로 나갔으면,
		if (myTransform.position.y > 60.0f) {

			// 탄환을 제거한다.
			Destroy(gameObject); // gameObject: Bullet Prefab
		}
    }
}
