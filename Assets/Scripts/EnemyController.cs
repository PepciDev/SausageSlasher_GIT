using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool dead = false;

    public Transform upBodyPos;
    public Transform lowBodyPos;

    public Animator enemyAnimator;

    public GameObject enemyRagdoll;
    public GameObject upperBody;
    public GameObject lowerBody;

    public GameObject rightArmor;
    public GameObject leftArmor;
    public GameObject doubleArmorL;
    public GameObject doubleArmorR;
    public GameObject shieldVisual;
    public GameObject shieldShatter;
    public GameObject hitParticle;

    public GameObject headband;
    public GameObject helmet;
    bool helmetOn = false;

    bool shield = false;
    bool doubleArmor = false;

    public GameObject shatterParticle;
    public GameObject ambushParticle;

    public GameObject sparkParticle;
    public Transform katana;

    public GameObject shoulderPosL;
    public GameObject shoulderPosR;

    int points = 10;
    Spawner spawner;
    PlayerController playerc;

    bool idling = true;

    bool readyToAttack = true;
    bool attackCancelled = false;

    bool lockPlayerAttack = false;

    bool slashingLeft = false;
    bool slashingRight = false;
    bool slashingFront = false;
    public bool longSlash = false;

    bool deflectStart = false;
    bool deflectingLeft = false;
    bool deflectingRight = false;

    int leftHealth = 1;
    int rightHealth = 1;
    public bool randomizeStats = true;

    int damageAmount = 1;

    bool stunned = false;
    bool stunTransition = false;
    bool smallStunTransition = false;

    public bool spawning = true;

    //enemy difficulty 0 - 5
    public int difficulty = 0;

    //each number corresponds to a specific attack
    /// 1 = long left
    /// 2 = long right
    /// 3 = left
    /// 4 = right 
    /// 5 = front

    int randomAttack = 4;

    int lastAttack = 0;
    int lastestAttack = 0;

    //idling time / delay between attacks
    float attackDelay;

    //how many chain attacks can the enemy do in a row
    int chainMax = 5;
    int chainCurrent = 0;
    bool chainStun = false;
    bool chainAttack = false;

    //enemy speed
    float enemySpeed = 1f;

    void Start()
    {
        //TEMP TIME TEST
        //Time.timeScale = 0.65f;

        spawner = GameObject.Find("Spawner").GetComponent<Spawner>();

        //Randomize stats at start:
        if (randomizeStats)
        {
            //HEALTH AND ARMOR
            // x% -random cahnce that the health is 2, health is 1 by default
            if (RandomChance(50))
            {
                leftHealth = 2;
                //left side armor
                leftArmor.SetActive(true);
            }
            if (RandomChance(50))
            {
                rightHealth = 2;
                //right side armor
                rightArmor.SetActive(true);
            }
            //in case of double armor
            if (rightArmor.activeSelf && leftArmor.activeSelf)
            {
                //armor on both sides
                doubleArmor = true;
                doubleArmorL.SetActive(true);
                doubleArmorR.SetActive(true);

                //set different armors non active
                rightArmor.SetActive(false);
                leftArmor.SetActive(false);
            }

            //CHAIN ATTACKS
            //chainMax = difficulty;
            if (RandomChance(40))
            {
                chainCurrent = Random.Range(0, chainMax + 1);
            }
            else
            {
                chainCurrent = 0;
            }

            //SHIELD
            if (RandomChance(50))
            {
                shield = true;
                shieldVisual.SetActive(true);

                //headband & helmet visuals
                helmetOn = true;
                headband.SetActive(false);
                helmet.SetActive(true);
            }

            //ENEMY SPEED
            enemySpeed = Random.Range(1f, 1.35f);
            enemyAnimator.speed = enemySpeed;
            Debug.Log(enemySpeed);
        }

        attackDelay = 3f;
        playerc = GameObject.Find("Player").GetComponent<PlayerController>();

        //spawn
        idling = false;
        spawning = true;
        enemyAnimator.SetInteger("AnimationInt", 10);
    }

    void Update()
    {
        //SPAWN
        if (spawning)
        {
            //last frame
            if (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Spawn") && enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !enemyAnimator.IsInTransition(0))
            {
                spawning = false;
                enemyAnimator.SetInteger("AnimationInt", 0);
                idling = true;

                lockPlayerAttack = false;
            }
        }

        //SLASHING
        //attack countdown start & attack delay set
        if (idling && readyToAttack)
        {
            attackCancelled = false;

            readyToAttack = false;
            attackDelay = Random.Range(1.2f, 2.5f);
            StartCoroutine("AttackStarter");
        }

        //ULTIMATE ATTACKED
        if (playerc.ulting)
        {
            //enemy cant attack while ulting
            readyToAttack = false;
            attackCancelled = true;

            if (playerc.ultHit)
            {
                //enemy gets hit by ult
                playerc.ultHit = false;
                //Debug.Log(playerc.FireIceElectric);

                lockPlayerAttack = true;

                smallStunTransition = true;
                enemyAnimator.SetInteger("AnimationInt", 9);
            }
        }

        //attack method
        if (slashingRight || slashingLeft || slashingFront)
        {
            Attack();
        }

        //DEFLECTING
        //deflect start
        if ((playerc.attackAction && !stunned && idling) && (playerc.slashingLeft || playerc.slashingRight))
        {
            idling = false;
            playerc.attackAction = false;
            attackCancelled = true;
            deflectStart = true;
        }

        //deflect at right time
        if (deflectStart && (playerc.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SlashLeft") ||
            playerc.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SlashRight")) &&
            playerc.playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f)
        {
            if (playerc.slashingLeft)
            {
                deflectingLeft = true;
                enemyAnimator.SetInteger("AnimationInt", -1);
                deflectStart = false;
            }
            else if (playerc.slashingRight)
            {
                deflectingRight = true;
                enemyAnimator.SetInteger("AnimationInt", -2);
                deflectStart = false;
            }
        }

        //deflect method
        if (deflectingLeft || deflectingRight)
        {
            Deflect();
        }


        //PLAYER CAN SLASH MANAGEMENT
        if ((idling || stunned) && !lockPlayerAttack)
        {
            playerc.canSlash = true;
        }
        else
        {
            playerc.canSlash = false;
        }

        //ENEMY STUN
        if (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.EnemyStunned"))
        {
            if (playerc.hit && stunned)
            {
                if (shield)
                {
                    ShieldBreak();
                }
                else
                {
                    //enemy loses health depending on the diretion of the players attack (left or right)
                    if (playerc.slashingLeft)
                    {
                        //right armor breaks
                        if (rightHealth == 2)
                        {
                            if (doubleArmor)
                            {
                                doubleArmorR.SetActive(false);
                            }
                            else
                            {
                                rightArmor.SetActive(false);
                            }

                            Instantiate(shatterParticle, shoulderPosR.transform.position, shoulderPosR.transform.rotation);
                        }

                        rightHealth -= 1;
                    }
                    else if (playerc.slashingRight)
                    {
                        //left armor breaks
                        if (leftHealth == 2)
                        {
                            if (doubleArmor)
                            {
                                doubleArmorL.SetActive(false);
                            }
                            else
                            {
                                leftArmor.SetActive(false);
                            }

                            Instantiate(shatterParticle, shoulderPosL.transform.position, shoulderPosL.transform.rotation);
                        }

                        leftHealth -= 1;
                    }

                    //SLICED DEATH -if either sides health is <= 0
                    if ((leftHealth <= 0 || rightHealth <= 0) && !dead)
                    {
                        dead = true;
                        Sliced();
                    }
                }

                playerc.hit = false;
                lockPlayerAttack = true;
                playerc.attackAction = false;

                smallStunTransition = true;
                enemyAnimator.SetInteger("AnimationInt", 9);

            }

            if (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !enemyAnimator.IsInTransition(0))
            {
                readyToAttack = true;
                stunned = false;
                idling = true;
                enemyAnimator.SetInteger("AnimationInt", 0);
            }

        }

        //SMALL STUN TRANSITION
        if (smallStunTransition)
        {
            if ((enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.EnemySmallStun") && enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !enemyAnimator.IsInTransition(0)))
            {
                lockPlayerAttack = false;
                readyToAttack = true;
                stunned = false;

                idling = true;
                enemyAnimator.SetInteger("AnimationInt", 0);

                smallStunTransition = false;
            }
        }

        //CHAIN STUN
        if (chainStun && enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.EnemyChainAttackBreak"))
        {
            if (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !enemyAnimator.IsInTransition(0))
            {
                attackDelay = 0f;
                StartCoroutine("AttackStarter");
                idling = true;
            }
        }

        //DESPAWN
        if (idling && playerc.health <= 0)
        {
            idling = false;
            enemyAnimator.SetInteger("AnimationInt", 11);
        }
        if (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.EnemyDespawn"))
        {
            if (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f)
            {
                //dissapear
                Destroy(this.gameObject);
            }
        }

        //STUN TRANSITION
        if (stunTransition && enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.EnemyStun"))
        {

            if (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.75f)
            {
                //transition end 1 frame
                enemyAnimator.SetInteger("AnimationInt", 8);
                stunTransition = false;
            }
        }
    }

    void ShieldBreak()
    {
        //shield breaks
        shield = false;
        shieldVisual.SetActive(false);
        Instantiate(shieldShatter, shieldVisual.transform.position, shieldShatter.transform.rotation);
    }

    void Sliced()
    {
        //final hit particles
        Instantiate(hitParticle, new Vector3(upBodyPos.position.x, upBodyPos.position.y + 0.3f, upBodyPos.position.z), hitParticle.transform.rotation);

        //player cant attack when the enemy is dead (prevents some exploits, propably?.)
        lockPlayerAttack = true;
        playerc.canSlash = false;

        GameObject lowerInstantion = Instantiate(lowerBody, lowBodyPos.position, lowerBody.transform.rotation);
        GameObject upperInstantion = Instantiate(upperBody, new Vector3(upBodyPos.position.x, upBodyPos.position.y + 0.3f, upBodyPos.position.z), upperBody.transform.rotation);
        //jos helmet on olemassa nii ragdoll data transfer
        if (helmetOn)
        {
            upperInstantion.GetComponent<RagdollController>().helmetOn = true;
        }

        //randomize chance for ghost and inform spawner and dont make enemy ragdoll dissapear yet, if not, normal routine
        if (RandomChance(60))
        {
            //ghost
            upperInstantion.GetComponent<RagdollController>().ghost = true;
            lowerInstantion.GetComponent<RagdollController>().ghost = true;
            spawner.SpawnGhost();
            spawner.tempPoints = points;
            Destroy(this.gameObject);
        }
        else
        {
            //spawn new enemy and add score
            spawner.StartCoroutine("SpawnDelay");
            spawner.AddScore(points);
            Destroy(this.gameObject);
        }
    }

    void Slashed()
    {
        //player cant attack when the enemy is dead (prevents some exploits)
        lockPlayerAttack = true;
        playerc.canSlash = false;

        GameObject instantion = Instantiate(enemyRagdoll, transform.position, transform.rotation);
        //jos helmet on olemassa nii ragdoll data transfer
        if (helmetOn)
        {
            instantion.GetComponent<RagdollController>().helmetOn = true;
        }

        //spawn new enemy and add score
        spawner.StartCoroutine("SpawnDelay");
        spawner.AddScore(points);
        Destroy(this.gameObject);
    }

    void Attack()
    {
        if (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.EnemySlashLeft") 
            || enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.EnemySlashRight")
            || enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.EnemySlashFront")
            || enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.EnemyAmbushStrikeL")
            || enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.EnemyAmbushStrikeR"))
        {
            if ((!longSlash && enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.62f) 
                || (longSlash && enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f))
            {
                //player deflect/
                if ((slashingFront && playerc.deflectingFront)
                || (slashingLeft && playerc.deflectingLeft)
                || (slashingRight && playerc.deflectingRight))
                {
                    if (playerc.deflecting)
                    {
                        //randomizaa chain attack
                        if (chainCurrent > 0)
                        {
                            lockPlayerAttack = true;
                            chainAttack = true;
                            chainCurrent -= 1;
                        }
                        else if (chainCurrent <= 0 && RandomChance(65))
                        {
                            lockPlayerAttack = false;
                            chainAttack = false;
                            chainCurrent = Random.Range(1, chainMax + 1);
                        }

                        if (chainAttack)
                        {
                            chainStun = true;
                            enemyAnimator.SetInteger("AnimationInt", 6);
                        }
                        else
                        {
                            stunTransition = true;
                            stunned = true;
                            enemyAnimator.SetInteger("AnimationInt", 7);
                        }

                        slashingFront = false;
                        slashingLeft = false;
                        slashingRight = false;
                        longSlash = false;

                        //stop enemy slash effect
                        this.transform.GetChild(0).GetComponent<AnimationEventManager>().EndSlash();

                        //player deflect spark particles
                        Instantiate(sparkParticle, playerc.katana.GetChild(0).transform.position, playerc.katana.rotation);
                    }
                    //player take damage
                    else if (!playerc.stunned)
                    {
                        DamagePlayer();
                    }
                }
                //player take damage
                else if (!playerc.stunned)
                {
                    DamagePlayer();
                }
            }

            if (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !enemyAnimator.IsInTransition(0))
            {
                slashingFront = false;
                slashingLeft = false;
                slashingRight = false;
                longSlash = false;

                readyToAttack = true;
                idling = true;
                enemyAnimator.SetInteger("AnimationInt", 0);
            }
        }
    }

    void DamagePlayer()
    {
        //player take damage
        if (!playerc.stunned)
        {
            //hit particles to player position
            //Instantiate(hitParticle, playerc.transform.position, hitParticle.transform.rotation);

            if (!playerc.shield)
            {
                playerc.health -= damageAmount;
                playerc.stunned = true;

                lockPlayerAttack = false;
            }
            else
            {
                playerc.ShieldBreak();

                playerc.stunned = true;
                lockPlayerAttack = false;
            }
        }
    }

    void Deflect()
    {
        if (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.EnemyLeftDeflect")
            || enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.EnemyRightDeflect"))
        {
            if (playerc.hit)
            {
                playerc.StartCoroutine("AttackCooldown");

                playerc.hitting = false;
                playerc.hit = false;
                playerc.stunned = true;

                //stop player slash effect
                playerc.transform.GetChild(0).GetComponent<AnimationEventManager>().EndSlash();

                //deflect spark particles
                Instantiate(sparkParticle, katana.GetChild(0).transform.position, katana.rotation);
            }

            if (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !enemyAnimator.IsInTransition(0))
            {
                readyToAttack = true;
                deflectingLeft = false;
                deflectingRight = false;
                idling = true;
                enemyAnimator.SetInteger("AnimationInt", 0);
                
            }
        }
    }

    bool RandomChance(int treshold)
    {
        int random = Random.Range(1, 101);

        if (random <= treshold)
        {
            return true;
        }
        else 
        {
            return false;
        }
        //NOTHING AFTER THE RETURN!!!
    }

    IEnumerator AttackStarter()
    {
        yield return new WaitForSeconds(attackDelay);

        if (!attackCancelled)
        {
            if (idling)
            {
                //random attack decision
                randomAttack = Random.Range(1, 6);

                while ((lastestAttack + lastAttack) / 2 == randomAttack)
                {
                    randomAttack = Random.Range(3, 6);
                }

                lastestAttack = lastAttack;
                lastAttack = randomAttack;

                //KOITA CASE JUTTUA

                //hyökkää random attackin mukaan
                enemyAnimator.SetInteger("AnimationInt", randomAttack);
                if (randomAttack == 1)
                {
                    idling = false;
                    slashingLeft = true;
                    longSlash = true;
                }
                if (randomAttack == 2)
                {
                    idling = false;
                    slashingRight = true;
                    longSlash = true;
                }
                if (randomAttack == 3)
                {
                    idling = false;
                    slashingLeft = true;
                }
                else if (randomAttack == 4)
                {
                    idling = false;
                    slashingRight = true;
                }
                else if (randomAttack == 5)
                {
                    idling = false;
                    slashingFront = true;
                }
            }
        }
        else
        {
            attackCancelled = false;
        }
    }
}

