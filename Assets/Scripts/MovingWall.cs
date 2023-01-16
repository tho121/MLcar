using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : MonoBehaviour
{
    public bool isMovingRight = true;
    public float moveSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        if(Random.value < 0.5f)
		{
            isMovingRight = false;
		}
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dir = isMovingRight ? 1.0f : -1.0f;
        transform.localPosition += transform.right * dir * Time.fixedDeltaTime * moveSpeed;
    }

	private void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("Wall"))
        {
            isMovingRight = !isMovingRight;
        }
    }
}
