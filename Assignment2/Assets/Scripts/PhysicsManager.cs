
using System.Collections.Generic;

using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    private List<Ball> balls = new List<Ball>();   //keeps track of the cannonballs
    private List<Ball> ballsToDelete = new List<Ball>();   //cannonballs to delete 
   
    private static float gravity = 9.8f;

    public static List<Balloon> balloons = new List<Balloon>(); //keeps track of the existing balloons
    private static List<Balloon> balloonsToDelete = new List<Balloon>();

    private void Start()
    {
        //create balloons every 2s
        InvokeRepeating("createBalloon", 1.0f, 2.0f);
    }

    //modify the angle of the current cannon with the up and down arrow
    public void changeInclinaison(LineRenderer cannonRdr, bool isCannon1)
    {
        //get position cannon before pressing key
        Vector3[] cannonPos = new Vector3[cannonRdr.positionCount];
        cannonRdr.GetPositions(cannonPos);

        float angle = Vector3.Angle(new Vector3(1, 0, 0), cannonPos[1] - main.groundPos); //angle between cannon and ground

        //cannons cannot go further than a 90 degrees angle nor in the ground
            if ( (Input.GetKey("up") &&  angle<89 && isCannon1) || (Input.GetKey("down") && angle < 170 && !isCannon1) )
            {
                //dont modify cannon length : each point possible is on a circle
                float radius = Mathf.Sqrt(Mathf.Pow(cannonPos[1][0] - cannonPos[0][0], 2) + Mathf.Pow(cannonPos[1][1] - cannonPos[0][1], 2)); //r=sqrt((x-h)^2+(y-k)^2)
                float changeX = cannonPos[1][0] - 0.1f;
                float changeY = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(changeX - cannonPos[0][0], 2)) + cannonPos[0][1]; // y'= sqrt(r^2 - (x'-h)^2 )+k where center=(h,k)

                Vector3 changeAngle = new Vector3(changeX, changeY, 0);
                cannonRdr.SetPosition(1, changeAngle);
            }

            if ( (Input.GetKey("down") && angle > 15f && isCannon1) || (Input.GetKey("up") && angle > 90 && !isCannon1)) 
            {
                float radius = Mathf.Sqrt(Mathf.Pow(cannonPos[1][0] - cannonPos[0][0], 2) + Mathf.Pow(cannonPos[1][1] - cannonPos[0][1], 2)); //r=sqrt((x-h)^2+(y-k)^2)
                float changeX = cannonPos[1][0] + 0.1f;
                float changeY = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(changeX - cannonPos[0][0], 2)) + cannonPos[0][1]; // y'= sqrt(r^2 - (x'-h)^2 )+k where center=(h,k)

                Vector3 changeAngle = new Vector3(changeX, changeY, 0);
                cannonRdr.SetPosition(1, changeAngle);
            }

    }

    //initialize the ball to fire
    private void createBall()
    {
        GameObject ball = Instantiate(Resources.Load("Point", typeof(GameObject))) as GameObject;
        SpriteRenderer ballRdr = ball.GetComponent<SpriteRenderer>();
        ballRdr.color = Color.black;
        ball.transform.localScale = new Vector3(10, 10, 10);

        //define initial position the ball appears at : on a bigger circle than the one that defines the cannon
        Vector3[] cannonPos = new Vector3[main.currentCannon.positionCount];
        main.currentCannon.GetPositions(cannonPos);
        float radius = (Mathf.Sqrt(Mathf.Pow(cannonPos[1][0] - cannonPos[0][0], 2) + Mathf.Pow(cannonPos[1][1] - cannonPos[0][1], 2))) + 1f; // circle around the cannon
        float x = cannonPos[1][0] + 1f;
        if (!main.isCannon1) x = cannonPos[1][0] - 1f;     //if cannon2 then shoot the other way
        float y = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(x - cannonPos[0][0], 2)) + cannonPos[0][1];

        Vector3 init = new Vector3(x, y, 0);
        ball.transform.position = init;

        float angle = Vector3.Angle(new Vector3(1, 0, 0), cannonPos[1] - main.groundPos); //angle between cannon and ground

        Ball b = new Ball(ball, angle, main.muzzleVelocity);
        balls.Add(b);

    }

    //instantiate balloon every 2s
    public void createBalloon()
    {
        GameObject body = Instantiate(Resources.Load("Line", typeof(GameObject))) as GameObject;
        GameObject balloonString = Instantiate(Resources.Load("Line", typeof(GameObject))) as GameObject;
        
        Balloon balloon = new Balloon(body, balloonString);
        balloons.Add(balloon);
    }


    private void Update()
    {
        //change velocity
        if (Input.GetKey(KeyCode.RightArrow) && main.muzzleVelocity<=100)
        {
            main.muzzleVelocity++;
            main.velocityTxt.text = "Muzzle velocity : " + main.muzzleVelocity.ToString();
        }
        if (Input.GetKey(KeyCode.LeftArrow) && main.muzzleVelocity>=10)
        {
            main.muzzleVelocity--;
            main.velocityTxt.text = "Muzzle velocity : " + main.muzzleVelocity.ToString();
        }


        //shoot
        if (Input.GetKeyDown(KeyCode.Space))
        {
            createBall();
        }
        foreach(Ball b in balls)
        {
            b.collide();    //does the cannonball collide with anything
            if (!b.endBounce) b.instantiatedBall.transform.position = trajectory(b);      //add force to each cannon ball
            deleteBall(b);
        }
        helperClearDeletedBalls();

        //change barrel elevation with arrow up and down
        changeInclinaison(main.currentCannon, main.isCannon1);

        //balloons movement
        foreach(Balloon balloon in balloons)
        {
            balloon.rise();
            deleteBalloon(balloon);
        }
        helperClearDeletedBalloons();

    }

    private Vector3 trajectory(Ball b)
    {
        Vector3 pos = b.instantiatedBall.transform.position;
        //the trajectory is a parabola defined by the ball velocity (muzzlevelo and aangle)
        //as time goes by the gravity effect will be more felt 
        float x_next = pos.x + b.velocity.x * Time.deltaTime ;
        float y_next = pos.y + b.velocity.y * Time.deltaTime - gravity * Mathf.Pow(b.time, 2)* Time.deltaTime;
        b.time += 0.008f;   //I implement my own time when the ball is fired

        return new Vector3(x_next, y_next , 0);
    }

   
    //if ball is out of the frame or lands in water or stops moving for a prolonged amount of time
    private void deleteBall(Ball b)
    {
        bool outOfFrame = (Mathf.Abs(b.instantiatedBall.transform.position.x) > createOutline.frameWidth);

        if ( outOfFrame || b.inWater )
        {
            ballsToDelete.Add(b);
            Destroy(b.instantiatedBall);
            return;
        }

        //stationary
        if (b.endBounce)
        {
            b.time += 0.01f;
            if (b.time > 5)
            {
                ballsToDelete.Add(b);
                Destroy(b.instantiatedBall);
                return;
            }
        }
    }

    private void helperClearDeletedBalls()
    {
        //remove deleted balls from list
        foreach (Ball bd in ballsToDelete)
        {
            balls.Remove(bd);
        }
        ballsToDelete.Clear();

    }

    //ballon is out of frame or got hit by a cannon
    private void deleteBalloon(Balloon b)
    {
        //out of frame OR encountered a cannon
        if (Mathf.Abs(b.centerPos.y) > createOutline.frameHeight || Mathf.Abs(b.centerPos.x) > createOutline.frameWidth
            || b.bursted)
        {
            balloonsToDelete.Add(b);
            Destroy(b.balloonBody);
            Destroy(b.balloonStr);
        }
    }

    private void helperClearDeletedBalloons()
    {
        //remove deleted balloons from list
        foreach (Balloon bd in balloonsToDelete)
        {
            balloons.Remove(bd);
        }
        balloonsToDelete.Clear();

    }

}

