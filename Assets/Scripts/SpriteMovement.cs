using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMovement : MonoBehaviour
{

    private List<Transform> nodes;

    public string name = "Bob";

    private List<float[]> wallEdges = new List<float[]>();

    public float prefSpeed = 0.65f;
    public float safeDistance = 0.25f;
    public float safeDistanceWall = 0.25f;
    public float myRadius = 0.2f;

    Vector3 currVelocity = new Vector3(0, 0, 0);

    public int N = 5;

    public float vision = 2.5f;
    
    public Vector3 startPosition;

    public Vector3 nextNode = new Vector3(-5.0f, 3.0f, 0.0f);
    public Vector3 randomForce = new Vector3(0, 0, 0);
    System.Random rnd = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPosition;

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

        int index = rnd.Next(1);
        int multiplier = rnd.Next(5);
        randomForce[index] = multiplier * 0.015f;
        Move();
    }

    private void Move()
    {
        Vector3 force = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 goalForce = getGoalForce(0.5f);
        Vector3 wallForce = getWallForces();
        force += randomForce;
        force += goalForce;
        force += wallForce; 
        Vector3 evasiveForces = getEvasiveForces(force);
        force += evasiveForces;
        if(force.magnitude > 0.0326f) {
            float num = (1.0f - (force.magnitude - 0.0325f) / force.magnitude);
            force *= num;
        }
        currVelocity = force;
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
            if(distance - myRadius < safeDistanceWall) {

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
                if(nextNode[1] <= edges[3] && nextNode[1] >= edges[2]) { //goal is between wall
                    if(curr[1] > (edges[3] - (edges[3] - edges[2]) / 2)) {
                        normal += new Vector3(0.0f, 0.1f, 0.0f);
                    }
                }

                float top = safeDistance + myRadius - distance;
                float bottom = Mathf.Pow(Mathf.Abs(distance - myRadius), 1.0f);
                returnVec += normal * (top / bottom);
            } else {
                continue;
            }
        }
        
        
        return returnVec;
         
    }

    private bool checkIfStillCollides(Vector3 newVelocity, SortedDictionary<float, SpriteMovement> dictionary) {

        if(newVelocity.magnitude > 0.0326f) {
            float num = (1.0f - (newVelocity.magnitude - 0.0325f) / newVelocity.magnitude);
            newVelocity *= num;
        }

        foreach(KeyValuePair<float, SpriteMovement> entry in dictionary) {
            Vector3 futurePos = entry.Value.transform.position + entry.Value.currVelocity;
            float safeCombined = safeDistance * 2;
            safeCombined = Mathf.Pow(safeCombined, 2);

            Vector3 velDiff = newVelocity - entry.Value.currVelocity;
            Vector3 xjMinxi = futurePos - transform.position;

            float a = (Mathf.Pow(velDiff[0], 2) + Mathf.Pow(velDiff[1], 2) + Mathf.Pow(velDiff[2], 2));
                float b = (0 - 2) * (velDiff[0] + velDiff[1] + velDiff[2]);
                float c = (safeCombined) - (Mathf.Pow(xjMinxi[0], 2)) - (Mathf.Pow(xjMinxi[1], 2)) - (Mathf.Pow(xjMinxi[2], 2));
                float bSqr = Mathf.Pow(b, 2);
                float time1 = -1;
                float time2 = -1;
                if(bSqr - 4 * a * c < 0) {
                    continue;
                } else {
                    time1 = b + Mathf.Sqrt(bSqr - 4 * a * c);
                    time1 = time1 / (2 * a);
                    time2 = b - Mathf.Sqrt(bSqr - 4 * a * c);
                    time2 = time2 / (2 * a);
                }
                if(time1 > 0 || time2 > 0) {
                    return true;
                } 

        }

        return false;

    }

    private Vector3 getEvasiveForces(Vector3 currForce) {


        Collider[] closeCollisions = Physics.OverlapSphere(transform.position, vision);

        SortedDictionary<float, SpriteMovement> confirmedCollisions = new SortedDictionary<float, SpriteMovement>();

        foreach (var closeCollision in closeCollisions) 
        {
            if(closeCollision.transform.position == transform.position) { //thats me
                continue;
            } else {
                SpriteMovement scriptInfo = closeCollision.GetComponent<SpriteMovement>();
                Vector3 futurePos = closeCollision.transform.position;
                float safeCombined = safeDistance;
                safeCombined = Mathf.Pow(safeCombined, 2);
                Vector3 velDiff = currVelocity - scriptInfo.currVelocity;
                Vector3 xjMinxi = futurePos - transform.position;

                float a = (Mathf.Pow(velDiff[0], 2) + Mathf.Pow(velDiff[1], 2));
                float b = (0 - 2) * (velDiff[0] + velDiff[1]);
                float c = (safeCombined) - (Mathf.Pow(xjMinxi[0], 2)) - (Mathf.Pow(xjMinxi[1], 2));
                float bSqr = Mathf.Pow(b, 2);
                float time1 = -1;
                float time2 = -1;
                if(bSqr - 4 * a * c < 0) {
                    // do nothing
                } else {
                    time1 = b + Mathf.Sqrt(bSqr - 4 * a * c);
                    time1 = time1 / (2 * a);
                    time2 = b - Mathf.Sqrt(bSqr - 4 * a * c);
                    time2 = time2 / (2 * a);
                }
                if(time1 > 0 && time2 > 0) {
                    confirmedCollisions.Add(Mathf.Min(time1, time2), scriptInfo);
                } else if (time1 > 0) {
                    confirmedCollisions.Add(time1, scriptInfo);
                } else if (time2 > 0) {
                    confirmedCollisions.Add(time2, scriptInfo);
                }
            }
        }

        Vector3 returnVec = new Vector3(0, 0, 0);
        
        int i = 0;
        if(confirmedCollisions.Count > 0) {
            foreach(KeyValuePair<float, SpriteMovement> entry in confirmedCollisions)
            {
                if(i == N) {
                    break;
                }
                Vector3 avoidance = new Vector3(0, 0, 0);
                Vector3 ci = transform.position + entry.Key * currForce;
                Vector3 cj = entry.Value.transform.position + entry.Key * entry.Value.currVelocity;
                avoidance = ci - cj;
                avoidance = avoidance / (avoidance.magnitude);
                returnVec += avoidance;
                bool stillCollides = checkIfStillCollides(returnVec + currForce, confirmedCollisions);
                if(!stillCollides) {
                    break;
                }
                i++;
            }
        }

        if(i != 0) {
            return returnVec / i;
        } else {
            return returnVec;
        };
    }
}
