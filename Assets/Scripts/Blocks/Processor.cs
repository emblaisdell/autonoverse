using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Processor : Block {

	override public int memoryMapSize {
		get {
			return 2;
		}
	}

	byte PC = 0;
	byte condition = 0;

	byte PCPosition;

	float timer = 0;
	static float frequency = 100f;
	static float cycleTime = 1 / frequency;

	void Start () {
		PCPosition = (byte)memoryMapPosition;
	}

	void Update () {
		timer += Time.deltaTime;
		while (timer >= cycleTime) {
			Exec ();
			timer -= cycleTime;
		}
	}

	void Exec () {
		int toAddress = ship.Read (PC);
		PC++;
		int fromAddress = ship.Read (PC);
		PC++;
		if (toAddress != PCPosition || condition != 0) {
			Write (toAddress, Read(fromAddress));
		}
	}

	override public byte Read(int offset) {
		if (offset == 0) {
			return PC;
		} else {
			return condition;
		}
	}

	override public void Write(int offset, byte value) {
		if (offset == 0) {
			PC = value;
		} else {
			condition = value;
		}
	}

	override public void Interact (char c) {}

	override public void InteractSlider (Player player, float value) {}
}
