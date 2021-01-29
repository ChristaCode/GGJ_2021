using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreator : MonoBehaviour {

    [Header("Setup")]
    [SerializeField] private float _triggerActiveThreshold = 0.55f;
    [SerializeField] private float _triggerDeactivatectiveThreshold = 0.35f;
    [SerializeField] private float _thumbstickActiveThreshold = 0.55f;

    [SerializeField] private Transform _objSpawnPointPrimary = null; //left is primary
    [SerializeField] private Transform _objSpawnPointSecondary = null;

    [Space(1)]
    [Header("Objects")]
    [SerializeField] private List<GameObject> _objectList = new List<GameObject>();

    [SerializeField] private List<GameObject> _ghostObjectListLeft = new List<GameObject>();
    [SerializeField] private List<GameObject> _ghostObjectListRight = new List<GameObject>();

    [SerializeField] private GameObject _currentSelectedObjectLeft = null;
    [SerializeField] private GameObject _currentSelectedObjectRight = null;

    [SerializeField] private int _currentObjIndexLeft = 0;
    [SerializeField] private int _currentObjIndexRight = 0;

    [SerializeField] private List<Renderer> _leftGhostRenderer = new List<Renderer>();
    [SerializeField] private List<Renderer> _rightGhostRenderer = new List<Renderer>();
        
    [Space(1)]
    [Header("Audio visual")]
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioClip _placementSFX = null;
    [SerializeField] private AudioClip _invalidSFX = null;
    [SerializeField] private AudioClip _cycleObject = null;

    [SerializeField] private Material _validPlacementMat = null;
    [SerializeField] private Material _invalidPlacementMat = null;

    private bool _rightTriggerHeld = false;
    private bool _leftTriggerHeld = false;

    private bool _leftStickUp = false;
    private bool _leftStickDown = false;
    private bool _rightStickUp = false;
    private bool _rightStickDown = false;

    private bool _canPlacePrimary = true;
    private bool _canPlaceSecondary = true;

    private void Start() {       
        _currentSelectedObjectLeft = _objectList[0];
        _currentSelectedObjectRight = _objectList[0];
        _currentObjIndexLeft = 0;
        _currentObjIndexRight = 0;
        _ghostObjectListLeft[_currentObjIndexLeft].SetActive(true);
        _ghostObjectListRight[_currentObjIndexRight].SetActive(true);
        _currentSelectedObjectLeft.SetActive(true);
        _currentSelectedObjectRight.SetActive(true);
        SetValidPlacement(true);
        SetValidPlacement(false);
    }

    private void Update() {
        if (!_leftTriggerHeld && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) >= _triggerActiveThreshold) {
            if (_canPlacePrimary) {
                Instantiate(_currentSelectedObjectLeft, _ghostObjectListLeft[_currentObjIndexLeft].transform.position, _ghostObjectListLeft[_currentObjIndexLeft].transform.rotation);
                _audioSource.PlayOneShot(_placementSFX);
            }
            else {
                _audioSource.PlayOneShot(_invalidSFX);
            }

            _leftTriggerHeld = true;
        }
        else if (!_rightTriggerHeld && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) >= _triggerActiveThreshold) {
            if (_canPlaceSecondary) {
                Instantiate(_currentSelectedObjectRight, _objSpawnPointSecondary.position, _objSpawnPointSecondary.rotation);
                //_audioSource.PlayOneShot(_placementSFX);
            }
            else {
                _audioSource.PlayOneShot(_invalidSFX);
            }

            _rightTriggerHeld = true;
        }
        else if (_leftTriggerHeld && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) <= _triggerDeactivatectiveThreshold) {
            _leftTriggerHeld = false;
        }
        else if (_rightTriggerHeld && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) <= _triggerDeactivatectiveThreshold) {
            _rightTriggerHeld = false;
        }

        else if (OVRInput.GetDown(OVRInput.Button.Four)) {
            CycleObjectLeft(true);
        }
        else if (OVRInput.GetDown(OVRInput.Button.Three)) {
            CycleObjectLeft(false);
        }

        else if (OVRInput.GetDown(OVRInput.Button.One)) {
            CycleObjectRight(true);
        }
        else if (OVRInput.GetDown(OVRInput.Button.Two)) {
            CycleObjectRight(false);
        }
    }

    private void CycleObjectLeft(bool moveUp) {
        _ghostObjectListLeft[_currentObjIndexLeft].SetActive(false);
        int nextItem = _objectList.IndexOf(_currentSelectedObjectLeft);

        if (moveUp) {
            if (nextItem == 0) {
                nextItem = _objectList.Count-1;
            }
            else {
                nextItem -= 1;
            }
        }
        else {
            if (nextItem == _objectList.Count-1) {
                nextItem = 0;
            }
            else {
                nextItem += 1;
            }
        }

        _currentObjIndexLeft = nextItem;
        Debug.LogWarning(_currentObjIndexLeft);
        _currentSelectedObjectLeft = _objectList[_currentObjIndexLeft];

        _ghostObjectListLeft[_currentObjIndexLeft].SetActive(true);
        SetValidPlacement(true);
        print("Turned on " + _currentSelectedObjectLeft.name);
    }

    private void CycleObjectRight(bool moveUp) {
        _ghostObjectListRight[_currentObjIndexRight].SetActive(false);
        
        int nextItem = _objectList.IndexOf(_currentSelectedObjectRight);

        if (moveUp) {
            if (nextItem == 0) {
                nextItem = _objectList.Count-1;
            }
            else {
                nextItem -= 1;
            }
        }
        else {
            if (nextItem == _objectList.Count-1) {
                nextItem = 0;
            }
            else {
                nextItem += 1;
            };
        }

        _currentObjIndexRight = nextItem;
        Debug.LogWarning(_currentObjIndexRight);
        _currentSelectedObjectRight = _objectList[_currentObjIndexRight];

        _ghostObjectListRight[_currentObjIndexRight].SetActive(true);
        SetValidPlacement(false);

    }

    public void SetInvalidPlacement(bool isLeftSpawnPoint) { 
        if (isLeftSpawnPoint) {
            _leftGhostRenderer[_currentObjIndexLeft].material = _invalidPlacementMat;
            _canPlacePrimary = false;
        }
        else {
            _rightGhostRenderer[_currentObjIndexRight].material = _invalidPlacementMat;
            _canPlaceSecondary = false;
        }
    }

    public void SetValidPlacement(bool isLeftSpawnPoint) {
        if (isLeftSpawnPoint) {
            _leftGhostRenderer[_currentObjIndexLeft].material = _validPlacementMat;
            _canPlacePrimary = true;
        }
        else {
            _rightGhostRenderer[_currentObjIndexRight].material = _validPlacementMat;
            _canPlaceSecondary = true;
        }
    }
}
