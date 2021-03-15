
using UnityEngine;

public class cannonsOutline : MonoBehaviour
{
    private GameObject cannon1;
    private GameObject cannon2;

    // Start is called before the first frame update
    public void createCannons(createOutline co)
    {
        //we need to position the cannons above the ground 
        LineRenderer groundRdr = co.getGroundRenderer();
        Vector3[] groundPos = new Vector3[groundRdr.positionCount];
        groundRdr.GetPositions(groundPos);

        cannon1= Instantiate(Resources.Load("Line", typeof(GameObject))) as GameObject;
        LineRenderer c1rdr = cannon1.GetComponent<LineRenderer>();
        cannon2 = Instantiate(Resources.Load("Line", typeof(GameObject))) as GameObject;
        LineRenderer c2rdr = cannon2.GetComponent<LineRenderer>();
        //cannons are black
        c1rdr.startColor = Color.black; c1rdr.endColor = Color.black;
        c2rdr.startColor = Color.black; c2rdr.endColor = Color.black;
        //cannons form
        c1rdr.startWidth = 2; c1rdr.endWidth = 2;
        c2rdr.startWidth = 2; c2rdr.endWidth = 2;

        //set position cannons : c1 in middle of first flat line and c2 in beginning last flat line
        c1rdr.SetPosition(0, new Vector3(groundPos[10][0] , groundPos[10][1]+0.2f, 0));
        c1rdr.SetPosition(1, new Vector3(groundPos[10][0] + 2, groundPos[10][1]+5, 0)); //inclinaison of the cannon
        c2rdr.SetPosition(0, new Vector3(groundPos[167][0], groundPos[167][1]+0.2f , 0));
        c2rdr.SetPosition(1, new Vector3(groundPos[167][0] -2 , groundPos[167][1] +5, 0)); //inclinaison of the cannon


    }

    public LineRenderer getCannon1Renderer()
    {
        return cannon1.GetComponent<LineRenderer>();
    }

    public LineRenderer getCannon2Renderer()
    {
        return cannon2.GetComponent<LineRenderer>();
    }

}
