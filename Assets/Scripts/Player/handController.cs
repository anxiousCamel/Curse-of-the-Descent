using UnityEngine;

public class handController : MonoBehaviour
{
    private Data_Player data;

    void Awake()
    {
        data = GetComponent<Data_Player>();
    }

    void Update()
    {
        HandleLeftHand();
        HandleRightHand();
    }

    void HandleLeftHand()
    {
        if (data.state.isTryingGrabLeft && !data.state.isGrabbingLeft)
        {
            data.leftHand.ChangeAnimationState(Data_Player.HandAnim.AnimationState.HandTryGrab);
        }
        else if (data.state.isGrabbingLeft)
        {
            data.leftHand.ChangeAnimationState(Data_Player.HandAnim.AnimationState.HandGrab);
        }
        else
        {
            data.leftHand.ChangeAnimationState(Data_Player.HandAnim.AnimationState.HandIdle);
        }
    }

    void HandleRightHand()
    {
        if (data.state.isTryingGrabRight && !data.state.isGrabbingRight)
        {
            data.rightHand.ChangeAnimationState(Data_Player.HandAnim.AnimationState.HandTryGrab);
        }
        else if (data.state.isGrabbingRight)
        {
            data.rightHand.ChangeAnimationState(Data_Player.HandAnim.AnimationState.HandGrab);
        }
        else
        {
            data.rightHand.ChangeAnimationState(Data_Player.HandAnim.AnimationState.HandIdle);
        }
    }
}