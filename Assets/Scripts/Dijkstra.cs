using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//Functions for the Dijkstra button and users click
namespace PathFinding {
    public class Dijkstra : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    
        public static bool DijkstrabuttonPressed;
        
        public void OnPointerDown(PointerEventData eventData){
            DijkstrabuttonPressed = true;
        }
        
        public void OnPointerUp(PointerEventData eventData){
            DijkstrabuttonPressed = false;
        }
    }
}

