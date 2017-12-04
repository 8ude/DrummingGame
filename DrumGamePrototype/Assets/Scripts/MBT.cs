using System;

[Serializable]
public struct MBT : IComparable {
	public int Measure;
	public int Beat;
	public int Tick;

	public MBT(int measure, int beat, int tick) {
		Measure = measure;
		Beat = beat;
		Tick = tick;
	}

	public override string ToString() {
		string value = "M:" + Measure + " B: " + Beat + " T: " + Tick;
		return value;
	}

	public int CompareTo(object obj) {
		MBT objMBT = (MBT)obj;
		int objTicks = objMBT.Measure * (4 * 96) + (objMBT.Beat * 96) + objMBT.Tick;
		int thisTicks = this.Measure * (4 * 96) + (this.Beat * 96) + this.Tick;
		if (thisTicks == objTicks) {
			return 0;
		}
		else if (thisTicks > objTicks) {
			return -1;
		}
		else {
			return 1;
		}
	}
}  
