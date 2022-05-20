using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeDate : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;

    // Update is called once per frame
    void Update()
    {
        textMeshProUGUI.text = System.DateTime.Now.ToString("yyyy MM dd, HH:mm:ss");
    }
}
