using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : Block {

	override public int memoryMapSize {
		get {
			return 0;
		}
	}

	override public byte Read(int offset) {
		return 0;
	}

	override public void Write(int offset, byte value){}
}
