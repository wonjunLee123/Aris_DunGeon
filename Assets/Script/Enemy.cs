using System.Collections;
using UnityEngine;
using Game.AttackElement;

public class Enemy : MonoBehaviour, ITurnActor
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;
    public Vector2 blockCheckSize = new Vector2(0.9f, 0.9f);

    private Transform player;
    private Vector2Int currentTile;
    private CombatUnit combatUnit;
    private EnemyA enemyA;

    void Start()
    {
        StartCoroutine(Register());
        combatUnit = GetComponent<CombatUnit>();
        enemyA = GetComponent<EnemyA>();
    }

    IEnumerator Register()
    {
        yield return new WaitUntil(() => TurnManager.Instance != null && UnitManager.Instance != null);

        currentTile = UnitManager.Instance.WorldToTilePos(transform.position);
        UnitManager.Instance.RegisterUnit(currentTile, gameObject);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        TurnManager.Instance.Register(this);
    }

    public IEnumerator TakeTurn()
    {
        // 플레이어가 없거나 비활성화된 경우 턴 종료
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            Debug.Log("⚫ 플레이어 없음 → 턴 종료");
            yield break;
        }

        Vector2Int playerTile = UnitManager.Instance.WorldToTilePos(player.position);

        // 근접 공격 가능?
        int dx = Mathf.Abs(playerTile.x - currentTile.x);
        int dy = Mathf.Abs(playerTile.y - currentTile.y);

        if (dx <= 1 && dy <= 1)
        {
            CombatUnit playerUnit = player.GetComponent<CombatUnit>();
            PlayerA playerA = player.GetComponent<PlayerA>();

            if (playerUnit != null && playerA != null)
            {
                float mult = CalculateElementMultiplier(enemyA, playerA);
                int finalDamage = Mathf.RoundToInt(combatUnit.attackPower * mult);
                Debug.Log($"👹 {name}이(가) 플레이어 공격! 피해: {finalDamage}");
                playerUnit.TakeDamage(finalDamage);
            }

            yield return new WaitForSeconds(0.2f);
            yield break;
        }

        // 플레이어 방향으로 한 칸 이동
        Vector2Int dir = new Vector2Int(
            Mathf.Clamp(playerTile.x - currentTile.x, -1, 1),
            Mathf.Clamp(playerTile.y - currentTile.y, -1, 1)
        );

        Vector2Int nextTile = currentTile + dir;

        if (IsBlocked(nextTile))
        {
            Debug.Log($"🚫 {name} 이동 불가 (막힘)");
            yield break;
        }

        // 실제 이동
        Vector2 worldTarget = (Vector2)nextTile;
        while (Vector2.Distance(transform.position, worldTarget) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, worldTarget, moveSpeed * Time.deltaTime);
            yield return null;
        }

        UnitManager.Instance.MoveUnit(currentTile, nextTile, gameObject);
        currentTile = nextTile;

        yield return new WaitForSeconds(0.1f);
    }

    bool IsBlocked(Vector2Int tile)
    {
        Vector2 center = (Vector2)tile;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, blockCheckSize, 0f);

        foreach (var hit in hits)
        {
            if (hit == null || hit.gameObject == gameObject) continue;

            if (hit.CompareTag("Unwalkable") ||
                hit.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                return true;
        }

        GameObject unit = UnitManager.Instance.GetUnitAtPosition(tile);
        if (unit != null && unit != gameObject) return true;
        return false;
    }

    float CalculateElementMultiplier(EnemyA attacker, PlayerA target)
    {
        if (attacker == null || target == null) return 1f;

        if (attacker.Fire && target.Wind) return 1.5f;
        if (attacker.Wind && target.Water) return 1.5f;
        if (attacker.Water && target.Fire) return 1.5f;
        if (attacker.LightA && target.Dark) return 1.5f;
        if (attacker.Dark && target.LightA) return 1.5f;

        if (attacker.Fire && target.Water) return 0.75f;
        if (attacker.Wind && target.Fire) return 0.75f;
        if (attacker.Water && target.Wind) return 0.75f;

        return 1f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 center = Application.isPlaying ? (Vector2)currentTile : (Vector2)transform.position;
        Gizmos.DrawWireCube(center, blockCheckSize);
    }
}
