using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//Functions for the Astar button and users click
namespace PathFinding {
    public class AStar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    
        public static bool AStarbuttonPressed;
        
        public void OnPointerDown(PointerEventData eventData){
            AStarbuttonPressed = true;
        }
        
        public void OnPointerUp(PointerEventData eventData){
            AStarbuttonPressed = false;
        }
    }
}

