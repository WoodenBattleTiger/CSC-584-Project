using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//Functions for the BFS button and users click
namespace PathFinding {
    public class BFSSelector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    
        public static bool BFSbuttonPressed;
        
        public void OnPointerDown(PointerEventData eventData){
            BFSbuttonPressed = true;
        }
        
        public void OnPointerUp(PointerEventData eventData){
            BFSbuttonPressed = false;
        }
    }
}

