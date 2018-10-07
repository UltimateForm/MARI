using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour {

    Rigidbody rb;
    public float Speed = 10;
    public Vector3 Heading;
    public GameCamera CameraMech;

	void Start () {
        rb = GetComponent<Rigidbody>();
	}

    public void FixedUpdate()
    {
        if (!Rollin.Main.State.HasFlag(GameStates.Playing) || Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }
        float moveSideWays = Input.GetAxis("Side");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveSideWays, 0.0f, moveVertical);
        if (CameraMech)
        {
            Heading = CameraMech.transform.rotation.eulerAngles;
            var headedMovement = Quaternion.Euler(Heading) * movement;
            rb.AddForce(headedMovement * Speed * Time.deltaTime);
        }
        else rb.AddForce(movement * Speed, ForceMode.Impulse);
        if (transform.position.y <= -2f)
        {
            Rollin.Main.GameOver();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!Rollin.Main.State.HasFlag(GameStates.Playing)) return;
        if (collision.gameObject.tag == "gameover")
        {
            Rollin.Main.GameOver();
        }
        else if(collision.gameObject.tag == "victory")
        {
            Rollin.Main.Victory();
        }
    }
}
