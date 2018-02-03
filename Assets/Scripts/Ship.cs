using System.Collections;
using System.Collections.Generic;

public class Ship {

	public const int memoryMapLength = byte.MaxValue + 1;
	public List<Block> blocks = new List<Block>();
	public List<Block>[] memoryMap = new List<Block>[memoryMapLength];

	public List<Tank> tanks = new List<Tank>();

	public byte Read (int position) {
		System.Diagnostics.Debug.WriteLine (position);
		List<Block> blocks = memoryMap [position];
		if (blocks == null) {
			return 0;
		}
		return blocks [0].Read (position - blocks[0].memoryMapPosition);
	}

	public void Write (int position, byte value) {
		List<Block> blocks = memoryMap [position];
		if (blocks == null) {
			return;
		}
		foreach (Block block in blocks) {
			block.Write (position - block.memoryMapPosition, value);
		}
	}

	public void AddBlock(Block block, int position) {
		//System.Diagnostics.Debug.WriteLine (block.memoryMapSize);
		block.memoryMapPosition = position;
		blocks.Add(block);
		for(int i=0; i<block.memoryMapSize; i++) {
			int pos = (position + i) % memoryMapLength;
			if (memoryMap[pos] == null) {
				memoryMap[pos] = new List<Block> ();
			}
			memoryMap[pos].Add(block);
		}

		if (block is Tank) {
			tanks.Add ((Tank)block);
		}
	}

	public void AddColor (int primary) {
		int sum = 0;
		foreach (Tank tank in tanks) {
			sum += byte.MaxValue - tank.color [primary];
		}
		if (sum > 0) {
			int rand = UnityEngine.Random.Range (1, sum);
			int total = 0;
			foreach (Tank tank in tanks) {
				total += byte.MaxValue - tank.color [primary];
				if (rand <= total) {
					tank.AddPrimary (primary);
					return;
				}
			}
		}
	}

	public static Ship NewShip() {
		return new Ship ();
	}

}
