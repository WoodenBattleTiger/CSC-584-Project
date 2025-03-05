using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding {

public class Player : MonoBehaviour
{
    


        public float speed = 5.0f; 

        //private bool isMoving = false;

        void Update()
        {
           
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector2 movement = new Vector2(horizontalInput, verticalInput);

            // isMoving = (horizontalInput != 0 || verticalInput != 0);

            // if (isMoving)
            // {
            //     transform.Translate(movement * speed * Time.deltaTime);
            // }
        
        }
    
}
}