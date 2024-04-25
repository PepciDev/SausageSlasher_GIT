using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    float destroyTime = 1.2f;

    void Start()
    {
        Destroy(this.gameObject, destroyTime); 
    }
}
