using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    PlayerController playerc;
    Spawner spawner;

    Animator ghostAnimator;

    public GameObject rigidKatana;
    public GameObject dissapearParticles;
    public Transform katanaPos;
    public Transform chestPos;

    public bool slashing = false;

    bool slashingLeft = false;
    bool slashingRight = false;
    bool slashingFront = false;

    int damageAmount = 1;

    void Start()
    {
        spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
        ghostAnimator = transform.GetChild(0).GetComponent<Animator>();
        playerc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            ghostAnimator.SetInteger("GhostAnimationInt", 0);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            ghostAnimator.SetInteger("GhostAnimationInt", 1);
        }

        //appearing
        if (!slashing && ghostAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.GhostAppear"))
        {
            if (ghostAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f)
            {
                slashing = true;
                int random = Random.Range(1, 4);
                ghostAnimator.SetInteger("GhostAnimationInt", random);

                switch (random)
                {
                    case 1:
                        slashingLeft = true;
                        break;

                    case 2:
                        slashingRight = true;
                        break;

                    case 3:
                        slashingFront = true;
                        break;
                }
            }
        }

        //slashing
        if (slashing && (ghostAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.GhostSlashLeft") 
            || ghostAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.GhostSlashRight") 
            || ghostAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.GhostSlashFront")))
        {
            if (ghostAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.48f)
            {
                if ((slashingFront && playerc.deflectingFront)
                    || (slashingLeft && playerc.deflectingLeft)
                    || (slashingRight && playerc.deflectingRight))
                {
                    //attack deflected & nollaa points
                    if (playerc.deflecting)
                    {
                        Die(5);
                    }
                }
                else if (!playerc.stunned)
                {
                    //player takes damage
                    DamagePlayer(); 
                }
            }

            if (ghostAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !ghostAnimator.IsInTransition(0))
            {
                //die when the animation ends
                Die(0);
            }
        }
    }

    void Die(int extraPoints)
    {
        //dissapear and instantiate katana + particles + spawner jutut
        Instantiate(rigidKatana, katanaPos.position, katanaPos.rotation);
        Instantiate(dissapearParticles, chestPos.position, dissapearParticles.transform.rotation);

        //spawn new enemy if the player isnt dead
        if (GameObject.Find("Player") != null)
        {
            spawner.StartCoroutine("SpawnDelay");
        }

        spawner.AddScore(extraPoints);

        Destroy(gameObject);
    }

    void DamagePlayer()
    {
        //player take damage
        if (!playerc.stunned)
        {
            if (!playerc.shield)
            {
                playerc.health -= damageAmount;
                playerc.stunned = true;
            }
            else
            {
                playerc.ShieldBreak();
                playerc.stunned = true;
            }
        }
    }
}
