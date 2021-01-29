using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class AudioSetup : MonoBehaviour {
    [SerializeField] AudioSource _audioSource = null;
    [Range(0.5f, 2)]
    [SerializeField] private float _minPitch = 0.95f;
    [Range(0.5f, 2)]
    [SerializeField] private float _maxPitch = 1.05f;

    private const int PLACEMENT_GHOST_LAYER = 8;
    void Start(){
        var n = Random.Range(_minPitch, _maxPitch);
        _audioSource.pitch = n;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.layer == PLACEMENT_GHOST_LAYER) {
            return;
        }

        _audioSource.volume = other.relativeVelocity.magnitude + 0.2f;

        if (!_audioSource.isPlaying) {
            _audioSource.Play();
        }
    }
}
