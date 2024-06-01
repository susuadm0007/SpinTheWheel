using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class WheelManager : MonoBehaviour
{
    public static WheelManager instance;
    public int numberOfSections;
    public GameObject wheelItemPrefab;
    public List<WheelItem> wheelItems;
   
    public enum ITEM_TYPES
    {
        None,
        Airbud,
        Camera,
        Coin,
        Laptop,
        Microphone,
        Mobile,
        Money,
        Watch
    }
    public enum SPIN_DIRECTION
    {
        Clockwise,
        CounterClockwise
    }
    public SPIN_DIRECTION spinDirection;
    public List<AnimationCurve> spinCurves;
    public int minSpinTime = 3, maxSpinTime = 4;
    public List<Sprite> itemSprites;
    public UnityEvent onReward;
    public AnimationCurve rewardCurve;
    public GameObject rewardEffectPrefab;

    public float rewardScale = 2f;
    public AnimationCurve rewardScaleCurve;

    public WheelItem winningItem;
    private GameObject rewardDisplay;
    public GameObject spinButton;
    public GameObject text1;
    public GameObject text2;
    public GameObject admin;
    public GameObject reward;
    public GameObject character1;
    //public GameObject character2;
    //public GameObject effect;

    private void Start()
    {
        
    }
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        /*if(effect.GetComponent<Image>().IsActive()) {
            effect.transform.Rotate(new Vector3(0,0,-15)*Time.deltaTime);
        }*/
    }
    public void SimulateTheWheel(int numberOfSpins)
    {
        WheelResults wheelResults = new WheelResults();
        
        for(int i = 0; i <= numberOfSpins - 1; i++)
        {
            PickWinningItem(wheelResults);
        }

        for (int j = 1; j <= Enum.GetNames(typeof(ITEM_TYPES)).Length - 1; j++)
        {
            wheelResults.output += "Found " + Enum.GetName(typeof(ITEM_TYPES), j).ToString() + " " + wheelResults.itemsWon.FindAll(x => x.itemType.ToString() == Enum.GetName(typeof(ITEM_TYPES), j)).Count + " time(s).\n";
        }

        Debug.Log("Results from " + wheelResults.itemsWon.Count + " won items:\n" + wheelResults.output);
    }

    public void PickWinningItem(WheelResults wheelResults = null)
    {
        float weightedSum = 0f;
        for (int i = 0; i <= wheelItems.Count - 1; i++)
        {
            weightedSum += wheelItems[i].dropChance;
        }
        float selectedItem = UnityEngine.Random.Range(0, weightedSum);
        for (int j = 0; j <= wheelItems.Count - 1; j++)
        {
            selectedItem -= wheelItems[j].dropChance;
            if (selectedItem <= 0 || j == wheelItems.Count - 1)
            {
                winningItem = wheelItems[j];
                if (wheelResults != null)
                {
                    wheelResults.itemsWon.Add(wheelItems[j]);
                }
                break;
            }
        }
    }

    public void SpinTheWheel()
    {
        spinButton.GetComponent<Button>().interactable = false;
        admin.GetComponent<Button>().interactable = false;
        PickWinningItem();
        AudioManager.instance.Play("Click");
       
        StartCoroutine(AnimateSpin(GameManager.Instance.spinTime, winningItem));
        
    }

    IEnumerator AnimateSpin(float spinTime, WheelItem winningItem)
    {
        //Debug.Log("Winning item: " + winningItem.itemType.ToString() + winningItem.prefix + winningItem.itemValue + winningItem.affix + "\nTarget angle: " + winningItem.itemObject.transform.localEulerAngles.z);
        Debug.Log("SpinSpeed: " + GameManager.Instance.spinSpeed.ToString() + " SpinTime: " +  spinTime.ToString());
        AnimationCurve randomSpinCurve;
        if(spinCurves.Count == 0)
        {
            Debug.LogWarning("No spin animation curves defined! Creating and using an example curve...");
            spinCurves.Add(CreateSampleAnimation());
        }
        randomSpinCurve = spinCurves[UnityEngine.Random.Range(0, spinCurves.Count)];

        float spinTimer = 0f;

        Transform childToSpin = transform.GetChild(1);
        float startAngle = -(360 / wheelItems.Count);

        float spinMagnitude = 360*GameManager.Instance.spinSpeed;

        if(spinDirection == SPIN_DIRECTION.CounterClockwise)
        {
            spinMagnitude *= -1;
        }

        float landingAngle = (spinMagnitude * spinTime) + winningItem.itemObject.transform.localEulerAngles.z - ((360 / wheelItems.Count / 2) * 2);
        
        while (spinTimer < spinTime)
        {
            float newRotation;
            newRotation = landingAngle * -randomSpinCurve.Evaluate(spinTimer / spinTime);
            childToSpin.localEulerAngles = new Vector3(0, 0, newRotation + startAngle);
            spinTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //AudioManager.instance.Stop("Count");
        yield return new WaitForSeconds(1);
        AudioManager.instance.Stop("Bg");
        onReward.Invoke();
    }

    public AnimationCurve CreateSampleAnimation()
    {
        Keyframe[] sampleKeyframes = new Keyframe[2];
        sampleKeyframes[0] = new Keyframe(0, 0);
        sampleKeyframes[1] = new Keyframe(1, 1);
        AnimationCurve sampleCurve = new AnimationCurve(sampleKeyframes);
        return sampleCurve;
    }

    public void StartReward()
    {
        StartCoroutine(AnimateReward(winningItem));
    }

    IEnumerator AnimateReward(WheelItem winningItem)
    {
        float animationTimer = 0f;
        float animationTime = 1f;
        AudioManager.instance.Play("Over");
        for (int i = 0; i <= transform.childCount - 1; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        spinButton.SetActive(false);
        admin.SetActive(false);
        text1.SetActive(false);
        text2.SetActive(false);
        character1.SetActive(false);
        //character2.SetActive(false);
        
        reward.SetActive(true);
        rewardDisplay = GameObject.Instantiate(winningItem.itemObject);
        rewardDisplay.transform.SetParent(gameObject.transform);
        rewardDisplay.transform.localPosition = new Vector3(0,150,0);
        rewardDisplay.transform.localEulerAngles = Vector3.zero;
        rewardDisplay.transform.localScale = Vector3.one;

        GameManager.Instance.confetti.SetActive(true);
        GameManager.Instance.confetti2.SetActive(true);
        GameManager.Instance.confetti3.SetActive(true);

        GameObject rewardSprite = rewardDisplay.transform.GetChild(0).gameObject;

        Vector3 startPosition = rewardSprite.transform.localPosition;
        Vector3 startScale = rewardSprite.transform.localScale;

        if(rewardCurve == null)
        {
            Debug.Log("Creating an example curve to use for the reward...");
            rewardCurve = CreateSampleAnimation();
        }

        if (rewardCurve == null)
        {
            Debug.Log("Creating an example curve to use for the reward scale...");
            rewardScaleCurve = CreateSampleAnimation();
        }

        while (animationTimer < animationTime && rewardDisplay != null)
        {
            rewardSprite.transform.localPosition = new Vector3(0, startPosition.y * rewardCurve.Evaluate(animationTimer / animationTime), 0);
            rewardSprite.transform.localScale = new Vector3(startScale.x + (rewardScale * rewardScaleCurve.Evaluate(animationTimer / animationTime)), startScale.y + (rewardScale * rewardScaleCurve.Evaluate(animationTimer / animationTime)), startScale.z + (rewardScale * rewardScaleCurve.Evaluate(animationTimer / animationTime)));
            animationTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        if (rewardEffectPrefab != null && rewardDisplay != null)
        {
            GameObject rewardEffect = GameObject.Instantiate(rewardEffectPrefab);
            rewardEffect.transform.SetParent(rewardDisplay.transform, true);
        }

        yield return null;
    }

    public void ClaimReward()
    {
        StopCoroutine("AnimateReward");
        ResetRotation();
        if (rewardDisplay)
        {
            GameObject.Destroy(rewardDisplay);
        }
        spinButton.GetComponent<Button>().interactable = true;
        admin.GetComponent<Button>().interactable = true;
        AudioManager.instance.Play("Click");
        AudioManager.instance.Play("Bg");
      
    }

    void ResetRotation()
    {
        transform.GetChild(1).localEulerAngles = new Vector3(0, 0, -360 / wheelItems.Count);
        spinButton.SetActive(true);
        admin.SetActive(true);
        text1.SetActive(true);
        text2.SetActive(true);
        character1.SetActive(true);
        //character2.SetActive(true);
        reward.SetActive(false);
        GameManager.Instance.confetti.SetActive(false);
        GameManager.Instance.confetti2.SetActive(false);
        GameManager.Instance.confetti3.SetActive(false);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(WheelManager))]
public class WheelEditor: Editor
{
    public override void OnInspectorGUI()
    {   
        serializedObject.Update();
        var wheelManager = (WheelManager)target;

        SerializedProperty itemPrefab = serializedObject.FindProperty("wheelItemPrefab");
        EditorGUILayout.PropertyField(itemPrefab, new GUIContent("Wheel Item Prefab:"), true);

        SerializedProperty spinDirection = serializedObject.FindProperty("spinDirection");
        EditorGUILayout.PropertyField(spinDirection, new GUIContent("Spin Direction:"), true);

        SerializedProperty spinCurves = serializedObject.FindProperty("spinCurves");
        EditorGUILayout.PropertyField(spinCurves, new GUIContent("Spin Curves:"), true);

        SerializedProperty minSpinTime = serializedObject.FindProperty("minSpinTime");
        EditorGUILayout.PropertyField(minSpinTime, new GUIContent("Minimum Spin Time (in seconds):"), true);

        SerializedProperty maxSpinTime = serializedObject.FindProperty("maxSpinTime");
        EditorGUILayout.PropertyField(maxSpinTime, new GUIContent("Maximum Spin Time (in seconds):"), true);

        SerializedProperty itemSprites = serializedObject.FindProperty("itemSprites");
        EditorGUILayout.PropertyField(itemSprites, new GUIContent("Item Sprites:"), true);

        SerializedProperty onReward = serializedObject.FindProperty("onReward");
        EditorGUILayout.PropertyField(onReward, new GUIContent("Reward Events:"), true);

        SerializedProperty rewardCurve = serializedObject.FindProperty("rewardCurve");
        EditorGUILayout.PropertyField(rewardCurve, new GUIContent("Reward Animation Curve:"), true);

        SerializedProperty rewardScaleCurve = serializedObject.FindProperty("rewardScaleCurve");
        EditorGUILayout.PropertyField(rewardScaleCurve, new GUIContent("Reward Scaling Curve:"), true);
        
        SerializedProperty rewardScale = serializedObject.FindProperty("rewardScale");
        EditorGUILayout.PropertyField(rewardScale, new GUIContent("Reward Display Scale when Won: "), true);

        SerializedProperty rewardEffectPrefab = serializedObject.FindProperty("rewardEffectPrefab");
        EditorGUILayout.PropertyField(rewardEffectPrefab, new GUIContent("Reward Effect Prefab: "), true);

        SerializedProperty winningItem = serializedObject.FindProperty("winningItem");
        EditorGUILayout.PropertyField(winningItem, new GUIContent("Item Won:"), true);

        SerializedProperty spinButton = serializedObject.FindProperty("spinButton");
        EditorGUILayout.PropertyField(spinButton, new GUIContent("Spin Button:"), true);

        SerializedProperty text1 = serializedObject.FindProperty("text1");
        EditorGUILayout.PropertyField(text1, new GUIContent("Text 1:"), true);

        SerializedProperty text2 = serializedObject.FindProperty("text2");
        EditorGUILayout.PropertyField(text2, new GUIContent("Text 2:"), true);

        SerializedProperty admin = serializedObject.FindProperty("admin");
        EditorGUILayout.PropertyField(admin, new GUIContent("Admin Button:"), true);

        SerializedProperty reward = serializedObject.FindProperty("reward");
        EditorGUILayout.PropertyField(reward, new GUIContent("Reward BG:"), true);

        SerializedProperty character1 = serializedObject.FindProperty("character1");
        EditorGUILayout.PropertyField(character1, new GUIContent("Character 1:"), true);

        /*SerializedProperty character2 = serializedObject.FindProperty("character2");
        EditorGUILayout.PropertyField(character2, new GUIContent("Character 2:"), true);*/

        /*SerializedProperty effect = serializedObject.FindProperty("effect");
        EditorGUILayout.PropertyField(effect, new GUIContent("Effect:"), true);*/

        SerializedProperty wheelNumberOfSections = serializedObject.FindProperty("numberOfSections");
        EditorGUILayout.IntSlider(wheelNumberOfSections, 2, 16, "Number of sections:");

        if (GUILayout.Button("Update wheel items (Will Erase Extra Items!)"))
        {
            UpdateWheel(wheelManager, wheelNumberOfSections);
            return;
        }

        if(GUILayout.Button("Delete all wheel items"))
        {
            DeleteWheelItems(wheelManager);
            return;
        }

        if (GUILayout.Button("Simulate 1 Spin"))
        {
            wheelManager.SimulateTheWheel(1);
        }

        if (GUILayout.Button("Simulate 1000 Spins"))
        {
            wheelManager.SimulateTheWheel(1000);
        }

        SerializedProperty wheelItems = serializedObject.FindProperty("wheelItems");
        EditorGUILayout.PropertyField(wheelItems, new GUIContent("Wheel Items:"), true);

        serializedObject.ApplyModifiedProperties();

        UpdateWheelItems(wheelManager);
    }

    void UpdateWheel(WheelManager wheelManager, SerializedProperty wheelNumberOfSections)
    {
        int newItemCount = 0;
        if (wheelManager.wheelItems.Count > wheelNumberOfSections.intValue)
        {
            if (wheelManager.wheelItems.Count > 0)
            {
                newItemCount = Mathf.Clamp(wheelManager.wheelItems.Count - wheelManager.numberOfSections, 0, 128);
                for (int i = 0; i <= newItemCount; i++)
                {
                    GameObject.DestroyImmediate(wheelManager.wheelItems[wheelManager.wheelItems.Count - 1].itemObject);
                    wheelManager.wheelItems.RemoveAt(wheelManager.wheelItems.Count - 1);
                }
            }
        }
        if (wheelManager.wheelItems.Count <= wheelNumberOfSections.intValue)
        {
            if (wheelManager.wheelItems.Count == 0)
            {
                newItemCount = wheelManager.numberOfSections;
            }
            else
            {
                newItemCount = Mathf.Clamp(wheelManager.numberOfSections - wheelManager.wheelItems.Count, 0, 128);
            }
            if (newItemCount != 0)
            {
                for (int i = 0; i <= newItemCount - 1; i++)
                {
                    WheelItem newItem = new WheelItem();
                    GameObject newWheelObj = GameObject.Instantiate(wheelManager.wheelItemPrefab);
                    newWheelObj.transform.SetParent(wheelManager.transform.GetChild(1), false);
                    newItem.itemObject = newWheelObj;
                    wheelManager.wheelItems.Add(newItem);
                }
            }
        }
        if(wheelManager.spinCurves.Count == 0)
        {
            wheelManager.spinCurves.Add(wheelManager.CreateSampleAnimation());
        }
        OrganizeWheelItems(wheelManager);
    }

    void DeleteWheelItems(WheelManager wheelManager)
    {
        Undo.RegisterCompleteObjectUndo(wheelManager, "Delete wheel items");
        for (int i = 0; i <= wheelManager.wheelItems.Count - 1; i++)
        {
            Undo.DestroyObjectImmediate(wheelManager.wheelItems[i].itemObject);
        }
        wheelManager.wheelItems.RemoveRange(0, wheelManager.wheelItems.Count);
        OrganizeWheelItems(wheelManager);
    }

    void OrganizeWheelItems(WheelManager wheelManager)
    {
        float itemRotation = 360 / wheelManager.wheelItems.Count;
        for(int i = 0; i <= wheelManager.wheelItems.Count - 1; i++)
        {
            wheelManager.wheelItems[i].itemObject.transform.localEulerAngles = new Vector3(0, 0, (itemRotation * -i) + (itemRotation / 2));
        }
        wheelManager.transform.GetChild(1).localEulerAngles = new Vector3(0, 0, -itemRotation);
    }

    void UpdateWheelItems(WheelManager wheelManager)
    {
        for (int i = 0; i <= wheelManager.wheelItems.Count - 1; i++)
        {
            //wheelManager.wheelItems[i].itemObject.transform.GetComponentInChildren<TextMeshProUGUI>().text = wheelManager.wheelItems[i].prefix + wheelManager.wheelItems[i].itemValue.ToString() + wheelManager.wheelItems[i].affix;
            if (wheelManager.wheelItems[i].itemType == WheelManager.ITEM_TYPES.None)
            {
                wheelManager.wheelItems[i].itemSprite = null;
                break;
            }
            for (int j = 0; j <= wheelManager.itemSprites.Count - 1; j++)
            {
                if (wheelManager.wheelItems[i].itemType.ToString().ToLower().Equals(wheelManager.itemSprites[j].name))
                {
                    wheelManager.wheelItems[i].itemSprite = wheelManager.itemSprites[j];
                    wheelManager.wheelItems[i].itemObject.transform.GetComponentInChildren<Image>().sprite = wheelManager.itemSprites[j];
                }
            }
        }
    }
}
#endif
[System.Serializable]
public class WheelItem
{
    public GameObject itemObject;
    [Range(0f, 100f)]
    [Tooltip("The chance of this item being selected (0% to 100%)")]
    public float dropChance = 1f;
    public Sprite itemSprite;
    public WheelManager.ITEM_TYPES itemType;
}

public class WheelResults
{
    public string output;
    public List<WheelItem> itemsWon = new List<WheelItem>();
}