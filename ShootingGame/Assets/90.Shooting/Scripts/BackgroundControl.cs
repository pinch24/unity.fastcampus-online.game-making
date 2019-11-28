using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundControl : MonoBehaviour
{
    public float ScrollSpeed = 0.1f;

    public Renderer myRenderer = null;

    void Start()
    {
        myRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        myRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.0f, -Time.time * ScrollSpeed));
    }
}
