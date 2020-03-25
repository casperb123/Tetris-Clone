using UnityEngine;

public class ShakeBehaviour : MonoBehaviour
{
    // Transform of the GameObject you want to shake
    private Transform transform;

    // Desired duration of the shake effect
    private float shakeDuration;

    // A measure of the magnitude for the shake
    private float shakeMagnitude;

    // A measure of how quickly the shake effect should evaporate
    private float dampingSpeed;

    // The initial position of the GameObject
    private Vector3 initialPosition;
    private OptionsMenu options;

    private void Awake()
    {
        if (transform is null)
            transform = GetComponent<Transform>();
    }

    private void Start()
    {
        options = OptionsMenu.Instance;
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
        if (options.ShakingEffect)
        {
            shakeMagnitude = magnitude;
            dampingSpeed = damping;
            shakeDuration = duration;
        }
    }
}
