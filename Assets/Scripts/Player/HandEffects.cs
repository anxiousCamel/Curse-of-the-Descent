using UnityEngine;

public class HandEffects : MonoBehaviour
{
    [SerializeField] private Data_Player data;

    private Transform cameraTransform;
    private CapsuleCollider proximitySensor;
    private void Awake()
    {
        if (data == null)
            data = GetComponent<Data_Player>();

        if (data.handLeft.handSprite != null)
            data.handLeft.originalLocalPos = data.handLeft.handSprite.transform.localPosition;

        if (data.handRight.handSprite != null)
            data.handRight.originalLocalPos = data.handRight.handSprite.transform.localPosition;

        cameraTransform = data.cameraSettings.mainCamera.transform;
        proximitySensor = data.cameraSettings.proximitySensor;


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
        UpdateHandProximityRecoil(hand);
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

private void UpdateHandProximityRecoil(Data_Player.HandStats hand)
{
    var sensor = data.cameraSettings.proximitySensor;
    if (cameraTransform == null || hand.handSprite == null || sensor == null)
    {
        //Debug.LogWarning("Componentes ausentes: cameraTransform, handSprite ou proximitySensor");
        return;
    }

    Vector3 sensorPos = sensor.transform.position;
    float halfHeight = sensor.height * 0.5f - sensor.radius;
    Vector3 up = sensor.transform.up;
    Vector3 point1 = sensorPos + up * halfHeight;
    Vector3 point2 = sensorPos - up * halfHeight;

    Collider[] hits = Physics.OverlapCapsule(point1, point2, sensor.radius, data.collision.groundLayer);
    //Debug.Log($"[{hand.handSprite.name}] Hits encontrados: {hits.Length}");

    float closest = float.MaxValue;
    Vector3 closestPoint = Vector3.zero;

    foreach (var hit in hits)
    {
        // Ignora o próprio sensor
        if (hit.transform == sensor.transform)
        {
            //Debug.Log($"[{hand.handSprite.name}] Ignorando colisão com o próprio sensor");
            continue;
        }

        Vector3 contact = hit.ClosestPoint(sensorPos);
        float distance = Vector3.Distance(sensorPos, contact);

        Debug.DrawLine(sensorPos, contact, Color.red);
        //Debug.Log($"[{hand.handSprite.name}] Hit em: {hit.gameObject.name}, Distância: {distance:F3}");

        if (distance < closest)
        {
            closest = distance;
            closestPoint = contact;
        }
    }

    Vector3 offset = Vector3.zero;

    if (closest < float.MaxValue)
    {
        float t = Mathf.InverseLerp(hand.proximityRecoilEnd, hand.proximityRecoilStart, closest);
        offset = hand.recoilDirection * (1f - t);
        //Debug.Log($"[{hand.handSprite.name}] Distância mais próxima: {closest:F3}, T: {t:F3}, Offset aplicado: {offset}");
    }
    else
    {
        //Debug.Log($"[{hand.handSprite.name}] Nenhuma colisão relevante detectada.");
    }

    hand.handSprite.transform.localPosition = hand.originalLocalPos + offset;
}



}