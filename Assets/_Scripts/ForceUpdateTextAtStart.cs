using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceUpdateTextAtStart : MonoBehaviour
{
    public TMPro.TMP_InputField textField;
    public bool update;

    void Start()
    {
        textField = GetComponent<TMPro.TMP_InputField>();
        update = true;
    }


    void Update()
    {
        if (update)
        {
            update = false;
            if (textField != null)
            {
                textField.text = textField.text + "J";
                textField.text= textField.text.Substring(0, textField.text.Length-1);
            }
        }
    }
}
