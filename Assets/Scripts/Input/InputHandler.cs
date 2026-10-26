using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    CarHandler carHandler;

    [Header("One button controls")]
    [SerializeField]
    KeyCode actionKey = KeyCode.Space;

    //How long the key must be held before it counts as a hold (brake) instead of a tap (lane switch)
    [SerializeField]
    float holdThreshold = 0.2f;

    int currentLane = 0;

    float keyDownTime = 0;
    bool isKeyHeld = false;
    bool isBraking = false;

    private void Awake()
    {
        if (!CompareTag("Player"))
        {
            Destroy(this);
            return;
        }
    }

    void Start()
    {
        //Start in whichever lane is closest to where the car was placed
        currentLane = GetClosestLane(transform.position.x);
    }

    void Update()
    {
        //Key pressed, start timing it
        if (Input.GetKeyDown(actionKey))
        {
            keyDownTime = Time.unscaledTime;
            isKeyHeld = true;
            isBraking = false;
        }

        //Key still held, once it passes the threshold it becomes a brake
        if (isKeyHeld && Input.GetKey(actionKey))
        {
            if (Time.unscaledTime - keyDownTime >= holdThreshold)
                isBraking = true;
        }

        //Key released, if it never became a hold it was a tap so switch lane
        if (isKeyHeld && Input.GetKeyUp(actionKey))
        {
            if (!isBraking)
                currentLane = (currentLane + 1) % Utils.CarLanes.Length;

            isKeyHeld = false;
            isBraking = false;
        }

        //Tell the car which lane to be in, CarHandler moves it there without overshooting
        carHandler.SetLaneTarget(Utils.CarLanes[currentLane]);

        //Brake while holding, otherwise always accelerate
        carHandler.SetInput(new Vector2(0, isBraking ? -1.0f : 1.0f));

        if (Input.GetKeyDown(KeyCode.R))
        {
            //Restore time scale
            Time.timeScale = 1.0f;

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    int GetClosestLane(float x)
    {
        int closestLane = 0;

        for (int i = 1; i < Utils.CarLanes.Length; i++)
        {
            if (Mathf.Abs(Utils.CarLanes[i] - x) < Mathf.Abs(Utils.CarLanes[closestLane] - x))
                closestLane = i;
        }

        return closestLane;
    }
}