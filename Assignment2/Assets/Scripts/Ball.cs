using System;
using UnityEngine;

// class to represent the cannon ball
public class Ball
{
    public GameObject instantiatedBall; //the object to visually represent the cannon ball
    public float time;    //keep tracks of how long the ball has existed
    public float muzzleVelo;    //the velocity at which we fired that ball
    public bool endBounce;  //is there enough force to bounce ? True if not possible to bounce
    public bool inWater;    //landed in water
    public Vector3 posCollision= new Vector3(0,0,0);    //where did the collision happen
    public Vector3 velocity;        //the actual velocity that takes in account the angle at which we fired
    float cos;  //easier to save cos and sin to deal with collision new angles later
    float sin;

    public Ball(GameObject b, float _angle, float movSpeed)
    {
        this.instantiatedBall = b;
        this.cos = Mathf.Cos(_angle * (Mathf.PI / 180.0f)); //radians value of the angle
        this.sin = Mathf.Sin(_angle * (Mathf.PI / 180.0f));
        this.time = 1f;
        this.muzzleVelo = movSpeed; //corresponds to the main velocity we can modify
        this.endBounce = false;
        this.velocity = new Vector3(this.muzzleVelo * this.cos, this.muzzleVelo * this.sin, 0);
    }

    //deals with all the possible objects a cannonball can collide with
    public void collide()
    {
        if (!this.endBounce) //check the ball is not stationary
        {
            // x and  correspond to the coordinates of the ball
            float x = this.instantiatedBall.transform.position.x;
            float y = this.instantiatedBall.transform.position.y;

           //collide with ground or water
            if (y <= main.groundPos.y)
            {
                //collide with ground
                bool sameColli = Mathf.Abs(x - posCollision.x) <= 0.1f //same collision is detected more than once sometimes
                                && (y > main.groundPos.y-0.2f) ;   //avoid mistaking same collision with velocity too low
          
                if ( !sameColli && Mathf.Abs(x - posCollision.x) <1f) this.endBounce = true;    //if 2 different collisions occured too close then stop bouncing

                this.posCollision = this.instantiatedBall.transform.position;   
                this.muzzleVelo /= 1.15f;   //reduce velocity of the ball since lost of energy when collision
                this.time = 1f;
                this.velocity = new Vector3(this.muzzleVelo *this.cos, this.muzzleVelo *this.sin, 0); //recompute velocity still taking in account the initial angle we fired with

                //collide with water
                if ((x <= main.outline.getPosition(main.outline.getRiverRenderer().positionCount - 1, false).x)
                     && (x >= main.outline.getPosition(0, false).x))
                {
                    this.endBounce = true;
                    this.inWater = true;
                }
            }

            //collide with mountains
            // use the equation of the line representing the side of a mountain to know if the ball collides with a mountain
            ////mountain1 is btw points 20 and 60
            if (x >= main.outline.getPosition(20, true).x &&    
                x <= main.outline.getPosition(40, true).x &&
                y <= main.outline.equationSideMountain(x, true, true)) //m1 side asc
            {
                bool sameColli = Mathf.Abs(x - posCollision.x) <= 0.2f ; //sometimes it collides more than once with mountain
                
                this.posCollision = this.instantiatedBall.transform.position;
                if (!sameColli)
                {
                    this.bounceHelper();
                }
            }
            else if (x >= main.outline.getPosition(40, true).x &&
               x <= main.outline.getPosition(60, true).x &&
                y <= main.outline.equationSideMountain(x, false, true)) //m1 side desc
            {
                bool sameColli = Mathf.Abs(x - posCollision.x) <= 0.2f ; //sometimes it collides more than once with mountain
                this.posCollision = this.instantiatedBall.transform.position;
                if (!sameColli)
                {
                    this.bounceHelper();
                }
            }
            //mountain2 is between points 120-160
            else if (x >= main.outline.getPosition(120, true).x &&
               x <= main.outline.getPosition(140, true).x &&
               y <= main.outline.equationSideMountain(x, true, false)) //m2 side asc
            {
                bool sameColli = Mathf.Abs(x - posCollision.x) <= 0.2f ; //sometimes it collides more than once with mountain
                this.posCollision = this.instantiatedBall.transform.position;
                if (!sameColli)
                {
                    this.bounceHelper();
                }
            }
            else if (x >= main.outline.getPosition(140, true).x &&
               x <= main.outline.getPosition(160, true).x &&
               y <= main.outline.equationSideMountain(x, false, false)) //m2 side desc
            {
                bool sameColli = Mathf.Abs(x - posCollision.x) <= 0.2f; //sometimes it collides more than once with mountain
                this.posCollision = this.instantiatedBall.transform.position;
                if (!sameColli)
                {
                    this.bounceHelper();
                }
            }


            //collide with balloons
            foreach (Balloon balloon in PhysicsManager.balloons)
            {
                if(x >= balloon.centerPos.x-balloon.width && x<= balloon.centerPos.x+balloon.width
                    && y>=balloon.centerPos.y-balloon.height/2 && y<=balloon.centerPos.y+balloon.height/2)
                {
                    balloon.bursted = true;
                }
            }

        }

    }

    //if collision with a mountain then we need to change the angle at which we fired since it will bounce in the opposite way
    private void bounceHelper()
    {
        this.muzzleVelo /= 1.3f;    //loss of energy is more important when hitting a mountain
        this.time = 1f;
        this.cos = -this.cos;
        this.velocity = new Vector3(this.muzzleVelo * this.cos,
                                this.muzzleVelo * this.sin, 0);
    }

}
