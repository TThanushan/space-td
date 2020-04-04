using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAScript : MonoBehaviour {

    

    public float moveSpeed;
    public int damage = 1;
    public Transform[] pathArray;

    //The index of the point to go.
    int pathPointIndex = 0;

    float startMoveSpeed;

    GameObject endNode;

    void Start() {
        pathArray = GameObject.FindGameObjectWithTag("PathPoints").GetComponent<PathPointsScript>().points;
        endNode = GameObject.FindGameObjectWithTag("Data").GetComponent<PoolObjectScript>().endNode;
    }

    void Awake()
    {
        startMoveSpeed = moveSpeed;
    }

    void OnEnable()
    {
        pathPointIndex = 0;
    }
        
    public float GetMoveSpeed
    {
        get
        { 
            return startMoveSpeed;
        }
    }

    public void Slow(float slowAmount)
    {
        moveSpeed = startMoveSpeed - (startMoveSpeed * slowAmount);
    }

    void Update () {
        
        haveIReachedTheEndFunc();
        MoveOnPathFunc();
        MoveToNextPoint();

        moveSpeed = startMoveSpeed;
    }

    void MoveOnPathFunc()
    {
        
        
        Vector2 dir = pathArray[pathPointIndex].position - transform.position;

        
        transform.Translate(dir.normalized * Time.deltaTime * moveSpeed);
        
    }

    void MoveToNextPoint()
    {
        Vector2 dir = pathArray[pathPointIndex].position - transform.position;
        float distanceThisFrame = moveSpeed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            if (pathPointIndex >= pathArray.Length - 1)
            {
                return;
            }
            
            pathPointIndex++;
        }
        
    }

    void haveIReachedTheEndFunc()
    {
        Vector2 dir = endNode.transform.position - transform.position;
        
        float distanceThisFrame = moveSpeed * Time.deltaTime;
        
        if (dir.magnitude <= distanceThisFrame)
        {
            
            UIScript.instance.DisplayText("-" + damage.ToString(), transform.position, 15, Color.red);

            PlayerStatsScript.instance.life -= damage;
            AudioManager.instance.Play("Boom", true);

            ShakeCamera.instance.Shake(0.2f, 0.2f);
            gameObject.SetActive(false);
        }
    }
}
