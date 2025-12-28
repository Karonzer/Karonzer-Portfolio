using System;
using UnityEngine;

/// <summary>
/// 공격 주체 타입
/// - 누가 공격했는지 구분할 때 사용
/// </summary>
public enum Type
{
	Player,
	Enemy,
}

/// <summary>
/// 오브젝트 풀 타입
/// - SpawnManager에서 어떤 풀을 사용할지 구분
/// </summary>
public enum PoolObjectType
{
	player,
	Projectile,
	Enemy,
	Item,
	Actor,
}

/// <summary>
/// 풀 오브젝트가 들어갈 부모 Transform 이름
/// </summary>
public enum PoolObjectTypeSpawnParents
{
	EnemySpawnGroup,
	ProjectileSpawn,
	ItemSpawnGroup,
	ActorSpawnGroup,
}

/// <summary>
/// 씬별 배경음악 타입
/// </summary>
public enum SceneBGMType
{
	None,
	Title,
	Loading,
	Battle
}

/// <summary>
/// 적 상태 머신에서 사용하는 상태 ID
/// </summary>
public enum StateID
{
	tracking,
	Attack,
	die,
}

/// <summary>
/// 버프 적용 대상
/// </summary>
public enum BuffType
{
	Player,
	Skill
}

/// <summary>
/// 버프 효과 종류
/// </summary>
public enum BuffEffectType
{
	DamagePercent, // 데미지 % 증가
	MoveSpeedPercent, // 이동속도 % 증가
	AttackSpeedPercent, // 공격속도 % 증가
}

/// <summary>
/// 업그레이드 선택 타입
/// </summary>
public enum UpgradeOptionType
{
	SkillUpgrade,   // 기존 스킬 강화
	SkillUnlock,     // 새로운 스킬 획득
	Global // 전체 능력 강화
}

/// <summary>
/// 업그레이드 효과 종류
/// </summary>
public enum UpgradeEffectType
{
	DamagePercent,          // 데미지 % 증가
	AttackSpeedPercent,     // 공격 간격 % 감소
	RangeFlat,              // 사거리 고정 증가
	ProjectileSpeedPercent, // 투사체 속도 % 증가
	ExplosionRangeFlat,     // 폭발 범위 고정 증가
	ExtraProjectileCount,    // 추가 발사 수 등

	MoveSpeed,          // 이동 % 속도
	CurrentHPAndMaxHP,  // 체력 % 증가
	CriticalDamage,     // 치명타 % 증가
	CriticalChance,     // 치명타 확률 % 증가
}

/// <summary>
/// UI 종류
/// </summary>
public enum UIType
{
	None,
	XPBar,
	BossHP,
	PlayerHP,
	PlayerHPFollow,
	UpgradePopUp,
	TextPopUp,
	Timer,
	GameOver,
	Pause,
	titleBtns,
	titleSelect,
}

/// <summary>
/// 사운드 종류
/// </summary>
public enum SoundType
{
	UI_Click,
	Player_Move,
	Player_Jump,
	Player_Die,
	Player_Hit,
	Enemy_attack,
	Enemy_Hit,
	Enmey_Projectile,
	Skill_Fireball,
	Skill_Lightning,
	Skill_Meteo,
	Skill_Shockwave,
	Skill_Penetrating,
	Item,
	Upgrade,
	LevelUp,
	BGM_Title,
	BGM_Battle,
}

/// <summary>
/// 상호작용 가능한 오브젝트 인터페이스
/// </summary>
public interface IInteractable
{
	GameObject currentObj { get; }
	bool CanInteract { get; }
	void Interact(GameObject interactor);
}

/// <summary>
/// UI 처리용 인터페이스
/// </summary>
public interface IUIHandler
{
	UIType Type { get; }
	void Show();
	void ShowAndInitialie(GameObject _obj = null);
	void Hide();
	public bool IsOpen { get; }
}

/// <summary>
/// 상태 머신 인터페이스
/// </summary>
public interface IState<T>
{
	StateID ID { get; }
	void OnEnter(T owner);
	void OnExit(T owner);
	void Tick(T owner);
}

/// <summary>
/// 경험치 테이블 인터페이스
/// </summary>
public interface IXPTable
{
	public int CurrentLevel { get; }
	public int CurrentXP { get; }
	public int MaxXP { get; }

	public event Action<int> OnLevelChanged;
	public event Action<int, int> OnXPChanged;
}

/// <summary>
/// 오디오 처리 인터페이스
/// </summary>
public interface IAudioHandler
{	
	public void Play(SoundType _type);
	public void Play_OneShot(SoundType _type);
	public void Stop();
}

/// <summary>
/// 버프 데이터 구조
/// </summary>
[System.Serializable]
public class BuffData
{
	public BuffType type;
	public BuffEffectType effectType;
	public float value;      // +20%, +30%
	public float duration;   // 몇 초 동안 유지?
	public string description;
}

/// <summary>
/// 플레이어 능력치 구조체
/// </summary>
[System.Serializable]
public struct PlayerStruct
{
	public string key;
	public float moveSpeed;
	public float maxHP;
	public float currentHP;
	public int criticalDamage;
	public int criticalChance;
	public string startAttackObj;
}

/// <summary>
/// 적 능력치 구조체
/// </summary>
[System.Serializable]
public struct EnemyStruct
{
	public string key;
	public float moveSpeed;
	public float maxHP;
	public float currentHP;
	public float attackInterval;
	public float rawDamage;
	public int damage => Mathf.RoundToInt(rawDamage);
	public float attackRange;
	public int xpItmeValue;
}

/// <summary>
/// 스킬(공격) 기본 스탯
/// </summary>
[System.Serializable]
public struct AttackStats
{
	[Header("Key (스킬 이름)")]
	public string key;

	[Header("Base")]
	public float rawDamage;
	public int baseDamage => Mathf.RoundToInt(rawDamage);
	public float baseAttackInterval;
	public float baseRange;
	public float baseProjectileSpeed;
	public float baseExplosionRange;
	public string projectileKey;
	public int ProjectileCount;
}

/// <summary>
/// 보스 스킬 데이터
/// </summary>
[System.Serializable]
public struct BossSkill
{
	public BossProjectile projectile;
}

/// <summary>
/// 보스 투사체 정보
/// </summary>
[System.Serializable]
public struct BossProjectile
{
	[Header("Key (스킬 이름)")]
	public string key;
	public int keyCount;
	public float baseProjectileSpeed;
	public float baseExplosionRange;
}

/// <summary>
/// 데미지 정보 전달용 구조체
/// </summary>
[System.Serializable]
public struct DamageInfo
{
	public int damage;
	public Vector3 hitPoint;
	public Type attacker;
	public bool isCritical;

	public DamageInfo(int damage, Vector3 hitPoint, Type attacker, bool isCritical)
	{
		this.damage = damage;
		this.hitPoint = hitPoint;
		this.attacker = attacker;
		this.isCritical = isCritical;
	}
}


/// <summary>
/// 체력 변경 감지 인터페이스
/// </summary>
public interface IHealthChanged
{
	public event Action<float, float> OnHealthChanged;
	public event Action<IDamageable> OnDead;
	float CurrentHPHealth { get; }
	float MaxHPHealth { get; }
}

/// <summary>
/// 데미지를 받을 수 있는 대상 인터페이스
/// </summary>
public interface IDamageable
{
	float CurrentHPDamege { get; }
	float MaxHPDamege { get; }
	GameObject CurrentObj { get; }
	public event Action<DamageInfo> OnDamaged;
	public event Action<IDamageable> OnDead;

	void Take_Damage(DamageInfo info);
}

/// <summary>
/// 몬스터 공격 인터페이스
/// </summary>
public interface IEnemyDoAttack
{
	public void Do_EnemyAttackEvent();
	public EnemyStruct EnemyStruct { get; }
}

/// <summary>
/// DBManager
/// 
/// 게임 전반에서 사용하는 공통 계산,상수 관리 클래스
/// </summary>
public static class DBManager
{
	// 투사체 생존 시간(초)
	private static int projectileSurvivalTime = 30;
	public static int ProjectileSurvivalTime => projectileSurvivalTime;

	// 치명타 계산용 랜덤
	private static System.Random rand = new System.Random();

	// 치명타 여부를 계산하고 최종 데미지를 반환
	public static int CalculateCriticalDamage(this PlayerStruct stats, int baseDamage, out bool isCritical)
	{
		int chance = rand.Next(0, 100);

		isCritical = chance < stats.criticalChance;

		if (isCritical)
		{
			float multiplier = 1f + (stats.criticalDamage / 100f);
			return Mathf.RoundToInt(baseDamage * multiplier);
		}
		else
		{
			return baseDamage;
		}
	}


}

