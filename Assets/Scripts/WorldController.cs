using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldController : MonoBehaviour
{
    private Transform[] _tWorldObjects;
    // The transforms of the blocks the world is made out of
    private float _fScrollSpeed;
    // The scroll speed of the world, in units/second
    private float _fDifficulty = .01f;
    // How quickly the game ramps up, with 0 being no rampup and 1 being double the speed once a second

    void Start()
    // Start is called before the first frame update
    {
        // Get the transforms of all the blocks that make up the world, and store it locally
        List<Transform> tAllObjects = new List<Transform>(GetComponentsInChildren<Transform>( /* Include Inactive Objects = */ false ));

        // Why GetComponentsInChildren includes the object itself is beyond me
        tAllObjects.Remove( this.GetComponent<Transform>() );
        _tWorldObjects = tAllObjects.ToArray();


        // Why am i initializing this here? C++ ptsd, that's why.
        _fScrollSpeed = 1.0f;
    }

    void Update()
    // Update is called once per frame
    {
        _fScrollSpeed *= 1 + ( _fDifficulty * Time.deltaTime );
        for ( int i = 0; i < _tWorldObjects.GetLength( 0 ); ++i )
        {
            Vector3 vPos = _tWorldObjects[i].position;
            _tWorldObjects[i].position = new Vector3( ( vPos.x - ( _fScrollSpeed*Time.deltaTime ) ), vPos.y, vPos.z );

            // If the object isn't on screen 
            // AND it's on the left of the screen 
            // AND sufficient time has passed for the level to be loaded and visibility to be calculated
            // then kill it with fire
            if ( !_tWorldObjects[i].gameObject.GetComponent<Renderer>().isVisible && _tWorldObjects[i].position.x < 0 && Time.frameCount > 20 )
                RemoveBlock( _tWorldObjects[i].gameObject );
        }
    }

    void RemoveBlock( GameObject gToBeRemoved )
    //If floor block, move to right of screen, else set inactive
    {
        if ( gToBeRemoved.transform.position.y == -1 )
        {
            // Entity is a floor block, move to right of screen
            Vector3 vPos = gToBeRemoved.transform.position;
            Renderer rVis = gToBeRemoved.gameObject.GetComponent<Renderer>();
            float fObjWidth = rVis.bounds.size.x;

            // Trick the game into thinking the screen size is a vector so i can get the screen size in game units
            // this is probably not the best way to do this, so if you know of another way to get the width of the screen in game units please feel free to replace this code
            float fScreenSize = 2 * (Camera.main.ScreenToWorldPoint( new Vector3( Camera.main.pixelWidth, 0, /* Distance from camera to plane = */ 10 ) ).x - Camera.main.transform.position.x);

            gToBeRemoved.transform.position = new Vector3( ( vPos.x + fScreenSize + fObjWidth ), vPos.y, vPos.z );
            return;
        }

        // Entity is not a floor block, set inactive
        gToBeRemoved.gameObject.SetActive( false );

    }
}
