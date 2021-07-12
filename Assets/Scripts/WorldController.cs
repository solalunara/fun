#if !WORLD_CS
#define WORLD_CS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Any variable marked as public is set in the unity editor, so the values can be changed for testing/debugging without recompiling the script
public class WorldController : MonoBehaviour
{
    public float fDifficulty;
    // How quickly the game ramps up, with 0 being no rampup and 1 being double the speed once a second
    public float fMaxSpeed;
    // The max speed of the game, for an infinite mode

    private List<Transform> _tWorldObjects;
    // The transforms of the blocks the world is made out of
    private List<Transform> _tGroundObjects = new List<Transform>();
    private float _fScrollSpeed;
    // The scroll speed of the world, in units/second
    private float _fGroundSize;
    // The size of the ground, so objects at the end of the ground can be moved to the beggining of the ground

    void Start()
    // Start is called before the first frame update
    {
        // Get the transforms of all the blocks that make up the world, and store it locally
        _tWorldObjects = new List<Transform>(GetComponentsInChildren<Transform>( /* Include Inactive Objects = */ false ));

        // Why GetComponentsInChildren includes the object itself is beyond me
        _tWorldObjects.Remove( this.GetComponent<Transform>() );

        DataChanged();

        // Initializing this here to make it more obvious that 1.0f is the starting speed and that it's not constant
        _fScrollSpeed = 1.0f;
    }

    void DataChanged()
    {
        _fGroundSize = 0;
        for ( int i = 0; i < _tWorldObjects.Count; ++i )
        {
            if ( _tWorldObjects[i].gameObject.name.Substring( 0, 6 ).Equals( "ground" ) )
            {
                _tGroundObjects.Add( _tWorldObjects[i] );
                Renderer rTemp = _tGroundObjects[i].gameObject.GetComponent<Renderer>();
                _fGroundSize += rTemp.bounds.size.x;
            }
        }
    }

    void Update()
    // Update is called once per frame
    {
        if ( _fScrollSpeed < fMaxSpeed )
            _fScrollSpeed *= 1 + ( fDifficulty * Time.deltaTime );
        
        for ( int i = 0; i < _tWorldObjects.Count; ++i )
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
        if ( _tGroundObjects.Contains( gToBeRemoved.transform ) )
        {
            // Entity is a floor block, move to right of ground blocks
            Vector3 vPos = gToBeRemoved.transform.position;
            gToBeRemoved.transform.position = new Vector3( ( vPos.x + _fGroundSize ), vPos.y, vPos.z );
            return;
        }

        // Entity is not a floor block, set inactive
        gToBeRemoved.gameObject.SetActive( false );
        _tWorldObjects.Remove( gToBeRemoved.transform );

    }
}
#endif //if !WORLD_CS