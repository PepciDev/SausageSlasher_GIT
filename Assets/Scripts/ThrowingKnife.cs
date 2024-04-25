using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnife : MonoBehaviour
{
    float timer = 2.6f;
    float destroyDelay = 0.6f;
    float dissapearTime = 0.01f;
    // 25f
    float speed = 28f;
    bool counting = true;

    public ParticleSystem trail;

    public Renderer render;

    Color32 invisible;

    void Start()
    {
        invisible = new Color32(255, 255, 255, 0);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        if (counting == true)
        {
            timer -= 1f * Time.deltaTime;
        }

        if (timer < 1f)
        {
            counting = false;

            //vaihda rendering mode transparentiksi
            render.material.SetOverrideTag("RenderType", "Transparent");
            render.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            render.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            render.material.SetInt("_ZWrite", 0);
            render.material.DisableKeyword("_ALPHATEST_ON");
            render.material.EnableKeyword("_ALPHABLEND_ON");
            render.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            render.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            render.material.color = Color.Lerp(render.material.color, invisible, dissapearTime);

            //nopeuta simulaatiota - jäljelle jääneet partikkelit lähtevät nopeammin
            var main = trail.main;
            main.simulationSpeed = 3f;
            //disable emission
            var emission = trail.emission;
            emission.enabled = false;

            Destroy(this.gameObject, destroyDelay);

            if (render.materials.Length == 2)
            {
                //vaihda toisen materiaalin rendering mode transparentiksi
                render.materials[1].SetOverrideTag("RenderType", "Transparent");
                render.materials[1].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                render.materials[1].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                render.materials[1].SetInt("_ZWrite", 0);
                render.materials[1].DisableKeyword("_ALPHATEST_ON");
                render.materials[1].EnableKeyword("_ALPHABLEND_ON");
                render.materials[1].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                render.materials[1].renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                render.materials[1].color = Color.Lerp(render.materials[1].color, invisible, dissapearTime);
            }
        }
    }
}
