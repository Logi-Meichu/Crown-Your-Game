using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafelockController : MonoBehaviour {

    private enum STATE {
        INIT,
        PW1,
        PW2,
        PW3,
        CRACKED
    }

    private enum TRANSITION {
        NULL,
        LEFT,
        RIGHT,
        DOOR
    }

    public GameObject door;
    public GameObject handle;
    public GameObject safelock;
    public AudioSource tick;
    public AudioSource crack;
    public AudioSource open;
    public int currentNumber;
    private int[] passwd;
    private float rotateDegree;
    private STATE state;
    private TRANSITION transition;
    private float openDoorEnd;

    // Use this for initialization
    void Start() {
        rotateDegree = 360.0f / 60.0f;
        state = STATE.INIT;
        transition = TRANSITION.NULL;
        openDoorEnd = -10.0f;
        ResetPasswd();
    }

    // Update is called once per frame
    void Update() {
        CheckInput();
        TransitionState();
        OpenDoor();
        Cheat();
    }

    void OnMouseDown() {
        Destroy(gameObject);
        Debug.Log("I am clicked");
    }

    private void Cheat() {
        if (Input.GetKeyDown(KeyCode.C)) state = STATE.PW3;
    }

    private void ResetPasswd() {
        passwd = new int[3];
        for(int i = 0; i < 3; i++) {
            passwd[i] = Random.Range(0, 59);
            Debug.Log(passwd[i]);
        }
        passwd[0] = 10;
        passwd[1] = 55;
        passwd[2] = 15;
    }

    private void CheckInput() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            transition = TRANSITION.LEFT;
            currentNumber = (currentNumber + 59) % 60;
            safelock.transform.Rotate(rotateDegree, 0, 0);
            tick.Play();
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            transition = TRANSITION.RIGHT;
            currentNumber = (currentNumber + 1) % 60;
            safelock.transform.Rotate(-rotateDegree, 0, 0);
            tick.Play();
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            transition = TRANSITION.DOOR;
        } else {
            transition = TRANSITION.NULL;
        }
    }

    private void TransitionState() {
        switch (state) {
            case STATE.INIT:
                StateInit();
                break;

            case STATE.PW1:
                StatePw1();
                break;

            case STATE.PW2:
                StatePw2();
                break;

            case STATE.PW3:
                StatePw3();
                break;

            case STATE.CRACKED:
                StateCracked();
                break;
        }
    }
    
    private void StateInit() {
        if (transition == TRANSITION.RIGHT) {
            if(currentNumber == passwd[0]) {
                state = STATE.PW1;
                crack.Play();
                Debug.Log("Break Pw1");
            }
        }
    }

    private void StatePw1() {
        if (transition == TRANSITION.LEFT) {
            if (currentNumber == passwd[1]) {
                state = STATE.PW2;
                crack.Play();
                Debug.Log("Break Pw2");
            }
        } else if (transition == TRANSITION.RIGHT) {
            if (currentNumber == passwd[0] + 1) {
                state = STATE.INIT;
                Debug.Log("Go back to Init");
            }
        }
    }

    private void StatePw2() {
        if (transition == TRANSITION.RIGHT) {
            if (currentNumber == passwd[2]) {
                state = STATE.PW3;
                crack.Play();
                Debug.Log("Break Pw3");
            }
        } else if (transition == TRANSITION.LEFT) {
            if (currentNumber == passwd[1] - 1) {
                state = STATE.INIT;
                Debug.Log("Go back to Init");
            }
        }
    }

    private void StatePw3() {
        if (transition == TRANSITION.LEFT) {
            if (currentNumber == passwd[2] - 1) {
                state = STATE.PW2;
                Debug.Log("Go back to Pw2");
            }
        } else if (transition == TRANSITION.RIGHT) {
            if (currentNumber == passwd[2] + 1) {
                state = STATE.INIT;
                Debug.Log("Go back to Init");
            }
        } else if (transition == TRANSITION.DOOR) {
            state = STATE.CRACKED;
            openDoorEnd = Time.time + 1.5f;
            Debug.Log("Break Safelock");
        }
    }

    private void StateCracked() {
        if (transition == TRANSITION.DOOR) {
            state = STATE.PW3;
            openDoorEnd = Time.time + 1.5f;
            Debug.Log("Go back to Pw3");
        }
    }

    private void OpenDoor() {
        if (Time.time > openDoorEnd) return;
        if (!open.isPlaying) open.Play();
        door.transform.Rotate(0, 0, 90 * Time.deltaTime * (state == STATE.CRACKED ? -1 : 1));
    }
}
