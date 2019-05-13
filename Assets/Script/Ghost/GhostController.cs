using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostController : MonoBehaviour
{

    public GameObject PLAYER;
    public GameObject MYSELF;
    public Transform startRayCast, endRayCast;
    public ParticleSystem MyParticleSystem;
    public Light MyPointlight;

    private float lenghtRaycast = 7.0f;

 
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Distance: " + Vector3.Distance(MYSELF.transform.position, PLAYER.transform.position));
        if(Vector3.Distance(MYSELF.transform.position, PLAYER.transform.position) > 0.2f)
        {
            FollowAction();
        }
        else
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.velocity = Vector3.zero;
        }
        SetRaycast();
    }

    private void FollowAction()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = PLAYER.transform.position;
    }

    private void SetRaycast()
    {

        RaycastHit hit;

        if (Physics.Raycast(startRayCast.transform.position, (endRayCast.transform.position - startRayCast.transform.position), out hit, lenghtRaycast))
        {
            //Debug.DrawLine(startRayCast.position, endRayCast.position, Color.red);
            //print("SET RAYCAST - name: " + hit.collider.name);
            Debug.DrawRay(startRayCast.position, (endRayCast.transform.position-startRayCast.transform.position) * hit.distance, Color.red);

            if (hit.collider.name == "Player")
            {
                MyPointlight.color = Color.red;
                MyParticleSystem.startColor = Color.red;

                //MYSELF.GetComponent<ParticleSystem>().startColor = Color.red;
            }
            else
            {
                MyPointlight.color = Color.green;
                MyParticleSystem.startColor = Color.yellow;

            }

        }
        else
        {
            Debug.DrawRay(MYSELF.transform.position, (endRayCast.transform.position - startRayCast.transform.position) * lenghtRaycast, Color.green);
            MyPointlight.color = Color.green;
            MyParticleSystem.startColor = Color.yellow;

        }
    }







}