using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tesseract : Block {

	const int fileLength = 128;
	byte[] filenameBytes = new byte[fileLength];
	byte filenameBytesPos = 0;

	string contents = "";
	byte contentPos = 0;

	bool newFile = false;

	override public int memoryMapSize {
		get {
			return 4;
		}
	}

	override public byte Read(int offset) {
		return 0;
		/*if (offset == 0) {
			return filenameBytes[filenameBytesPos];
		} else if (offset == 1) {
			return filenameBytesPos;
		} else if (offset == 2) {
			if (newFile) {
				TextAsset
			}
		} else {

		}*/
	}

	override public void Write(int offset, byte value){}

	override public void Interact (char c) {}

	override public void InteractSlider (Player player, float value) {}
}
