using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public enum CharacterType { Kid = 0, Dog, Brother };

    public Transform character;

    [SerializeField] float followSpeed;
    [SerializeField] float offset;

    private void FixedUpdate()
    {
        if (character == null)
            return;

        if (/*Input.GetKeyDown(KeyCode.D)*/Input.GetAxisRaw("Horizontal") > 0)
        {
            offset = Mathf.Abs(offset);
        }

        if (/*Input.GetKeyDown(KeyCode.A)*/Input.GetAxisRaw("Horizontal") < 0)
        {
            offset = -Mathf.Abs(offset);
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(character.position.x + offset,
                                                                          character.position.y + 4, transform.position.z),
                                                                          followSpeed);
    }
    void Update ()
    {
      
    }
}
