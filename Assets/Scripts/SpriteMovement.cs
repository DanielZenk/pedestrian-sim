using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMovement : MonoBehaviour
{

    private List<Transform> nodes;
    public Transform currPosition; 
    
    private float wallMinX;
    private float wallMaxX;
    private float wallMinY;
    private float wallMaxY;

    private float prefSpeed = 0.65f;
    private float safeDistance = 0.5f;
    private float myRadius = 0.5f;
    


    Vector3 nextNode = new Vector3(2.0f, 3.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(-2.0f, -4.0f, 0.0f);


        GameObject theWall = GameObject.Find("wallNodes");
        PathScript wallScript = theWall.GetComponent<PathScript>();
        
        List<Transform> wallNodes = wallScript.nodes;
        int i = 0;
        foreach(Transform node in wallNodes) {
            if(i == 0) {
                wallMinX = node.position.x;
                wallMaxX = node.position.x;
                wallMinY = node.position.y;
                wallMaxY = node.position.y;
                i++;
            } else {
                wallMinX = Mathf.Min(wallMinX, node.position.x);
                wallMinY = Mathf.Min(wallMinY, node.position.y);
                wallMaxX = Mathf.Max(wallMaxX, node.position.x);
                wallMaxY = Mathf.Max(wallMaxY, node.position.y);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 force = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 wallForce = getWallForces();
        force += getGoalForce(0.05f);
        force += getWallForces(); 
        if(force.magnitude > 0.0326f) {
            float num = (1.0f - (force.magnitude - 0.0325f) / force.magnitude);
            force *= num;
        }
        transform.position += force;
    }

    private Vector3 getGoalForce(float time) {
        Vector3 curr = transform.position;
        Vector3 update = time * (prefSpeed * ((nextNode - curr) / (Vector3.Distance(nextNode, curr))));
        return update;
    }

    private Vector3 getWallForces() {
        Vector3 curr = transform.position;
        float dx = Mathf.Max(wallMinX - curr[0], 0, curr[0] - wallMaxX);
        float dy = Mathf.Max(wallMinY - curr[1], 0, curr[1] - wallMaxY);
        float distance = Mathf.Sqrt(dx * dx + dy * dy);
        if(distance + myRadius > 1.9f) {
            return new Vector3(0.0f, 0.0f, 0.0f);
        } else {
            Vector3 normal = new Vector3(1.0f, 0.0f, 0.0f);
            float top = safeDistance + myRadius - distance;
            float bottom = Mathf.Pow(distance - myRadius, 0.25f);
            return normal * (top / bottom);
        }
    }
}
