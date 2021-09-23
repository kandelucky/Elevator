using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorsCreator : MonoBehaviour
{
    [SerializeField]
    [Header("Total floors - min 3/max 100")]
    [Range(3,100)]
    private int totalFloors;
    [SerializeField]
    private float floorsHeight;

    [Space(15)]
    [SerializeField]
    private GameObject firstFloor;
    [SerializeField]
    private GameObject standardFloor;
    [SerializeField]
    private GameObject lastFloor;

    [Space(15)]
    [SerializeField]
    private Transform floorsParent;
    [SerializeField]
    private Transform startPoint;

    [Space(15)]
    [SerializeField]
    private Camera cam;

    public Elevator elevator;

    private MyButton buttonUp;
    private MyButton buttonDown;
    private Floor floor;
    void Awake()
    {
        CreateFloors();
    }
    void CreateFloors()
    {
        elevator.allFloorsIntervals = new float[totalFloors];

        // create first floor
        GameObject floorObj = Instantiate(firstFloor, startPoint.position, Quaternion.identity, floorsParent);

        floor = floorObj.GetComponent<Floor>();
        floor.floorCount.text = "0";
        floor.elevator = elevator;
        floor.floorNumber = 0;

        buttonUp = floor.floorButtonUp.GetComponent<MyButton>();
        buttonUp.cam = cam;
        
        Vector3 height = startPoint.position;
        elevator.allFloorsIntervals[0] = height.y;
        // create next floors
        for (int i = 1; i < totalFloors; i++)
        {
            height.y += floorsHeight;
            // standard floors
            if (i < totalFloors - 1) floorObj = Instantiate(standardFloor, new Vector3(startPoint.position.x, height.y, startPoint.position.z), Quaternion.identity, floorsParent);
            // last floor
            else floorObj = Instantiate(lastFloor, new Vector3(startPoint.position.x, height.y, startPoint.position.z), Quaternion.identity, floorsParent);

            floor = floorObj.GetComponent<Floor>();
            floor.floorCount.text = i.ToString();
            floor.elevator = elevator;
            floor.floorNumber = i;

            // initiate floor buttons

            // if standard floors
            if (i < totalFloors - 1) buttonUp = floor.floorButtonUp.GetComponent<MyButton>();
            if (i < totalFloors - 1) buttonUp.cam = cam;

            buttonDown = floor.floorButtonDown.GetComponent<MyButton>();
            buttonDown.cam = cam;

            elevator.allFloorsIntervals[i] = height.y;
        }
    }
}
