using UnityEngine;
using UnityEngine.UI;

public class Floor : MonoBehaviour
{
    public Transform roomIndicatorDisplay;
    [Header("Current floor buttons")]
    public GameObject floorButtonUp;
    public GameObject floorButtonDown;
    public TextMesh floorCount;
    [Header("Current floor doors")]
    public Transform Doors;
    [HideInInspector]
    public int floorNumber;

    [HideInInspector]
    public Animator doorsAnim;
    [HideInInspector]
    public Text floorCurrentPosText;
    [HideInInspector]
    public Elevator elevator;

    [HideInInspector]
    public bool upDestination = false;
    [HideInInspector]
    public bool downDestination = false;


    private void Start()
    {
        doorsAnim = Doors.GetComponent<Animator>();
        floorCurrentPosText = roomIndicatorDisplay.GetChild(0).GetComponent<Text>();
    }
    // Hall buttons was clicked (upDest = clicked up destination button or down destination button)
    public void ButtonEvent(bool upDest)
    {
        // if elevator don't have registered numbers or if elevator is at the current hall
        if (elevator.registeredNums == 0 || floorNumber == elevator.currentFloorNum) elevator.HallButtonClicked(floorNumber);

        else if (upDest) // 
        {
            upDestination = true;
            if (elevator._tempTopFloor < floorNumber) elevator._tempTopFloor = floorNumber; // set for elevator highset number for destination
        }
        else
        {
            downDestination = true;
            if (elevator._tempBottomFloor > floorNumber) elevator._tempBottomFloor = floorNumber; // set for elevator lowest number for destination
        }
    }
    public void ResetButtonsAtTimers(bool isTimer)
    {
        if (isTimer)
        {
            if (floorButtonUp)
            {
                floorButtonUp.GetComponent<MyButton>().ResetBool();
                floorButtonUp.GetComponent<MyButton>().buttonResets = MyButton.ButtonResets.ByTimer;
            }
            if (floorButtonDown)
            {
                floorButtonDown.GetComponent<MyButton>().ResetBool();
                floorButtonDown.GetComponent<MyButton>().buttonResets = MyButton.ButtonResets.ByTimer;
            }
        }
        else
        {
            if (floorButtonUp)
            {
                floorButtonUp.GetComponent<MyButton>().ResetBool();
                floorButtonUp.GetComponent<MyButton>().buttonResets = MyButton.ButtonResets.ByBool;
            }
            if (floorButtonDown)
            {
                floorButtonDown.GetComponent<MyButton>().ResetBool();
                floorButtonDown.GetComponent<MyButton>().buttonResets = MyButton.ButtonResets.ByBool;
            }
        }
    }
}
