using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//Functions for the DFS button and users click
namespace PathFinding {
    public class DFS : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    
        public static bool DFSbuttonPressed;
        
        public void OnPointerDown(PointerEventData eventData){
            DFSbuttonPressed = true;
        }
        
        public void OnPointerUp(PointerEventData eventData){
            DFSbuttonPressed = false;
        }
    }
}

