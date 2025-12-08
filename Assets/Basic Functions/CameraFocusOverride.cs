using System.Collections;
using UnityEngine;

public class CameraFocusOverride : MonoBehaviour
{
    Transform camTransform;
    Vector3 originalPos;
    Quaternion originalRot;
    Coroutine running;

    [Tooltip("How quickly the camera moves to focus")]
    public float focusSpeed = 6f;

    [Tooltip("How far in front of the monster the camera should sit")]
    public Vector3 focusOffset = new Vector3(0f, 1.2f, -2.0f);

    void Awake()
    {
        camTransform = this.transform;
    }

    // Call this to focus on target for seconds seconds. This starts a coroutine automatically.
    public void FocusOnTransform(Transform target, float seconds)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(FocusRoutine(target, seconds));
    }

    IEnumerator FocusRoutine(Transform target, float seconds)
    {
        // save original transform so we can restore later
        originalPos = camTransform.position;
        originalRot = camTransform.rotation;

        float timer = 0f;

        // Calculate desired position relative to the target
        Vector3 desiredPos = target.position + target.transform.TransformDirection(focusOffset);
        Quaternion desiredRot = Quaternion.LookRotation(target.position - desiredPos, Vector3.up);

        // smooth move to desired
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * focusSpeed;
            camTransform.position = Vector3.Lerp(originalPos, desiredPos, t);
            camTransform.rotation = Quaternion.Slerp(originalRot, desiredRot, t);
            yield return null;
        }

        // stay focused for the duration
        yield return new WaitForSeconds(seconds);

        // restore smoothly
        t = 0f;
        Vector3 startPos = camTransform.position;
        Quaternion startRot = camTransform.rotation;
        while (t < 1f)
        {
            t += Time.deltaTime * focusSpeed;
            camTransform.position = Vector3.Lerp(startPos, originalPos, t);
            camTransform.rotation = Quaternion.Slerp(startRot, originalRot, t);
            yield return null;
        }

        running = null;
    }
}
