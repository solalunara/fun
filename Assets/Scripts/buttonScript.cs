using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonScript : MonoBehaviour
{
    public static bool GUI_USED = false;
    public void OnPress()
    {
        GUI_USED = true;
        SceneManager.LoadScene( "World" );
    }
}
