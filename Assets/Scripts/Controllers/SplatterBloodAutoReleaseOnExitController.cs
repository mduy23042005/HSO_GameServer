using UnityEngine;

public class SplatterBloodAutoReleaseOnExitController : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // T? ð?ng tr? GameObject v? Pool khi Animation k?t thúc
        if (PoolManager.Instance != null)
        {
            PoolManager.Instance.Release(animator.gameObject);
        }
    }
}