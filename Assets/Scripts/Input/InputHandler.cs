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
    KeyCode switchLaneKey = KeyCode.Space;

    int currentLane = 0;

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
        //Switch lane on a single button press
        if (Input.GetKeyDown(switchLaneKey))
            currentLane = (currentLane + 1) % Utils.CarLanes.Length;

        //Tell the car which lane to be in, CarHandler moves it there without overshooting
        carHandler.SetLaneTarget(Utils.CarLanes[currentLane]);

        //Always accelerate, the player only controls the lane
        carHandler.SetInput(new Vector2(0, 1.0f));

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