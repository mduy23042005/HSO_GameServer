using UnityEngine;

public class MobDieAutoReleaseOnExitController : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MobController mob = animator.GetComponentInParent<MobController>();
        if (mob != null)
            GameManager.Instance.GetComponent<MobsManager>().ApplyMobDead(mob.GetID());
    }
}
