using UnityEngine;

public class MobDieAutoReleaseOnExitController : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MobController mob = animator.GetComponentInParent<MobController>();
        if (mob != null)
            MobsManager.Instance.ApplyMobDead(mob.GetID());
    }
}
