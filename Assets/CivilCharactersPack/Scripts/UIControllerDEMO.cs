using UnityEngine;
using UnityEngine.UI;

public class UIControllerDEMO : MonoBehaviour
{
    public CharacterCustomization CharacterCustomization;
    public NetworkController NWC = null;
    public PlayFabsController PFC = null;

    //UI element number
    public Text head_text;

    public Text skin_text;

    public Text hat_text;
    public Text accessory_text;
    public Text shirt_text;
    public Text pant_text;
    public Text shoes_text;

    public Text playbutton_text;

    public Animator animator;
    public GameObject temp;

    void Awake()
    {
        NWC = GameObject.Find("NetworkController").GetComponent<NetworkController>();
        if(NWC == null){
            Debug.Log("There is no NetworkController");
        }

        PFC = GameObject.Find("NetworkController").GetComponentInChildren<PlayFabsController>();
        if(PFC == null){
            Debug.Log("There is no PlayFabsController");
        }
    }

    void Update()
    {
        //If were editing a male, swap CharCustomization Script
        if(PFC.isCustomizing && NWC.charSex == 0){
            CharacterCustomization = PFC.femaleCust.GetComponentInChildren<CharacterCustomization>();
            animator = PFC.femaleCust.GetComponentInChildren<Animator>();
            if(CharacterCustomization == null){
                Debug.Log("Couldn't locate CharacterCustomization Script For Female");}
        }

        if(PFC.isCustomizing && NWC.charSex == 1){
            CharacterCustomization = PFC.maleCust.GetComponentInChildren<CharacterCustomization>();
            animator = PFC.maleCust.GetComponentInChildren<Animator>();
            if(CharacterCustomization == null){
                Debug.Log("Couldn't locate CharacterCustomization Script For Male");}
        }
    }

    #region ButtonEvents
    public void HeadChange_Event(int next)
    {
        if (next == -1)
            CharacterCustomization.PrevHead();
        else if (next == 1)
            CharacterCustomization.NextHead();

        head_text.text = CharacterCustomization.headActiveIndex.ToString();
    }

    public void SkinColorChange_Event(int next)
    {
        if (next == -1)
            CharacterCustomization.PrevCharacterMaterial();
        else if (next == 1)
            CharacterCustomization.NextCharacterMaterial();

        skin_text.text = CharacterCustomization.materialActiveIndex.ToString();
    }

    public void HatChange_Event(int next)
    {
        if (next == -1)
            CharacterCustomization.PrevElement(CharacterCustomization.ClothesPartType.Hat);
        else if (next == 1)
            CharacterCustomization.NextElement(CharacterCustomization.ClothesPartType.Hat);

        if (CharacterCustomization.clothesActiveIndexes[CharacterCustomization.ClothesPartType.Hat] == -1)
            hat_text.text = "-";
        else
            hat_text.text = (CharacterCustomization.clothesActiveIndexes[CharacterCustomization.ClothesPartType.Hat] + 1).ToString();
    }

    public void AccessoryChange_Event(int next)
    {
        if (next == -1)
            CharacterCustomization.PrevElement(CharacterCustomization.ClothesPartType.Accessory);
        else if (next == 1)
            CharacterCustomization.NextElement(CharacterCustomization.ClothesPartType.Accessory);

        if (CharacterCustomization.clothesActiveIndexes[CharacterCustomization.ClothesPartType.Accessory] == -1)
            accessory_text.text = "-";
        else
            accessory_text.text = (CharacterCustomization.clothesActiveIndexes[CharacterCustomization.ClothesPartType.Accessory] + 1).ToString();
    }

    public void ShirtChange_Event(int next)
    {
        if (next == -1)
            CharacterCustomization.PrevElement(CharacterCustomization.ClothesPartType.TShirt);
        else if (next == 1)
            CharacterCustomization.NextElement(CharacterCustomization.ClothesPartType.TShirt);

        if (CharacterCustomization.clothesActiveIndexes[CharacterCustomization.ClothesPartType.TShirt] == -1)
            shirt_text.text = "-";
        else
            shirt_text.text = (CharacterCustomization.clothesActiveIndexes[CharacterCustomization.ClothesPartType.TShirt] + 1).ToString();
    }
    public void PantChange_Event(int next)
    {
        if (next == -1)
            CharacterCustomization.PrevElement(CharacterCustomization.ClothesPartType.Pants);
        else if (next == 1)
            CharacterCustomization.NextElement(CharacterCustomization.ClothesPartType.Pants);

        if (CharacterCustomization.clothesActiveIndexes[CharacterCustomization.ClothesPartType.Pants] == -1)
            pant_text.text = "-";
        else
            pant_text.text = (CharacterCustomization.clothesActiveIndexes[CharacterCustomization.ClothesPartType.Pants] + 1).ToString();
    }

    public void ShoesChange_Event(int next)
    {
        if (next == -1)
            CharacterCustomization.PrevElement(CharacterCustomization.ClothesPartType.Shoes);
        else if (next == 1)
            CharacterCustomization.NextElement(CharacterCustomization.ClothesPartType.Shoes);

        if (CharacterCustomization.clothesActiveIndexes[CharacterCustomization.ClothesPartType.Shoes] == -1)
            shoes_text.text = "-";
        else
            shoes_text.text = (CharacterCustomization.clothesActiveIndexes[CharacterCustomization.ClothesPartType.Shoes] + 1).ToString();
    }

    bool walk_active = false;
    public void PlayAnim()
    {
        walk_active = !walk_active;

        animator.SetBool("walk", walk_active);

        playbutton_text.text = (walk_active) ? "STOP" : "PLAY";
    }


    #endregion
}
