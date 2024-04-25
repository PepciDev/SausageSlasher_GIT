using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    bool start = false;
    float delay = 1f;
    float destroyDelay = 1.2f;
    float dissapearTime = 4f;

    public bool addForce = false;
    float startRotation = 45f;
    float torque = 9;
    float force = 5f;

    public Renderer render;

    Color32 invisible;

    void Start()
    {
        invisible = new Color32(255, 255, 255, 0);
        StartCoroutine("Delay");

        if (addForce)
        {
            //lil bt of random rotation
            this.gameObject.transform.eulerAngles = new Vector3(this.gameObject.transform.eulerAngles.x + Random.Range(-startRotation, startRotation),
                this.gameObject.transform.eulerAngles.y + (Random.Range(-startRotation, startRotation)/4f),
                this.gameObject.transform.eulerAngles.z + Random.Range(-startRotation, startRotation));

            //add random force and torque
            this.gameObject.GetComponent<Rigidbody>().AddTorque(Random.Range(-torque, torque), 0, Random.Range(-torque, torque), ForceMode.Impulse);
            this.gameObject.GetComponent<Rigidbody>().AddForce(0, Random.Range(force/8, force/4), 0, ForceMode.Impulse);
        }
    }

    void Update()
    {
        if (start)
        {
            //vaihda rendering mode transparentiksi
            render.material.SetOverrideTag("RenderType", "Transparent");
            render.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            render.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            render.material.SetInt("_ZWrite", 0);
            render.material.DisableKeyword("_ALPHATEST_ON");
            render.material.EnableKeyword("_ALPHABLEND_ON");
            render.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            render.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            render.material.color = Color.Lerp(render.material.color, invisible, dissapearTime * Time.deltaTime);

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

                render.materials[1].color = Color.Lerp(render.materials[1].color, invisible, dissapearTime * Time.deltaTime);
            }
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(delay);
        start = true;
        Destroy(this.gameObject, destroyDelay);
    }
}
