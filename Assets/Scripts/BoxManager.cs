using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoxManager : MonoBehaviour
{
    public void OnTriggerEnter2D( Collider2D collider )
    {
        SceneManager.LoadScene( "GUI" );
    }
}
