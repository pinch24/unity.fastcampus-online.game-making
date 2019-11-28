using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public float EnemySpeed = 50.0f;
    
    private Transform myTransform = null;

    public GameObject Explosion = null;

    void Start()
    {
        myTransform = GetComponent<Transform>();
    }

    void Update()
    {
        Vector3 moveAmount = EnemySpeed * Vector3.back * Time.deltaTime;
        myTransform.Translate(moveAmount);

        if (myTransform.position.y < -50.0f)
        {
            InitPosition();
        }
    }

    void InitPosition()
    {
        myTransform.position = new Vector3(Random.Range(-60.0f, 60.0f), 50.0f, 0.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BULLET")
        {
            Debug.Log("Bullet Trigger Enter");

            MainControl.Score += 100;
            
            Instantiate(Explosion, myTransform.position, Quaternion.identity);

            InitPosition();

            Destroy(other.gameObject);
        }
    }
}
