using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Data_Player), typeof(PlayerCollision))]
public class HandController : MonoBehaviour
{
    private Data_Player data;
    private PlayerCollision collision;

    void Awake()
    {
        data = GetComponent<Data_Player>();
        collision = GetComponent<PlayerCollision>();
    }

    void Update()
    {
        data.hand.grabCooldownTimerLeft -= Time.deltaTime;
        data.hand.grabCooldownTimerRight -= Time.deltaTime;

        HandleHand(
            isTrying: data.state.isTryingGrabRight,
            isGrabbing: ref data.state.isGrabbingRight,
            handOrigin: data.hand.handRightOrigin,
            handAnim: data.rightHand,
            originalParent: data.hand.originalRightHandParent,
            grabbedObject: ref data.hand.grabbedRightObject,
            lastGrabbedObject: ref data.hand.lastGrabbedRightObject,
            cooldownTimer: data.hand.grabCooldownTimerRight,
            isLeft: false
        );

        HandleHand(
            isTrying: data.state.isTryingGrabLeft,
            isGrabbing: ref data.state.isGrabbingLeft,
            handOrigin: data.hand.handLeftOrigin,
            handAnim: data.leftHand,
            originalParent: data.hand.originalLeftHandParent,
            grabbedObject: ref data.hand.grabbedLeftObject,
            lastGrabbedObject: ref data.hand.lastGrabbedLeftObject,
            cooldownTimer: data.hand.grabCooldownTimerLeft,
            isLeft: true
        );

        staminaCheck();
    }

    void staminaCheck()
    {
        // RIGHT
        if (data.state.isGrabbingRight && data.hand.grabbedRightObject != null)
        {
            data.handRight.stamina -= data.handRight.costStamina * Time.deltaTime;
            if (data.handRight.stamina <= 0f)
            {
                data.handRight.stamina = 0f;
                data.state.isTryingGrabRight = false;
                ReleaseGrab(ref data.state.isGrabbingRight, ref data.hand.grabbedRightObject, data.hand.handRightOrigin, data.hand.originalRightHandParent, data.rightHand, false);
            }
        }
        else if (!data.state.isGrabbingRight)
        {
            data.handRight.stamina += data.handRight.costStamina * 2f * Time.deltaTime;
            if (data.handRight.stamina > data.handRight.maxStamina)
                data.handRight.stamina = data.handRight.maxStamina;
        }

        // LEFT
        if (data.state.isGrabbingLeft && data.hand.grabbedLeftObject != null)
        {
            data.handLeft.stamina -= data.handLeft.costStamina * Time.deltaTime;
            if (data.handLeft.stamina <= 0f)
            {
                data.handLeft.stamina = 0f;
                data.state.isTryingGrabLeft = false;
                ReleaseGrab(ref data.state.isGrabbingLeft, ref data.hand.grabbedLeftObject, data.hand.handLeftOrigin, data.hand.originalLeftHandParent, data.leftHand, true);
            }
        }
        else if (!data.state.isGrabbingLeft)
        {
            data.handLeft.stamina += data.handLeft.costStamina * 2f * Time.deltaTime;
            if (data.handLeft.stamina > data.handLeft.maxStamina)
                data.handLeft.stamina = data.handLeft.maxStamina;
        }
    }



    void HandleHand(
        bool isTrying,
        ref bool isGrabbing,
        Transform handOrigin,
        Data_Player.HandAnim handAnim,
        Transform originalParent,
        ref GameObject grabbedObject,
        ref GameObject lastGrabbedObject,
        float cooldownTimer,
        bool isLeft
    )
    {
        if (isTrying && !isGrabbing)
        {
            TryGrab(ref isGrabbing, ref grabbedObject, ref lastGrabbedObject, handOrigin, handAnim, cooldownTimer, isLeft);
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
        ref GameObject lastGrabbedObject,
        Transform handOrigin,
        Data_Player.HandAnim handAnim,
        float cooldownTimer,
        bool isLeft
    )
    {
        if (cooldownTimer > 0f)
            return;

        float currentStamina = isLeft ? data.handLeft.stamina : data.handRight.stamina;
        float staminaCost = isLeft ? data.handLeft.costStamina : data.handRight.costStamina;

        if (currentStamina < staminaCost)
            return; // Sem energia

        if (collision.DetectGrabbable(out GameObject grabbable, out RaycastHit hit))
        {
            bool isMovingUp = data.rb.linearVelocity.y > data.hand.upwardVelocityThreshold;
            bool isOtherHandGrabbing =
                (isLeft && data.state.isGrabbingRight) ||
                (!isLeft && data.state.isGrabbingLeft);

            // Bloqueia apenas se estiver subindo E nenhuma mão estiver agarrando
            if (isMovingUp && !isOtherHandGrabbing)
                return;

            isGrabbing = true;
            grabbedObject = grabbable;

            handOrigin.SetParent(null);
            PositionAndRotateHand(handOrigin, hit, isLeft);

            if (isLeft)
                data.handLeft.stamina -= data.handLeft.costStamina;
            else
                data.handRight.stamina -= data.handRight.costStamina;

            handAnim.ChangeAnimationState(Data_Player.HandAnim.AnimationState.HandGrab);
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

        if (grabbedObject != null)
        {
            if (isLeft)
                data.hand.lastGrabbedLeftObject = grabbedObject;
            else
                data.hand.lastGrabbedRightObject = grabbedObject;
        }

        grabbedObject = null;

        handOrigin.SetParent(originalParent, worldPositionStays: false);

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
    }

    void LateUpdate()
    {
        ForceResetHandIfDetached(data.hand.handLeftOrigin, data.hand.originalLeftHandParent, data.hand.originalLeftHandLocalPosition, data.hand.originalLeftHandLocalRotation, data.state.isGrabbingLeft);
        ForceResetHandIfDetached(data.hand.handRightOrigin, data.hand.originalRightHandParent, data.hand.originalRightHandLocalPosition, data.hand.originalRightHandLocalRotation, data.state.isGrabbingRight);
    }

    void ForceResetHandIfDetached(Transform hand, Transform parent, Vector3 localPos, Quaternion localRot, bool isGrabbing)
    {
        if (isGrabbing) return;

        if (hand.parent != parent)
        {
            hand.SetParent(parent, worldPositionStays: false);
            hand.localPosition = localPos;
            hand.localRotation = localRot;
        }
    }

    void PositionAndRotateHand(Transform handOrigin, RaycastHit hit, bool isLeft)
    {
        if (handOrigin == null) return;

        Vector3 offsetPosition = hit.point + Vector3.Scale(hit.normal, data.hand.offset);
        handOrigin.position = offsetPosition;

        Quaternion rotation = Quaternion.LookRotation(-hit.normal);
        if (!isLeft)
            rotation *= Quaternion.Euler(0, 180f, 0);

        handOrigin.rotation = rotation;
    }
}
