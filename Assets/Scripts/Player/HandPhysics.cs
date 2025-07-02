using UnityEngine;

/// <summary>
/// Aplica física elástica entre as mãos e o corpo.
/// Gera impulso ao soltar.
/// </summary>
[RequireComponent(typeof(Data_Player))]
public class HandPhysics : MonoBehaviour
{
    private Data_Player data;
    void Awake()
    {
        data = GetComponent<Data_Player>();
    }

    void Update()
    {
        HandleHandJoint(
            isGrabbing: data.state.isGrabbingLeft,
            isTryingGrab: data.state.isTryingGrabLeft,
            handTransform: data.hand.handLeftOrigin,
            ref data.handPhysics.leftJoint
        );

        HandleHandJoint(
            isGrabbing: data.state.isGrabbingRight,
            isTryingGrab: data.state.isTryingGrabRight,
            handTransform: data.hand.handRightOrigin,
            ref data.handPhysics.rightJoint
        );
    }

    void HandleHandJoint(bool isGrabbing, bool isTryingGrab, Transform handTransform, ref SpringJoint joint)
    {
        if (isGrabbing && isTryingGrab)
        {
            if (joint == null)
            {
                joint = data.rb.gameObject.AddComponent<SpringJoint>();

                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = handTransform.position;
                joint.connectedBody = null;

                joint.anchor = Vector3.zero;
                joint.spring = data.handPhysics.spring;
                joint.damper = data.handPhysics.damper;
                joint.maxDistance = data.handPhysics.maxDistance;

                //Debug.Log("[HAND PHYSICS] Joint criado");
            }
            else
            {
                joint.connectedAnchor = handTransform.position;
            }
        }
        else
        {
            if (joint != null)
            {
                // Aplica impulso na direção do "puxão"
                Vector3 dir = (handTransform.position - data.transform.position).normalized;
                float forceMagnitude = data.handPhysics.releaseImpulseForce;
                data.rb.AddForce(dir * forceMagnitude, ForceMode.Impulse);

                Destroy(joint);
                joint = null;

                //Debug.Log("[HAND PHYSICS] Joint destruído + impulso aplicado");
            }
        }
    }
}
