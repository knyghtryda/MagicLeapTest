using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionStates : MonoBehaviour
{
    protected enum SpiderFSM
    {
        Static,
        Idle,
        Wander,
        Approach,
        Attack,
        Talk,
        Backup
    }

    private SpiderFSM spiderMode = SpiderFSM.Static;
    public Animation anim;
    public float idleTime = 10.0f;
    public float wanderDistance = 1.0f;
    public float maxSpeed = 1.0f;
    private float idleTimer;
    private Vector3 moveTarget;
    private Vector3 velocity = Vector3.zero;

    public void UpdateSpider()
    {
        float distance = (this.transform.position - Camera.main.transform.position).magnitude;
        switch (spiderMode)
        {
            case SpiderFSM.Idle:
                if(idleTimer <= 0f)
                {
                    spiderMode = SpiderFSM.Wander;
                }
                if (!anim.IsPlaying("Idle"))
                {
                    anim.Play("Idle");
                }
                idleTimer -= Time.deltaTime;
                break;
            case SpiderFSM.Wander:
                var objPosition = GetComponent<Transform>().position;
                if (moveTarget == null || transform.position == moveTarget)
                {
                    moveTarget = new Vector3(Random.Range(-wanderDistance, wanderDistance) + objPosition.x, objPosition.y, Random.Range(-wanderDistance, wanderDistance) + objPosition.z);
                }
                if(!anim.IsPlaying("Walk"))
                {
                    anim.CrossFade("Walk");
                }
                transform.position = Vector3.SmoothDamp(transform.position, moveTarget, ref velocity, 0.3f);
                break;

        }
    }

    public void activateSpider()
    {
        spiderMode = SpiderFSM.Idle;
    }

    public void deactivateSpider()
    {
        anim.Stop();
        idleTimer = idleTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        idleTimer = idleTime;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpider();   
    }
}
