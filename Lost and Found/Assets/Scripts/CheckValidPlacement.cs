using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckValidPlacement : MonoBehaviour {
    [SerializeField] ObjectCreator _objectCreatorScript = null;
    [SerializeField] private bool _isLeftHand;
    private GameObject _objectInBounds;

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Static Environment") {
            return;
        }

        _objectInBounds = other.gameObject;
        _objectCreatorScript.SetInvalidPlacement(_isLeftHand);
        Debug.LogWarning("placement is blocked because of " + other.name, gameObject);
    }

    private void OnTriggerExit(Collider other) {
        _objectInBounds = null;                                     //need to handle multiple things in a trigger at some point
        _objectCreatorScript.SetValidPlacement(_isLeftHand);
    }

    void Update() {
        if (!_isLeftHand && OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown)) {
            Destroy(_objectInBounds);
        }
        else if (_isLeftHand && OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown)) {
            Destroy(_objectInBounds);
        }
    }
}
