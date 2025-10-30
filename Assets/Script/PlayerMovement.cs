using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, ITurnActor
{
    [Header("이동 설정")]
    public float moveStep = 1f;
    private bool moved = false;
    private Vector2Int currentTile;

    private CombatUnit combatUnit;
    private InventorySystem inventory;
    private Animator anim;  // ✅ Animator 추가


    void Start()
    {
        combatUnit = GetComponent<CombatUnit>();
        inventory = GetComponent<InventorySystem>();
        anim = GetComponentInChildren<Animator>(); // ✅ Animator 가져오기

        if (combatUnit == null)
            Debug.LogWarning("⚠️ CombatUnit 컴포넌트가 없습니다!");
        if (inventory == null)
            Debug.LogWarning("⚠️ InventorySystem 컴포넌트가 없습니다!");
        if (anim == null)
            Debug.LogWarning("⚠️ Animator를 찾을 수 없습니다!");

        StartCoroutine(Register());
    }

    IEnumerator Register()
    {
        yield return new WaitUntil(() => TurnManager.Instance != null && UnitManager.Instance != null);

        currentTile = UnitManager.Instance.WorldToTilePos(transform.position);
        UnitManager.Instance.RegisterUnit(currentTile, gameObject);
        TurnManager.Instance.Register(this);

        if (CompareTag("Player"))
        {
            TurnManager.Instance.NextTurn();
        }
    }

    public IEnumerator TakeTurn()
    {
        moved = false;

        while (!moved)
        {
            Vector2Int direction = Vector2Int.zero;

            // ✅ 이동 입력 감지
            if (Input.GetKeyDown(KeyCode.W)) direction = Vector2Int.up;
            else if (Input.GetKeyDown(KeyCode.S)) direction = Vector2Int.down;
            else if (Input.GetKeyDown(KeyCode.A)) direction = Vector2Int.left;
            else if (Input.GetKeyDown(KeyCode.D)) direction = Vector2Int.right;
            else if (Input.GetKeyDown(KeyCode.Q)) direction = new Vector2Int(-1, 1);
            else if (Input.GetKeyDown(KeyCode.E)) direction = new Vector2Int(1, 1);
            else if (Input.GetKeyDown(KeyCode.Z)) direction = new Vector2Int(-1, -1);
            else if (Input.GetKeyDown(KeyCode.C)) direction = new Vector2Int(1, -1);

            // ✅ 애니메이션 실행 (움직임 입력 있을 때)
            anim?.SetBool("Run", direction != Vector2Int.zero);

            if (direction != Vector2Int.zero)
            {
                Vector2Int targetTile = currentTile + direction;

                Collider2D wall = Physics2D.OverlapPoint((Vector2)targetTile);
                if (wall != null && wall.CompareTag("Unwalkable"))
                {
                    Debug.Log("🚫 이동 불가: 벽이 있음");
                    anim?.SetBool("Run", false); // ✅ 이동 실패 시 멈춤
                    yield return null;
                    continue;
                }

                GameObject targetUnit = UnitManager.Instance.GetUnitAtPosition(targetTile);
                if (targetUnit != null && targetUnit.CompareTag("Enemy"))
                {
                    CombatUnit enemyUnit = targetUnit.GetComponent<CombatUnit>();
                    if (combatUnit != null && enemyUnit != null)
                    {
                        combatUnit.Attack(enemyUnit);
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ 공격 불가: CombatUnit이 누락됨");
                    }

                    anim?.SetBool("Run", false); // 공격 후 멈춤
                    moved = true;
                    continue;
                }

                yield return MoveTo(targetTile);
                moved = true;
            }
            else
            {
                anim?.SetBool("Run", false);
            }

            yield return null;
        }
    }

    IEnumerator MoveTo(Vector2Int targetTile)
    {
        Vector2 worldTarget = (Vector2)targetTile;
        while (Vector2.Distance(transform.position, worldTarget) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, worldTarget, 5f * Time.deltaTime);
            yield return null;
        }

        transform.position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));

        UnitManager.Instance.MoveUnit(currentTile, targetTile, gameObject);
        currentTile = targetTile;

        anim?.SetBool("Run", false); // ✅ 이동 완료 후 애니메이션 멈춤
    }
}
