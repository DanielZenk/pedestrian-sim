using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMove2 : MonoBehaviour
{

    public Transform path;
    private List<Transform> nodes;
    private int currentNode = 0;
    public float angle = 0.0f;

    Vector3 nextNode = new Vector3(0.0f, 0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for(int i = 0; i < pathTransforms.Length; i++)
        {
            if(pathTransforms[i] != path.transform) {
                nodes.Add(pathTransforms[i]);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //ApplySteer();
        Move();
    }

    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        relativeVector /= relativeVector.magnitude;
        angle = relativeVector.x / relativeVector.magnitude;
    }

    private void Move()
    {
        if(transform.position == nodes[currentNode].position || transform.position == nextNode) {
            currentNode++;
            if(currentNode >= nodes.Count) {
                
                currentNode = 0;
            }
            System.Random rand = new System.Random();
            float random1 = (float) rand.NextDouble() * 9;
            float random2 = (float) rand.NextDouble();
            if(random2 < 0.5) {
                random1 = 0 - random1;
            }
            float random3 = (float) rand.NextDouble() * 5;
            float random4 = (float) rand.NextDouble();
            if(random4 < 0.5) {
                random3 = 0 - random3;
            }
            nextNode = new Vector3(random1, random3, 0.0f);
        }
        transform.position = Vector3.MoveTowards(transform.position, nextNode, 0.05f);
    }
}
