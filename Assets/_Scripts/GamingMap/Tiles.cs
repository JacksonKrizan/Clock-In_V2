using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();
    public Color glowColor = Color.cyan;
    public BoxCollider TileBoxCollider;
    public Material redTileMaterial;
    public Material yellowTileMaterial;
    public Material whiteTileMaterial;
    public float yellowTileDuration = 1f;
    

    public Renderer TileRenderer;


    public float speed;



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
                speed = UnityEngine.Random.Range(3f, 10f);

                yield return new WaitForSeconds(speed);
                TileRenderer.material = yellowTileMaterial;
                yield return new WaitForSeconds(yellowTileDuration);
                TileBoxCollider.enabled = !TileBoxCollider.enabled;
                TileRenderer.material = redTileMaterial;

                yield return new WaitForSeconds(speed);
                TileBoxCollider.enabled = !TileBoxCollider.enabled;
                TileRenderer.material = whiteTileMaterial;

            }
        }

}
