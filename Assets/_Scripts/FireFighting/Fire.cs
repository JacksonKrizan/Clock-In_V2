using UnityEngine;
using Photon.Pun;

// A single fire. The master client owns the true intensity and streams it to
// everyone via this object's PhotonView (add the PhotonView and put this Fire in
// its Observed Components). Igniting (FireManager) and extinguishing (water hits)
// both change the master's value; everyone else just displays it.
[ExecuteAlways]
public class Fire : MonoBehaviourPun, IPunObservable
{
    [Header("Adjust this slider (0 = Off, 1 = Full)")]
    [Range(0f, 1f)] public float currentIntensity = 0f;

    [Header("Settings")]
    [SerializeField] private ParticleSystem firePS = null;
    [Tooltip("The particle count when the slider is at 1.0")]
    [SerializeField] private float maxEmissionRate = 10.0f;
    [Tooltip("How fast the fire grows back if not fully out (0 = stays put)")]
    [SerializeField] private float recoveryRate = 0.05f;

    [Header("Alert")]
    [Tooltip("Shown above the building while it is on fire (e.g. an icon/arrow)")]
    [SerializeField] private GameObject alertMarker;
    [SerializeField] private string buildingName = "a building";

    private bool wasBurning;

    private void Update()
    {
        // only the authority grows the fire back; everyone else shows the synced value
        if (Application.isPlaying && IsAuthority() &&
            currentIntensity > 0f && currentIntensity < 1f)
        {
            currentIntensity = Mathf.Clamp01(currentIntensity + recoveryRate * Time.deltaTime);
        }

        UpdateAlert();

        if (firePS != null)
        {
            var emission = firePS.emission;
            float newRate = currentIntensity * maxEmissionRate;
            emission.rateOverTime = newRate;
            emission.enabled = newRate > 0;
        }
    }

    // marker follows the synced intensity, so it shows on every client; the HUD
    // message fires once, the moment the building catches.
    void UpdateAlert()
    {
        bool burning = currentIntensity > 0f;

        if (alertMarker != null && alertMarker.activeSelf != burning)
            alertMarker.SetActive(burning);

        if (Application.isPlaying && burning && !wasBurning && MiniGameHUD.Instance != null)
            MiniGameHUD.Instance.ShowResponse("Fire at " + buildingName + "!");

        wasBurning = burning;
    }

    // master client (or offline) is the authority over the real intensity
    bool IsAuthority() => !PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient;

    // called by the extinguisher when water hits this fire
    public void Extinguish(float amount)
    {
        if (Application.isPlaying && PhotonNetwork.IsConnected)
            photonView.RPC(nameof(ReduceRPC), RpcTarget.MasterClient, amount);
        else
            ReduceRPC(amount);
    }

    [PunRPC]
    void ReduceRPC(float amount)
    {
        currentIntensity = Mathf.Clamp01(currentIntensity - amount);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) stream.SendNext(currentIntensity);   // master sends
        else currentIntensity = (float)stream.ReceiveNext();        // everyone else receives
    }
}
