using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transciever : Block {

	static List<Transciever> transcievers;

	public byte channel;
	Queue<byte> messages;

	void Start (){
		transcievers.Add (this);
	}

	override public int memoryMapSize {
		get {
			return 2;
		}
	}

	override public byte Read(int offset) {
		if (offset == 0) {
			return channel;
		} else {
			if (messages.Count > 0) {
				return messages.Dequeue ();
			} else {
				return 0;
			}
		}
	}

	void SendMessage(byte message) {
		foreach (Transciever transciever in transcievers) {
			if (transciever.channel == channel) {
				transciever.messages.Enqueue (message);
			}
		}
	}

	override public void Write(int offset, byte value){
		if (offset == 0) {
			channel = value;
		} else {
			SendMessage (value);
		}
	}
}
