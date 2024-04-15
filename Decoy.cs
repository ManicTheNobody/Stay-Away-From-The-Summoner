using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class Decoy : CharacterBody2D
{
	//Internal variables
	public Vector2 direction = Vector2.Zero;
	int speed = 450;
	//External references
	Timer lifeTime;

	[Signal]
	public delegate void OnVanishedEventHandler();
    public override void _Ready()
    {
        lifeTime = GetNode<Timer>("TimeToLive");
		lifeTime.Timeout += onExpire;
    }
    public override void _PhysicsProcess(double delta)
	{
		Velocity = direction * speed;
		MoveAndSlide();
	}
	private void onExpire(){
		EmitSignal(SignalName.OnVanished);
		QueueFree();
	}
}
