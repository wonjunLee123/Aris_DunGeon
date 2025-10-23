using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    private Queue<ITurnActor> turnQueue = new Queue<ITurnActor>();
    private bool isProcessingTurn = false;

    // ✅ 게임 오버(플레이어 사망) 상태
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        CombatUnit.OnUnitDied += HandleUnitDied;
    }

    void OnDisable()
    {
        CombatUnit.OnUnitDied -= HandleUnitDied;
    }

    // 플레이어 사망 이벤트 처리
    private void HandleUnitDied(CombatUnit unit)
    {
        if (unit != null && unit.CompareTag("Player"))
        {
            OnPlayerDied();
        }
    }

    // ✅ 플레이어 사망 시 턴 루프 종료
    public void OnPlayerDied()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("🛑 플레이어 사망 → 턴 루프 종료");
        turnQueue.Clear();
        StopAllCoroutines();
        isProcessingTurn = false;
    }

    /// <summary>새로운 유닛 등록</summary>
    public void Register(ITurnActor actor)
    {
        if (isGameOver || actor == null) return;

        if (!turnQueue.Contains(actor))
        {
            turnQueue.Enqueue(actor);
            Debug.Log($"턴 등록됨: {actor}");
        }
    }

    /// <summary>유닛 제거</summary>
    public void Unregister(ITurnActor actor)
    {
        if (actor == null) return;

        if (turnQueue.Contains(actor))
        {
            List<ITurnActor> temp = new List<ITurnActor>(turnQueue);
            temp.Remove(actor);
            turnQueue = new Queue<ITurnActor>(temp);
            Debug.Log($"턴 리스트에서 제거됨: {actor}");
        }
    }

    /// <summary>다음 턴 실행</summary>
    public void NextTurn()
    {
        if (isGameOver || isProcessingTurn || turnQueue.Count == 0)
            return;

        StartCoroutine(ProcessTurns());
    }

    private IEnumerator ProcessTurns()
    {
        isProcessingTurn = true;

        while (!isGameOver && turnQueue.Count > 0)
        {
            ITurnActor current = turnQueue.Dequeue();

            if (current == null) { yield return null; continue; }

            MonoBehaviour mb = current as MonoBehaviour;
            if (mb == null || !mb.gameObject.activeInHierarchy)
            {
                Debug.Log("💀 비활성화된 액터 턴 스킵");
                yield return null;
                continue;
            }

            Debug.Log($"현재 턴: {current}");
            yield return current.TakeTurn();     // ← 여기서 적이 즉시 종료하면 프레임 지연 없이 돌아갈 수 있음

            // ✅ 게임오버 상태면 즉시 종료
            if (isGameOver) break;

            // 행동 후에도 살아있으면만 재등록
            if (mb.gameObject.activeInHierarchy)
                turnQueue.Enqueue(current);

            // 🔸 최소한 한 프레임은 쉬어서 무한 스핀 방지
            yield return null;
        }

        isProcessingTurn = false;
    }
}
