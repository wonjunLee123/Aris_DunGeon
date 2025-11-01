using UnityEngine;
using Game.AttackElement;

public class CombatUnit : MonoBehaviour
{
    [Header("기본 스탯")]
    public int maxHP = 50;
    public int currentHP = 50;
    public int attackPower = 10;

    [Header("UI")]
    public HealthBarUI healthBarPrefab;
    private HealthBarUI healthBarInstance;

    private PlayerA playerA;
    private EnemyA enemyA;

    public static event System.Action<CombatUnit> OnUnitDied;

    void Start()
    {
        playerA = GetComponent<PlayerA>();
        enemyA = GetComponent<EnemyA>();

        // ✅ currentHP가 0 이하로 저장되어 있으면 자동 복구
        if (currentHP <= 0)
        {
            currentHP = maxHP;
            Debug.LogWarning($"{gameObject.name}의 HP가 0이어서 {maxHP}로 초기화되었습니다.");
        }

        // ✅ 체력바 생성 및 초기화
        if (healthBarPrefab != null)
        {
            Canvas uiCanvas = FindObjectOfType<Canvas>();
            if (uiCanvas != null)
            {
                healthBarInstance = Instantiate(healthBarPrefab, uiCanvas.transform);
                healthBarInstance.SetTarget(transform);
                UpdateHealthUI();
            }
        }
    }

    public void Attack(CombatUnit target)
    {
        if (target == null || target == this) return; // 🔒 자기 자신 공격 방지

        int baseDamage = attackPower;
        float multiplier = CalculateElementMultiplier(this, target);
        int finalDamage = Mathf.RoundToInt(baseDamage * multiplier);

        Debug.Log($"{gameObject.name}이(가) {target.name}을(를) 공격! 피해: {finalDamage} (배율 {multiplier}x)");
        target.TakeDamage(finalDamage);
    }

    float CalculateElementMultiplier(CombatUnit attacker, CombatUnit defender)
    {
        bool aFire = attacker.playerA?.Fire == true || attacker.enemyA?.Fire == true;
        bool aWater = attacker.playerA?.Water == true || attacker.enemyA?.Water == true;
        bool aWind = attacker.playerA?.Wind == true || attacker.enemyA?.Wind == true;
        bool aLight = attacker.playerA?.LightA == true || attacker.enemyA?.LightA == true;
        bool aDark = attacker.playerA?.Dark == true || attacker.enemyA?.Dark == true;

        bool dFire = defender.playerA?.Fire == true || defender.enemyA?.Fire == true;
        bool dWater = defender.playerA?.Water == true || defender.enemyA?.Water == true;
        bool dWind = defender.playerA?.Wind == true || defender.enemyA?.Wind == true;
        bool dLight = defender.playerA?.LightA == true || defender.enemyA?.LightA == true;
        bool dDark = defender.playerA?.Dark == true || defender.enemyA?.Dark == true;

        float mult = 1f;

        // 🔄 속성 상성
        if (aFire && dWind) mult = 1.5f;
        else if (aWind && dWater) mult = 1.5f;
        else if (aWater && dFire) mult = 1.5f;
        else if (aLight && dDark) mult = 1.5f;
        else if (aDark && dLight) mult = 1.5f;

        else if (aFire && dWater) mult = 0.75f;
        else if (aWind && dFire) mult = 0.75f;
        else if (aWater && dWind) mult = 0.75f;

        return mult;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Debug.Log($"{gameObject.name}이(가) {damage}의 피해를 입음 (남은 HP: {currentHP})");

        UpdateHealthUI();

        if (currentHP <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        Debug.Log($"{gameObject.name}이(가) 체력 {amount} 회복 (현재 {currentHP}/{maxHP})");
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthBarInstance != null)
            healthBarInstance.SetFill((float)currentHP / maxHP);
    }

    void Die()
    {
        Debug.Log($"{gameObject.name}이(가) 쓰러졌습니다!");

        OnUnitDied?.Invoke(this);

        if (UnitManager.Instance != null)
            UnitManager.Instance.UnregisterUnit(gameObject);

        ITurnActor actor = GetComponent<ITurnActor>();
        if (actor != null && TurnManager.Instance != null)
            TurnManager.Instance.Unregister(actor);

        if (CompareTag("Player"))
            TurnManager.Instance?.OnPlayerDied();

        if (healthBarInstance != null)
            Destroy(healthBarInstance.gameObject);

        gameObject.SetActive(false);
    }
}
