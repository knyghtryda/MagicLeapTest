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
        Startle,
        Talk,
        Backup
    }

    private SpiderFSM spiderMode = SpiderFSM.Static;
    public Animation anim;
    public float idleTime = 10.0f;
    public float wanderDistance = 0.5f;
    public float maxSpeed = 0.1f;
    private float idleTimer;
    private Vector3 moveTarget;
    private Vector3 velocity = Vector3.zero;
    private string[] sounds = new string[] { "dont be afraid", "dont hurt me", "excited", "please", "soft", "touch", "touch me", "trust" };
    private float soundTimer = 0f;

    public void UpdateSpider()
    {
        float distance = (this.transform.position - Camera.main.transform.position).magnitude;
        var objTransform = GetComponent<Transform>();
        switch (spiderMode)
        {
            case SpiderFSM.Idle:
                moveTarget = objTransform.position;
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
                if(idleTimer <= 0)
                {
                    playSound(sounds[Random.Range(0, sounds.Length - 1)]);
                    idleTimer = idleTime;
                }
                if (moveTarget == null || objTransform.position == moveTarget)
                {
                    moveTarget = new Vector3(Random.Range(-wanderDistance, wanderDistance) + objTransform.position.x, objTransform.position.y, Random.Range(-wanderDistance, wanderDistance) + objTransform.position.z);
                }
                if(!anim.IsPlaying("Walk"))
                {
                    anim.CrossFade("Walk");
                }
                float step = Time.deltaTime;
                objTransform.position = Vector3.SmoothDamp(this.transform.position, moveTarget, ref velocity, 2.0f);
                //objTransform.forward = Vector3.RotateTowards(objTransform.forward, moveTarget, step, 0.0f);
                objTransform.rotation = Quaternion.LookRotation(moveTarget);
                break;
            case SpiderFSM.Startle:
                var rand = Random.Range(0, 2);
                switch(rand)
                {
                    case 0:
                        playSound("dont hurt me");
                        break;
                    case 1:
                        playSound("dont be afraid");
                        break;
                    case 2:
                        playSound("trust");
                        break;
                }
                playSound("dont hurt me");
                StartCoroutine("startle");
                if (!anim.IsPlaying("Attack"))
                {
                    anim.CrossFade("Attack");
                }
                break;
            case SpiderFSM.Backup:
                if(moveTarget != objTransform.position)
                {
                    if (!anim.IsPlaying("Walk"))
                    {
                        anim.CrossFade("Walk");
                    }
                    objTransform.position = Vector3.SmoothDamp(this.transform.position, moveTarget, ref velocity, 1.0f);
                    //objTransform.rotation = Quaternion.LookRotation(moveTarget);
                } else
                {
                    spiderMode = SpiderFSM.Idle;
                    idleTimer = idleTime;
                }
                break;
            case SpiderFSM.Talk:

                break;

        }
        if (soundTimer > 0)
        {
            soundTimer -= Time.deltaTime;
        }

    }

    IEnumerator startle()
    {
        var objTransform = GetComponent<Transform>();
        var camera = Camera.main;
        var lookPosition = new Vector3(camera.transform.position.x, objTransform.position.y, camera.transform.position.z);
        objTransform.LookAt(lookPosition);
        anim.CrossFade("Attack");
        yield return new WaitForSeconds(anim["Attack"].length);
        spiderMode = SpiderFSM.Backup;
        moveTarget = objTransform.position + objTransform.forward.normalized * -0.1f;
    }

    public void activateSpider()
    {
        spiderMode = SpiderFSM.Idle;
    }

    public void deactivateSpider()
    {
        anim.Stop();
        idleTimer = idleTime;
        spiderMode = SpiderFSM.Static;
    }

    public void backUp()
    {
        spiderMode = SpiderFSM.Startle;
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        idleTimer = idleTime;
        moveTarget = GetComponent<Transform>().position;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpider();   
    }

    public void playSound(string sound)
    {
        if (soundTimer <= 0)
        {
            var source = GetComponentInChildren<AudioSource>();
            AudioClip[] sounds = Resources.LoadAll<AudioClip>("Sounds/" + sound);
            source.PlayOneShot(sounds[Random.Range(0, sounds.Length - 1)]);
            soundTimer = 5f;
        }
    }

}
