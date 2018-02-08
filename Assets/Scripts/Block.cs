using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block : MonoBehaviour {

	public Ship ship;
	public int memoryMapPosition;
	public abstract int memoryMapSize { get; }

	public abstract byte Read (int offset);
	public abstract void Write (int offset, byte value);
}
