using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRun : StateMachineBehaviour
{
    private float speed = 2.5f;
    private float attackRange = 5f;

    GameObject[] players;
    Transform player;
    Rigidbody2D boss;
    BossController bossController;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        players = CombatManager.players;

        if(players.Length != 0)
        {
            player = players[Random.Range(0, players.Length)].transform;
        }

        boss = animator.GetComponent<Rigidbody2D>();
        bossController = animator.GetComponent<BossController>();
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(player != null)
        {
            bossController.LookAtPlayer(player);

            Vector2 target = new Vector2(player.position.x, boss.position.y);
            Vector2 position = Vector2.MoveTowards(boss.position, target, speed * Time.fixedDeltaTime);

            boss.MovePosition(position);

            if (Vector2.Distance(player.position, boss.position) <= attackRange)
            {
                animator.SetTrigger("Attack");
            }
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }
}
