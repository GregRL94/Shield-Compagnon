using UnityEngine;
using UnityEngine.UI;

public class DashJauge : MonoBehaviour
{
    [SerializeField] private Image _dashFill;
    
    private void UpdateJauge(float value)
    {
        _dashFill.fillAmount = Mathf.Clamp01(value);
    }

    private void OnEnable()
    {
        CharacterMovement.UpdateJauge += UpdateJauge;
    }
    private void OnDisable()
    {
        CharacterMovement.UpdateJauge -= UpdateJauge;
    }

    private void OnDestroy()
    {
        CharacterMovement.UpdateJauge -= UpdateJauge;
    }
}
