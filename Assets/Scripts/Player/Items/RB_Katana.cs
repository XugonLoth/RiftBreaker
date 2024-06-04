using System.Collections;
using MANAGERS;
using UnityEngine;

public class RB_Katana : RB_Items
{
    public override void Attack() {
        base.Attack();
        RB_AudioManager.Instance.PlaySFX("LittleSwoosh", RB_PlayerController.Instance.transform.position,0);
    }
    
    public override void Bind()
    {
        base.Bind();
        //Set the current weapon on the animators
        _playerAnimator.SetFloat("WeaponID", 0);
        _colliderAnimator.SetFloat("WeaponID", 0);
    }

    public IEnumerator WaitForEndOfFrameToChargeAttack()
    {
        yield return new WaitForEndOfFrame();
        base.ChargedAttack();
    }

    public override void ChargedAttack()
    {
        //Reset directions
        RB_PlayerMovement.Instance.ResetDirection();
        StartCoroutine(WaitForEndOfFrameToChargeAttack());
        RB_AudioManager.Instance.PlaySFX("BigSwoosh", RB_PlayerController.Instance.transform.position,0);
    }
}
