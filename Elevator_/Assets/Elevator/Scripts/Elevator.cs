using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Elevator : MonoBehaviour
{
    [SerializeField]
    private GameObject elevatorObj;

    [SerializeField]
    private Transform floorsParent;
    private Floor currentFloorSCR;

    [HideInInspector]
    public int currentFloorNum = 0;
    private int targetFloor;
    private bool hallBoolsIsActive = false;

    [SerializeField]
    private float elevatorNormalSpeed;
    private float _elevatorSpeed;
    [SerializeField]
    private float elevatorMaxSpeed;

    [HideInInspector]
    public float[] allFloorsIntervals;

    private bool isDoubleNumers = false;
    private string _firstDoeubleNumString;

    private bool elevatorIsMoving = false;
    private bool elevatorIsWaiting = true; 
    private bool isUpMoving = true;
    private float _tempNextFloorY;
    #region Doors
    public Animator elevatorDoorAnim;
    [SerializeField]
    private float doorsWaitTime = 10f;
    private float _tempDoorsWaitTime;

    private bool doorsClosingIsStarted = false;
    private float doorsClosingTime = 0;
    private bool doorIsActive = false;
    private bool doorsIsOpened = false;
    private bool doNotClose;
    #endregion

    [Space(15)]
    [SerializeField]
    private GameObject doubleNumsButt;

    #region Display
    [Header("Display")]
    [SerializeField]
    private Text displayNextText;
    [SerializeField]
    private Text displayCurPosText;
    [SerializeField]
    private GameObject displayArrowUp;
    [SerializeField]
    private GameObject displayArrowDown;
    private string _tempText;
    #endregion

    #region Audios
    [Header("Audios")]
    [SerializeField]
    private AudioSource openingDoorsSound;
    [SerializeField]
    private AudioSource closingDoorsSound;
    [SerializeField]
    private AudioSource closingWarningSound;
    [SerializeField]
    private AudioSource movingIsEndWarningSound;
    [SerializeField]
    private AudioSource movingSound;
    [SerializeField]
    private AudioSource endMovingSound;
    [SerializeField]
    private AudioSource wrongSound;
    private bool soundsIsPlaying = false;
    #endregion

    List<int> normalDirNumbers = new List<int>();
    List<int> oppositeDirNumbers = new List<int>();
    [HideInInspector]
    public int _tempTopFloor = 0;
    [HideInInspector]
    public int _tempBottomFloor = -1;

    [HideInInspector]
    public int registeredNums;
    private bool isFirstNumOfDouble = false;

    void Start()
    {
        SetVariables();
    }
    void SetVariables()
    {
        elevatorObj.SetActive(true);

        if (floorsParent.childCount > 10) doubleNumsButt.SetActive(true); // if floors more then 10 - activate special button for set two number floors
        else doubleNumsButt.SetActive(false);

        currentFloorSCR = floorsParent.GetChild(currentFloorNum).GetComponent<Floor>(); // take current floor script

        _tempDoorsWaitTime = doorsWaitTime;
        _tempNextFloorY = floorsParent.GetChild(currentFloorNum + 1).transform.position.y;
        _elevatorSpeed = elevatorNormalSpeed;

        displayNextText.text = "<size=9>Please enter dastination floor</size>";
        ShowElevatorCurrentPos(currentFloorNum);
    }

    void Update()
    {
        if (!elevatorIsWaiting)
        {
            if (elevatorIsMoving)
            {
                ElevatorSpeedAndMovingSoundsController();

                if (isUpMoving && transform.position.y < allFloorsIntervals[targetFloor])
                {
                    transform.position += Vector3.up * Time.deltaTime * _elevatorSpeed;
                }
                else if (!isUpMoving && transform.position.y > allFloorsIntervals[targetFloor])
                {
                    transform.position -= Vector3.up * Time.deltaTime * _elevatorSpeed;
                }
                else EndMoving();

                CurrentFloorDetector();
            }
        } // todo - stop elevator

        if (doorsIsOpened) DoorsController();
    }

    // elevator functionality
    void SetNewDestination()  // choose from registered numbers next floor number
    {
        if (isUpMoving) ChooseNextFloorNumberUp(); // if the elevator going to up direction, next floor number will by only top
        else ChooseNextFloorNumberDown(); // or opposite direction
    }
    void ChooseNextFloorNumberUp()
    {
        if (normalDirNumbers.Count != 0)
        {
            targetFloor = normalDirNumbers[0];
            if (!doorIsActive && !elevatorIsMoving) ActivateElevator(); // if door is closed - go up to next nearest floor
        }
        else if (oppositeDirNumbers.Count != 0)
        {
            isUpMoving = false;
            ChooseNextFloorNumberDown(); // or change direction if there are requests in the opposite direction
        }
        else CheckHallButtons(); // if there are no registered numbers in elevator, check if hall buttons was clicked
    }
    void ChooseNextFloorNumberDown()
    {
        if (oppositeDirNumbers.Count != 0)
        {
            targetFloor = oppositeDirNumbers[oppositeDirNumbers.Count - 1];
            if (!doorIsActive && !elevatorIsMoving) ActivateElevator(); // if door is closed - go down to next nearest floor
        }
        else if (normalDirNumbers.Count != 0)
        {
            isUpMoving = true;
            ChooseNextFloorNumberUp(); // or change direction if there are requests in the opposite direction
        }
        else CheckHallButtons(); // if there are no registered numbers in elevator, check if hall buttons was clicked
    }
    void CheckHallButtons()
    {
        if (isUpMoving && _tempTopFloor > 0) // if there are upstair clicked hall button
        {
            if (_tempTopFloor > currentFloorNum)
            {
                targetFloor = _tempTopFloor;
                _tempTopFloor = 0;
                if (!doorIsActive && !elevatorIsMoving) ActivateElevator();
            }
        }
        else if (isUpMoving && _tempBottomFloor > -1) // if there are downstair clicked hall button
        {
            isUpMoving = false;
            targetFloor = _tempBottomFloor;
            _tempBottomFloor = -1;
            if (!doorIsActive && !elevatorIsMoving) ActivateElevator();
        }
        else if (!isUpMoving && _tempBottomFloor > -1) // if there are upstair clicked hall button
        {
            if (_tempBottomFloor < currentFloorNum)
            {
                targetFloor = _tempBottomFloor;
                _tempBottomFloor = -1;
                if (!doorIsActive && !elevatorIsMoving) ActivateElevator();
            }
        }
        else if (!isUpMoving && _tempTopFloor > 0) // if there are downstair clicked hall button
        {
            isUpMoving = true;
            targetFloor = _tempTopFloor;
            _tempTopFloor = 0;
            if (!doorIsActive && !elevatorIsMoving) ActivateElevator();
        }
        else elevatorIsWaiting = true;
    }

    void DistanceToNextFloor()
    {
        if (isUpMoving) _tempNextFloorY = floorsParent.GetChild(currentFloorNum + 1).transform.position.y;
        else _tempNextFloorY = floorsParent.GetChild(currentFloorNum - 1).transform.position.y;
    }

    IEnumerator UpdateList()
    {
        yield return new WaitUntil(() => doorsIsOpened == true);
        if (!hallBoolsIsActive)
        {
            if (isUpMoving)
            {
                if (normalDirNumbers.Count > 0)
                {
                    normalDirNumbers.RemoveAt(0);
                    normalDirNumbers.Sort();
                }
            }
            else
            {
                if (oppositeDirNumbers.Count > 0)
                {
                    oppositeDirNumbers.RemoveAt(oppositeDirNumbers.Count - 1);
                    oppositeDirNumbers.Sort();
                }
            }
            registeredNums--;
            ShowSelectedNumsInDisplay();
        }
        else hallBoolsIsActive = false;
    }

    // moving
    void ActivateElevator()
    {
        DistanceToNextFloor();
        elevatorIsMoving = true;
    }

    /// Speed and moving sounds
    /// if target floor is far than 2 floor, accelerate elevator. else, if speed was accelerated, slow down.
    /// if elevator is close to the target floor stop slowly
    /// play moving or endMoving sounds
    void ElevatorSpeedAndMovingSoundsController()
    {
        if (isUpMoving)
        {
            if (currentFloorNum + 2 < targetFloor && _elevatorSpeed < elevatorMaxSpeed) _elevatorSpeed += Time.deltaTime;// if next floor is too high
            else if (_elevatorSpeed > elevatorNormalSpeed) _elevatorSpeed -= Time.deltaTime * 1.7f;
            if (!soundsIsPlaying) PlayMovingOrEndSounds(soundsIsPlaying);
        }
        else
        {
            if (currentFloorNum - 2 > targetFloor && _elevatorSpeed < elevatorMaxSpeed) _elevatorSpeed += Time.deltaTime;// if next floor is too low
            else if (_elevatorSpeed > elevatorNormalSpeed) _elevatorSpeed -= Time.deltaTime * 1.7f;
            if (!soundsIsPlaying) PlayMovingOrEndSounds(soundsIsPlaying);
        }
        if (currentFloorNum == targetFloor)
        {
            if (_elevatorSpeed > 0.2f) _elevatorSpeed -= Time.deltaTime; // slow down elevator
            if (soundsIsPlaying) PlayMovingOrEndSounds(soundsIsPlaying); // play slow down sound
        }
        movingSound.pitch = 1 + _elevatorSpeed / 70; // play moving sound at the speed at which elevator goes
    }
    void PlayMovingOrEndSounds(bool endMoving)
    {
        if (!endMoving)
        {
            if (!movingSound.isPlaying) movingSound.Play();
            soundsIsPlaying = true;
        }
        else
        {
            if (movingSound.isPlaying) movingSound.Stop();
            if (!endMovingSound.isPlaying) endMovingSound.Play();
        }
    }

    void CurrentFloorDetector()
    {
        if (isUpMoving && transform.position.y > _tempNextFloorY - 1.5f) // catch next floor a little fast for speed control
        {
            if (currentFloorNum + 1 < floorsParent.childCount - 1) // if next floor available
            {
                currentFloorNum++;
                _tempNextFloorY = allFloorsIntervals[currentFloorNum + 1];
                CheckHallBools(currentFloorNum); // check if next hall buttons are active
                ShowElevatorCurrentPos(currentFloorNum);
            }
            else if (currentFloorNum != targetFloor) // else next is last floor
            {
                currentFloorNum++;
                ShowElevatorCurrentPos(currentFloorNum);
            }
        }
        if (!isUpMoving && transform.position.y < _tempNextFloorY + 1.5f) // catch next floor a little fast for speed control
        {
            if (currentFloorNum - 1 > 0) // if bottom floor is available
            {
                currentFloorNum--;
                _tempNextFloorY = allFloorsIntervals[currentFloorNum - 1];
                CheckHallBools(currentFloorNum); // check if next hall buttons are active
                ShowElevatorCurrentPos(currentFloorNum);
            }
            else if (currentFloorNum != targetFloor) // else bottom is first floor
            {
                currentFloorNum--;
                ShowElevatorCurrentPos(currentFloorNum);
            }
        }
    }
    void CheckHallBools(int nextFloor)// check if next hall buttons are active 
    {
        Floor _tempFloor = floorsParent.GetChild(nextFloor).GetComponent<Floor>();
        if (isUpMoving && _tempFloor.upDestination)
        {
            targetFloor = nextFloor;
            hallBoolsIsActive = true;
        }
        else if (!isUpMoving && _tempFloor.downDestination)
        {
            targetFloor = nextFloor;
            hallBoolsIsActive = true;
        }
    }

    void EndMoving()
    {
        elevatorIsMoving = soundsIsPlaying = false;

        movingIsEndWarningSound.Play();

        _elevatorSpeed = elevatorNormalSpeed;

        currentFloorSCR = floorsParent.GetChild(currentFloorNum).GetComponent<Floor>();

        StartCoroutine(OpenAndCloseDoors());

        StartCoroutine(UpdateList());
    }

    // doors functionality
    #region Doors
    void DoorsController()
    {
        if (_tempDoorsWaitTime > 0) _tempDoorsWaitTime -= Time.deltaTime * 1; // doors timer
        if (doorsClosingIsStarted && !doNotClose) // if doors closing is started and nobady is in sensor area
        {
            doorsClosingTime += Time.deltaTime;
            AudioClip a = closingDoorsSound.clip;
            if (doorsClosingTime > a.length) // wait when doors closes sound is end
            {
                DeactivateDoors();
                if (normalDirNumbers.Count > 0 || oppositeDirNumbers.Count > 0) SetNewDestination();// if there are more next floor numbers 
                else CheckHallButtons();
                doorsClosingIsStarted = false;
            }
        }
        else if (doorsClosingIsStarted && doNotClose) // if doors closing is started and somebody is in sensor area or Open door/floor buttons was activated
        {
            doorsClosingTime = 0;
            OpenDoorsAgain();
            doorsClosingIsStarted = false;
        }
    }

    void OpenTheDoors()
    {
        if (!doorsIsOpened) StartCoroutine(OpenAndCloseDoors());
        else if (_tempDoorsWaitTime <= 0) doNotClose = true;// if doors closing was starting and somebody clicks of (open door) buttons - door will by opened
    }
    IEnumerator OpenAndCloseDoors() // open and close doors normally
    {
        doorIsActive = true;
        StartCoroutine(OpenDoors());
        yield return new WaitWhile(() => openingDoorsSound.isPlaying);
        doorsIsOpened = true;
        currentFloorSCR.ResetButtonsAtTimers(true);// button in floor will by reseted to timer mode
        // wait
        yield return new WaitUntil(() => _tempDoorsWaitTime <= 0);
        currentFloorSCR.ResetButtonsAtTimers(false);// button in floor will by reseted to bool mode
        // start closing doors
        StartCoroutine(CloseDoors());
        doorsClosingIsStarted = true;
    }
    IEnumerator OpenDoors()
    {
        // open doors animations
        currentFloorSCR.doorsAnim.Play("OpenDoors");
        openingDoorsSound.Play();
        yield return new WaitForSeconds(0.1f);
        elevatorDoorAnim.Play("OpenDoors");
    }
    IEnumerator CloseDoors()
    {
        closingDoorsSound.Play();
        closingWarningSound.Play();

        elevatorDoorAnim.Play("CloseDoors");
        yield return new WaitForSeconds(0.1f);
        currentFloorSCR.doorsAnim.Play("CloseDoors");
    }

    void OpenDoorsAgain()// if closing was started 
    {
        // play animations backward
        currentFloorSCR.doorsAnim.StartPlayback();
        currentFloorSCR.doorsAnim.speed = -1f;
        elevatorDoorAnim.StartPlayback();
        elevatorDoorAnim.speed = -1f;

        ResetDoorsWaitTime();
        StartCoroutine(CloseDoorsAgain());
    }
    IEnumerator CloseDoorsAgain()
    {
        doNotClose = false;

        yield return new WaitUntil(() => _tempDoorsWaitTime <= 0);

        elevatorDoorAnim.StartPlayback();
        elevatorDoorAnim.speed = 1f;
        elevatorDoorAnim.Play("CloseDoors", -1, 0f);

        closingWarningSound.Play();
        closingDoorsSound.Play();

        yield return new WaitForSeconds(0.1f);
        currentFloorSCR.doorsAnim.StartPlayback();
        currentFloorSCR.doorsAnim.speed = 1f;
        currentFloorSCR.doorsAnim.Play("CloseDoors", -1, 0f);

        doorsClosingIsStarted = true;
    }

    public void OpenDoorsBySensor()  // if somebody is in photocell sensor area - doors dont starts closes
    {
        if (_tempDoorsWaitTime < (doorsWaitTime / 2) && _tempDoorsWaitTime > 0) ResetDoorsWaitTime(); //timer will by reset to half of start waiting time
        else if (_tempDoorsWaitTime <= 0) doNotClose = true;// if doors closing was starting and somebody enters in photocell sensor area - door will by opened
    }
    public void CloseDoorsImmediately()
    {
        if (doorsIsOpened) //if door is opened
        {
            _tempDoorsWaitTime = 0;
        }
    }
    void ResetDoorsWaitTime()
    {
        _tempDoorsWaitTime = doorsWaitTime / 2;
    }
    void DeactivateDoors()
    {
        currentFloorSCR.ResetButtonsAtTimers(false); // come buttons reset mode again to bool
        doorsIsOpened = false;
        _tempDoorsWaitTime = doorsWaitTime;
        doNotClose = false;
        doorsClosingTime = 0;
        doorIsActive = false;
    }
    #endregion

    // buttons functionality
    public void ElevatorFunctionButtsClicked(string buttonName)
    {
        switch (buttonName)
        {
            case "OpenDoors":
                OpenTheDoors();
                break;
            case "CloseDoors":
                CloseDoorsImmediately();
                break;
            case "DoubleNumers":
                isDoubleNumers = true;
                break;
            case "Ring":
                // Play ring sound, stop elevator, etc. ...
                break;
        }
    }
    public void ElevatorButtsClicked(int selectedN)
    {
        if (isDoubleNumers) DoubleNumers(selectedN);

        else if (selectedN != currentFloorNum && selectedN < allFloorsIntervals.Length)
        {
            CheckRegisteredNumsAndSetNew(selectedN);
        }
        else StartCoroutine(ShowErrorText(false));
    }
    public void HallButtonClicked(int floorN)
    {
        if (floorN == currentFloorNum && !elevatorIsMoving) OpenTheDoors(); // open current floor doors
        else if (normalDirNumbers.Count == 0 || oppositeDirNumbers.Count == 0)
        {
            CheckRegisteredNumsAndSetNew(floorN);
        }
    }

    void DoubleNumers(int selectedN) // create from two numbers one double, check if there are so many floors and set it
    {
        _tempText = displayNextText.text; // save current text in display 
        // if player started select double numers and first number is correct
        if (!isFirstNumOfDouble && selectedN > 0 && selectedN * 10 < allFloorsIntervals.Length)
        {
            isFirstNumOfDouble = true;

            _firstDoeubleNumString = selectedN.ToString(); // save 
            ShowMoreThanNine(selectedN + "-");             // and show first number
        }
        else if (isFirstNumOfDouble)
        {
            doubleNumsButt.GetComponent<MyButton>().ResetBool();

            // from first and second numbers become one doubled numer
            string tempDoubleNumString = _firstDoeubleNumString + selectedN.ToString();
            int tempDoubleNumInt = int.Parse(tempDoubleNumString);
            _firstDoeubleNumString = "";

            if (tempDoubleNumInt < allFloorsIntervals.Length) CheckRegisteredNumsAndSetNew(tempDoubleNumInt);
            else StartCoroutine(ShowErrorText(true));

            isFirstNumOfDouble = isDoubleNumers = false;
        }
        else // first number of double was incorrect
        {
            doubleNumsButt.GetComponent<MyButton>().ResetBool();
            isDoubleNumers = false;
            StartCoroutine(ShowErrorText(true));
        }
    }

    void CheckRegisteredNumsAndSetNew(int newFloorNum)
    {
        if (registeredNums != 0 && normalDirNumbers.Contains(newFloorNum) || oppositeDirNumbers.Contains(newFloorNum)) ShowErrorText(false); // there already are this number 

        else if (!elevatorIsMoving) SetNumerIfIdle(newFloorNum);
        else if (elevatorIsMoving) SetNumerIfMoving(newFloorNum);
    }

    void SetNumerIfIdle(int newFloorNum)
    {
        if (isUpMoving)
        {
            if (newFloorNum > currentFloorNum) normalDirNumbers.Add(newFloorNum);
            else oppositeDirNumbers.Add(newFloorNum);
        }
        else
        {
            if (newFloorNum < currentFloorNum) oppositeDirNumbers.Add(newFloorNum);
            else normalDirNumbers.Add(newFloorNum);
        }
        registeredNums++;
        normalDirNumbers.Sort();
        oppositeDirNumbers.Sort();

        elevatorIsWaiting = false;

        if (!doorIsActive) SetNewDestination();

        ShowSelectedNumsInDisplay();
    }
    void SetNumerIfMoving(int newFloorNum)
    {
        if (isUpMoving)
        {
            if (newFloorNum > currentFloorNum + 1) normalDirNumbers.Add(newFloorNum);
            else oppositeDirNumbers.Add(newFloorNum);
        }
        else
        {
            if (newFloorNum < currentFloorNum - 1) oppositeDirNumbers.Add(newFloorNum);
            else normalDirNumbers.Add(newFloorNum);
        }
        registeredNums++;
        normalDirNumbers.Sort();
        oppositeDirNumbers.Sort();

        SetNewDestination();

        ShowSelectedNumsInDisplay();
    }

    //elevator display
    void ShowMoreThanNine(string floorN)
    {
        if (registeredNums == 0) displayNextText.text = floorN; // if is first number in display, just show number (1-)
        else displayNextText.text += "," + floorN; // or show first comma symbol and then number(1, 1-)
    }
    void ShowSelectedNumsInDisplay()
    {
        displayNextText.text = "<size=9>Please enter dastination floor</size>";
        if (isUpMoving)
        {
            if (oppositeDirNumbers.Count != 0)
            {
                for (int i = 0; i < oppositeDirNumbers.Count; i++)
                {
                    if (i == 0) displayNextText.text = oppositeDirNumbers[0].ToString();
                    else displayNextText.text += "," + oppositeDirNumbers[i].ToString();
                }
                for (int i = 0; i < normalDirNumbers.Count; i++)
                {
                    if (i == 0) displayNextText.text += ",<color=green><size=13>" + normalDirNumbers[0].ToString() + "</size></color>";
                    else displayNextText.text += "," + normalDirNumbers[i].ToString();
                }
            }
            else
            {
                for (int i = 0; i < normalDirNumbers.Count; i++)
                {
                    if (i == 0) displayNextText.text = "<color=green><size=13>" + normalDirNumbers[0].ToString() + "</size></color>";
                    else displayNextText.text += "," + normalDirNumbers[i].ToString();
                }
            }
        }
        else
        {
            if (normalDirNumbers.Count != 0)
            {
                for (int i = 0; i < oppositeDirNumbers.Count; i++)
                {
                    if (i == 0 && oppositeDirNumbers.Count == 1) displayNextText.text = "<color=green><size=13>" + oppositeDirNumbers[i].ToString() + "</size></color>";
                    else if (i == 0) displayNextText.text = oppositeDirNumbers[0].ToString();
                    else if (i == oppositeDirNumbers.Count - 1) displayNextText.text += ",<color=green><size=13>" + oppositeDirNumbers[i].ToString() + "</size></color>";
                    else displayNextText.text += "," + oppositeDirNumbers[i].ToString();
                }
                for (int i = 0; i < normalDirNumbers.Count; i++)
                {
                    if (i == 0) displayNextText.text = "<color=green><size=13>" + normalDirNumbers[0].ToString() + "</size></color>";
                    else displayNextText.text += "," + normalDirNumbers[i].ToString();
                }
            }
            else
            {
                if (oppositeDirNumbers.Count != 0)
                {
                    for (int i = 0; i < oppositeDirNumbers.Count; i++)
                    {
                        if (i == 0 && oppositeDirNumbers.Count == 1) displayNextText.text = "<color=green><size=13>" + oppositeDirNumbers[i].ToString() + "</size></color>";
                        else if (i == 0) displayNextText.text = oppositeDirNumbers[0].ToString();
                        else if (i == oppositeDirNumbers.Count - 1) displayNextText.text += ",<color=green><size=13>" + oppositeDirNumbers[i].ToString() + "</size></color>";
                        else displayNextText.text += "," + oppositeDirNumbers[i].ToString();
                    }
                }
            }
        }

    }
    IEnumerator ShowErrorText(bool wrongDoubleNumber)
    {
        string tempText;

        if (wrongDoubleNumber) tempText = _tempText;
        else tempText = displayNextText.text; ;

        wrongSound.Play();
        displayNextText.text = "WRONG FLOOR!";

        if (!elevatorIsMoving) yield return new WaitForSeconds(1.0f);
        else yield return new WaitForSeconds(0.5f);

        displayNextText.text = tempText;
    }

    // floors display and arrows
    void ShowElevatorCurrentPos(int floorN)
    {
        for (int i = 0; i < floorsParent.childCount; i++)
        {
            Floor tempFloor = floorsParent.GetChild(i).GetComponent<Floor>();
            ChangeArrows(tempFloor);
            tempFloor.floorCurrentPosText.text = floorN.ToString();
            displayCurPosText.text = floorN.ToString();
        }
    }
    void ChangeArrows(Floor tempFloor)
    {
        if (isUpMoving)
        {
            tempFloor.roomIndicatorDisplay.GetChild(1).gameObject.SetActive(true); // up Arrow in doors indicator
            tempFloor.roomIndicatorDisplay.GetChild(2).gameObject.SetActive(false); // down Arrow in doors indicator
            displayArrowUp.gameObject.SetActive(true);
            displayArrowDown.gameObject.SetActive(false);
        }
        else
        {
            tempFloor.roomIndicatorDisplay.GetChild(1).gameObject.SetActive(false); // up Arrow in doors indicator
            tempFloor.roomIndicatorDisplay.GetChild(2).gameObject.SetActive(true); // down Arrow in doors indicator
            displayArrowUp.gameObject.SetActive(false);
            displayArrowDown.gameObject.SetActive(true);
        }
    }
}
