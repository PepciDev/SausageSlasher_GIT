using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public GameObject materialGO;
    public GameObject headbandMaterialGO;
    public GameObject helmet;
    public GameObject headband;
    public bool helmetOn = false;
    public bool ghost = false;
    public GameObject katanaVisual;

    public GameObject particlePos;
    public GameObject smokeParticles;
    public Rigidbody rb;

    Renderer render;
    Renderer headwareRender;

    Color32 burnColor;
    Color32 fadeColor;

    bool burn = false;

    bool headwareCheck = true;

    float burnTime = 1.8f;
    float fadeTime = 7f;
    float destroyDelay = 1.2f;
    bool particleInstatiate = false;
    float randomforcex = 0f;
    float randomforcez = 0f;
    float randomforcey = 0f;

    void Start()
    {
        burn = true;
        rb = rb.GetComponent<Rigidbody>();
        render = materialGO.GetComponent<Renderer>();

        randomforcex = Random.Range(-1.8f, 1.8f);
        randomforcez = Random.Range(-1f, 2.2f);

        randomforcey = Random.Range(1.5f, 2.8f);

        //jos ei löydy headbandia = kehon alaosa
        if (headbandMaterialGO == null && helmet == null)
        {
            headwareCheck = false;
            rb.AddForce(new Vector3(randomforcex/5f, randomforcey/8f, randomforcez/6f), ForceMode.Impulse);
            rb.AddTorque(new Vector3(randomforcex, randomforcey/2f, randomforcey/2f));
        }
        //jos löytyy headband = kehon yläosa
        else
        {
            if (helmetOn)
            {
                helmet.SetActive(true);
                headband.SetActive(false);

                headwareRender = helmet.GetComponent<Renderer>();
                rb.AddForce(new Vector3(randomforcex, randomforcey, randomforcez), ForceMode.Impulse);
                rb.AddTorque(new Vector3(Mathf.Pow(randomforcex, 4), randomforcey / 2f, randomforcey / 2f));

                helmet.GetComponent<Rigidbody>().AddForce(new Vector3(randomforcex / 5, randomforcey / 12, randomforcez / 5), ForceMode.Impulse);
                helmet.GetComponent<Rigidbody>().AddTorque(new Vector3(Mathf.Pow(randomforcex, 4), randomforcey / 2f, randomforcey / 2f));
            }
            else
            {
                headwareRender = headbandMaterialGO.GetComponent<Renderer>();
                rb.AddForce(new Vector3(randomforcex, randomforcey, randomforcez), ForceMode.Impulse);
                rb.AddTorque(new Vector3(Mathf.Pow(randomforcex, 4), randomforcey / 2f, randomforcey / 2f));
            }
        }

        if (ghost)
        {
            //hide the katana and dont start dissapearing
            if (headwareCheck)
            {
                //incase of upper body, hide katana
                katanaVisual.SetActive(false);
            }

            burn = false;
        }

        burnColor = new Color32(55, 55, 55, 255);
        fadeColor = new Color32(55, 55, 55, 0);
    }

    void Update()
    {
        //in case of ghost, wait untill ghost is gone, then start dissapearing
        if (ghost && GameObject.Find("EnemyGhost(Clone)") == null)
        {
            ghost = false;
            burn = true;
        }

        //jos jotain paskaa tapahtuu (palaa)
        if (burn == true)
        {
            render.material.color = Color.Lerp(render.material.color, burnColor, burnTime * Time.deltaTime);

            if (headwareCheck == true)
            {
                headwareRender.material.color = Color.Lerp(headwareRender.material.color, burnColor, burnTime * Time.deltaTime);
            }
        }

        if (render.material.color.r < burnColor.r * 0.005)
        {
            burn = false;

            //vaihda rendering mode transparentiksi
            render.material.SetOverrideTag("RenderType", "Transparent");
            render.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            render.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            render.material.SetInt("_ZWrite", 0);
            render.material.DisableKeyword("_ALPHATEST_ON");
            render.material.EnableKeyword("_ALPHABLEND_ON");
            render.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            render.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            //vaihda headbandin rendering mode transparentiksi
            if (headwareCheck == true)
            {
            headwareRender.material.SetOverrideTag("RenderType", "Transparent");
            headwareRender.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            headwareRender.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            headwareRender.material.SetInt("_ZWrite", 0);
            headwareRender.material.DisableKeyword("_ALPHATEST_ON");
            headwareRender.material.EnableKeyword("_ALPHABLEND_ON");
            headwareRender.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            headwareRender.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }

            render.material.color = Color.Lerp(render.material.color, fadeColor, fadeTime * Time.deltaTime);

            if (headwareCheck == true)
            {
                headwareRender.material.color = Color.Lerp(headwareRender.material.color, fadeColor, fadeTime * Time.deltaTime);
            }

            Destroy(gameObject, destroyDelay);


            if (!particleInstatiate)
            {
                Instantiate(smokeParticles, particlePos.transform.position, new Quaternion(0, 0, 0, 0));
                particleInstatiate = true;
            }
        }
    }
}
