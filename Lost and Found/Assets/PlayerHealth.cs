using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    public static PlayerHealth Instance;
    public float CurrentHealth = 0f;
    [SerializeField] private float MaxHealth = 100f;

    void Start() {
        Instance = this;
        CurrentHealth = MaxHealth;
    }

    public void AdjustHealth(float value) {
        if (PlayerCharacter.Instance.PlayerState == PlayerState.Inactive) {
            return;
        }

        CurrentHealth += value;
        //HUD.Instance.UpdateHealth();

        if (CurrentHealth <= 0) {
            CurrentHealth = 0;
            PlayerCharacter.Instance.Dead();
            //ToggleRagdoll.Instance.ActivateRagdoll();   //TODO: try using rewired only for this
        }
    }
}
