using UnityEngine;

public class HandEffects : MonoBehaviour
{
    [SerializeField] private Data_Player data;

    private Transform cameraTransform;
    private void Awake()
    {
        if (data == null)
            data = GetComponent<Data_Player>();

        if (data.handLeft.handSprite != null)
            data.handLeft.originalLocalPos = data.handLeft.handSprite.transform.localPosition;

        if (data.handRight.handSprite != null)
            data.handRight.originalLocalPos = data.handRight.handSprite.transform.localPosition;
    }

    private void Update()
    {
        ApplyEffectsToHand(data.handLeft);
        ApplyEffectsToHand(data.handRight);
    }

    private void ApplyEffectsToHand(Data_Player.HandStats hand)
    {
        if (hand.handSprite == null) return;

        UpdateHandColor(hand);
        UpdateHandShake(hand);
    }

    private void UpdateHandColor(Data_Player.HandStats hand)
    {
        float percent = Mathf.Clamp01(hand.stamina / hand.maxStamina);
        Color targetColor = Color.Lerp(Color.red, Color.white, percent);
        hand.handSprite.color = new Color(targetColor.r, targetColor.g, targetColor.b, hand.handSprite.color.a);
    }

    private void UpdateHandShake(Data_Player.HandStats hand)
    {
        if (hand.handSprite == null) return;

        float percent = Mathf.Clamp01(hand.stamina / hand.maxStamina);
        float shakeAmount = hand.maxShakeIntensity * (1f - percent);

        Vector3 offset = new Vector3(
            (Mathf.PerlinNoise(Time.time * hand.shakeSpeed, 0f) - 0.5f),
            (Mathf.PerlinNoise(0f, Time.time * hand.shakeSpeed) - 0.5f),
            0f
        ) * shakeAmount * 2f;

        hand.handSprite.transform.localPosition = hand.originalLocalPos + offset;
    }
}