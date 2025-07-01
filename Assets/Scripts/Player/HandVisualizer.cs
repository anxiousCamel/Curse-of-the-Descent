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

        // Raycast da câmera (visão do jogador)
        if (Camera.main != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * data.hand.grabRange);
        }
    }
}
#endif
