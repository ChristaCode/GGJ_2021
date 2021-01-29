using UnityEngine;
using System.Collections;

public class CamTarget : MonoBehaviour {
    public static CamTarget Instance;
    [SerializeField] private float _rotationSpeed = 3f;

    private void Awake() {
        Instance = this;
    }

    public void OrbitCamera(float inputX, float inputY) {
        transform.rotation *= Quaternion.AngleAxis(inputX * _rotationSpeed, Vector3.up);
        transform.rotation *= Quaternion.AngleAxis(-inputY * _rotationSpeed, Vector3.right);
        //transform.Rotate(inputY * _rotationSpeed * Time.deltaTime, inputX * _rotationSpeed * Time.deltaTime, 0);

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