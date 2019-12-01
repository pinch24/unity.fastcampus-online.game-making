using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Camera Properties")]
    private Transform myTransform = null;
    public GameObject Target = null;
    private Transform targetTransform = null;

    public enum CameraViewPointState { FIRST, SECOND, THIRD }
    public CameraViewPointState CameraState = CameraViewPointState.THIRD;

    [Header("3rd View Camera")]
    public float Distance = 5.0f;
    public float Height = 1.5f;
    public float HeightDamping = 2.0f;
    public float RotationDamping = 3.0f;

    [Header("2nd View Camera")]
    public float RotateSpeed = 10.0f;

    [Header("1nd View Camera")]
    public float SensitivityX = 5.0f;
    public float SensitivityY = 5.0f;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    public Transform FirstCameraSocket = null;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();

        if (Target != null)
        {
            targetTransform = Target.transform;
        }
    }

    void ThirdView()
    {
        float wantedRotationAngle = targetTransform.eulerAngles.y;
        float wantedHeight = targetTransform.position.y + Height;

        float currentRotationAngle = myTransform.eulerAngles.y;
        float currentHeight = myTransform.position.y;

        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, RotationDamping * Time.deltaTime);
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, HeightDamping * Time.deltaTime);
        
        Quaternion currentRotation = Quaternion.Euler(0.0f, currentRotationAngle, 0f);
        
        myTransform.position = targetTransform.position;
        myTransform.position -= currentRotation * Vector3.forward * Distance;
        myTransform.position = new Vector3(myTransform.position.x, currentHeight, myTransform.position.z);
        
        myTransform.LookAt(targetTransform);
    }

    void SecondView()
    {
        myTransform.RotateAround(targetTransform.position, Vector3.up, RotateSpeed * Time.deltaTime);
        myTransform.LookAt(targetTransform);
    }

    void FirstView()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationX = myTransform.localEulerAngles.y + mouseX * SensitivityX;
        rotationX = (rotationX > 180.0f) ? rotationX - 360.0f: rotationX;
        
        rotationY = rotationY + mouseY * SensitivityY;
        rotationY = (rotationY > 180.0f) ? rotationY - 360.0f: rotationY;

        myTransform.localEulerAngles = new Vector3(-rotationY, rotationX, 0.0f);
        myTransform.position = FirstCameraSocket.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        else if (targetTransform == null)
        {
            targetTransform = Target.transform;
        }

        switch (CameraState)
        {
            case CameraViewPointState.THIRD:
                ThirdView();
                break;

            case CameraViewPointState.SECOND:
                SecondView();
                break;

            case CameraViewPointState.FIRST:
                FirstView();
                break;
        }
    }
}
