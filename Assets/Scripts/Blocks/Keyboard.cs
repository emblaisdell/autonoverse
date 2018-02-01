using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : Block {

	private Queue<byte> chars = new Queue<byte> ();

	override public int memoryMapSize {
		get {
			return 1;
		}
	}

	override public byte Read(int offset) {
		if (chars.Count > 0) {
			return chars.Dequeue ();
		} else {
			return 0;
		}
	}

	override public void Write(int offset, byte value){}

	override public void Interact (char c) {
		chars.Enqueue ((byte)c);
	}

	override public void InteractSlider (Player player, float value) {}
}
