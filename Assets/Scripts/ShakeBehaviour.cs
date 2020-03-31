using UnityEngine;

public class ShakeBehaviour : MonoBehaviour
{
    private Transform transform;
    private float shakeDuration;
    private float shakeMagnitude;
    private float dampingSpeed;
    private Vector3 initialPosition;
    private SavedOptions options;

    private void Awake()
    {
        if (transform is null)
            transform = GetComponent<Transform>();
    }

    private void Start()
    {
        options = SaveSystem.GetOptions();
    }

    private void OnEnable()
    {
        initialPosition = transform.localPosition;
    }

    private void FixedUpdate()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.fixedDeltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0;
            transform.localPosition = initialPosition;
        }
    }

    /// <summary>
    /// Triggers the shake effect
    /// </summary>
    /// <param name="duration">The duration of the shake effect</param>
    /// <param name="magnitude">How much it shoulf shake</param>
    /// <param name="damping">How quickly the shake should evaporate</param>
    public void TriggerShake(float duration = 1, float magnitude = 1, float damping = 1)
    {
        options = SaveSystem.GetOptions();

        if (options.ShakingEffect)
        {
            shakeMagnitude = magnitude;
            dampingSpeed = damping;
            shakeDuration = duration;
        }
    }
}
