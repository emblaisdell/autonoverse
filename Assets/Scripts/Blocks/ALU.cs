using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ALU : Block {

	byte input1 = 0;
	byte input2 = 0;
	byte opCode = 0;
	byte output = 0;

	override public int memoryMapSize {
		get {
			return 4;
		}
	}

	void Calculate() {
		switch (opCode) {
		case 0:
			output = (byte)(input1 + input2);
			return;
		case 1:
			output = (byte)(input1 - input2);
			return;
		case 2:
			output = (byte)(input1 * input2);
			return;
		case 3:
			if (input2 == 0) {
				output = byte.MaxValue;
			} else {
				output = (byte)(input1 / input2);
			}
			return;
		case 4:
			output = (byte)(input1 % input2);
			return;
		/*case 5: //exponentiation
			return;*/
		case 6:
			output = (byte)(-input1);
			return;
		case 16:
			output = (byte)(input1 & input2);
			return;
		case 17:
			output = (byte)(input1 | input2);
			return;
		case 18:
			output = (byte)(input1 ^ input2);
			return;
		case 19:
			output = (byte)(~input1);
			return;
		case 32:
			output = Bool (input1 == input2);
			return;
		case 33:
			output = Bool (input1 != input2);
			return;
		case 34:
			output = Bool (input1 < input2);
			return;
		case 35:
			output = Bool (input1 <= input2);
			return;
		case 36:
			output = Bool (input1 > input2);
			return;
		case 37:
			output = Bool (input1 >= input2);
			return;
		default:
			output = 0;
			return;
		}
	}

	static byte Bool (bool b) {
		return b ? byte.MaxValue : byte.MinValue;
	}

	override public byte Read(int offset) {
		if (offset == 0) {
			return input1;
		} else if (offset == 1) {
			return input2;
		} else if (offset == 2) {
			return opCode;
		} else {
			return output;
		}
	}

	override public void Write(int offset, byte value){
		if (offset < 3) {
			if (offset == 0) {
				input1 = value;
			} else if (offset == 1) {
				input2 = value;
			} else if (offset == 2) {
				opCode = value;
			}
			Calculate ();
			ship.Write ((memoryMapPosition + 3) % Ship.memoryMapLength, output);
		}
	}
}
