using UnityEngine;

public class DwarfAnimator : MonoBehaviour
{
    Animator animator;

    int walk = Animator.StringToHash("walk");
    int dig = Animator.StringToHash("dig");
    int mine = Animator.StringToHash("mine");
    int fall = Animator.StringToHash("fall");
    int climb = Animator.StringToHash("climb ledge");
    int stopSign = Animator.StringToHash("stop sign");

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Walk() => animator.Play(walk);
    public void Dig() => animator.Play(dig);
    public void Mine() => animator.Play(mine);
    public void Fall() => animator.Play(fall);
    public void Climb() => animator.Play(climb);
    public void StopSign() => animator.Play(stopSign);
}
