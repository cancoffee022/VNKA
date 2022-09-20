using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Waiting,
    Walking,
    Chasing
}

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private EnemyState state;
    public EnemyState State
    {
        get => state;
        set
        {
            EnemyState prevState = state;
            state = value;
            if (value != prevState)
            {

            }
        }
    }
    NavMeshAgent nav;

    public string enemyName;
    public string location;
    public bool isInside;


    public Transform spawnPoint;
    public Transform target;
    public Collider chasingRangeCollider;
    public Animator animator;

    // public float chasingRange;
    public float walkingRange = 5;
    public float chasingRange = 5;
    public float minMoveRange = 0.3f;
    public float waitingTime = 3.0f;
    //public int walkingIntervalTime;
    public float speed;
    public float chaseSpeed;

    private float rangeX;
    private float rangeZ;
    private float distance;

    private int searchRange;

    public Vector3 walkPosition;

    public bool isPositionSpawn;

    public Vector3 stuckPos;
    bool move = false;
    // Start is called before the first frame update

    void Start()
    {
        animator = this.GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;
        nav = GetComponent<NavMeshAgent>();
        nav.speed = speed;
        StartCoroutine(SetRandomPosition());
    }



    void LateUpdate()
    {
        if (GameManager.instance.isBattle == false)
        {
            if (Mathf.Abs((Mathf.Abs(gameObject.transform.position.x) - Mathf.Abs(walkPosition.x))) < 0.3f &&
                (Mathf.Abs(Mathf.Abs(gameObject.transform.position.z) - Mathf.Abs(walkPosition.z))) < 0.3f && move == true)
            {
                //Debug.Log("도착");
                move = false;
                StopAllCoroutines();
                animator.SetBool("isWalk", false);
                StartCoroutine(SetRandomPosition());
                gameObject.GetComponent<SphereCollider>().enabled = true;
            }
        }
    }

    private void OnTriggerStay(Collider collider)
    {

        if (collider.tag == "Player")
        {
            //StopCoroutine(BackToSpawnPoint());
            animator.SetBool("isWalk", true);
            StartCoroutine(ChasingPlayer());
        }
        if (GameManager.instance.characterController.npcChat == true && GameManager.instance.noneNpcChat.tutorial == false)
            nav.speed = 0f;
        else
            nav.speed = speed;

        if ((gameObject.transform.position - spawnPoint.position).magnitude > chasingRange)
        {
            //Debug.Log("X");
            animator.SetBool("isWalk", false);
            StopAllCoroutines();
            StartCoroutine(SetRandomPosition());
            gameObject.GetComponent<SphereCollider>().enabled = false;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            animator.SetBool("isWalk", false);
            //nav.ResetPath();
            StartCoroutine(SetRandomPosition());
            //Debug.Log("triggerExit");
        }

    }

    public void UpdateAnimatonParameter()
    {

    }

    IEnumerator ChasingPlayer()
    {
        nav.speed = chaseSpeed;
        yield return nav.SetDestination(target.position);
    }

    /*IEnumerator BackToSpawnPoint()
    {
        StopCoroutine(ChasingPlayer());
        nav.ResetPath();
        yield return new WaitForSeconds(walkingIntervalTime);
        nav.speed = 2f;
        nav.SetDestination(spawnPoint.position);
        Debug.Log("backtospawn");
    }*/

    IEnumerator SetRandomPosition()
    {
        //Debug.Log("random");
        nav.ResetPath();
        nav.speed = speed;
        rangeX = Random.Range(-(float)walkingRange, (float)walkingRange);
        int x = Random.Range((int)0, (int)2);
        float waitTime = Random.Range(1.5f, waitingTime);
        //Debug.Log("대기 시간은" + waitTime);
        //Debug.Log("범위"+rangeX);

        if (x == 0)
            rangeZ = walkingRange - rangeX;
        else
            rangeZ = -(walkingRange - rangeX);
        //Debug.Log(spawnPoint.localPosition.x + rangeX);
        walkPosition = new Vector3(spawnPoint.position.x + rangeX, this.transform.position.y, spawnPoint.position.z + rangeZ);
        distance = (gameObject.transform.position - walkPosition).magnitude;
        //Debug.Log(walkPosition);
        NavMeshPath path = new NavMeshPath();

        if (distance >= minMoveRange)
        {
            nav.CalculatePath(walkPosition, path);
            yield return new WaitForSeconds(waitTime);
            nav.SetPath(path);
            move = true;
            animator.SetBool("isWalk", true);
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(SetRandomPosition());
            animator.SetBool("isWalk", false);
            //Debug.Log("다시계산");
        }
        yield return null;
    }
}