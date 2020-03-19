using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float waitBeforeSelfDestruct = 1f;
    
    void Start()
    {
        StartCoroutine(StartSelfDestruct());
    }

    IEnumerator StartSelfDestruct()
    {
        yield return new WaitForSeconds(waitBeforeSelfDestruct);
        Destroy(gameObject);
    }
}
