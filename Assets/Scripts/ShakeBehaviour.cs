using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeBehaviour : MonoBehaviour
{
    // Transform of the GameObject you want to shake
    private Transform transform;

    // The duration it shakes
    private float duration = 0;

    // Desired duration of the shake effect
    [SerializeField]
    private float shakeDuration = .5f;

    // A measure of the magnitude for the shake
    [SerializeField]
    private float shakeMagnitude = .2f;

    // A measure of how quickly the shake effect should evaporate
    [SerializeField]
    private float dampingSpeed = 5;

    // The initial position of the GameObject
    private Vector3 initialPosition;

    private void Awake()
    {
        if (transform is null)
            transform = GetComponent<Transform>();
    }

    private void OnEnable()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        if (duration > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            duration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            duration = 0;
            transform.localPosition = initialPosition;
        }
    }

    public void TriggerShake()
    {
        duration = shakeDuration;
    }
}
