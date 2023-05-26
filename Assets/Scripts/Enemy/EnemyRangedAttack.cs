using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyRangedAttack : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    public Transform player; // 플레이어 Transform
    [SerializeField] private Animator animator; // 애니메이터
    public Transform target; // 플레이어의 위치
    public ObjectPool missilePool;
    private bool isAttacking; // 현재 공격 중인지 여부
    [SerializeField] private bool isDeath;
    private float distance; // 플레이어와의 거리

    private float timer;
    private float timerDuration;

    private GameManagers gameManagerScript;    //게임 매니저 스크립트
    private MonsterManager monsterManagerScript;

    private void Start()
    {
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagers>();
        monsterManagerScript = GameObject.Find("MonsterManage").GetComponent<MonsterManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        timer = 0f;
        timerDuration = 5f;
        isDeath = false;
    }

    private void Update()
    {
        if (gameManagerScript.IsPlaying() && !gameManagerScript.IsStageClear())
        {
            distance = Vector3.Distance(transform.position, player.position);
            navMeshAgent.SetDestination(player.position);

            if (isDeath)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                    gameObject.SetActive(false);
            }
            else if (!isDeath)
            {
                //목적지 도착
                if (distance <= 12f)
                {
                    StartAttack();
                    navMeshAgent.isStopped = true;
                    animator.SetTrigger("Idle");
                }
                else
                {
                    animator.SetTrigger("Run");
                    navMeshAgent.isStopped = false;
                }
            }
        }
    }

    private void StartAttack()
    {
        timer += Time.deltaTime;
        transform.LookAt(player.transform);
        if (timer >= timerDuration)
        {
            isAttacking = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
            rotation *= Quaternion.Euler(0f, -90f, 0f);
            missilePool.GetObject(transform.position + transform.forward, rotation);
            timer = 0;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            // Bullet과 충돌하면 death 애니메이션 재생 및 파괴
            isDeath = true;
            animator.SetTrigger("Death");
            monsterManagerScript.monsterCount++;
        }
    }

    
}