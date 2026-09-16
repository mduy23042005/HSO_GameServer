using UnityEngine;

public class SplatterBloodAutoReleaseOnExitController : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PoolManager.Instance != null)
        {
            PoolManager.Instance.Release(animator.gameObject);
        }
    }
}