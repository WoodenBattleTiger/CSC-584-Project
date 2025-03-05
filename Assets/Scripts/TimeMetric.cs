using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//Functions for the Quit button and users click
namespace PathFinding {
    public class TimeMetric : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    
        public static bool timeMetricButtonPressed;
        
        public void OnPointerDown(PointerEventData eventData){
            timeMetricButtonPressed = true;
        }
        
        public void OnPointerUp(PointerEventData eventData){
            timeMetricButtonPressed = false;
        }
    }
}

