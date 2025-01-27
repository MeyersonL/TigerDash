using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    private Quaternion startRotation;
    public float smoothSpeed = 0.125f;
    public float timeElapsed = 0;
    public bool inPuzzle = false;

    void Start() {
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.gray;
        RenderSettings.fogDensity = 0.25f;
    }

    void Update() {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        if (timeElapsed == 0) startRotation = transform.rotation;
        timeElapsed += Time.deltaTime;
        if (!inPuzzle) {
            transform.position = smoothedPosition;
            transform.LookAt(target);
        }
        else {
            transform.position = Vector3.Lerp(smoothedPosition, new Vector3(2.75f, 2.75f, -5.4f), timeElapsed * 2);
            transform.rotation = Quaternion.Slerp(startRotation, Quaternion.identity, timeElapsed * 2);
        }
    }
}