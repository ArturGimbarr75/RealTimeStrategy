using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class HealthDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject _healthBarParent;
    [SerializeField]
    private Image _healthBarImage;
    [SerializeField]
    private Gradient _healthBarGradient;

    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void Start()
    {
        _health.OnClientHealthUpdated += HealthChangedUpdate;
        HealthChangedUpdate(_health.CurrentHealth, _health.MaxHealth);
    }

    private void HealthChangedUpdate(int current, int max)
    {
        float health = (float)current / max;
        _healthBarImage.fillAmount = health;
        _healthBarImage.color = _healthBarGradient.Evaluate(health);
    }

    private void OnDestroy()
    {
        _health.OnClientHealthUpdated -= HealthChangedUpdate;
    }

    private void OnMouseEnter()
    {
        _healthBarParent.SetActive(true);
    }

    private void OnMouseExit()
    {
        _healthBarParent.SetActive(false);
    }
}
