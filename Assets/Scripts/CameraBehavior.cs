using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{

    BoxCollider2D[] cCameraEdge;
    Camera cCamera;
    // Start is called before the first frame update
    const float fColliderWidth = 1;
    void Start()
    {
        cCameraEdge = GetComponents<BoxCollider2D>();
        cCamera = GetComponent<Camera>();


        Vector2[] points = 
        {
            //cCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)), 
            //cCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)), 
            //cCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)), 
            //cCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)),
            //cCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0))
            cCamera.ScreenToWorldPoint( new Vector3( 0, Screen.height/2, 0 ) ), // left side
            cCamera.ScreenToWorldPoint( new Vector3( Screen.width/2, 0, 0) ), // bottom
            cCamera.ScreenToWorldPoint( new Vector3( Screen.width, Screen.height/2, 0) ), // right side
            cCamera.ScreenToWorldPoint( new Vector3( Screen.width/2, Screen.height, 0) ) // top
        };
        // sizes
        // width = right - left 
        float fWidth = (points[2] - points[0]).x;
        // height = top - bottom
        float fHeight = (points[3] - points[1]).y + fColliderWidth*4;

        // left
        cCameraEdge[0].offset = points[0] - new Vector2( fColliderWidth, 0 );
        cCameraEdge[0].size = new Vector2( fColliderWidth*2, fHeight );
        // bottom
        cCameraEdge[1].offset = points[1] - new Vector2( 0, fColliderWidth );
        cCameraEdge[1].size = new Vector2( fWidth, fColliderWidth*2 );
        // right
        cCameraEdge[2].offset = points[2] + new Vector2( fColliderWidth, 0 );
        cCameraEdge[2].size = new Vector2( fColliderWidth*2, fHeight );
        // top
        cCameraEdge[3].offset = points[3] + new Vector2( 0, fColliderWidth );
        cCameraEdge[3].size = new Vector2( fWidth, fColliderWidth*2 );

        //List<Vector2> pointsV2 = new List<Vector2>(V3arrToV2arr(points));
        //eCameraEdge.SetPoints(pointsV2);
    }

    private Vector2[] V3arrToV2arr(Vector3[] inputArr) 
    {

        Vector2[] outputArr = new Vector2[inputArr.Length];

        for(int i = 0; i<inputArr.Length; i++) 
        {
            outputArr[i] = (Vector2) inputArr[i];
        }

        return outputArr;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
