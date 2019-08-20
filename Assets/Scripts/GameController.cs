using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // public Text speedText;
    // public Text distText;
    // public Text hintText;
    // public Text windText;
    // public Text gateText;
    // public Text gateBtnText;
    // public Text recordText;
    // public Text paramText;
    // public Slider gateSlider;
    // public Slider debugSlider;
    // public Slider windSlider;
    // public InputField smoothInput;
    // public InputField sensInput;
    // public InputField rotInput;
    // public Toggle modeToggle;
    // public GameObject options;
    public GameObject jumperObject;
    public GameObject gateObject;
    private JumperController jumperController;
    public CamerasController camerasController;

    public MeshScript meshScript;
    // public GateController gateController;

    void Start()
    {
        jumperController = jumperObject.GetComponent<JumperController>();
        // speedText.text = "";
        // distText.text = "";
        // hintText.text = "Press 'Space' to start the jump";
        // windText.text = "";
        // gateText.text = "";
        // gateBtnText.text = "";

        // if (PlayerPrefs.HasKey("wind"))
        // {
        //     windSlider.value = PlayerPrefs.GetFloat("wind");
        //     jumperController.windForce = PlayerPrefs.GetFloat("wind");
        // }

        // if (PlayerPrefs.HasKey("mode"))
        // {
        //     modeToggle.isOn = jumperController.oldMode = (PlayerPrefs.GetInt("mode") == 1 ? true : false);
        // }
        // if (PlayerPrefs.HasKey("smooth"))
        // {
        //     jumperController.smoothCoef = PlayerPrefs.GetFloat("smooth");
        //     smoothInput.text = "" + jumperController.smoothCoef;
        // }
        // if (PlayerPrefs.HasKey("rot"))
        // {
        //     jumperController.rotCoef = PlayerPrefs.GetFloat("rot");
        //     rotInput.text = "" + jumperController.rotCoef;
        // }

        // if (PlayerPrefs.HasKey("sensitivity"))
        // {
        //     jumperController.sensCoef = PlayerPrefs.GetFloat("sensitivity");
        //     sensInput.text = "" + jumperController.sensCoef;
        // }

        // if (PlayerPrefs.HasKey("record"))
        // {
        //     int tmp = PlayerPrefs.GetInt("record");
        //     recordText.text = "Record: " + (tmp / 2) + "." + (tmp % 2) * 5 + " m"; ;
        // }


        if (!PlayerPrefs.HasKey("camera"))
        {
            camerasController.EnableCamera(0);
        }
        else
        {
            camerasController.EnableCamera(PlayerPrefs.GetInt("camera"));
        }

        // if (PlayerPrefs.HasKey("lastGate"))
        // {
        //     gateSlider.value = PlayerPrefs.GetInt("lastGate");
        // }
        // GateSliderChange();
        // GateButtonClick();
        // WindSlider();
        // ChangeControls();
    }

    void Update()
    {
        // for(int i = 0; i < 12; i++)
        // {
        //     if (Input.GetKeyDown(KeyCode.F1 + i))
        //     {
        //         camerasController.EnableCamera(i);
        //     }
        // }

        if(Input.GetKeyDown(KeyCode.F1)) camerasController.EnableCamera(0);
        else if(Input.GetKeyDown(KeyCode.F2)) camerasController.EnableCamera(1);
        else if(Input.GetKeyDown(KeyCode.F3)) camerasController.EnableCamera(2);
        else if(Input.GetKeyDown(KeyCode.F4)) camerasController.EnableCamera(3);
        else if(Input.GetKeyDown(KeyCode.F5)) camerasController.EnableCamera(4);
        else if(Input.GetKeyDown(KeyCode.F6)) camerasController.EnableCamera(5);
        else if(Input.GetKeyDown(KeyCode.F7)) camerasController.EnableCamera(6);

        if (Input.GetKeyDown(KeyCode.R))
        {
            jumperController.ResetValues();
            jumperObject.GetComponent<Transform>().position = meshScript.jumperPosition;
            gateObject.GetComponent<Transform>().position = meshScript.jumperPosition;
            Debug.Log(meshScript.jumperRotation);
            jumperObject.GetComponent<Transform>().rotation = meshScript.jumperRotation;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
        // paramText.text = "angle: " + (float)jumperController.angle + "\njumperAngle: " + jumperController.jumperAngle;

        // int val = (int)(jumperController.windForce * 10);
        // bool sign = (val < 0);
        // if (sign)
        // {
        //     val *= -1;
        // }

        // //windText.text = "<==" + (sign ? "-" : "") + (val / 10) + "." + (val % 10) + " m/s";
        // //float windAngle = Vector2.Angle(Vector2.up, jumperController.windDir);
        // //windText.transform.Rotate(0.0f, 0.0f, windAngle - windText.transform.rotation.eulerAngles.z);
        // windText.text = (sign ? "-" : "") + (val / 10) + "." + (val % 10) + " m/s";

        // //Debug.Log(windAngle + " " +  windText.transform.rotation.eulerAngles.z);
        // float speed = jumperObject.GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
        // int tmp = (int)(jumperController.Distance * 2);

        // //Debug.Log(tmp);
        // if (jumperController.OnInrun == true)
        // {
        //     speedText.text = "Speed: " + (int)(speed * 10) / 10 + "." + (int)(speed * 10) % 10 + " km/h";
        // }

        // if ((int)(jumperController.Distance * 2) > PlayerPrefs.GetInt("record"))
        // {
        //     PlayerPrefs.SetInt("record", tmp);
        // }
        // distText.text = "Distance: " + (tmp / 2) + "." + (tmp % 2) * 5 + " m";
        // if (jumperController.State == 1) hintText.text = "Press 'R' to restart\nPress 'Space' to take-off";
        // else if (jumperController.State == 1) hintText.text = "Press 'R' to restart\nPress 'Space' to take-off";
        // else if (jumperController.State == 2) hintText.text = "Press 'R' to restart\nSteer with 'Up' and 'Down' arrows \nPress 'Space' to land";

        // debugSlider.value = jumperController.jumperAngle;

    }

    // public void GateSliderChange()
    // {
    //     gateBtnText.text = "Set gate to: " + gateSlider.value;
    // }

    // public void GateButtonClick()
    // {
    //     if (jumperController.State == 0)
    //     {
    //         gateController.GateSet((int)gateSlider.value);
    //         gateText.text = "Gate: " + gateController.currentGate;
    //         PlayerPrefs.SetInt("lastGate", gateController.currentGate);
    //     }

    // }

    // public void WindSlider()
    // {
    //     jumperController.windForce = windSlider.value;
    //     PlayerPrefs.SetFloat("wind", jumperController.windForce);
    // }

    // public void ChangeControls()
    // {
    //     jumperController.oldMode = modeToggle.isOn;
    //     PlayerPrefs.SetInt("mode", (jumperController.oldMode ? 1 : 0));
    //     options.SetActive(!jumperController.oldMode);
    // }

    // public void ChangeCoeffs()
    // {
    //     PlayerPrefs.SetFloat("smooth", float.Parse(smoothInput.text));
    //     PlayerPrefs.SetFloat("sensitivity", float.Parse(sensInput.text));
    //     PlayerPrefs.SetFloat("rot", float.Parse(rotInput.text));
    //     jumperController.smoothCoef = PlayerPrefs.GetFloat("smooth");
    //     jumperController.sensCoef = PlayerPrefs.GetFloat("sensitivity");
    //     jumperController.rotCoef = PlayerPrefs.GetFloat("rot");
    // }


}
