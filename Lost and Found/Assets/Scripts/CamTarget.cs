using UnityEngine;
using System.Collections;

public class CamTarget : MonoBehaviour {
    public static CamTarget Instance;
    [SerializeField] private float _rotationSpeed = 3f;
    float currentXRotation;

    private void Awake() {
        Instance = this;
    }

    public void OrbitCamera(float inputX, float inputY) {
        //transform.localRotation *= Quaternion.AngleAxis(inputX * _rotationSpeed, Vector3.up);
        //transform.rotation *= Quaternion.AngleAxis(-inputY * _rotationSpeed, Vector3.right);     

        var rot = Quaternion.FromToRotation(transform.up, PlayerCharacter.Instance.UprightRotation) * transform.rotation;       //add this to align to surface
        transform.rotation = rot;

        var angles = transform.localEulerAngles;
        angles.z = 0;

        var angle = transform.localEulerAngles.x;

        if (angle > 180 && angle < 340) {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40) {
            angles.x = 40;
        }

        transform.localEulerAngles = angles;
    }

}