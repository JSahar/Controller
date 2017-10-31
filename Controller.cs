using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading;
using SocketIO;
using System;
using System.Text.RegularExpressions;

public class Controller : MonoBehaviour
{
    public Login login; 
    public SocketIOComponent socket;
    public forearmScript playGameObj;
    public forearmScript player; /// <summary>
                          /// I added this line for solving this command: player.OnCommandMove += OnCommandMove;
                          /// </summary>
                          /// 
    // Use this for initialization
    void Start() //OK
    {
        Controller ex = new Controller();
        ex.StartTimer(2000);

        StartCoroutine(ConnectToServer());
        socket.On("USER_CONNECT", OnUserConnected);
        socket.On("PLAY", OnUserPlay);
            socket.On("MOVE", OnUserMove);
            socket.On("USER_DISCONNECTED", OnUserDisconnected);
        player.gameObject.SetActive(true); 
        player.OnCommandMove += OnCommandMove;  // what is it for???
        login.Button.onClick.AddListener(OnClickPalyBtn);

        Debug.Log("Get the message from server in START");
            

    }
    void OnCommandMove(Vector3 vec3)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        Vector3 position = new Vector3(vec3.x, vec3.y, vec3.z);
        data["position"] = position.x + "," + position.y + "," + position.z;
        socket.Emit("MOVE", new JSONObject(data));
        Debug.Log("Get the message from server in OnCommandMove");

    }
    void OnUserMove(SocketIOEvent obj)//OK
    {   
        GameObject player = GameObject.Find(JsonToString(obj.data.GetField("name").ToString(), "\"")) as GameObject;
        player.transform.position = JsonToVector3(JsonToString(obj.data.GetField("position").ToString(), "\""));
        Debug.Log("Get the message from server in OnUserMove");
        


    }
    string JsonToString(string target, string s)//???
    {
        string[] newString = Regex.Split(target, s);
        return newString[1];
    }
    Vector3 JsonToVector3(string target)//?? Is it the position? why Icant see it on the server??
    {
        Vector3 newVector;
        string[] newString = Regex.Split(target, ",");
        newVector = new Vector3(float.Parse(newString[0]), float.Parse(newString[1]), float.Parse(newString[2]));
        return newVector;
    }

    void OnUserDisconnected(SocketIOEvent obj)
    {//destroy game obj
        Destroy(GameObject.Find(JsonToString(obj.data.GetField("name").ToString(), "\"")));

    }

    void OnClickPalyBtn()  //OK
    {
        
        player.gameObject.SetActive(true);
        login.gameObject.SetActive(false);
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["name"] = "ForeArm";
        // Vector3 position = new Vector3(0, 0, 0);  ///
        Vector3 position = player.transform.position;
        data["position"] = position.x + "," + position.y + "," + position.z;
        
        socket.Emit("PLAY", new JSONObject(data));
        socket.Emit("MOVE", new JSONObject(data));
        
        Debug.Log("Get the message from server in OnClickPlayBtn");
        //socket.Emit("MOVE", new JSONObject(data));
    }

    IEnumerator ConnectToServer() //OK
    {
        yield return new WaitForSeconds(0.5f);

        socket.Emit("USER_CONNECT");
        Debug.Log("Get the message from server in ConnectToServer");
        // yield return new WaitForSeconds(0.5f);
       
        /* Dictionary<string, string> data = new Dictionary<string, string>();
         data["name"] = "wachiii";
         Vector3 position = new Vector3(0, 0, 0);
         data["position"] = position.x + "," + position.y + "," + position.z;
         socket.Emit("PLAY", new JSONObject(data));*/
    }

    private void OnUserConnected(SocketIOEvent evt)
    {
        Debug.Log("Get the message from server is: " + evt.data + "OnUserConnected");
        GameObject otherPlayer = GameObject.Instantiate(playGameObj.gameObject, playGameObj.position, Quaternion.identity) as GameObject;
        //create a player ...
        forearmScript otherPlayerCom = otherPlayer.GetComponent<forearmScript>();
        otherPlayerCom.playerName = JsonToString(evt.data.GetField("name").ToString(), "\"");
        otherPlayerCom.transform.position = JsonToVector3(JsonToString(evt.data.GetField("position").ToString(), "\""));
        otherPlayerCom.id = JsonToString(evt.data.GetField("id").ToString(), "\"");
        // }
    }

     private void OnUserPlay(SocketIOEvent evt)
    {
        Debug.Log("Get the message from server is: " + evt.data);
        
     //   player.gameObject.SetActive(true);
        login.gameObject.SetActive(false);


        GameObject player = GameObject.Instantiate(playGameObj.gameObject, playGameObj.position, Quaternion.identity) as GameObject;
        

        forearmScript playerCom = player.GetComponent<forearmScript>();

        playerCom.playerName = JsonToString(evt.data.GetField("name").ToString(), "\"");
        playerCom.transform.position = JsonToVector3(JsonToString(evt.data.GetField("position").ToString(), "\""));
        playerCom.id = JsonToString(evt.data.GetField("id").ToString(), "\"");
       // player.playerObj= player; // Erroro!!!!

    }
    public void StartTimer(int dueTime)
    {
        Timer t = new Timer(new TimerCallback(TimerProc));
        t.Change(dueTime, 0);
    }

    private void TimerProc(object state)
    {
        // The state object is the Timer object.
        Timer t = (Timer)state;
        t.Dispose();
        Console.WriteLine("The timer callback executes.");
    }
}