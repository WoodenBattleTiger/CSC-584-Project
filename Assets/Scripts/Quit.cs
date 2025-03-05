using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//Functions for the Quit button and users click
namespace PathFinding {
    public class Quit : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    
        public static bool QuitbuttonPressed;
        
        public void OnPointerDown(PointerEventData eventData){
            QuitbuttonPressed = true;
        }
        
        public void OnPointerUp(PointerEventData eventData){
            QuitbuttonPressed = false;
        }
    }
}

