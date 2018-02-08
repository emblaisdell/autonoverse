using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class Editor : Block {

	TextMesh screen;
	int offset = 0;
	int posX = 0;
	int posY = 0;
	const int height = 8;
	const int lineLength = 2 + 3 + 8 + 1;

	bool flashing = true;
	float flashTimer = 0f;
	const float flashTime = 0.2f;

	override public int memoryMapSize {
		get {
			return 0;
		}
	}

	void Start () {
		screen = transform.GetChild (0).GetComponent<TextMesh> ();
	}

	void Update () {
		flashTimer -= Time.deltaTime;
		if(flashTimer < 0f) {
			flashing = !flashing;
			flashTimer += flashTime;
		}
		char[] text = new char[height * lineLength + 1];
		text [height * lineLength] = '\0';
		int readPos;
		for (int i = 0; i < height; i++) {
			readPos = (offset + i) % Ship.memoryMapLength;
			string index = System.Convert.ToString (readPos, 16).PadLeft (2, '0');
			for (int j = 0; j < 2; j++) {
				text [lineLength * i + j] = index [j];
			}
			text [lineLength * i + 2] = ' ';
			text [lineLength * i + 3] = ':';
			text [lineLength * i + 4] = ' ';
			string line = System.Convert.ToString (ship.Read (readPos), 2).PadLeft (8, '0');
			for (int j = 0; j < 8; j++) {
				text [lineLength * i + j + 5] = line [j];
			}
			text [lineLength * i + 13] = '\n';
		}
		if (flashing) {
			text [posX + 5 + lineLength * posY] = '_';
		}
		screen.text = new string(text);
	}

	override public byte Read(int offset) {
		return 0;
	}

	override public void Write(int offset, byte value) {

	}

	public void Interact (char c) {
		c = char.ToLower (c);
		if (c == '0') {
			int pos = (offset + posY)%Ship.memoryMapLength;
			ship.Write (pos, (byte)((~(0x80>>posX)) & ship.Read(pos)));
		}
		if (c == '1') {
			int pos = (offset + posY)%Ship.memoryMapLength;
			ship.Write (pos, (byte)((0x80>>posX) | ship.Read(pos)));
		}
		if (c == 'w') {
			if (posY > 0) {
				posY -= 1;
			} else if (offset > 0) {
				offset -= 1;
			} else {
				offset = Ship.memoryMapLength - 1;
			}
		}
		if (c == 'a') {
			if (posX > 0) {
				posX -= 1;
			} else {
				posX = 8 - 1;
			}
		}
		if (c == 's') {
			if (posY < height - 1) {
				posY += 1;
			} else if (offset < Ship.memoryMapLength - 1) {
				offset += 1;
			} else {
				offset = 0;
			}
		}
		if (c == 'd') {
			if (posX < 8 - 1) {
				posX += 1;
			} else {
				posX = 0;
			}
		}
	}
}
