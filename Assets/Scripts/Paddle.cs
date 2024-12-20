using System.Collections;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    #region Singleton
    private static Paddle _instance;
    public static Paddle Instance => _instance;
    public bool PaddleIsTransforming { get; set; }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    private Camera mainCamera;
    private float paddleInitialY;
    private SpriteRenderer sr;
    //private BoxCollider2D boxCol;



    private void Start()
    {
        mainCamera = Camera.main;
        paddleInitialY = this.transform.position.y;
        sr = GetComponent<SpriteRenderer>();
        //boxCol = GetComponent<BoxCollider2D>();

        // Debugging: Log paddle details
        Debug.Log("Paddle initialized.");
    }

    private void Update()
    {
        PaddleMovement();
    }


    private void PaddleMovement()
    {
        // Define fixed left and right clamps
        float leftClampWorld = -6.7f;
        float rightClampWorld = 6.7f;

        // Get mouse position in world space
        float mousePositionWorldX = mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, 0, mainCamera.nearClipPlane)
        ).x;

        // Clamp paddle position in world space
        float clampedX = Mathf.Clamp(mousePositionWorldX, leftClampWorld, rightClampWorld);

        // Debugging: Log the clamped paddle position
        Debug.Log($"Clamped Paddle X Position: {clampedX}");

        this.transform.position = new Vector3(clampedX, paddleInitialY, 0);
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Ball")
        {
            Rigidbody2D ballRb = coll.gameObject.GetComponent<Rigidbody2D>();
            Vector3 hitPoint = coll.contacts[0].point;
            Vector3 paddleCenter = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y);

            ballRb.linearVelocity = Vector2.zero; // Reset ball velocity

            float difference = paddleCenter.x - hitPoint.x;

            if (hitPoint.x < paddleCenter.x)
            {
                ballRb.AddForce(new Vector2(-(Mathf.Abs(difference * 200)), BallsManager.Instance.initialBallSpeed));
            }
            else
            {
                ballRb.AddForce(new Vector2((Mathf.Abs(difference * 200)), BallsManager.Instance.initialBallSpeed));
            }

            // Debugging: Log collision info
            Debug.Log($"Ball Hit Point: {hitPoint}, Paddle Center: {paddleCenter}, Difference: {difference}");
        }

    }

    public void ResetPosition()
    {
        transform.position = new Vector3(0, transform.position.y, 0); // Center the paddle
    }


    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || mainCamera == null) return;

        float leftClampWorld = -6.7f;
        float rightClampWorld = 6.7f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftClampWorld, -5, 0), new Vector3(leftClampWorld, 5, 0));
        Gizmos.DrawLine(new Vector3(rightClampWorld, -5, 0), new Vector3(rightClampWorld, 5, 0));
    }
}


//this.transform.position;
