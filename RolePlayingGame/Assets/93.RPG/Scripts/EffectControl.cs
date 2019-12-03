using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectControl : MonoBehaviour
{
    public float radius = 5.0f;
    public float power = 200.0f;
    public float upwardModifier = 3.0f;
    
    private void Start()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        
        foreach (Collider col in colliders)
        {
            if (col.gameObject.CompareTag("PlayerAttack") == true)
            {
                continue;
            }
            
            Rigidbody rigid = col.GetComponent<Rigidbody>();
            if (rigid != null)
            {
                rigid.AddExplosionForce(power, explosionPos, radius, upwardModifier);
            }
        }
    }
}
