using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float targetMoveSpeed = 0.01f;
    float targetRotationSpeed = 0.02f;
    float camYOffset = 2.3f;
    float camZOffset = 3.2f;

    PlayerController playerc;
    Transform player;
    Transform enemy;
    Transform ghost;
    public GameObject ultraObject;
    Transform ultPos;
    bool camUlting = false;
    bool canSwitch = true;

    EnemyController enemyC;
    GhostController ghostC;

    Vector3 startPos;
    Quaternion targetRotation;
    Vector3 targetPosition;
    Vector3 targetMoveposition;

    bool playerAlive = false;
    bool enemyAlive = false;
    bool ghostAlive = false;


    void Start()
    {
        startPos = transform.position;
        ultPos = GameObject.Find("UltimateCameraPos").transform;
    }

    void Update()
    {
        //player/enemy null check
        //Find enemy
        if (GameObject.Find("Enemy(Clone)") != null)
        {
            if (!enemyAlive)
            {
                enemy = GameObject.Find("Enemy(Clone)")
                .transform.GetChild(0)
                .transform.GetChild(0)
                .transform.GetChild(0)
                .transform.Find("chest");

                enemyC = GameObject.Find("Enemy(Clone)").GetComponent<EnemyController>();

                enemyAlive = true;
            }
        }
        else
        {
            enemyAlive = false;
        }

        //Find ghost
        if (GameObject.Find("EnemyGhost(Clone)") != null)
        {
            if (!ghostAlive)
            {
                ghost = GameObject.Find("EnemyGhost(Clone)")
                .transform.GetChild(0)
                .transform.Find("chest");

                ghostC = GameObject.Find("EnemyGhost(Clone)").GetComponent<GhostController>();

                ghostAlive = true;
            }
        }
        else
        {
            ghostAlive = false;
        }

        //Find player
        if (GameObject.Find("Player") != null)
        {
            if (!playerAlive)
            {
                player = GameObject.Find("Player")
                .transform.GetChild(0)
                .transform.GetChild(0)
                .transform.GetChild(0)
                .transform.Find("chest");

                playerc = GameObject.Find("Player").GetComponent<PlayerController>();

                playerAlive = true;
            }
        }
        else
        {
            playerAlive = false;
        }

        //TARGET POSITION ASSIGN

        if (player != null && enemy != null)
        {
            //enemy & player alive

            //ult on / off
            if (playerc.ulting && canSwitch)
            {
                canSwitch = false;
                camUlting = true;

                targetMoveSpeed = 0.06f;
                targetRotationSpeed = 0.1f;
            }
            if (playerc.ultHit)
            {
                camUlting = false;
            }
            if (!playerc.ulting && !canSwitch)
            {
                canSwitch = true;

                targetMoveSpeed = 0.01f;
                targetRotationSpeed = 0.02f;
            }

            Debug.Log(camUlting);

            if (!camUlting)
            {
                //not ulting
                targetPosition = Vector3.Lerp(player.position, enemy.position, 0.5f);

                //jos vihollinen tekee longslashia niin kamera ei liiku, mutta rotatee viel‰
                if (enemyC.GetComponent<EnemyController>().longSlash == false)
                {
                    targetMoveposition = targetPosition;
                }
            }
            else
            {
                //go to ult position when ulting
                targetMoveposition = ultPos.position;
                targetPosition = ultraObject.transform.position;
            }
        }
        else if (enemy == null && player != null && ghost != null)
        {
            //ghost & player alive

            targetMoveposition = targetPosition;

            //jos ghost heitt‰‰ katanaa, muuta target
            if (ghostC.GetComponent<GhostController>().slashing == false)
            {
                targetPosition = Vector3.Lerp(player.position, ghostC.katanaPos.position, 0.5f);
            }
            else
            {
                targetPosition = Vector3.Lerp(player.position, ghost.position, 0.5f);
            }
        }
        else if (ghost == null && enemy == null && player != null)
        {
            //player alive
            targetPosition = Vector3.Lerp(targetPosition, new Vector3(player.position.x, player.position.y + 0.2f, player.position.z + 1.4f), 0.5f);
        }
        
        //camera rotation
        if (targetPosition != Vector3.zero)
        {
            //Camera look at target position

            targetRotation = Quaternion.LookRotation(targetPosition - transform.position);

            //transform.LookAt(lerpPosition);
        }

        //Quaternion Lerp - smooth turning
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, targetRotationSpeed);

        //camera move
        if (!camUlting)
        {
            //not ulting
            transform.position = Vector3.Lerp(transform.position, new Vector3(startPos.x,
            targetMoveposition.y + camYOffset,
            targetMoveposition.z - camZOffset), targetMoveSpeed);
        }
        else
        {
            //ulting
            transform.position = Vector3.Lerp(transform.position, new Vector3(targetMoveposition.x,
            targetMoveposition.y,
            targetMoveposition.z), targetMoveSpeed);
        }


    }
}
