#if !WORLD_CS
#define WORLD_CS
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Any variable marked as public is set in the unity editor, so the values can be changed for testing/debugging without recompiling the script
public class WorldController : MonoBehaviour
{
    public static float fDifficulty = .01f;
    // General difficulty of the game controls, listed below.
        // how quickly the game ramps up (+)
        // how far objects will spawn off screen (-)
        // how long between spawns of hostile blocks (-)
        // the max speed (+)

    private float fMaxSpeed = 10 * ( fDifficulty * 50 + .5f );
    // The max speed of the game, so it doesn't grow exponentially and become impossible after a certain point
    private List<Transform> _tWorldObjects;
    // The transforms of the blocks the world is made out of
    private float _fScrollSpeed;
    // The scroll speed of the world, in units/second
    private float _fGroundSize;
    // The size of the ground, so objects at the end of the ground can be moved to the beggining of the ground
    private float _fSpawnDist;
    // The distance from the right of the screen that objects will spawn
    private float _fLastSpawnDist;
    // The distance the world was at when the last block spawn took place
    private float _fSpawnDelta = 33 / ( fDifficulty * 50 + .5f );
    // The space in world units between spawns.
    private float _fDistance;
    // The current distance of the world, calculated as the sum of speed*time every frame

    void Start()
    // Start is called before the first frame update
    {
        // If the GUI has not been used, and this scene is loading, send the game to the GUI
        if ( !buttonScript.GUI_USED )
            SceneManager.LoadScene( "GUI" );

        // Get the transforms of all the blocks that make up the world, and store it locally
        _tWorldObjects = new List<Transform>(GetComponentsInChildren<Transform>( /* Include Inactive Objects = */ false ));

        // Why GetComponentsInChildren includes the object itself is beyond me
        _tWorldObjects.Remove( this.GetComponent<Transform>() );

        // Assign flags for ground objects (objects present at init are all ground, objects that spawn later won't be)
        for ( int i = -1; ++i < _tWorldObjects.Count; )
        {
            BoxManager bmTemp = _tWorldObjects[i].gameObject.AddComponent<BoxManager>() as BoxManager;
            bmTemp.iEFlags |= WorldFlags.isGround;
        }

        // We need this to calculate the ground width
        GroundChanged();

        // This needs to be here to reset every time the game is started, i think?
        _fScrollSpeed = 1.0f;
        _fDistance = 0.0f;

        // Have the game always start out with 3 stacked blocks, to show the player it's possible to jump over them by forcing it when it's slow
        SpawnBlocks( WorldFlags.isBottom );
        SpawnBlocks( WorldFlags.isMiddle );
        SpawnBlocks( WorldFlags.isTop );
    }

    void GroundChanged()
    // Call this when ground width needs to be calculated
    {
        _fGroundSize = 0;
        for ( int i = -1; ++i < _tWorldObjects.Count; )
        {
            if ( ( WorldFlags.isGround & _tWorldObjects[i].GetComponent<BoxManager>().iEFlags ) != 0 )
            {
                Renderer rTemp = _tWorldObjects[i].gameObject.GetComponent<Renderer>();
                _fGroundSize += rTemp.bounds.size.x;
            }
        }
    }

    void Update()
    // Update is called once per frame
    {
        // I don't think this actually works, i'm pretty sure different framerates will impact the acceleration of the game. Whatever, i'll fix it at some point tm
        if ( _fScrollSpeed < fMaxSpeed )
            _fScrollSpeed *= 1 + ( fDifficulty * Time.deltaTime );

        // Calculate the distance
        _fDistance += _fScrollSpeed * Time.deltaTime;

        // _fscrollspeed puts it at 1 second to the right, decreases with increasing difficulty, such that at difficulty .01 it's exactly 1 second
        _fSpawnDist = _fScrollSpeed / ( fDifficulty * 100 );
        
        // Move world objects
        for ( int i = -1; ++i < _tWorldObjects.Count; )
        {
            Vector3 vPos = _tWorldObjects[i].position;
            _tWorldObjects[i].position = new Vector3( ( vPos.x - ( _fScrollSpeed*Time.deltaTime ) ), vPos.y, vPos.z );

            // If the object isn't on screen 
            // AND it's on the left of the screen 
            // AND sufficient time has passed for the level to be loaded and visibility to be calculated
            // then kill it with fire
            if ( !_tWorldObjects[i].gameObject.GetComponent<Renderer>().isVisible && _tWorldObjects[i].position.x < 0 && Time.timeSinceLevelLoad > 1 )
                RemoveBlock( _tWorldObjects[i].gameObject );
        }

        // If enough distance has passed to spawn more blocks
        if ( _fDistance >= _fLastSpawnDist + _fSpawnDelta )
        {
            _fLastSpawnDist = _fDistance;
            WorldFlags wfSpawn = WorldFlags.isNone;

            // Have a random chance for top, middle, and bottom blocks individually
            if ( Random.value > .5f )
                wfSpawn |= WorldFlags.isBottom;
            if ( Random.value > .5f )
                wfSpawn |= WorldFlags.isMiddle;
            if ( Random.value > .5f )
                wfSpawn |= WorldFlags.isTop;

            // Spawn the blocks
            SpawnBlocks( wfSpawn );
        }
    }

    void RemoveBlock( GameObject gToBeRemoved )
    //If floor block, move to right of screen, else set inactive
    {
        if ( ( gToBeRemoved.GetComponent<BoxManager>().iEFlags & WorldFlags.isGround ) != 0 )
        {
            // Entity is a floor block, move to right of ground blocks
            Vector3 vPos = gToBeRemoved.transform.position;
            gToBeRemoved.transform.position = new Vector3( ( vPos.x + _fGroundSize ), vPos.y, vPos.z );
            return;
        }

        // Entity is not a floor block, set inactive
        _tWorldObjects.Remove( gToBeRemoved.transform );
        Object.Destroy( gToBeRemoved );

    }

    private bool SpawnBlocks( WorldFlags wFlags )
    {
        // I initially tried coding a way to spawn ground blocks during runtime. It was too much work.
        // Therefore, i've made it so if the block that's being spawned is a ground block, the function bails.
        if ( ( wFlags & WorldFlags.isGround ) != 0 )
        {
            Debug.Log( "Spawning ground block in SpawnBlocks(WF wf)?" );
            return false;
        }

        // Right of the screen + an inverse function of difficulty
        float fSpawn = Camera.main.ScreenToWorldPoint( new Vector3( Screen.width, 0, 0 ) ).x + _fSpawnDist;

        // Where to look for the resources to spawn the block
        string sTexturePath = "dev/dev_1x1";
        string sSpritePath = "Sprites/Default";

        // pixels per unit
        int ippu = 100;

        // Go through the given flags, and seperate them into seperate blocks with different heights
        if ( ( wFlags & WorldFlags.isBottom ) != 0 )
            SpawnBlock( new Vector2( fSpawn, SpawnHeights.fBottomBlockHeight ), "HostileBottomBlock", sTexturePath, sSpritePath, ippu, true, WorldFlags.isBottom );
        
        if ( ( wFlags & WorldFlags.isMiddle ) != 0 )
            SpawnBlock( new Vector2( fSpawn, SpawnHeights.fMiddleBlockHeight ), "HostileMiddleBlock", sTexturePath, sSpritePath, ippu, true, WorldFlags.isMiddle );

        if ( ( wFlags & WorldFlags.isTop ) != 0 )
            SpawnBlock( new Vector2( fSpawn, SpawnHeights.fTopBlockHeight ), "HostileTopBlock", sTexturePath, sSpritePath, ippu, true, WorldFlags.isTop );

        return true;
    }
    private void SpawnBlock( Vector2 vSpawn, string name, string sTexturePath, string sSpritePath, float ippu, bool isHostile, WorldFlags flags )
    // creates a block with specified details
    {
        // I'm not even going to bother to comment this code. Just look at the method name and the inputs, and ignore the actual code.
        GameObject gBlock = new GameObject( name );
        SpriteRenderer srBlock = gBlock.AddComponent<SpriteRenderer>() as SpriteRenderer;
        Texture2D tex = Resources.Load( sTexturePath ) as Texture2D;
        BoxManager bmBlock = gBlock.AddComponent<BoxManager>() as BoxManager;
        srBlock.material = new Material( Shader.Find( sSpritePath ) );
        srBlock.sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), new Vector2( .5f, .5f ), ippu );
        bmBlock.iEFlags = flags;
        vSpawn.x += tex.width/2 / ippu;
        gBlock.transform.position = vSpawn;
        BoxCollider2D bcBlock = gBlock.AddComponent<BoxCollider2D>() as BoxCollider2D;
        if ( isHostile )
            bcBlock.isTrigger = true;
        _tWorldObjects.Add( gBlock.transform );
    }
}

public static class SpawnHeights
{
    public static readonly float fBottomBlockHeight = -3f;
    public static readonly float fMiddleBlockHeight = -1f;
    public static readonly float fTopBlockHeight = 1f;
}