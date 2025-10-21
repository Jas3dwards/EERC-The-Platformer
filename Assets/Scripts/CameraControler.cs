using UnityEngine;

public class CameraControl  : MonoBehaviour{
    [SerializeField] private Transform player;

    private void Update(){
 
        transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);    
    }

}
