using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    [SerializeField] private float _frontGroundCheckOffset;
    [SerializeField] private float _groundCheckOffset;
    [SerializeField] private float _groundCheckDistance;
    [SerializeField] private float _correctionSpeed = 0.1f;
    [SerializeField] private Rigidbody _rb;

    private const float GRAVITY = 9.81f;

    void Start() {
        _rb = GetComponent<Rigidbody>();
    }

    void Update() {
        RaycastHit frontHit;
        RaycastHit backHit;

        Debug.DrawRay(transform.position + transform.forward * _frontGroundCheckOffset, -transform.up * _groundCheckDistance, Color.magenta);
        Debug.DrawRay(transform.position - transform.forward * _frontGroundCheckOffset, -transform.up * _groundCheckDistance, Color.magenta);

        if (Physics.Raycast(transform.position + transform.forward * _frontGroundCheckOffset, -transform.up, out frontHit, _groundCheckDistance) && Physics.Raycast(transform.position - transform.forward * _frontGroundCheckOffset, -transform.up, out backHit, _groundCheckDistance)) {
            var upright = Vector3.Cross(transform.right, -(frontHit.point - backHit.point).normalized);
            Debug.DrawRay(transform.position, upright * 10, Color.red);

        }
    }
}