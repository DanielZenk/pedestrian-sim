﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class PathScript : MonoBehaviour
{

    public Color lineColor;

    public List<Transform> nodes = new List<Transform>();

    void OnDrawGizmos()
    {
        Gizmos.color = lineColor;

        Transform[] pathTransforms = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for(int i = 0; i < pathTransforms.Length; i++)
        {
            if(pathTransforms[i] != transform) {
                nodes.Add(pathTransforms[i]);
            }
        }

        for(int i = 0; i < nodes.Count; i++)
        {
            Vector3 currentNode = nodes[i].position;
            Vector3 previousNode = Vector3.zero;
            if(i > 0) {
                previousNode = nodes[i - 1].position; 
            } else {
                previousNode = nodes[nodes.Count - 1].position;
            }
            Gizmos.DrawLine(previousNode, currentNode);
            Gizmos.DrawWireSphere(currentNode, 0.01f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
