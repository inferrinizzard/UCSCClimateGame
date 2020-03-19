using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TreeGrowth : MonoBehaviour
{
    public Sprite treeStage1;
    public Sprite treeStage2;
    public Sprite treeStage3;
    public Sprite treeStage4;
    
    public Tilemap tilemap;

    public PlantTree treeGrid;
    public int treeStage;

    public bool growing = false;
    public float growSpeed;

    public SpriteRenderer m_Sprite;

    private Vector3Int treeCellPosition;

    public int nearByTreeCount;
    // Start is called before the first frame update
    void Awake()
    {
        growSpeed = 3f;
        treeStage = 1;
        m_Sprite.sprite = treeStage1;
        nearByTreeCount = 0;
        
        treeCellPosition = tilemap.WorldToCell(gameObject.transform.position);
        
        StartCoroutine(Grow());
        //m_Sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckProximity();
        if (treeStage < 4)
        {
            if (!growing)
            {
                treeStage += 1;
                UpdateTreeVFX(treeStage);
                StartCoroutine(Grow());
            }
        }
        
    }
    void CheckProximity()
    {
        //Debug.Log(transform.position);
        Debug.Log(treeCellPosition + "up" + treeCellPosition + Vector3Int.up);
        if (treeGrid.gridTreeInfo.ContainsKey(treeCellPosition + Vector3Int.up))
        {
            Debug.Log("two close");
            nearByTreeCount += 1;
        }
    }

    void UpdateTreeVFX(int m_treeStage)
    {
        switch (m_treeStage)
        {
            case 1:
                m_Sprite.sprite = treeStage1;
                break;
            case 2:
                m_Sprite.sprite = treeStage2;
                break;
            case 3:
                m_Sprite.sprite = treeStage3;
                break;
            case 4:
                m_Sprite.sprite = treeStage4;
                break;
            default:
                break;

        }
    }

    IEnumerator Grow()
    {
        growing = true;
        yield return new WaitForSeconds(growSpeed);
        growing = false;
    }
}
