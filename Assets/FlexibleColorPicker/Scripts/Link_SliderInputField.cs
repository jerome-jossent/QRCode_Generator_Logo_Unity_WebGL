using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Link_SliderInputField : MonoBehaviour
{
    [SerializeField] Slider mySlider;
    [SerializeField] TMPro.TMP_InputField myField;

    void Start()
    {
        myField.text = mySlider.value.ToString();
        mySlider.onValueChanged.AddListener(OnSliderChanged);
        myField.onValueChanged.AddListener(OnFieldChanged);
    }

    private void OnSliderChanged(float number)
    {
        if (myField.text != number.ToString())
        {
            myField.text = number.ToString();
        }
    }

    private void OnFieldChanged(string text)
    {
        if (mySlider.value.ToString() != text)
        {
            if (float.TryParse(text, out float number))
            {
                mySlider.value = number;
            }
        }
    }
}
