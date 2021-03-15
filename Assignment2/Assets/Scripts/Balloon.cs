using UnityEngine;

public class Balloon
{
    public GameObject balloonBody;
    public GameObject balloonStr;
    public LineRenderer bodyRdr;        //easier to store the lineRenderers also as they will be accessed a lot
    public LineRenderer stringRdr;
    public Vector3 centerPos;
    public float height;
    public float width;
    public bool bursted;    //did it encounter a cannonball

    private static float riseFactor = 10f;  //how fast do the balloons go up

    public Balloon(GameObject body, GameObject str)
    {
        bursted = false;
        balloonBody = body;
        balloonStr = str;
        LineRenderer _bodyRdr = balloonBody.GetComponent<LineRenderer>();
        LineRenderer _stringRdr = balloonStr.GetComponent<LineRenderer>();
        this.height = 3;
        this.width = 2;

        this.bodyRdr = _bodyRdr;
        this.stringRdr = _stringRdr;

        bodyRdr.startColor = Color.red; bodyRdr.endColor = Color.red;
        stringRdr.startColor = Color.black; stringRdr.endColor = Color.black;

        bodyRdr.positionCount = 7;
        stringRdr.positionCount = 5;

        Vector3 position = generateRandomInitPos(); //initial pos must be random in the water
        //create point 0 and the other positions will follow naturally
        bodyRdr.SetPosition(0, position);
        bodyRdr.SetPosition(1, new Vector3(position.x + this.width/2, position.y - this.height/4, 0));
        bodyRdr.SetPosition(2, new Vector3(position.x + this.width / 2, position.y - this.height*3/4, 0));
        bodyRdr.SetPosition(3, new Vector3(position.x, position.y - this.height, 0));
        bodyRdr.SetPosition(4, new Vector3(position.x - this.width / 2, position.y - this.height*3/4, 0));
        bodyRdr.SetPosition(5, new Vector3(position.x - this.width / 2, position.y - this.height/4, 0));
        bodyRdr.SetPosition(6, position);

        float r = 0;
        for (int i = 0; i < 5; i++)
        {
            stringRdr.SetPosition(i, new Vector3(position.x + r, position.y-this.height - i*0.5f, 0));
            r = (Random.value*2-1)/10;  //string movement is a bit random due to movement
        }

        this.centerPos = new Vector3(position.x, position.y - height / 2,0);
    }

    private Vector3 generateRandomInitPos()
    {
        //y between riverLine and flat line under water + size balloon
        float yCoord = Random.value * ( main.outline.getPosition(0,false ).y - main.outline.getPosition(90, true).y ) + main.outline.getPosition(90, true).y +this.height;
        //x between the 2 mountains
        float xCoord = Random.value * (main.outline.getPosition(100, true).x - main.outline.getPosition(80, true).x) + main.outline.getPosition(80, true).x;

        return new Vector3(xCoord, yCoord,0) ;
    }

    //balloon goes up 
    public void rise()
    {
        float windForce = 0;
        //if higher than mountains then wind impacts
        if (this.centerPos.y > main.outline.getPosition(40, true).y) windForce = main.wind * Time.deltaTime;

        this.centerPos.y += riseFactor*Time.deltaTime;

        Vector3[] posBody = new Vector3[this.bodyRdr.positionCount];
        bodyRdr.GetPositions(posBody);
        for (int i= 0; i< bodyRdr.positionCount; i++)
        {
            bodyRdr.SetPosition(i, posBody[i] + new Vector3(windForce, riseFactor*Time.deltaTime , 0));
        }
        Vector3[] posStr = new Vector3[this.stringRdr.positionCount];
        stringRdr.GetPositions(posStr);
        float r = 0;
        for (int i = 0; i < stringRdr.positionCount; i++)
        {
            if(i!=0) r = (Random.value * 2 - 1)/20;
            stringRdr.SetPosition(i, posStr[i] + new Vector3(windForce + r, riseFactor*Time.deltaTime, 0));
           
        }

    }

   

}
