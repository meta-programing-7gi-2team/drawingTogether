using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorToggle : MonoBehaviour
{
    private Toggle toggle;
    [SerializeField] private GameObject check;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        check = transform.GetChild(0).gameObject;
    }
    public void onValueChanged()
    {
        if (toggle.isOn)
            check.SetActive(true);
        else
            check.SetActive(false);
    }
}
