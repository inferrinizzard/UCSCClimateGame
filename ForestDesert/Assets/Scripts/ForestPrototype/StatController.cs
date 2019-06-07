using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatController : MonoBehaviour
{
    // Text objects to display values for 
    public Text MoneyText; // Money
    public Text MoneyChangeText; // The amount charged when a tree is bought - NOT USED
    public Text TempText; // Current temperature

    // Text that is spawned that moves upwards (for polish) when you are charged money
    public VertMoveText ChangeTextPrefab;

    // Reference to the desert shifter
    public DesertShifter ShifterReference;

    // The curve that controls the scale of MoneyText when you are charged
    // Moneytext is ballooned and then slowly decreased to normal size over time
    public AnimationCurve BalloonCurve;

    // A curve that would store the lerp progress of a the shifting money charged text - OLD 
    public AnimationCurve ChangeLerpCurve;

    // Stores the current and total time it takes to balloon the money text
    private float MoneyBalloonCurrTime = 0f;
    private float MoneyBalloonTotTime = .3f;
    // Whether or not we are tweening the moneytext
    private bool bMoneyBallooning = false;

    // Stores the current and total time it takes to shift the charge text
    private float ChangeCurrTime = 0f;
    private float ChangeTotTime = 1.5f;
    // Whether we are shifting the charge text
    private bool bChangeMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoneyText.text = "$" + string.Format("{0:0,0}", GlobalStatics.CashMoney);
        TempText.text = "Temperature: " + string.Format("{0:0,0.00}", GlobalStatics.Temperature);

        if (bMoneyBallooning)
            BalloonMoney();
        if (bChangeMoving)
            MoveChange();
    }

    // Called when we spawn a tree
    // Creates a vertmovetext object (amount charged that moves up)
    // Sets money ballooning to true so we can tween it's scale
    public void CashChange(float change)
    {
        MoneyBalloonCurrTime = 0f;
        bMoneyBallooning = true;

        VertMoveText v = Instantiate(ChangeTextPrefab, transform) as VertMoveText;
        v.Initialize(new Vector2(-110f, 160f), new Vector2(-110f, 200f), change);
    }

    // OLD WAY OF PANNING CHARGE TEXT
    private void MoveChange()
    {
        float lerpY = Mathf.Lerp(160f, 200f, ChangeLerpCurve.Evaluate(ChangeCurrTime/ChangeTotTime));
        MoneyChangeText.rectTransform.anchoredPosition = new Vector2(-110, lerpY);

        Color newC = MoneyChangeText.color;
        newC.a = 1f - ChangeLerpCurve.Evaluate(ChangeCurrTime / ChangeTotTime);
        MoneyChangeText.color = newC;

        ChangeCurrTime += Time.deltaTime;

        if (ChangeCurrTime > ChangeTotTime)
            bChangeMoving = false;
    }

    // Tweens the scale of the moneytext by evaluating a curve
    private void BalloonMoney()
    {
        float targetScale = BalloonCurve.Evaluate( MoneyBalloonCurrTime / MoneyBalloonTotTime );

        MoneyText.rectTransform.localScale = new Vector3(targetScale, targetScale, targetScale);

        MoneyBalloonCurrTime += Time.deltaTime;

        if (MoneyBalloonCurrTime >= MoneyBalloonTotTime)
            bMoneyBallooning = false;
    }
}
