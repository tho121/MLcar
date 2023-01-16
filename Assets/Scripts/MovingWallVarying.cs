using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWallVarying : MonoBehaviour
{
    public bool isMovingRight = true;
    public float moveSpeedMax = 1.0f;
    public float moveSpeedMin = 0.5f;

    private float moveSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        if(Random.value < 0.5f)
		{
            isMovingRight = false;
		}

        moveSpeed = getNewSpeed();
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
            moveSpeed = getNewSpeed();
        }
    }

    private float getNewSpeed()
	{
        return (moveSpeedMax - moveSpeedMin) * Random.value + moveSpeedMin;

    }
}
