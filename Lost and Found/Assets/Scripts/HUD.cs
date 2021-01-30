using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public static HUD Instance;    
    [SerializeField] private Text _healthValue = null;
    [SerializeField] private Text _score = null;    

    [SerializeField] private Image _healthBar = null;

    public GameObject GameOverUI = null;
    public GameObject PauseUI = null;

    private const float HEALTHBAR_VAL_MULTIPLIER = 0.01f;

    private void Start() {
        Instance = this;
    }

    void Update() {
        
    }

    public void UpdateHealth() {
        float newHealth = PlayerHealth.Instance.CurrentHealth;
        _healthValue.text = newHealth.ToString("F0");
        _healthBar.fillAmount = newHealth * HEALTHBAR_VAL_MULTIPLIER;
    }
}
