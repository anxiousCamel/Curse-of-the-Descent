// File: Assets/Scripts/Player/HandController.cs
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Controla as interações de agarrar e soltar das mãos do jogador.
/// Gerencia animações, cooldowns, estamina e posicionamento das mãos.
/// </summary>
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
        AtualizarCooldown();
        AtualizarControleDeMaoDireita();
        AtualizarControleDeMaoEsquerda();
        AtualizarEstaminaDasMaos();
    }

    void LateUpdate()
    {
        ReiniciarMaoSeDesanexada(data.hand.handLeftOrigin, data.hand.originalLeftHandParent, data.hand.originalLeftHandLocalPosition, data.hand.originalLeftHandLocalRotation, data.state.isGrabbingLeft);
        ReiniciarMaoSeDesanexada(data.hand.handRightOrigin, data.hand.originalRightHandParent, data.hand.originalRightHandLocalPosition, data.hand.originalRightHandLocalRotation, data.state.isGrabbingRight);
    }

    #region Atualizações Gerais

    void AtualizarCooldown()
    {
        data.hand.grabCooldownTimerLeft -= Time.deltaTime;
        data.hand.grabCooldownTimerRight -= Time.deltaTime;
    }

    void AtualizarControleDeMaoDireita()
    {
        ControlarMao(
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
    }

    void AtualizarControleDeMaoEsquerda()
    {
        ControlarMao(
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
    }

    #endregion

    #region Estamina

    void AtualizarEstaminaDasMaos()
    {
        AtualizarEstaminaUnica(ref data.handRight, ref data.state.isGrabbingRight, ref data.state.isTryingGrabRight, data.hand.grabbedRightObject, data.hand.handRightOrigin, data.hand.originalRightHandParent, data.rightHand, false);
        AtualizarEstaminaUnica(ref data.handLeft, ref data.state.isGrabbingLeft, ref data.state.isTryingGrabLeft, data.hand.grabbedLeftObject, data.hand.handLeftOrigin, data.hand.originalLeftHandParent, data.leftHand, true);
    }

    void AtualizarEstaminaUnica(ref Data_Player.HandStats mao, ref bool isGrabbing, ref bool isTrying, GameObject objeto, Transform origem, Transform originalParent, Data_Player.HandAnim animacao, bool isLeft)
    {
        if (isGrabbing && objeto != null)
        {
            mao.stamina -= mao.costStamina * Time.deltaTime;
            if (mao.stamina <= 0f)
            {
                mao.stamina = 0f;
                isTrying = false;
                Soltar(ref isGrabbing, ref objeto, origem, originalParent, animacao, isLeft);
            }
        }
        else if (!isGrabbing)
        {
            mao.stamina += mao.costStamina * 2f * Time.deltaTime;
            if (mao.stamina > mao.maxStamina)
                mao.stamina = mao.maxStamina;
        }
    }

    #endregion

    #region Controle de Mão

    void ControlarMao(bool isTrying, ref bool isGrabbing, Transform handOrigin, Data_Player.HandAnim handAnim, Transform originalParent, ref GameObject grabbedObject, ref GameObject lastGrabbedObject, float cooldownTimer, bool isLeft)
    {
        if (isTrying && !isGrabbing)
            TentarAgarrar(ref isGrabbing, ref grabbedObject, ref lastGrabbedObject, handOrigin, handAnim, cooldownTimer, isLeft);
        else if (!isTrying && isGrabbing)
            Soltar(ref isGrabbing, ref grabbedObject, handOrigin, originalParent, handAnim, isLeft);
        else if (!isTrying && !isGrabbing)
            handAnim.ChangeAnimationState(Data_Player.HandAnim.AnimationState.HandIdle);
    }

    void TentarAgarrar(ref bool isGrabbing, ref GameObject grabbedObject, ref GameObject lastGrabbedObject, Transform handOrigin, Data_Player.HandAnim handAnim, float cooldownTimer, bool isLeft)
    {
        if (cooldownTimer > 0f) return;

        float staminaAtual = isLeft ? data.handLeft.stamina : data.handRight.stamina;
        float custoStamina = isLeft ? data.handLeft.costStamina : data.handRight.costStamina;

        if (staminaAtual < custoStamina) return;

        if (collision.DetectGrabbable(out GameObject grabbable, out RaycastHit hit))
        {
            bool subindo = data.rb.linearVelocity.y > data.hand.upwardVelocityThreshold;
            bool outraMaoAgarrando = (isLeft && data.state.isGrabbingRight) || (!isLeft && data.state.isGrabbingLeft);

            if (subindo && !outraMaoAgarrando) return;

            isGrabbing = true;
            grabbedObject = grabbable;
            handOrigin.SetParent(null);
            PosicionarERotacionarMao(handOrigin, hit, isLeft);

            if (isLeft)
                data.handLeft.stamina -= custoStamina;
            else
                data.handRight.stamina -= custoStamina;

            handAnim.ChangeAnimationState(Data_Player.HandAnim.AnimationState.HandGrab);
        }
        else
        {
            handAnim.ChangeAnimationState(Data_Player.HandAnim.AnimationState.HandTryGrab);
        }
    }

    void Soltar(ref bool isGrabbing, ref GameObject grabbedObject, Transform handOrigin, Transform originalParent, Data_Player.HandAnim handAnim, bool isLeft)
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
        handOrigin.localPosition = isLeft ? data.hand.originalLeftHandLocalPosition : data.hand.originalRightHandLocalPosition;
        handOrigin.localRotation = isLeft ? data.hand.originalLeftHandLocalRotation : data.hand.originalRightHandLocalRotation;

        handAnim.ChangeAnimationState(Data_Player.HandAnim.AnimationState.HandIdle);
    }

    #endregion

    #region Reset e Posicionamento

    void ReiniciarMaoSeDesanexada(Transform hand, Transform parent, Vector3 localPos, Quaternion localRot, bool isGrabbing)
    {
        if (isGrabbing || hand.parent == parent) return;

        hand.SetParent(parent, worldPositionStays: false);
        hand.localPosition = localPos;
        hand.localRotation = localRot;
    }

    void PosicionarERotacionarMao(Transform handOrigin, RaycastHit hit, bool isLeft)
    {
        if (handOrigin == null) return;

        Vector3 posicao = hit.point + Vector3.Scale(hit.normal, data.hand.offset);
        handOrigin.position = posicao;

        Quaternion rotacao = Quaternion.LookRotation(-hit.normal);
        if (!isLeft) rotacao *= Quaternion.Euler(0, 180f, 0);
        handOrigin.rotation = rotacao;
    }

    #endregion
}
