using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public bool inCombat;
    [Header("Modifiers")]
    public float skillHpModifierReal = 1;
    public float skillHpModifier{ get { return skillHpModifierReal; }set { skillHpModifierReal = value; uiBar.Setup(() => hp, hp, maxHp); }}
    public float skillManaModifier = 1;
    public float skillSpeedModifier = 1;
    public float skillDmgModifier = 1;
    public float dmgModifier;
    // Properties with custom getters for max values
    public float maxHp
    {
        get { return defaultHp * skillHpModifier; }
      
    }

    public float maxMana
    {
        get { return defaultMana * skillManaModifier; }
    }

    public float maxSpeed
    {
        get { return defaultSpeed * skillSpeedModifier; }
    }

    [Header("Refrences")]
    HitDetection hitDetection;
    [Header("Stats")]
    public LayerMask faction;
    public float hp;
    public float mana;
    public float speed;

    public float xpp;
    public GameObject coinPrefab;

    private bool oneTime;
    public void FixedUpdate()
    {
        inCombat = false;
    }
    public void OnDestroy()
    {
        int numberOfCoins = Random.Range(2, 6);
        if(coinPrefab != null)
        {
            for (int i = 0; i < numberOfCoins; i++)
            {
                // Calculate a random position around the enemy within the spawn radius
                Vector3 randomPosition = transform.position + Random.insideUnitSphere * 2;

                // Instantiate the coin prefab at the random position
                GameObject coin = Instantiate(coinPrefab, randomPosition, Quaternion.identity);

                // Add a random force to the coin to make it jump
                Rigidbody coinRigidbody = coin.GetComponent<Rigidbody>();
                if (coinRigidbody != null)
                {
                    Vector3 randomDirection = Random.insideUnitSphere.normalized;
                    float randomForce = Random.Range(5, 10);
                    coinRigidbody.AddForce(randomDirection * randomForce, ForceMode.Impulse);
                }
            }
        }
      
    }
    public void LateUpdate()
    {
        if (inCombat && oneTime == false)
        {
            oneTime = true;
            if(GetComponent<SoundManager>() != null)
            GetComponent<SoundManager>().PlayMusic("CombatMusic");

        }
        if (inCombat == false && oneTime == true)
        {
            oneTime = false;
            if (GetComponent<SoundManager>() != null)
                GetComponent<SoundManager>().PlayMusic("Music1");
        }
    }
    public float xp
    {
        get { return xpp; }
        set
        {
        
            xpp = value;
            if (xpp >= maxXp)
                AddLevel();
        }
    }

    protected float maxXp = 100;
    public int level;
    public int skillPoint;


    private float defaultHp;
    private float defaultMana;
    private float defaultSpeed;

    [SerializeField] UiBar uiBar;

    public bool doOnce = false;


    public virtual void AddLevel()
    {
       
            level++;
            skillPoint++;
            maxXp *= 2f;
            
        
    }

    public virtual void Start()
    {
        hitDetection = GetComponent<HitDetection>();
        defaultHp = hp;
        defaultMana = mana;
        defaultSpeed = speed;

        uiBar.Setup(() => hp, hp, maxHp);
    }
    public void Update()
    {
        if(hp <= 0 && doOnce== false)
        {
            doOnce = true;
            
                Debug.Log("death");
                FindObjectOfType<PL_Stats>().xp += 50;
           
            Destroy(this.gameObject);
        }
       
            mana += Time.deltaTime + 10f;
        
        if (mana > maxMana)
            mana = maxMana;
    }
}
