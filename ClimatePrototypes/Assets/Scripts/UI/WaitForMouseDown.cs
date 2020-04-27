using UnityEngine;

public class WaitForMouseDown : CustomYieldInstruction {
	int buttonCode;
	public override bool keepWaiting {
		get => !Input.GetMouseButtonDown(buttonCode);
	}

	public WaitForMouseDown(int button = 0) => buttonCode = button;
}
