using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UITimer : MonoBehaviour
{
    //Reference to our Text component
    private Text ourText;

    //Float to hold our time in miliseconds
    private float millisec = 0;
    //Int to hold our time in seconds
    private int sec = 0;
    //Int to hold our time in minutes
    private int min = 0;



	// Use this for initialization
	private void Awake ()
    {
        //Getting the reference to our text component
        this.ourText = this.GetComponent<Text>();
	}

	
	// Update is called once per frame
	private void Update ()
    {
        //Adding the amount of time passed to our time in miliseconds
        this.millisec += Time.deltaTime;

        //If our time in milliseconds gets above 1 second, we move it over to our "seconds" variable
        if(this.millisec >= 1)
        {
            this.millisec -= 1;
            this.sec += 1;

            //If our time in seconds gets above 60, we move it over to our "minutes" variable
            if(this.sec > 59)
            {
                this.sec -= 60;
                this.min += 1;
            }
        }

        //Making a string to hold our current time in milliseconds
        string milString = "";
        //If our milliseconds are in the single digits, we need to tack on a 0
        int roundedMillisec = Mathf.RoundToInt(this.millisec * 100);
        if(roundedMillisec < 10)
        {
            milString = "0" + roundedMillisec;
        }
        //If our milliseconds are rounded up to 100, we put the time at 0
        else if(roundedMillisec == 100)
        {
            milString = "00";
        }
        //If our milliseconds are in the double digits
        else
        {
            milString = "" + roundedMillisec;
        }

        //Making a string to hold our current time in seconds
        string secString = "";
        //If our seconds are in the single digits, we need to tack on a 0
        if(this.sec < 10)
        {
            secString = "0" + this.sec;
        }
        //If our seconds are in the double digits
        else
        {
            secString = "" + this.sec;
        }

        //Setting our text string to show all of our times
        this.ourText.text = "" + this.min + ":" + secString + ":" + milString;
	}
}
