using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Ajustes de Salud")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invincibilityDuration = 3f;
    [SerializeField] private float blinkInterval = 0.2f;

    [Header("Referencias Visuales")]
    [SerializeField] private Renderer playerRenderer; // Arrastra aquí el modelo que parpadeará

    private int _currentHealth;
    private bool _isInvincible;

    private void Start()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (_isInvincible) return;

        _currentHealth -= amount;
        Debug.Log($"Vida restante: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(BecomeInvincible());
        }
    }

    private IEnumerator BecomeInvincible()
    {
        _isInvincible = true;
        float timer = 0;

        while (timer < invincibilityDuration)
        {
            // Alternar visibilidad del renderer
            playerRenderer.enabled = !playerRenderer.enabled;

            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        playerRenderer.enabled = true; // Asegurar que quede visible al terminar
        _isInvincible = false;
    }

    private void Die()
    {
        Debug.Log("Player ha muerto");
        GameManager.Instance.ShowGameOver(); // Avisamos al manager
        gameObject.SetActive(false);
    }
}