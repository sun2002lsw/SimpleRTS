using UnityEngine;
using UnityEngine.AI;

public class UnitAnimation : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        moveAnimation();
    }

    void moveAnimation()
    {
        if (IsAttacking())
            return;

        bool isMoving = navMeshAgent.velocity.magnitude > 2;
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
            animator.ResetTrigger("attack"); // attack is triggered, but target is dead
    }

    public void Hold(bool set)
    {
        animator.SetBool("isHolding", set);
    }

    public bool IsHolding()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Hold");
    }

    public void Attack()
    {
        animator.SetBool("isMoving", false);
        animator.SetTrigger("attack");
    }

    public bool IsAttacking()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
    }

    public void TakeDamage()
    {
        bool isIdle = animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        bool isHold = false;
        if (isIdle || isHold)
            animator.SetTrigger("takeDamage");
    }

    public void Death()
    {
        int deathAnimationIdx = Random.Range(1, 3); // 1 or 2
        animator.SetTrigger("death" + deathAnimationIdx);
    }
}
