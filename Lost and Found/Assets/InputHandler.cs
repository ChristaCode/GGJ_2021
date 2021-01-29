using UnityEngine;
using System.Collections;
using Rewired;

public class InputHandler : MonoBehaviour {
    public static InputHandler Instance;
    [SerializeField] private PlayerCharacter _myPlayerController;

    public float upDownInputSpeed = 3f;
    private Player _player;    

    public bool IsRagdoll = false;
    public bool CanRecoverFromRagdoll = false;
    public bool CamBeingMovedByPlayer = true;

    public float JumpCharge = 0f;

    void Start() {
        Instance = this;
        int playerId = 0;
        _player = ReInput.players.GetPlayer(playerId);    
    }

    void Update() {
        if (PlayerCharacter.Instance.PlayerState == PlayerState.Dead) { //TODO: Just change this to dead, or add a Dead state
            return;
        }
        if (_player.GetButtonDown("Pause")) {
            //GameLoop.Instance.Pause();
            return;
        }

        _myPlayerController.InputDirection = new Vector3(_player.GetAxis("Move Horizontal"), 0, _player.GetAxis("Move Vertical")).normalized; //TODO: normalising makes speed consistent across directions, but removes input graduation

        var camRotationX = _player.GetAxis("Look Horizontal");
        var camRotationY = _player.GetAxis("Look Vertical");

        if (camRotationY != 0 || camRotationY != 0) {
            CamBeingMovedByPlayer = true;
            CamTarget.Instance.OrbitCamera(camRotationX, camRotationY); //TODO: blocking out because it works like shit right now
        }
        else {
            CamBeingMovedByPlayer = false;
        }

        if (_player.GetButton("Jump")) {
            JumpCharge += Time.deltaTime;
        }

        if (_player.GetButtonUp("Jump")) {
            _myPlayerController.Jump(JumpCharge);
            JumpCharge = 0f;
        }

        if (_player.GetButtonDown("Attack Hands")) {
            if (_myPlayerController.Grounded) {
                _myPlayerController.AttackPunch();
            }            
        }

        if (_player.GetButtonDown("Attack Feet")) {
            if (_myPlayerController.Grounded) {
                _myPlayerController.AttackKick();
            }
        }

        if (!_myPlayerController.Grounded) {
            if (_player.GetAxis("Spin Left") != 0) {
                float inputPressure = _player.GetAxis("Spin Left");
                _myPlayerController.SpinHorizontal(inputPressure);
            }
            else if (_player.GetAxis("Spin Right") != 0) {
                float inputPressue = _player.GetAxis("Spin Right");
                _myPlayerController.SpinHorizontal(inputPressue);
            }            
        }                
    }
}
