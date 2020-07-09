using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    public List<BezierSpline> roads = new List<BezierSpline>();
    public List<GameObject> cars = new List<GameObject>();

    public int population = 20;

    private int priorityCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < population; i++)
        {
            SpawnCarOnRoad();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnCarOnRoad()
    {
        int randRoad = Random.Range(0, roads.Count);
        int randCar = Random.Range(0, cars.Count);

        GameObject newCar = Instantiate(cars[randCar]);
        SplineWalker walker = newCar.AddComponent<SplineWalker>() as SplineWalker;

        // Get Duration from road
        RoadHelper rh = roads[randRoad].gameObject.GetComponent<RoadHelper>();
        if(rh != null) {
            walker.duration = rh.timeToCompleteLoop;
        } else {
            walker.duration = 10.0f;
        }

        walker.spline = roads[randRoad];
        walker.mode = SplineWalkerMode.Loop;
        walker.progress = Random.Range(0.0f, 1.0f);
        walker.lookForward = true;

        CarManager carMan = newCar.GetComponent<CarManager>();
        if(carMan != null) {
            carMan.priority = priorityCount++;
        }
    }
}
