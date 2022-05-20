using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(5 * Time.deltaTime,5 * Time.deltaTime,5 * Time.deltaTime);
    }
}
