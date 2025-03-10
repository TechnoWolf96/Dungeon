using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Creature, IAttackWithWeapon
{
    public event CreatureEvent onPlayerDeath;
    private static Player _singleton;
    public static Player singleton { get => _singleton; }
    [HideInInspector] public bool controled = true;
    public PlayerWeapon currentWeapon { get; set; }
    public Transform weaponSlot { get; protected set; }

    public const float takeItemRadius = 0.85f;
    [HideInInspector] public bool pause = false;



    private void Awake()
    {
        _singleton = this;
        anim = GetComponent<Animator>();
        weaponSlot = gameObject.transform.Find("Weapon");
    }


    protected override void Update()
    {
        base.Update();
        Move();
    }

    public void AttackMoment()
    {
        if (PlayerWeaponChanger.singleton.currentWeaponType == CurrentWeaponType.Ranged)
        {
            RangedWeaponPlayer rangedWeapon = (RangedWeaponPlayer)currentWeapon;
            rangedWeapon.bullet = PlayerWeaponChanger.singleton.GetAndUseArrow();
        }
        currentWeapon.Attack();
    }

    private void Move()
    {
        if ((Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
            && controled) anim.SetBool("Move", true);
        else anim.SetBool("Move", false);
    }

    public void RangedAttack()
    {
        if (InventoryInfoSlot.singleton.rangedWeaponSlot.transform.childCount == 0) return; // ��� � ����� ������ - ������ �� ������
        if (PlayerWeaponChanger.singleton.currentWeaponType != CurrentWeaponType.Ranged)
        {
            PlayerWeaponChanger.singleton.SelectWeapon(CurrentWeaponType.Ranged);
        }
        if (ArrowCounter.singleton.count == 0) return;
        anim.SetTrigger("Attack");
    }

    public void CloseAttack()
    {
        if (InventoryInfoSlot.singleton.meleeWeaponSlot.transform.childCount == 0) return; // ��� � ����� ������ - ������ �� ������
        if (PlayerWeaponChanger.singleton.currentWeaponType != CurrentWeaponType.Melee)
        {
            PlayerWeaponChanger.singleton.SelectWeapon(CurrentWeaponType.Melee);
        }
        anim.SetTrigger("Attack");
    }

    public void PlaySoundAttack() => currentWeapon.PlaySound();


    public void TakeItem()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, takeItemRadius);
        foreach (var item in colliders)
        {
            if (item.TryGetComponent(out DroppedItem dropItem))
            {
                dropItem.Take();
                return;
            }
        }
        
    }

    public override void Death()
    {
        base.Death();
        SceneManager.LoadScene(0);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, takeItemRadius);
    }

}
