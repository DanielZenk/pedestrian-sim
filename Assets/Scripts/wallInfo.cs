using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallInfo : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector3 myPos;
    public float minX = -0.57f;
    public float maxX = 1.952f;
    public float maxY = -0.945f;
    public float minY = -1.19f;
    public Vector3 normalY = new Vector3(0.0f, 1.0f, 0.0f);
    public Vector3 normalX = new Vector3(1.0f, 0.0f, 0.0f);

    void Start()
    {
        myPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
