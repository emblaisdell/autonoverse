using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class Tesseract : Block {

	const int fileLength = 128;
	byte[] filenameBytes = new byte[fileLength+1]; // last byte always '\0'
	byte filenameBytesPos = 0;

	string contents = "";
	byte contentPos = 0;

	bool newFile = false;

	const byte EOF = 0x1a;

	byte value;

	override public int memoryMapSize {
		get {
			return 4;
		}
	}

	void Start () {
		SaveAndLoadManager.SaveGame ("gamesave");
	}

	string GetPath() {
		return Encoding.ASCII.GetString (filenameBytes);
	}
	void LoadFile() {
		StreamReader reader = new StreamReader (GetPath (), Encoding.ASCII);
		contents = reader.ReadToEnd ();
	}
	byte GetValue() {
		if (contentPos >= contents.Length) {
			return EOF;
		}
		return (byte)(contents [contentPos]);
	}

	override public byte Read(int offset) {
		if (offset == 0) {
			byte output = filenameBytes[filenameBytesPos];
			filenameBytesPos++;
			return output;
		} else if (offset == 1) {
			return filenameBytesPos;
		} else if (offset == 2) {
			if (newFile) {
				LoadFile ();
				newFile = false;
			}
			byte output = GetValue();
			contentPos++;
			return output;
		} else {
			return contentPos;
		}
	}

	override public void Write(int offset, byte value){
		if (offset == 0) {
			filenameBytes [filenameBytesPos] = value;
			filenameBytesPos++;
			if (filenameBytesPos >= fileLength) {
				filenameBytesPos = 0;
			}
		} else if (offset == 1) {
			filenameBytesPos = (byte)(value % fileLength);
		} else if (offset == 3) {
			contentPos = value;
		}
	}

	override public void Interact (char c) {}

	override public void InteractSlider (Player player, float value) {}
}
