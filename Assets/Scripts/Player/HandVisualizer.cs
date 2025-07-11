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
    }
}
#endif
