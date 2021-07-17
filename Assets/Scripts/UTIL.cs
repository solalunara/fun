using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> Various utilities. The methods in this class have no comments, as they are assumed to be self explanatory. </summary>
public static class UTIL 
{
    public static void Assert( bool bIn ){Debug.Assert(bIn);}
    public static Transform SpawnBlock(Vector2 vSpawn, string name = "default", string sTexturePath = "", string sMaterialPath = "", float ippu = 100, bool isHostile = false, bool bShift = false, WorldFlags flags = WorldFlags.isNone)
    {
        if ( sTexturePath.Equals("") || sMaterialPath.Equals("") ) return null;
        GameObject gBlock = new GameObject(name);
        gBlock.transform.position = vSpawn;
        SetProperties( gBlock, sTexturePath, sMaterialPath, ippu, bShift );
        BoxManager bmBlock = gBlock.AddComponent<BoxManager>() as BoxManager;
        bmBlock.iEFlags = flags;
        BoxCollider2D bcBlock = gBlock.AddComponent<BoxCollider2D>() as BoxCollider2D;
        if (isHostile) bcBlock.isTrigger = true;
        return gBlock.transform;
    }
    public static Vector2[] V3arrToV2arr(Vector3[] inputArr)
    {
        Vector2[] outputArr = new Vector2[inputArr.Length];
        for (int i = 0; i < inputArr.Length; i++)
            outputArr[i] = (Vector2)inputArr[i];
        return outputArr;
    }
    public static void LoadScene( string sName )
    //so that you don't have to import scenemanegement to be able to change the scene
    {
        SceneManager.LoadScene( sName );
    }
    public static void SetProperties( GameObject gObj, string sTexturePath = "", string sMaterialPath = "", float ippu = 100, bool bShift = false )
    {
        Texture2D tex;
        if ( !sTexturePath.Equals("") ) tex = Resources.Load( sTexturePath ) as Texture2D;
        else tex = gObj.GetComponent<Texture2D>();
        Assert( tex != null );
        gObj.TryGetComponent<SpriteRenderer>( out SpriteRenderer sr );
        Assert( sr != null || !sMaterialPath.Equals("") );
        if ( sr == null ) sr = gObj.AddComponent<SpriteRenderer>() as SpriteRenderer;
        if ( !sMaterialPath.Equals("") ) sr.material = new Material(Shader.Find(sMaterialPath));
        sr.sprite = Sprite.Create( tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), ippu);
        if ( bShift ) gObj.transform.position = new Vector3( (tex.width / 2 / ippu) + gObj.transform.position.x, gObj.transform.position.y, gObj.transform.position.z );
    }
}
