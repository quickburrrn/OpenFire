using UnityEngine;

public class CameraShake : MonoBehaviour
{
    //Vector3 cameraInitialPosition;
    public float shakeMagnitude = 0f, shakeTime = 0f;
    public Camera mainCamera;

    public void ShakeIt(float magnitude, float time)
    {
        //cameraInitialPosition = transform.position;
        shakeMagnitude = magnitude ; shakeTime = time;
        InvokeRepeating("StartCameraShaking", 0f, 0.005f);
        Invoke("StopCameraShaking", shakeTime);
    }

    public void ConstantShake(float magnitude, float time)
    {
        //cameraInitialPosition = transform.position;
        shakeMagnitude = magnitude; shakeTime = time;
        InvokeRepeating("StartCameraShaking", 0f, 0.005f);
    }

    void StartCameraShaking()
    {
        //Debug.Log(shakeMagnitude);
        float cameraShakingOffsetX = Random.value * shakeMagnitude * 2 -shakeMagnitude;
        float cameraShakingOffsetY = Random.value * shakeMagnitude * 2 - shakeMagnitude;
        //Debug.Log(cameraShakingOffsetY + " " + cameraShakingOffsetX);

        Vector3 cameraIntermadiatePosition = transform.position;
        cameraIntermadiatePosition.x += cameraShakingOffsetX;
        cameraIntermadiatePosition.y += cameraShakingOffsetY;
        transform.position = cameraIntermadiatePosition;
    }

    public void StopCameraShaking() 
    {
        CancelInvoke("StartCameraShaking");
        //transform.position = cameraInitialPosition; 
    }
}
