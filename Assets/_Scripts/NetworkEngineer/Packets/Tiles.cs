using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();
    //public Color glowColor = Color.cyan;
    public BoxCollider TileBoxCollider;
    public Material redTileMaterial;
    public Material yellowTileMaterial;
    public Material whiteTileMaterial;
    public float yellowTileDuration;
    

    public Renderer TileRenderer;


    [SerializeField] float speed;

    // shared across every tile: no more than this many can be red at once
    [SerializeField] int maxRedTiles = 5;
    static int redCount = 0;
    bool isRed = false;



        void Start()
        {
            TileRenderer = GetComponent<Renderer>();

            TileBoxCollider = GetComponent<BoxCollider>();
            StartCoroutine(TilesFaded());
        }

        IEnumerator TilesFaded()
        {
            while (true)
            {
                speed = UnityEngine.Random.Range(5f, 10f);

                yield return new WaitForSeconds(speed);
                TileRenderer.material = yellowTileMaterial;
                yield return new WaitForSeconds(yellowTileDuration);

                // only go red if we're under the cap right now; otherwise stay white
                if (redCount >= maxRedTiles)
                {
                    TileRenderer.material = whiteTileMaterial;
                    continue;
                }

                redCount++;
                isRed = true;
                TileBoxCollider.enabled = false;
                TileRenderer.material = redTileMaterial;

                yield return new WaitForSeconds(speed / 2f);

                TileBoxCollider.enabled = true;
                TileRenderer.material = whiteTileMaterial;
                isRed = false;
                redCount--;

            }
        }

    void OnDisable()
    {
        // if this tile gets destroyed/disabled while red, free its slot
        if (isRed) { redCount--; isRed = false; }
    }

}
