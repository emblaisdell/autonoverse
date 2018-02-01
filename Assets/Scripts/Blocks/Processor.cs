using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Processor : Block {

	override public int memoryMapSize {
		get {
			return 2;
		}
	}

	const int numRegisters = 32;
	byte[] registers = new byte[numRegisters];
	bool running;

	float timer = 0;
	static float frequency = 100f;
	static float cycleTime = 1 / frequency;

	void Start () {
		registers [0] = 0; // PC
	}

	void Update () {
		timer += Time.deltaTime;
		while (timer >= cycleTime) {
			Exec ();
			timer -= cycleTime;
		}
	}

	void Exec () {
		if (running && registers[0] < byte.MaxValue) {
			byte instruction = ship.Read ((int) registers[0]);
			int opCode = (instruction & 0xc0) >> 6;
			int r1 = (instruction & 0x38) >> 3;
			int r2 = instruction & 0x07;

			if (opCode == 0x0) { // set
				int PC = registers[0];
				registers [r1] = ship.Read(PC+1);
				registers[0]++;
				if (r1 != r2) {
					registers [r1] = ship.Read(registers[PC+2]);
					registers[0]++;
				}
			} else if (opCode == 0x1) { // save
				ship.Write(registers[r1], registers[r2]);
			} else if (opCode == 0x2) { // load
				registers[r1] = ship.Read(registers[r2]);
			} else { // branch
				if (registers [r1] != 0x0) {
					registers [0] = (byte)(registers [r2] - 1);
				}
			}

			registers [0]++;
		}
	}

	override public byte Read(int offset) {
		if (offset == 0) {
			return registers [0];
		} else {
			return running ? byte.MaxValue : byte.MinValue;
		}
	}

	override public void Write(int offset, byte value) {
		if (offset == 0) {
			registers [0] = value;
		} else {
			running = (value & 0x1) != 0;
		}
	}

	override public void Interact (char c) {}

	override public void InteractSlider (Player player, float value) {}
}
