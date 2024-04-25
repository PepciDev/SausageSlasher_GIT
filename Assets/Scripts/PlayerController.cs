using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool dead = false;

    Vector3 upBodyPos;
    Vector3 lowBodyPos;

    public Light ultraLight;
    public Material[] ultMaterial;
    public GameObject[] ultParticles;
    public Renderer ultraRenderer;
    public int FireIceElectric = 1;

    public Animator playerAnimator;

    public GameObject playerRagdoll;
    public GameObject upperBody;
    public GameObject lowerBody;
    public GameObject shieldVisual;
    public GameObject shieldShatter;

    public bool shield = true;

    public ParticleSystem blinkParticle;
    public ParticleSystem trailParticle;

    public Transform katana;

    bool idling = true;

    public int health = 1;

    public bool slashingLeft = false;
    public bool slashingRight = false;

    public bool canSlash = false;

    public bool deflectingLeft = false;
    public bool deflectingRight = false;
    public bool deflectingFront = false;
    public bool deflecting = false;

    public bool ulting = false;
    public bool ultHit = false;
    bool tempUlt = false;

    public bool attackAction = false;
    float attackCooldown = 1f;
    bool attackCooldownBool = false;
    public bool hitting = false;
    public bool hit = false;

    public bool stunned = false;


    void Start()
    {
        playerAnimator.SetInteger("AnimationInt", 0);

        if (shield)
        {
            shieldVisual.SetActive(true);
        }
    }

    void Update()
    {
        //ULTIMATE

        //ultimate type
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            FireIceElectric = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FireIceElectric = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            FireIceElectric = 2;
        }

        //use ultimate
        if (!ulting && canSlash && idling && Input.GetKeyDown(KeyCode.U))
        {
            idling = false;
            ulting = true;

            playerAnimator.SetInteger("AnimationInt", 7);
            ultraRenderer.material = ultMaterial[FireIceElectric];

            for (int i = 0; i <= 2; i ++)
            {
                ultParticles[i].SetActive(false);
            }
            ultParticles[FireIceElectric].SetActive(true);

            //light color change
            switch (FireIceElectric)
            {
                case 0:
                    //light red
                    ultraLight.color = new Color32(255, 94, 0, 255);
                    break;

                case 1:
                    //light blue
                    ultraLight.color = new Color32(0, 108, 255, 255);
                    break;

                case 2:
                    //light yellow
                    ultraLight.color = new Color32(255, 202, 0, 255);
                    break;
            }
        }

        //ultimate in progress
        if ((playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Ultimate") || playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SlashRight")))
        {

            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f)
            {
                //ultimate hitting enemy
                if (!tempUlt)
                {
                    tempUlt = true;
                    ultHit = true;
                }
            }

            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !playerAnimator.IsInTransition(0))
            {
                idling = true;
                ulting = false;
                ultHit = false;
                tempUlt = false;
                playerAnimator.SetInteger("AnimationInt", 0);
            }
        }

        //DEFLECTING

        //deflect left input / start
        if (Input.GetKeyDown(KeyCode.A) && idling)
        {
            idling = false;
            deflectingLeft = true;
            playerAnimator.SetInteger("AnimationInt", 1);
        }

        //deflect right input / start
        if (Input.GetKeyDown(KeyCode.D) && idling)
        {
            idling = false;
            deflectingRight = true;
            playerAnimator.SetInteger("AnimationInt", 2);
        }

        //deflect front input / start
        if (Input.GetKeyDown(KeyCode.W) && idling)
        {
            idling = false;
            deflectingFront = true;
            playerAnimator.SetInteger("AnimationInt", 5);
        }
        
        //deflect method
        if (deflectingFront || deflectingLeft || deflectingRight)
        {
            Deflect();
        }

        //SLASHING

        //slash left input / start
        if (!attackCooldownBool && canSlash && Input.GetButtonDown("Fire1") && idling)
        {
            idling = false;
            slashingLeft = true;
            attackAction = true;
            playerAnimator.SetInteger("AnimationInt", 3);
        }

        //slash right input / start
        if (!attackCooldownBool && canSlash && Input.GetButtonDown("Fire2") && idling)
        {
            idling = false;
            slashingRight = true;
            attackAction = true;
            playerAnimator.SetInteger("AnimationInt", 4);
        }

        //attack method
        if (slashingRight || slashingLeft)
        {
            Attack();
        }

        //STUN

        if (stunned)
        {
            deflecting = false;
            deflectingFront = false;
            deflectingLeft = false;
            deflectingRight = false;

            slashingLeft = false;
            slashingRight = false;
            hitting = false;
            attackAction = false;

            idling = false;
            playerAnimator.SetInteger("AnimationInt", 6);
            Stunned();
        }

        //DEAD

        if (health <= 0)
        {
            Sliced();
        }
    }

    void Sliced()
    {
        upBodyPos = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
        lowBodyPos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        Instantiate(upperBody, upBodyPos, transform.rotation);
        Instantiate(lowerBody, lowBodyPos, transform.rotation);
        Destroy(this.gameObject);
    }

    void Slashed()
    {
        Instantiate(playerRagdoll, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }

    void Attack()
    {
        
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SlashLeft") || playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SlashRight"))
        {
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.53f && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f)
            {
                //player is hitting enemy
                if (!hitting)
                {
                    hitting = true;
                    hit = true;
                }
            }

            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !playerAnimator.IsInTransition(0))
            {
                slashingLeft = false;
                slashingRight = false;
                idling = true;
                hitting = false;
                attackAction = false;
                StartCoroutine("AttackCooldown");
                playerAnimator.SetInteger("AnimationInt", 0);
            }
        }
    }

    void Deflect()
    {
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.LeftDeflect")  
            || playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.RightDeflect") 
            || playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.FrontDeflect"))
        {
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.05f && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.44f)
            {
                //deflecting
                deflecting = true;
            }
            else
            {
                deflecting = false;
            }

            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !playerAnimator.IsInTransition(0))
            {
                deflectingLeft = false;
                deflectingRight = false;
                deflectingFront = false;
                deflecting = false;
                idling = true;
                playerAnimator.SetInteger("AnimationInt", 0);
            }
        }
    }

    void Stunned()
    {
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Stun")) 
        {
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !playerAnimator.IsInTransition(0))
            {
                stunned = false;
                idling = true;
                playerAnimator.SetInteger("AnimationInt", 0);
            }
        }
    }

    public void ShieldBreak()
    {
        //shield breaks
        shield = false;
        Instantiate(shieldShatter, shieldVisual.transform.position, shieldShatter.transform.rotation);
        shieldVisual.SetActive(false);
    }

    public IEnumerator AttackCooldown()
    {
        attackCooldownBool = true;
        yield return new WaitForSecondsRealtime(attackCooldown);
        attackCooldownBool = false;
    }
}
