using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyManager : MonoBehaviour
{
    float fValue;
    public void Awake()
    {
        GetComponent<Slider>().value = ( WorldController.fDifficulty - .01f ) * 10;
    }
    public void DataChanged()
    {
        fValue = GetComponent<Slider>().value;
        fValue /= 10;
        fValue += .01f;
        WorldController.fDifficulty = fValue;
    }
}
