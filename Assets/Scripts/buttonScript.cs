using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonScript : MonoBehaviour
{
    public static bool GUI_USED = false;
    public void OnPress()
    {
        GUI_USED = true;
        UTIL.LoadScene( "World" );
    }
}
