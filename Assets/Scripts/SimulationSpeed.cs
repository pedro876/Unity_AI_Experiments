using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSpeed : MonoBehaviour
{
    [SerializeField] float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = speed;
    }
}
