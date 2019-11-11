using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breach : MonoBehaviour
{

    [SerializeField] private GameObject exitBreach;
    private Breach exitBreachComp;

    private Transform player;

    [SerializeField] private bool active = true;

    public bool Active { get { return active;} set { active = value; } }

    void Start()
    {
        exitBreachComp = exitBreach.GetComponent<Breach>();
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if (active && exitBreachComp.Active)
            {
                player = other.GetComponent<Transform>();
                

                SetState(false);
                float rotDiff = -Quaternion.Angle(transform.rotation, exitBreach.transform.rotation);
                rotDiff += 180;
                player.Rotate(Vector3.up, rotDiff);
                Vector3 breachToPlayer = player.position - transform.position;
                Vector3 posOffset = Quaternion.Euler(0f, rotDiff, 0f) * breachToPlayer;
                player.position = exitBreach.transform.position + posOffset;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            player = null;
            exitBreachComp.SetState(true);
        }
    }

    public void SetState(bool state)
    {
        active = state;
    }
}
