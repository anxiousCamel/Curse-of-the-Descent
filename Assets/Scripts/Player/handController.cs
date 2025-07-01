using UnityEngine;

[RequireComponent(typeof(Data_Player), typeof(PlayerCollision))]
public class handController : MonoBehaviour
{
    private Data_Player data;
    private PlayerCollision collision;

    private GameObject grabbedRightObject;
    private GameObject grabbedLeftObject;

    void Awake()
    {
        data = GetComponent<Data_Player>();
        collision = GetComponent<PlayerCollision>();
    }

    void Update()
    {
        UpdateHand(
            isTrying: data.state.isTryingGrabRight,
            isGrabbing: ref data.state.isGrabbingRight,
            handOrigin: data.hand.handRightOrigin,
            handAnim: data.rightHand,
            originalParent: data.hand.originalRightHandParent,
            grabbedObject: ref grabbedRightObject,
            isLeft: false
        );

        UpdateHand(
            isTrying: data.state.isTryingGrabLeft,
            isGrabbing: ref data.state.isGrabbingLeft,
            handOrigin: data.hand.handLeftOrigin,
            handAnim: data.leftHand,
            originalParent: data.hand.originalLeftHandParent,
            grabbedObject: ref grabbedLeftObject,
            isLeft: true
        );
    }

    void UpdateHand(
        bool isTrying,
        ref bool isGrabbing,
        Transform handOrigin,
        Data_Player.HandAnim handAnim,
        Transform originalParent,
        ref GameObject grabbedObject,
        bool isLeft
    )
    {
        if (isTrying && !isGrabbing)
        {
            TryGrab(ref isGrabbing, ref grabbedObject, handOrigin, handAnim, isLeft);
        }
        else if (!isTrying && isGrabbing)
        {
            ReleaseGrab(ref isGrabbing, ref grabbedObject, handOrigin, originalParent, handAnim, isLeft);
        }
        else if (!isTrying && !isGrabbing)
        {
            handAnim.ChangeAnimationState(Data_Player.HandAnim.AnimationState.HandIdle);
        }
    }

    void TryGrab(
        ref bool isGrabbing,
        ref GameObject grabbedObject,
        Transform handOrigin,
        Data_Player.HandAnim handAnim,
        bool isLeft
    )
    {
        if (collision.DetectGrabbable(out GameObject grabbable, out RaycastHit hit))
        {
            isGrabbing = true;
            grabbedObject = grabbable;

            handOrigin.SetParent(null);
            PositionAndRotateHand(handOrigin, hit, isLeft);

            handAnim.ChangeAnimationState(Data_Player.HandAnim.AnimationState.HandGrab);
            Debug.Log($"[{(isLeft ? "Left" : "Right")}] Agarrou: {grabbable.name} em {hit.point}");
        }
        else
        {
            handAnim.ChangeAnimationState(Data_Player.HandAnim.AnimationState.HandTryGrab);
        }
    }

    void ReleaseGrab(
        ref bool isGrabbing,
        ref GameObject grabbedObject,
        Transform handOrigin,
        Transform originalParent,
        Data_Player.HandAnim handAnim,
        bool isLeft
    )
    {
        isGrabbing = false;
        grabbedObject = null;

        handOrigin.SetParent(originalParent);

        if (isLeft)
        {
            handOrigin.localPosition = data.hand.originalLeftHandLocalPosition;
            handOrigin.localRotation = data.hand.originalLeftHandLocalRotation;
        }
        else
        {
            handOrigin.localPosition = data.hand.originalRightHandLocalPosition;
            handOrigin.localRotation = data.hand.originalRightHandLocalRotation;
        }

        handAnim.ChangeAnimationState(Data_Player.HandAnim.AnimationState.HandIdle);
        Debug.Log($"[{(isLeft ? "Left" : "Right")}] Soltou");
    }

    void PositionAndRotateHand(Transform handOrigin, RaycastHit hit, bool isLeft)
    {
        Vector3 offsetPosition = hit.point + Vector3.Scale(hit.normal, data.hand.offset);
        handOrigin.position = offsetPosition;

        Quaternion rotation = Quaternion.LookRotation(-hit.normal);
        if (!isLeft)
        {
            rotation *= Quaternion.Euler(0, 180f, 0);
        }

        handOrigin.rotation = rotation;
    }
}
