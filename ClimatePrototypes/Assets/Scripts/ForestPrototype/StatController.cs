using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatController : MonoBehaviour
{
	public Text MoneyText;
	public Text MoneyChangeText;
	public Text TempText;
	public Text GrowthRateText;

	public VertMoveText ChangeTextPrefab;

	public DesertShifter ShifterReference;

	public AnimationCurve BalloonCurve;
	public AnimationCurve ChangeLerpCurve;

	private float MoneyBalloonCurrTime = 0f;
	private float MoneyBalloonTotTime = .3f;
	private bool bMoneyBallooning = false;

	private float ChangeCurrTime = 0f;
	private float ChangeTotTime = 1.5f;
	private bool bChangeMoving = false;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		MoneyText.text = "$" + string.Format("{0:0,0}", GlobalStatics.cashMoney);
		TempText.text = "Temperature: " + string.Format("{0:0,0.00}", GlobalStatics.temperature);
		GrowthRateText.text = "Growth Rate: " + string.Format("{0:0,0.00}", ShifterReference.DesertGrowthRate * 10f) + "in/s";

		if (bMoneyBallooning)
			BalloonMoney();
		if (bChangeMoving)
			MoveChange();
	}

	public void CashChange(float change)
	{
		MoneyBalloonCurrTime = 0f;
		bMoneyBallooning = true;

		//ChangeCurrTime = 0f;
		//bChangeMoving = true;
		// spawn change

		VertMoveText v = Instantiate(ChangeTextPrefab, transform) as VertMoveText;
		v.Initialize(new Vector2(-110f, 160f), new Vector2(-110f, 200f), change);
	}

	private void MoveChange()
	{
		float lerpY = Mathf.Lerp(160f, 200f, ChangeLerpCurve.Evaluate(ChangeCurrTime / ChangeTotTime));
		MoneyChangeText.rectTransform.anchoredPosition = new Vector2(-110, lerpY);

		Color newC = MoneyChangeText.color;
		newC.a = 1f - ChangeLerpCurve.Evaluate(ChangeCurrTime / ChangeTotTime);
		MoneyChangeText.color = newC;

		ChangeCurrTime += Time.deltaTime;

		if (ChangeCurrTime > ChangeTotTime)
			bChangeMoving = false;
	}

	private void BalloonMoney()
	{
		float targetScale = BalloonCurve.Evaluate(MoneyBalloonCurrTime / MoneyBalloonTotTime);

		MoneyText.rectTransform.localScale = new Vector3(targetScale, targetScale, targetScale);

		MoneyBalloonCurrTime += Time.deltaTime;

		if (MoneyBalloonCurrTime >= MoneyBalloonTotTime)
			bMoneyBallooning = false;
	}
}
