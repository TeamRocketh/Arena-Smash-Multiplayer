using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    float time;

    private void OnEnable()
    {
        time = Time.time;
    }

    private void Update()
    {
        if (Time.time - time > 1)
            Destroy(transform.parent.gameObject);
    }
}
