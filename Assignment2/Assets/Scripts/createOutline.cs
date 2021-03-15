
using UnityEngine;

public class createOutline : MonoBehaviour
{
    public Camera cam;

    private GameObject groundLine;            // Lines we will instantiate.
    private GameObject riverLine;

    public static float frameWidth;
    public static float frameHeight;



    public LineRenderer getRiverRenderer()
    {
        return riverLine.GetComponent<LineRenderer>();
    }

   public LineRenderer getGroundRenderer()
    {
        return groundLine.GetComponent<LineRenderer>();
    }

    public Vector3 getPosition(int x, bool forGround)   //return vector position of the point x . If forGround is false then position for river
    {
        Vector3[] pos;
        if (forGround) {
            pos = new Vector3[groundLine.GetComponent<LineRenderer>().positionCount];
            groundLine.GetComponent<LineRenderer>().GetPositions(pos);
        }
        else {
            pos = new Vector3[riverLine.GetComponent<LineRenderer>().positionCount];
            riverLine.GetComponent<LineRenderer>().GetPositions(pos);
        }
       
        return pos[x];
    }

    //computes f(x) where f is the equation of a side of a mountain
    public float equationSideMountain(float x, bool isAscSide, bool isMountain1)
    {
        //descending side mounain 2
        Vector3 a = getPosition(140, true);
        Vector3 b = getPosition(160, true);
        if (isAscSide && isMountain1)       //ascending side m1
        {
             a = getPosition(20, true);
             b = getPosition(40, true);
        }
        else if (!isAscSide && isMountain1)  //desc side m1
        {
            a = getPosition(40, true);
            b = getPosition(60, true);
        }
        else if (isAscSide && !isMountain1)  //asc side m2
        {
            a = getPosition(120, true);
            b = getPosition(140, true);
        }

        float slope = (b.y - a.y) / (b.x - a.x);
        return slope * x + (b.y - slope * b.x);
    }

    // Start is called before the first frame update
    public void createTerrain()
    {
        Color groundColor = Color.black;
        Color riverColor = Color.blue;

        groundLine = Instantiate(Resources.Load("Line", typeof(GameObject))) as GameObject;
        riverLine = Instantiate(Resources.Load("Line", typeof(GameObject))) as GameObject;

        LineRenderer groundRdr = groundLine.GetComponent<LineRenderer>();
        groundRdr.positionCount = 181;
        LineRenderer riverRdr = riverLine.GetComponent<LineRenderer>();
        riverRdr.positionCount = 99;

        groundRdr.startColor = groundColor; groundRdr.endColor = groundColor;
        riverRdr.startColor = riverColor; riverRdr.endColor = riverColor;


        frameWidth = 2f*cam.orthographicSize;
        frameHeight = cam.orthographicSize;
        float startPosX = -frameHeight * cam.aspect -10;
        float endPosX = -startPosX +10;
        float groundPosY = -frameHeight/2;
        float mountainHeight = groundPosY + 0.8f*frameHeight ;
        float mountainWidth = frameWidth/5;
        float mountain1Start = -frameWidth/3 ;
        float mountain1end = mountain1Start + mountainWidth;
        float mountain2Start = frameWidth/3;
        float mountain2end = mountain2Start + mountainWidth;

      
        //set the random ground outline 
        float x = startPosX; float y = groundPosY;              //first flat line
        float nextX = mountain1Start; float nextY = groundPosY;
        int dizaine = 20; int factor = 0;
        for(int i=0; i<groundRdr.positionCount; i++)
        {
            if (i == dizaine)
            {
                switch(dizaine){ 
                    case 20 :           //first side mountain1
                        x = mountain1Start; y = groundPosY;
                        nextX = mountain1Start + mountainWidth / 2; nextY = mountainHeight;
                        break;
                    case 40:            // second side mountain1
                        x = mountain1Start + mountainWidth / 2; y = mountainHeight;
                        nextX = mountain1end; nextY = groundPosY;
                        break;
                    case 60:            //second side mountain1 under water
                        x = mountain1end; y = groundPosY;
                        nextX = mountain1end + mountainWidth / 5; nextY = groundPosY - (mountainHeight * 0.8f);
                        break;
                    case 80:            //flat line under water
                        x = mountain1end + mountainWidth / 5; y = groundPosY - (mountainHeight * 0.8f);
                        nextX = mountain2Start - mountainWidth / 5; nextY = groundPosY - (mountainHeight * 0.8f);
                        break;
                    case 100:           //first side mountain2 under water
                        x = mountain2Start - mountainWidth / 5; y = groundPosY - (mountainHeight * 0.8f);
                        nextX = mountain2Start; nextY = groundPosY;
                        break;
                    case 120:           //first side mountain2
                        x = mountain2Start; y = groundPosY;
                        nextX = mountain2Start + mountainWidth / 2; nextY = mountainHeight;
                        break;
                    case 140:           //second side mountain2
                        x = mountain2Start + mountainWidth / 2; y = mountainHeight;
                        nextX = mountain2end; nextY = groundPosY;
                        break;
                    case 160:           // last flat line
                        x = mountain2end; y = groundPosY;
                        nextX = endPosX; nextY = groundPosY;
                        break;
                    case 180:           //end
                        x = endPosX; y = groundPosY;
                        nextX = endPosX; nextY = groundPosY;
                        break;
                }
                dizaine += 20;
                factor = 0;
            }
            groundRdr.SetPosition(i, new Vector3(x + (nextX-x)*factor/20 , y + (nextY-y)*factor/20 + Random.value, 0));
            factor++;
        }

        //set the random river outline
       for(int i=0; i<riverRdr.positionCount; i++)
        {
            riverRdr.SetPosition(i, new Vector3(0.2f + mountain1end + (mountain2Start - mountain1end)*i / riverRdr.positionCount ,groundPosY + Random.value, 0));  
        }

        smoothLine(riverRdr);
        smoothLine(groundRdr);

    }

    //interpolate so that there isn't a sharp angle : since our base outline is random at each point it imitates perlinNoise
    void smoothLine(LineRenderer rdr)
    {
        Vector3[] pos = new Vector3[rdr.positionCount];

        for (int x=1; x<rdr.positionCount-2; x++)
        {
            rdr.GetPositions(pos);

            float alpha = (1 - Mathf.Cos(Mathf.PI * Random.value)) / 2;
            float newY = alpha+ pos[x][1];

            if (pos[x - 1][1] == pos[x + 1][1]) Debug.Log("equal");

            if (pos[x - 1][1] != pos[x + 1][1])
            { //interpolate if not the same y coord
                 newY = ((1 - alpha) * pos[x - 1][1] + (alpha) * (pos[x + 1][1]));
            }

            rdr.SetPosition(x, new Vector3(pos[x][0], newY, 0));


        }

    }
    


}
