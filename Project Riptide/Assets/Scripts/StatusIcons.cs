using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusIcons : MonoBehaviour
{
    [System.Serializable]
    private struct NamedSprite
    {
        public StatusType name;
        public Sprite sprite;
        public Sprite opposite;
        public float oppositeThreshold;
    }

    [SerializeField]
    private List<NamedSprite> sprites;

    private Dictionary<StatusType, NamedSprite> spriteDict;

    [SerializeField]
    private GameObject iconPrefab;
    

    [SerializeField]
    private float _maxRowSize;
    [SerializeField]
    private float _spriteWidth;
    [SerializeField]
    private float _spriteHeight;
    // Start is called before the first frame update
    void Start()
    {
        spriteDict = new Dictionary<StatusType, NamedSprite>();
        for(int i = 0; i < sprites.Count;i++)
        {
            spriteDict[sprites[i].name] = sprites[i];
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    public void RearrangeStatuses(List<StatusEffect> statusEffects)
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        int numObjs = statusEffects.Count;

        int numRows = Mathf.CeilToInt((1.0f * numObjs) / _maxRowSize);

        int i = 0;

        float startY = _spriteHeight * numRows;
        float startX = -_spriteWidth * Mathf.Min(_maxRowSize, numObjs - i) / 2.0f;

        float currentY = startY;
        float currentX = startX;
        

        foreach(StatusEffect s in statusEffects)
        {
            GameObject iconClone = Instantiate(iconPrefab, transform);
            if(spriteDict[s.Type].opposite == null || s.Level >= spriteDict[s.Type].oppositeThreshold)
            {
                iconClone.GetComponent<SpriteRenderer>().sprite = spriteDict[s.Type].sprite;
            } else
            {
                iconClone.GetComponent<SpriteRenderer>().sprite = spriteDict[s.Type].opposite;
            }
            
            iconClone.transform.localPosition = new Vector3(currentX, currentY, 0);

            i++;
            
            if(i == _maxRowSize)
            {
                currentX = -_spriteWidth * Mathf.Min(_maxRowSize, numObjs - i) / 2.0f;
                currentY -= _spriteHeight;
                i = 0;
            } else
            {
                currentX += _spriteWidth;
            }
            
        }
    }
}
