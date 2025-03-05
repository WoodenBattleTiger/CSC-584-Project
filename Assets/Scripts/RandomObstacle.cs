using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//Functions for the Random Obstacles button and users click
//Creates a new random obstacle on the grid if button is pressed
namespace PathFinding {
    public class RandomObstacle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    
        public static bool RandomObstaclebuttonPressed;
        
        public void OnPointerDown(PointerEventData eventData){
           RandomObstaclebuttonPressed = true;
        }
        
        public void OnPointerUp(PointerEventData eventData){
            RandomObstaclebuttonPressed = false;
        }

        
    }
}

