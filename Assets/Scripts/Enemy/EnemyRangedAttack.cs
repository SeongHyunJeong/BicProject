using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyRangedAttack : MonoBehaviour
{
    public Transform player; // 플레이어 Transform
    public GameObject missilePrefab; // 미사일 프리팹
    public float missileSpeed = 10f; // 미사일 속도
    public float trackingDistance = 100f; // 추적 거리
    public float trackingSpeed = 5f; // 추적 속도
    public float idleDistance = 7f; // 공격 거리
    public float attackDuration = 3f; // 공격 지속 시간
    public Animator animator; // 애니메이터
    public Transform target; // 플레이어의 위치
    private NavMeshAgent navMeshAgent;
    private float defaultSpeed; // 기본 이동 속도
    private float attackTimer; // 공격 타이머
    private bool isAttacking; // 현재 공격 중인지 여부
    private float distance; // 플레이어와의 거리
    public ObjectPool missilePool;
    public Transform missileStartTransform;


    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        defaultSpeed = navMeshAgent.speed; // 기본 이동 속도 저장
        navMeshAgent.enabled = true;
        
    }

    private void Update()
    {
        distance = Vector3.Distance(transform.position, player.position);
        navMeshAgent.SetDestination(player.position);

        if (distance > trackingDistance) // 추적 범위 밖
        {
            StopTracking();
        }
        else if (distance <= idleDistance) // 공격 범위 내
        {
            if (!isAttacking)
            {
                StartAttack();
            }
            else
            {
                UpdateAttack();
            }
        }
        else // 추적 범위 내
        {
            StopAttack(); // 추가: 공격 중지
            TrackPlayer();
        }

        // 애니메이션 업데이트
        if (isAttacking)
        {
            animator.SetTrigger("Attack");
        }
        else if (distance <= trackingDistance) // 추적 범위 내에서만 Run 애니메이션 재생
        {
            animator.SetTrigger("run");
        }
    }

    private void StopAttack()
    {
        // 공격 중지
        isAttacking = false;
        attackTimer = 0f;
    }

    private void TrackPlayer()
    {
        // 플레이어를 추적합니다.
        navMeshAgent.speed = trackingSpeed;
    }

    private void StopTracking()
    {
        // 이동을 멈추고 네비게이션 경로를 초기화합니다.
        navMeshAgent.speed = defaultSpeed;
    }

    private void StartAttack()
    {
        // 이동을 멈추고 공격 애니메이션을 재생합니다.
        navMeshAgent.speed = 0f;
        animator.SetTrigger("Attack");

        isAttacking = true;
        attackTimer = 0f;

        FireMissile(); // 미사일 발사
    }

    private void UpdateAttack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackDuration)
        { 
            EndAttack();
        }
    }

    private void FireMissile()
    {
        
        Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
        rotation *= Quaternion.Euler(0f, -90f, 0f);
        GameObject missileObject = Instantiate(missilePrefab, transform.position + transform.forward * 0.5f, rotation);
        Missile missile = missileObject.GetComponent<Missile>();
        
        if (missile != null)
        {
            missile.SetTarget(player);
            missile.Launch(missileSpeed); // 미사일 발사 설정
        }
        

        missilePool.GetObject(transform.position + transform.forward * 0.5f);
    }

    private void EndAttack()
    {
        // 공격을 종료합니다.
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            // Bullet과 충돌하면 death 애니메이션 재생 및 파괴
            animator.SetTrigger("Death");
            // 애니메이션 재생 시간만큼 딜레이 후에 오브젝트 파괴
            float deathAnimationLength = GetDeathAnimationLength();
            StartCoroutine(DestroyAfterDelay(deathAnimationLength));
        }
    }

    private float GetDeathAnimationLength()
    {
        // Death 애니메이션 클립의 길이를 가져옴
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == "Death")
            {
                return clip.length;
            }
        }
        return 0f;
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 딜레이 후에 오브젝트 파괴
        Destroy(gameObject);
    }
}