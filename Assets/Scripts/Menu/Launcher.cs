using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Net.Http.Headers;
using Photon.Realtime;
using System.Linq;
public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    //[SerializeField] GameObject startGameButton;
    //[SerializeField] List<int> mapsChoices;// = new List<int>();
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject PlayerListItemPrefab;

    public int mapNumber = 1;
    [SerializeField] TMP_InputField mapNumberInput = null;
    [SerializeField] List<GameObject> noShowForNonMasterClient;// = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }
    public void OnMapNumberInputValueChanged()
    {
        // Called while the TMP input value changes. Don't overwrite the input
        // text here — that will reset the user's typing. Instead, try to parse
        // the current input and update the backing int if valid.
        if (mapNumberInput == null)
            return;

        string txt = mapNumberInput.text;
        if (string.IsNullOrEmpty(txt))
        {
            // Let the user type; don't clobber the field on empty input.
            return;
        }

        int parsed;
        if (int.TryParse(txt, out parsed))
        {
            // Keep a sensible minimum (1) to avoid loading map 0.
            mapNumber = Mathf.Max(1, parsed);
        }
        // If parsing fails, we leave the text as-is. Optionally you can sanitize
        // non-digit characters here and write back once (preferably on EndEdit).
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000");
    }
    // Update is called once per frame

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {

        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;


        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent) 
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        //startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        foreach (GameObject go in noShowForNonMasterClient)
        {
            go.SetActive(PhotonNetwork.IsMasterClient);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        foreach (GameObject go in noShowForNonMasterClient)
        {
            go.SetActive(PhotonNetwork.IsMasterClient);
        }
    }

    public override void OnCreateRoomFailed(short returnCode,string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(mapNumber);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");

    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
