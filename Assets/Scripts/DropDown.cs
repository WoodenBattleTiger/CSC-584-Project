using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDown : MonoBehaviour
{
    public Dropdown dropdown; 

    public void GetDropdownValue()
    {
        int value_idx = dropdown.value;

        string value_text = dropdown.options[value_idx].text;

        Debug.Log(value_text);
    }
}
