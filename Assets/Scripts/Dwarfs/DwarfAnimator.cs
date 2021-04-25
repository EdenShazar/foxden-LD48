using UnityEngine;

public class DwarfAnimator : MonoBehaviour
{
    Animator animator;
    BaseDwarf dwarf;

    readonly int walk = Animator.StringToHash("walk");
    readonly int dig = Animator.StringToHash("dig");
    readonly int mine = Animator.StringToHash("mine");
    readonly int fall = Animator.StringToHash("fall");
    readonly int climbLedge = Animator.StringToHash("climb ledge");
    readonly int stopSign = Animator.StringToHash("stop sign");
    readonly int hammer = Animator.StringToHash("hammer");
    readonly int climbRope = Animator.StringToHash("climb rope");
    readonly int pullUpRope = Animator.StringToHash("pull up rope");

    int previousAnimationHash;

    bool previousIsFalling = false;

    public void Initialize(BaseDwarf dwarf)
    {
        this.dwarf = dwarf;
        animator = GetComponent<Animator>();
        Play(walk);
    }

    void Update()
    {
        if (dwarf.IsFalling && !previousIsFalling)
            animator.Play(fall);
        else if (!dwarf.IsFalling && previousIsFalling)
            PlayPreviousAnimation();

        previousIsFalling = dwarf.IsFalling;
    }

    public void Walk() => Play(walk);
    public void Dig() => Play(dig);
    public void Mine() => Play(mine);
    public void Fall() => Play(fall);
    public void ClimbLedge() => Play(climbLedge);
    public void StopSign() => Play(stopSign);
    public void Hammer() => Play(hammer);
    public void ClimbRope() => Play(climbRope);
    public void PullUpRope() => Play(pullUpRope);

    void Play(int animationHash)
    {
        animator.Play(animationHash);
        previousAnimationHash = animationHash;
    }

    void PlayPreviousAnimation()
    {
        animator.Play(previousAnimationHash);
    }
}
