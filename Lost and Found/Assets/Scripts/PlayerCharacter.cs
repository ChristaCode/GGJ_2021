using UnityEngine;
using System.Collections;

public enum PlayerState {
    Idle,
    Running,
    Jumping,
    Falling,

    Inactive,
    Dead
}

public class PlayerCharacter : MonoBehaviour {
    public static PlayerCharacter Instance;

    [Header("General")]
    public PlayerState PlayerState;
    public float _currentVelocityMagnitude = 0f;
    [SerializeField] private float _maxVelocityforLanding = 1f;
    public Vector3 CurrentVelocity;

    [Space]
    public Vector3 InputDirection = Vector3.zero;
    public FollowTransform FollowSeagullTransform = null;

    [SerializeField] private Transform _startPos = null;
    [SerializeField] private bool _goToStart = true;
    [SerializeField] private LayerMask _raycastIgnoredLayersGrounded;
    [SerializeField] private LayerMask _raycastIgnoredLayersCrash;

    public bool Grounded = true;
    public bool Idle = false;

    //private Animator _anim;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private CharacterController _charController;
           
    [Space]
    //[Range(0.0001f, 0.1f)]
    [Tooltip("Time in seconds to reach terminal force")]
    [SerializeField] private float _accelerateDuration = 1f;

    //cached values
    private float _flightTime = 0f;
    private float _currentForwardForce = 0f;
    private float timeElapsed = 0f;
    [SerializeField] private float _spinSpeed = 1f;

    [Header("Movement Parametres")]
    [SerializeField] private float _runSpeed = 4f;
    [SerializeField] private float _turnSpeedGrounded = 20f;
    [SerializeField] private float _baseJumpPower = 100f;

    [Header("Misc Variables")]
    [SerializeField] private float _groundCheckDistance = 0.13f;
    private float _groundCheckOffset = 0.01f;
    private float _minCrashVelocity = 0.1f;
    private float _groundedRagdollCollisionForce = 2f;
    private float _airbourneRagdollCollisionForce = 3f;
    private Coroutine _lastLerpRoutine = null;

    void Start() {
        Instance = this;
        //_anim = GetComponent<Animator>();
        _charController = GetComponent<CharacterController>();

        if (_goToStart) {
            ResetPlayerCharacter();
        }
    }

    void Update() {
        CurrentVelocity = _rb.velocity;
        _currentVelocityMagnitude = _rb.velocity.magnitude;
        //_anim.SetFloat("velocity", InputDirection.magnitude);

        GroundedCheck();

        if (Grounded) {
            GroundedMovement();
        }
        else {
            AirbourneMovement();
        }
    }

    void GroundedCheck() {
        RaycastHit hit;

        Debug.DrawRay(transform.position + Vector3.up * _groundCheckOffset, Vector3.down * _groundCheckDistance, Color.blue);

        if (Physics.Raycast(transform.position + Vector3.up * _groundCheckOffset, Vector3.down, out hit, _groundCheckDistance, ~_raycastIgnoredLayersGrounded)) {
            if (!Grounded) {
                Land();
                Grounded = true;
            }
        }
        else {
            if (Grounded) {
                Grounded = false;
            }
        }
    }     

    private void StartSpeedChange(float duration, float targetForce, float forceAtStateChange) {
        if (_lastLerpRoutine != null) {
            StopCoroutine(_lastLerpRoutine);
        }

        _lastLerpRoutine = StartCoroutine(LerpForceChange(duration, targetForce, forceAtStateChange));
    }

    public void Crash() {
        //ToggleRagdoll.Instance.ActivateRagdoll();
        //SFX squawk
        //sfx bump
    }


    public void GroundedMovement() {
        Vector3 camDirection = Camera.main.transform.rotation * InputDirection;                //this takes all 3 axes (good for something flying in 3d space)           
        Vector3 targetDirection = new Vector3(camDirection.x, 0, camDirection.z);              //add this line if the character moves along the ground

        if (InputDirection != Vector3.zero) {                                                  //turn the character to face the direction of travel when there is input
            transform.rotation = Quaternion.Slerp(
                                transform.rotation,
                                Quaternion.LookRotation(targetDirection),
                                Time.deltaTime * _turnSpeedGrounded
                                );

            if (PlayerState != PlayerState.Running) {
                PlayerState = PlayerState.Running;
            }

        }
        else {
            if (PlayerState == PlayerState.Running) {
                PlayerState = PlayerState.Idle;
            }
        }

        _rb.velocity = targetDirection * _runSpeed * InputDirection.magnitude;     //use camDirection here instead of targetDirection if you want a flying character. normalized prevents char moving at double speed diagonally

    }
    
    public void AirbourneMovement() {
        Vector3 targetDirection = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);       //TODO: what is this doing exactly that isn't being done in the banking method?
        targetDirection = targetDirection.normalized;

        switch (PlayerState) {            
            case PlayerState.Jumping:
                _rb.AddForce(targetDirection * _currentForwardForce * Time.deltaTime);
                break;            
            case PlayerState.Falling:
                return;
            default:
                break;
        }
    }

    public void Jump(float jumpCharge) {
        if(jumpCharge < 1) {
            jumpCharge = 1;
        }

        print("jumpCharge is " + jumpCharge);
        _rb.velocity = new Vector3(0, _baseJumpPower * jumpCharge, _baseJumpPower * jumpCharge);
        _rb.AddTorque(0, _baseJumpPower * jumpCharge, 0);
    }

    private void FaceDirectionOfVelocity() {
        Vector3 playerDirection = _rb.velocity;                                             //this takes all 3 axes (good for something flying in 3d space)           

        transform.rotation = Quaternion.Slerp(
        transform.rotation,
        Quaternion.LookRotation(playerDirection),                                           //use playerdirection here for all axes, targetdirection for pitching only
        Time.deltaTime * playerDirection.magnitude
        );
    }
        
    public void SpinHorizontal(float inputVal) {
        _rb.AddTorque(0, inputVal * _spinSpeed, 0);        
    }        

    private void Fall() {
        if (Grounded) {
            _rb.AddForce(0, 0, 1);      //slight push forward so we don't get stuck on ledge
        }

        Grounded = false;
        //_anim.SetBool("IsGrounded", false);
       // _anim.SetBool("IsFlying", true);
      //  _anim.SetTrigger("Hit");

        PlayerState = PlayerState.Falling;
    }

    public void Land() {

       // _anim.SetBool("IsGrounded", true);
      //  _anim.SetBool("IsFlying", false);
      //  _anim.SetFloat("Forward", 0);
      //  _anim.SetFloat("Velocity", 1);
//
        _rb.velocity = Vector3.zero;    //TODO: make a nicer transition to land eventually. See if this fixes bump on landing
        _flightTime = 0;
        _rb.mass = 1f;

        LevelOut(0f);
    }

    public void AttackPunch() {
        //_anim.SetTrigger("Attack");
    }

    public void AttackKick() {
        //_anim.SetTrigger("Attack");
    }

    public void IsHit() {
       // _anim.SetTrigger("Hit");
    }

    void OnCollisionEnter(Collision other) {
        float velocityToRagdoll;

        if (Grounded) {
            velocityToRagdoll = _groundedRagdollCollisionForce;
        }
        else {
            velocityToRagdoll = _airbourneRagdollCollisionForce;
        }

        float relVelocity = other.relativeVelocity.magnitude;

        if (relVelocity >= velocityToRagdoll) {
            PlayerHealth.Instance.AdjustHealth(-relVelocity * 3);
            Crash();
        }        
    }

    public void Dead() {
        PlayerState = PlayerState.Dead;
        GameLoop.Instance.Died();
    }

    public void ResetPlayerCharacter() {
        transform.position = _startPos.position;
        transform.rotation = _startPos.rotation;
    }

    private void LevelOut(float duration) {       //this levels out the seagull entirely      //TODO: May use this to level out seagull when it lands
        Quaternion uprightRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        if (timeElapsed < duration) {
            transform.rotation = Quaternion.Slerp(transform.rotation, uprightRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
        }
        else {
            transform.rotation = uprightRotation;
        }
    }

    public void TransitionStateFromRagdoll() {
        Land();
    }

    IEnumerator LerpForceChange(float duration, float targetForce, float forceAtStateChange) {
        float timeElapsed = 0;
        while (timeElapsed < duration) {
            _currentForwardForce = Mathf.Lerp(forceAtStateChange, targetForce, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _currentForwardForce = targetForce;
    }
}
