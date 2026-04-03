using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Playerr : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 movement;

    [Header("공격 설정")]
    public float attackRange = 3f;      // 공격 가능 거리
    public int attackDamage = 10;       // 공격력
    public LayerMask enemyLayer;        // 적중할 적의 레이어

    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        // 탑뷰에서 캐릭터가 물리 충돌로 인해 넘어지는 것을 방지
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 1. 입력 처리 (Update에서 처리)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // 대각선 이동 시 속도 증가를 막기 위해 normalized 사용
        movement = new Vector3(moveX, 0f, moveZ).normalized;

        // 2. 좌클릭 공격 입력 처리
        if (Input.GetMouseButtonDown(0))
        {
            TryAttackTarget();
        }
    }

    void FixedUpdate()
    {
        // 3. 물리 기반 이동 적용 (FixedUpdate에서 처리)
        MovePlayer();
    }

    private void MovePlayer()
    {
        // Rigidbody.MovePosition을 사용하여 물리 엔진과 충돌하지 않게 부드러운 이동 처리
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        // 이동 방향으로 캐릭터가 부드럽게 회전하도록 처리 (선택 사항)
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, toRotation, 720f * Time.fixedDeltaTime);
        }
    }

    private void TryAttackTarget()
    {
        // 카메라에서 현재 마우스 커서 위치로 향하는 Ray 생성
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Raycast를 발사하여 클릭한 대상 검출 (enemyLayer에 속한 오브젝트만 감지)
        if (Physics.Raycast(ray, out hit, 100f, enemyLayer))
        {
            GameObject target = hit.collider.gameObject;

            // 캐릭터가 클릭한 타겟을 바라보도록 회전 (y축은 고정)
            Vector3 lookPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(lookPoint);

            // 내 위치와 타겟 위치의 거리 계산
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance <= attackRange)
            {
                // 타겟이 사거리 안에 있을 때 공격 실행
                Debug.Log($"[{target.name}]에게 {attackDamage}의 데미지를 입혔습니다!");

                // 실제 게임 적용 시 아래와 같이 적의 체력을 깎는 코드를 호출합니다.
                // target.GetComponent<EnemyHealth>().TakeDamage(attackDamage);
            }
            else
            {
                Debug.Log("타겟이 공격 사거리보다 멀리 있습니다!");
            }
        }
    }
}