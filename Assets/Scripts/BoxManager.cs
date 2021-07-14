using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class BoxManager : MonoBehaviour
{
    public WorldFlags iEFlags;
    public void OnTriggerEnter2D( Collider2D collider )
    {
        SceneManager.LoadScene( "GUI" );
    }
}

[Flags]
public enum WorldFlags
{
    isNone = 0,
    isBottom = 1 << 0,
    isMiddle = 1 << 1,
    isTop = 1 << 2,
    isGround = 1 << 3
}