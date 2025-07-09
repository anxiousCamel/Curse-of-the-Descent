#if UNITY_EDITOR
using UnityEngine;

[ExecuteAlways]
public class HandVisualizer : MonoBehaviour
{
    public Data_Player data;

    private void OnDrawGizmos()
    {
        if (data == null)
            data = GetComponent<Data_Player>();

        if (data == null || data.hand == null)
            return;

        // Desenha o raycast da câmera (direção do grab)
        if (data.cameraSettings.mainCamera != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 origin = data.cameraSettings.mainCamera.transform.position;
            Vector3 direction = data.cameraSettings.mainCamera.transform.forward;
            Gizmos.DrawRay(origin, direction * data.hand.grabRange);
        }

        // Desenha a cápsula de proximidade (se houver)
        CapsuleCollider proximitySensor = data.cameraSettings.proximitySensor;
        if (proximitySensor != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 p1 = proximitySensor.transform.position + proximitySensor.transform.up * (proximitySensor.height / 2 - proximitySensor.radius);
            Vector3 p2 = proximitySensor.transform.position - proximitySensor.transform.up * (proximitySensor.height / 2 - proximitySensor.radius);
            Gizmos.DrawWireSphere(p1, proximitySensor.radius);
            Gizmos.DrawWireSphere(p2, proximitySensor.radius);
        }
    }
}
#endif
