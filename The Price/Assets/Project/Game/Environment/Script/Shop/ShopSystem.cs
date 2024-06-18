using System.Collections.Generic;
using UnityEngine;

public class ShopSystem : MonoBehaviour {

    [Header("Data Base")]
    public int quantityForSale;
    public List<int> prices = new List<int>();

    [Header("Prefabs")]
    public InteractiveObject objectObj;
    public InteractiveSkill skillObj;
    public InteractiveFlair flairObj;

    [Header("Private Listed")]
    private ObjectPlacement _objects;
    private SkillPlacement _skills;
    private FlairSystem _flair;

    [Header("Element in Scene")]
    public GameObject[] positionPerObject;

    [Header("Chances")]
    public float chancesObject;
    public float chancesFlair;

    private void Start()
    {
        _objects = FindAnyObjectByType<ObjectPlacement>();
        _skills = FindAnyObjectByType<SkillPlacement>();
        _flair = FindAnyObjectByType<FlairSystem>();

        CreateShop();
    }
    private void CreateShop()
    {
        for(int i = 0; i < quantityForSale; i++)
        {
            int rnd = Random.Range(0, 100);

            int rndAmount = Random.Range(0, prices.Count);

            if(rnd < chancesObject)
            {
                // CREAR OBJETO
                Object obj = _objects.RandomPool();

                InteractiveObject objInScene = Instantiate(objectObj.gameObject, positionPerObject[i].transform.position, Quaternion.identity, transform).GetComponent<InteractiveObject>();

                objInScene.obj = obj; 
                objInScene.isShop = true;
                objInScene.priceInGold = prices[rndAmount];

                objInScene.nameContent = obj.itemName.ToString();
                objInScene.descContent = obj.description.ToString();
            }
            else if(rnd >= chancesObject && rnd < chancesFlair)
            {
                // CREAR FLAIR
                TypeFlair flair = _flair.RandomFlairInSelector();
                int amount = _flair.CalculateAmount();
                TypeFlair affected = _flair.RandomAffectedFlair(flair);

                InteractiveFlair objInScene = Instantiate(flairObj.gameObject, positionPerObject[i].transform.position, Quaternion.identity, transform).GetComponent<InteractiveFlair>();

                objInScene.flair = flair;
                objInScene.amount = amount;
                objInScene.affected = affected;
                objInScene.isShop = true;
                objInScene.priceInGold = prices[rndAmount];

                objInScene.nameContent = LanguageManager.GetValue("Game", 25) + " " + LanguageManager.GetValue("Game", (26 + (int)flair));
                objInScene.descContent = _flair.CreateContentDescription(flair, amount, affected);
            }
            else
            {
                // CREAR SKILL
                SkillManager skill = _skills.RandomPool();

                InteractiveSkill objInScene = Instantiate(skillObj.gameObject, positionPerObject[i].transform.position, Quaternion.identity, transform).GetComponent<InteractiveSkill>();
                
                objInScene.skill = skill;
                objInScene.isShop = true;
                objInScene.priceInGold = prices[rndAmount];

                objInScene.nameContent = skill.skillName.ToString();
                objInScene.descContent = skill.descName.ToString();
            }
        }
    }
}