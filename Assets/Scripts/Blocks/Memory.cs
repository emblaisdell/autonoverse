using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory : Block {

	const int size = 64;
	byte[] memory = new byte[size];

	override public int memoryMapSize {
		get {
			return size;
		}
	}

	override public byte Read(int offset) {
		return memory[offset];
	}

	override public void Write(int offset, byte value){
		memory [offset] = value;
	}
}
