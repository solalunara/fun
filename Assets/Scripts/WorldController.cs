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
    // How quickly the game ramps up, with 0 being no rampup and 1 being double the speed once a second
    // also how far objects will spawn off screen
    // and how long between spawns of hostile blocks
    // and the max speed
    private float fMaxSpeed = 10 * ( fDifficulty * 50 + .5f );
    // The max speed of the game, for an infinite mode

    private List<Transform> _tWorldObjects;
    // The transforms of the blocks the world is made out of
    private float _fScrollSpeed;
    // The scroll speed of the world, in units/second
    private float _fGroundSize;
    // The size of the ground, so objects at the end of the ground can be moved to the beggining of the ground
    private float _fSpawnDist;
    // The distance from the right of the screen that objects will spawn
    private const float _fBottomBlockHeight = -3f;
    private const float _fMiddleBlockHeight = -1f;
    private const float _fTopBlockHeight = 1f;
    private float _fLastSpawnDist;
    private float _fSpawnDelta = 33 / ( fDifficulty * 50 + .5f );
    private float _fDistance;

    void Start()
    // Start is called before the first frame update
    {
        if ( !buttonScript.GUI_USED )
            SceneManager.LoadScene( "GUI" );
        // Get the transforms of all the blocks that make up the world, and store it locally
        _tWorldObjects = new List<Transform>(GetComponentsInChildren<Transform>( /* Include Inactive Objects = */ false ));

        // Why GetComponentsInChildren includes the object itself is beyond me
        _tWorldObjects.Remove( this.GetComponent<Transform>() );

        // assign flags for ground objects (objects present at init are all ground, objects that spawn later won't be)
        for ( int i = -1; ++i < _tWorldObjects.Count; )
        {
            BoxManager bmTemp = _tWorldObjects[i].gameObject.AddComponent<BoxManager>() as BoxManager;
            bmTemp.iEFlags |= WorldFlags.isGround;
        }

        GroundChanged();

        // This needs to be here to reset every time the game is started
        _fScrollSpeed = 1.0f;
        _fDistance = 0.0f;

        //_mNewBlockMaterial = new Material();

        SpawnBlocks( WorldFlags.isMiddle );
        SpawnBlocks( WorldFlags.isTop );
        SpawnBlocks( WorldFlags.isBottom );
    }

    void GroundChanged()
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
        if ( _fScrollSpeed < fMaxSpeed )
            _fScrollSpeed *= 1 + ( fDifficulty * Time.deltaTime );

        _fDistance += _fScrollSpeed * Time.deltaTime;

        // _fscrollspeed puts it at 1 second to the right, decreases with increasing difficulty, such that at difficulty .01 it's exactly 1 second
        _fSpawnDist = _fScrollSpeed / ( fDifficulty * 100 );
        
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

        if ( _fDistance >= _fLastSpawnDist + _fSpawnDelta )
        {
            _fLastSpawnDist = _fDistance;
            WorldFlags wfSpawn = WorldFlags.isNone;

            if ( Random.value > .5f )
                wfSpawn |= WorldFlags.isBottom;
            if ( Random.value > .5f )
                wfSpawn |= WorldFlags.isMiddle;
            if ( Random.value > .5f )
                wfSpawn |= WorldFlags.isTop;

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
        if ( ( wFlags & WorldFlags.isGround ) != 0 )
        {
            Debug.Log( "Spawning ground block in SpawnBlocks(WF wf)?" );
            return false;
        }

        float fSpawn = Camera.main.ScreenToWorldPoint( new Vector3( Screen.width, 0, 0 ) ).x + _fSpawnDist;
        string sTexturePath = "dev/dev_1x1";
        string sSpritePath = "Sprites/Default";
        float fppu = 100f;

        if ( ( wFlags & WorldFlags.isBottom ) != 0 )
            SpawnBlock( new Vector2( fSpawn, _fBottomBlockHeight ), "HostileBottomBlock", sTexturePath, sSpritePath, fppu, WorldFlags.isBottom );
        
        if ( ( wFlags & WorldFlags.isMiddle ) != 0 )
            SpawnBlock( new Vector2( fSpawn, _fMiddleBlockHeight ), "HostileMiddleBlock", sTexturePath, sSpritePath, fppu, WorldFlags.isMiddle );

        if ( ( wFlags & WorldFlags.isTop ) != 0 )
            SpawnBlock( new Vector2( fSpawn, _fTopBlockHeight ), "HostileTopBlock", sTexturePath, sSpritePath, fppu, WorldFlags.isTop );

        return true;
    }
    private void SpawnBlock( Vector2 vSpawn, string name, string sTexturePath, string sSpritePath, float fppu, WorldFlags flags )
    // creates a block with specified details
    {
        
            GameObject gBlock = new GameObject( name );

            SpriteRenderer srBlock = gBlock.AddComponent<SpriteRenderer>() as SpriteRenderer;
            Texture2D tex = Resources.Load( sTexturePath ) as Texture2D;
            BoxManager bmBlock = gBlock.AddComponent<BoxManager>() as BoxManager;


            srBlock.material = new Material( Shader.Find( sSpritePath ) );
            srBlock.sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), new Vector2( .5f, .5f ), fppu );
            bmBlock.iEFlags = flags;
            vSpawn.x += tex.width/2 / fppu;
            gBlock.transform.position = vSpawn;
            
            BoxCollider2D bcBlock = gBlock.AddComponent<BoxCollider2D>() as BoxCollider2D;
            bcBlock.isTrigger = true;

            _tWorldObjects.Add( gBlock.transform );
    }
}