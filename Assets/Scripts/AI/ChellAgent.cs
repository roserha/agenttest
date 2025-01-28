using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class ChellAgent : Agent
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float TimeBegan;
    // Update is called once per frame
    void Update()
    {
        if(Hold && Vector3.Distance(transform.position, WeightedCube.transform.position) < 2.5f)
        {
            WeightedCube.GetComponent<Rigidbody>().useGravity = false;
            WeightedCube.transform.position = new Vector3(WeightedCube.transform.position.x, 1.5f, WeightedCube.transform.position.z);
            WeightedCube.transform.SetParent(this.transform);
        }

        if ((!Hold && Vector3.Distance(transform.position, WeightedCube.transform.position) < 2.5f) || (Hold && Vector3.Distance(transform.position, WeightedCube.transform.position) >= 2.5f))
        {
            WeightedCube.GetComponent<Rigidbody>().useGravity = true;
            WeightedCube.transform.parent = null;
        }

        print(Vector3.Distance(WeightedCube.transform.position, ButtonPos));
    }

    public GameObject BigButton;
    public GameObject WeightedCube;
    public bool[] StepsCompleted;
    Vector3 ButtonPos;
    public override void OnEpisodeBegin()
    {
        TimeBegan = Time.time;
        StepsCompleted = new bool[12];
        transform.position = Vector3.up;
        WeightedCube.transform.parent = null;
        WeightedCube.GetComponent<Rigidbody>().useGravity = true;
        WeightedCube.transform.position = new Vector3((Random.value * 13.25f) + 1.5f, 1, (Random.value * -7.75f) + 3.875f);
        WeightedCube.GetComponent<Rigidbody>().velocity = Vector3.zero;

        ButtonPos = new Vector3((Random.value * 10.875f) + 3f, 1.5f, (Random.value * -3f) + 3f);

        while(Vector3.Distance(WeightedCube.transform.position, ButtonPos) < 3f)
        {
            ButtonPos = new Vector3((Random.value * 10.875f) + 3f, 1.5f, (Random.value * -3f) + 3f);
        }

        BigButton.transform.position = ButtonPos - (Vector3.up * 1.1875f);
    }

    public bool Hold;
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(Hold);

        sensor.AddObservation(ButtonPos);
        sensor.AddObservation(BigButton.GetComponentInChildren<BigButton>().On);

        sensor.AddObservation(WeightedCube.transform.position);
    }

    public float speed = 5f;
    public override void OnActionReceived(float[] vectorAction)
    {
        //Actions
        GetComponent<CharacterController>().SimpleMove(new Vector3(vectorAction[1] * speed, 0,-vectorAction[0] * speed));
        Hold = vectorAction[2] >= 0.5f;

        //Rewards
        bool isHolding = Hold && Vector3.Distance(transform.position, WeightedCube.transform.position) < 2.5f;


        //Aproach Cube
        if (Vector3.Distance(transform.position, WeightedCube.transform.position) < 10.0f && IsOnStep(0)) { SetReward(1.0f); StepsCompleted[0] = true; }
        if (Vector3.Distance(transform.position, WeightedCube.transform.position) < 05.0f && IsOnStep(1)) { SetReward(1.0f); StepsCompleted[1] = true; }
        if (Vector3.Distance(transform.position, WeightedCube.transform.position) < 02.5f && IsOnStep(2)) { SetReward(1.0f); StepsCompleted[2] = true; }

        //Pick Up Cube
        if (isHolding && IsOnStep(3)) { SetReward(1.0f); StepsCompleted[3] = true; }

        //Aproach Button Holding Cube
        if (Vector3.Distance(WeightedCube.transform.position, ButtonPos) < 10.0f && isHolding && IsOnStep(4)) { SetReward(1.0f); StepsCompleted[4] = true; }
        if (Vector3.Distance(WeightedCube.transform.position, ButtonPos) < 05.0f && isHolding && IsOnStep(5)) { SetReward(1.0f); StepsCompleted[5] = true; }
        if (Vector3.Distance(WeightedCube.transform.position, ButtonPos) < 02.5f && isHolding && IsOnStep(6)) { SetReward(1.0f); StepsCompleted[6] = true; }
        if (Vector3.Distance(WeightedCube.transform.position, ButtonPos) < 01.0f && isHolding && IsOnStep(7)) { SetReward(1.0f); StepsCompleted[7] = true; }

        //Drop Cube
        if (!isHolding && Vector3.Distance(WeightedCube.transform.position, ButtonPos) < 01.0f && IsOnStep(8)) { SetReward(1.0f); StepsCompleted[8] = true; }

        //Go to Opened Door and Finish Episode
        if (BigButton.GetComponentInChildren<BigButton>().On && transform.position.x > 5 && (transform.position.z < 4f && transform.position.z > -4f) && IsOnStep(9)) { SetReward(1.0f); StepsCompleted[9] = true; }
        if (BigButton.GetComponentInChildren<BigButton>().On && transform.position.x > 10 && (transform.position.z < 2f && transform.position.z > -2f) && IsOnStep(10)) { SetReward(1.0f); StepsCompleted[10] = true; }
        if (BigButton.GetComponentInChildren<BigButton>().On && transform.position.x > 13.5f && (transform.position.z < 1f && transform.position.z > -1f) && IsOnStep(11)) { SetReward(1.0f); StepsCompleted[11] = true; }
        if (BigButton.GetComponentInChildren<BigButton>().On && transform.position.x > 17 && Time.time < TimeBegan + 60)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        //If 60 seconds passed since beggining of episode, finish it
        if(Time.time >= TimeBegan + 60)
        {
            EndEpisode();
        }
    }

    public override float[] Heuristic()
    {
        var action = new float[3];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        action[2] = Input.GetAxis("Fire1");
        return action;
    }

    public bool IsOnStep(int n)
    {
        bool check = true;
        for(int i = 0; i < n; i++)
        {
            if (!StepsCompleted[i]) check = false;
        }
        if (StepsCompleted[n]) check = false;
        return check;
    }
}
