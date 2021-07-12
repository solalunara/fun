using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{

    EdgeCollider2D eCameraEdge;
    Camera cCamera;
    // Start is called before the first frame update
    void Start()
    {
        eCameraEdge = GetComponent<EdgeCollider2D>();
        cCamera = GetComponent<Camera>();


        Vector3[] points = 
        {
            cCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)), 
            cCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)), 
            cCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)), 
            cCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)),
            cCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0))

        };

        List<Vector2> pointsV2 = new List<Vector2>(V3arrToV2arr(points));
        eCameraEdge.SetPoints(pointsV2);
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
