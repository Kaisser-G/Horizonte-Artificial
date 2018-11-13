using System;

public class Datos
{
    int pitch = null;
    int yaw = null;
    int roll = null;

	public void AddPitch(int p) { pitch = p; }

    public void AddRoll(int r) { roll = r; }

    public void AddYaw(int y) { yaw = y; }

    public int Roll() { return roll; }

    public int Pitch() { return pitch; }

    public int Yaw() { return yaw; }
}
