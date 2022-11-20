using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    bool pressed;
    public void PauseGame()
    {
        if(!pressed)
        {
            pressed = true;
            Time.timeScale = 0;
        }
        else
        {
            pressed = false;
            Time.timeScale = 1;
        }
    }
    
}
