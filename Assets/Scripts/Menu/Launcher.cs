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
    public List<string> mapOption;

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
        if (string.IsNullOrEmpty(mapNumberInput.text))
        {
            mapNumber = 1;
            mapNumberInput.text = mapNumber.ToString();
            return;
        }
        mapNumberInput.text = mapNumber.ToString();
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
    public void OpenQuitMenu()
    {
        MenuManager.Instance.OpenMenu("exit");
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
    public void MapOption3DMenu()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
