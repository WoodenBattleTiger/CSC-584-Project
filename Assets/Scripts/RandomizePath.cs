using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//Functions for the Randomize Path button and users click
//Creates a random start and end point if button is pressed
namespace PathFinding {
    public class RandomizePath : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    
        public static bool RandomizePathbuttonPressed;
        
        public void OnPointerDown(PointerEventData eventData){
            RandomizePathbuttonPressed = true;
        }
        
        public void OnPointerUp(PointerEventData eventData){
            RandomizePathbuttonPressed = false;
        }
    }
}

