using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMovement2 : MonoBehaviour
{

    private List<Transform> nodes;
    public Transform currPosition; 
    
    private float wallMinX;
    private float wallMaxX;
    private float wallMinY;
    private float wallMaxY;

    private List<float[]> wallEdges = new List<float[]>();

    private float prefSpeed = 0.65f;
    private float safeDistance = 0.5f;
    private float myRadius = 0.2f;
    
    public Vector3 currVelocity = new Vector3(0, 0, 0);


    Vector3 nextNode = new Vector3(-5.0f, -3.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(-5.0f, 3.0f, 0.0f);

        GameObject[] walls = GameObject.FindGameObjectsWithTag("wallNodes");
        int x = 0;
        foreach(GameObject wall in walls) {
            Transform[] ws = wall.GetComponentsInChildren<Transform>();

            int i = 0;
            foreach(Transform node in ws) {
                if(i == 0 && node.name != "wallNodes") {
                    wallEdges.Add(new float[] {node.position.x, node.position.x, node.position.y, node.position.y});
                    i++;
                } else if (node.name != "wallNodes") {
                    wallEdges[x][0] = Mathf.Min(wallEdges[x][0], node.position.x); //min x
                    wallEdges[x][1] = Mathf.Max(wallEdges[x][1], node.position.x); //max x
                    wallEdges[x][2] = Mathf.Min(wallEdges[x][2], node.position.y); //min y
                    wallEdges[x][3] = Mathf.Max(wallEdges[x][3], node.position.y); //max y
                }
            }

            x++;
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
        Vector3 goalForce = getGoalForce(0.5f);
        Vector3 wallForce = getWallForces();
        force += goalForce;
        force += wallForce; 
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
        int x = 0; //will need to loop through walls
        Vector3 returnVec = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 normal = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 curr = transform.position;
        foreach(float[] edges in wallEdges) {
            float dx = Mathf.Max(edges[0] - curr[0], 0, curr[0] - edges[1]);
            float dy = Mathf.Max(edges[2] - curr[1], 0, curr[1] - edges[3]);
            float distance = Mathf.Sqrt(dx * dx + dy * dy);
            if(distance - myRadius < safeDistance) {

                if(curr[1] > edges[3]) { //we are above the wall
                    normal += new Vector3(0.0f, 0.1f, 0.0f);
                }
                if(curr[1] < edges[2]) { //we are below the wall
                    normal += new Vector3(0.0f, -0.1f, 0.0f);
                }
                if(curr[0] > edges[1]) { //we are right of the wall
                    normal += new Vector3(0.1f, 0.0f, 0.0f);
                }
                if(curr[0] < edges[0]) { //we are left of the wall
                    normal += new Vector3(-0.1f, 0.0f, 0.0f);
                }


                float top = safeDistance + myRadius - distance;
                float bottom = distance - myRadius;
                returnVec += normal * (top / bottom);
            } else {
                continue;
            }
        }
        
        
        return returnVec;
         
    }

    private Vector3 getEvasiveForces() {



        return new Vector3(0.0f, 0.0f, 0.0f);
    }
}
