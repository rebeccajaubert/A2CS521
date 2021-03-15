
using UnityEngine;
using UnityEngine.UI;

public class main : MonoBehaviour
{
    public GameObject terrain;
    public GameObject cannons;

    public static Text velocityTxt;
    public static Text windTxt;

    public static createOutline outline;
    private cannonsOutline cannons_Outline;

    public static  LineRenderer currentCannon;
    public static Vector3 groundPos;
    public static bool isCannon1 = true;

    public static float wind =0f;
    //change muzzleVelocity with L and R arrows
    public static float muzzleVelocity = 50f;

    // Start is called before the first frame update
    void Start()
    {
        //create the terrain outline (different at each game)
        outline = terrain.GetComponent<createOutline>();
        outline.createTerrain();
        cannons_Outline = cannons.GetComponent<cannonsOutline>();
        cannons_Outline.createCannons(outline);

        currentCannon = cannons_Outline.getCannon1Renderer();
        groundPos = outline.getPosition(10,true) ; //cannon1 is at pt 10

        //call every 2s the wind method to generate new value
        InvokeRepeating("WindChanges", 1.0f, 2.0f);

        //display the initial values of wind and velocity
        velocityTxt = GameObject.Find("veloText").GetComponent<Text>();
        windTxt = GameObject.Find("windText").GetComponent<Text>();
        velocityTxt.text = "Muzzle velocity : " + muzzleVelocity.ToString();
        windTxt.text = "Wind  : " + wind.ToString();
    }

    
    private void Update()
    {
        //toggle between cannons
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isCannon1)
            {
                currentCannon = cannons_Outline.getCannon2Renderer();
                groundPos = outline.getPosition(167,true); //cannon2 is at pt 167
                isCannon1 = false;
            }
            else
            {
                currentCannon = cannons_Outline.getCannon1Renderer();
                groundPos = outline.getPosition(10,true);
                isCannon1 = true;
            }

        }
    }

    //wind changes randomly (in both directions) every 2s
    private void WindChanges()
    {
        float sign  = Random.value;
        if (sign > 0.5f) sign = 1;
        else { sign = -1; }
        wind = sign * Random.value * (10 - 1) + 1;
        windTxt.text = "Wind  : " + wind.ToString();
    }



}
