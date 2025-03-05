using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PathFinding {
    public class TimeDisplay : MonoBehaviour
    {
        public Text timeText;
        public float elapsedTime;
        public bool isTimerRunning = false;
        public string finalString = "";
        public int seconds;
        public int tempBFS;

        // Intializes the values and calls update
        public void Start()
        {
            elapsedTime = 0.0f;
            isTimerRunning = false;
            Update();
        }

        // Updates the time
        public void Update()
        {

            if (isTimerRunning)
            {
                elapsedTime += UnityEngine.Time.deltaTime;
                
                UpdateTimeText();
                
            } else {
                
                elapsedTime = 0.0f;
               
                timeText.text = "TIME: " + finalString;
            }

        }
        
        // Text formatting
        public void UpdateTimeText()
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            seconds = Mathf.FloorToInt(elapsedTime % 60f);
            string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
            finalString = timeString;
            timeText.text = "TIME: " + timeString; 
        
        }

       // Call this method to start the timer
        public void StartTimer()
        { 
            elapsedTime = 0f; 
           
            isTimerRunning = true;
            
        }

        // Call this method to stop the timer
        public void StopTimer()
        {
            isTimerRunning = false;
        }

        // Resets timer
        public void ResetTimer()
        {
            elapsedTime = 0f;
        }

    }
}