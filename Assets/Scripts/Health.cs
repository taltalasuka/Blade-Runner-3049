using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    public float GetCurrentHealthPercentage()
    {
        return (float) currentHealth/maxHealth;
    }
    private Character _character;

    private void Awake()
    {
        currentHealth = maxHealth;
        _character = GetComponent<Character>();
    }

    public void ApplyDamage(int damage)
    {
        currentHealth -= damage;
        CheckHealth();
    }

    private void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            _character.SwitchStateTo(Character.CharacterState.Dead);
        }
    }

    public void AddHealth(int value)
    {
        currentHealth += value;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
