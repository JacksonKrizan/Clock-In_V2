using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Net.Http.Headers;
using Photon.Realtime;
using System.Linq;

public class BackToMenu : MonoBehaviour
{
    public void BackToMenu1()
    {
        PhotonNetwork.LoadLevel(0);
    }
}
