using UnityEngine;
using System.Collections;

public class FollowTransform : MonoBehaviour {
    public Transform FollowTarget = null;
    public bool FollowRotation = false;

    [SerializeField] private bool _matchStartingRotation = false;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _takeoffLerpDuration = 1f;      //lerp speed of the camera when banking 
    [SerializeField] private float _lerpDuration = 6f;      //lerp speed of the camera when banking 

    private Coroutine _lastLerpRoutine = null;

    private void Start() {
        if (_matchStartingRotation) {
            transform.rotation = FollowTarget.rotation;
        }
    }

    void FixedUpdate() {
        transform.position = FollowTarget.position + _offset;

        if (FollowRotation) {
            //var currentRotation = transform.eulerAngles;
            //currentRotation.x = Mathf.Lerp(currentRotation.x, PlayerCharacter.Instance.transform.rotation.x, Time.deltaTime);
            //transform.eulerAngles = currentRotation;
            //LerpCamera();

        }
    }

    public void LerpCamera() {
        transform.rotation = Quaternion.Slerp(transform.rotation, FollowTarget.rotation, _lerpDuration * Time.deltaTime); //TODO: would like this on 2 axes but using method in grounded movement didn't work
    }

    public void PositionCameraForTakeoff() {
        if (_lastLerpRoutine != null) {
            StopCoroutine(_lastLerpRoutine);
        }
        _lastLerpRoutine = StartCoroutine(LerpRotation(transform.rotation, FollowTarget.rotation, _takeoffLerpDuration));
    }

    IEnumerator LerpRotation(Quaternion currentRotation, Quaternion targetRotation, float duration) {
        float timeElapsed = 0;
        while (timeElapsed < duration) {
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}