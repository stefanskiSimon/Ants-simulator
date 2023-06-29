using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject menel;
    public BoxCollider2D ziemia;
    public float nextposition;
    float timer;
    Vector2 GoalDirection;
    public float speed;
    public Vector3 target = new Vector3(0,0,0);
    public List<int> number_of_food = new List<int>();
    bool eat = false;
    public float radius = 0.5f;
    [Range(1, 360)] public float angle = 45f;
    public LayerMask targetLayer;
    public LayerMask ObstacleLayer;
    public GameObject Food;
    Vector3 kingdom = new Vector3(9, -4, 0);
    public bool Canseefood;
    public NavMeshAgent nav;
    public int k = 5;
    void Start()
    {
        nav = gameObject.GetComponent<NavMeshAgent>();
        nav.updateRotation = false;
        nav.updateUpAxis = false;
        Food = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(Check());

    }

    private IEnumerator Check()
    {  
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FOV();
        }
    }

    private void FOV()
    { 
        Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);

        if (rangeCheck.Length > 0)
        {
            Transform goal = rangeCheck[0].transform;
            GoalDirection = (goal.position - transform.position).normalized;

            if (Vector2.Angle(transform.up, GoalDirection) < angle / 2)
            {
                float Distance = Vector2.Distance(transform.position, goal.position);

                if (!Physics2D.Raycast(transform.position, GoalDirection, Distance, ObstacleLayer))
                {
                    Canseefood = true;
                    
                }
                else
                    Canseefood = false;
            }
            else
                Canseefood = false;
        }
        else if (Canseefood)
            Canseefood = false;
    }

    private void OnDrawGizmos()
    { 
        Gizmos.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, radius);

        Vector3 angle1 = DirectonFromAngle(transform.eulerAngles.z, -angle / 2);
        Vector3 angle2 = DirectonFromAngle(-transform.eulerAngles.z, angle / 2);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + angle1 * radius);
        Gizmos.DrawLine(transform.position, transform.position + angle2 * radius);

        if (Canseefood)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, Food.transform.position);
        }
    }

    private Vector2 DirectonFromAngle(float EulerY, float angleDegrees)
    { 
        angleDegrees -= EulerY;

        return new Vector2(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }
    void Update()
    {
        
        if (eat)
        {
            int h = 1;
            number_of_food.Add(h);
            eat = false;
        }

        timer += Time.deltaTime;
        if(timer >= nextposition)
        {
            if(Canseefood == true)
            {
                nav.SetDestination(Food.transform.position); 
            }
            else
            {
                NewPosition();
            }
            timer = 0;   
        }
      
    }

    void NewPosition()
    {
        Bounds bounds = this.ziemia.bounds;
        //float x = menel.transform.position.x;
        //float y = menel.transform.position.y;
        float posx = Random.Range(bounds.min.x, bounds.max.x);
        float posy = Random.Range(bounds.min.y, bounds.max.y);

        target = new Vector3(posx,posy,Mathf.Clamp(transform.position.z,0,0));
        Vector3 ps = menel.transform.position;
        ps.z = 0;
        if (number_of_food.Count >= k)
        {
            nav.SetDestination(kingdom);
        }
        else
            nav.SetDestination(target);

    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            eat = true;
            Destroy(collider.gameObject);
        }
        else if (collider.name.StartsWith("Mrowisko"))
        {
            if (number_of_food.Count >= k)
            {
                number_of_food.Clear();
                Instantiate(menel);
                k++;
            }
        }
    }
}
