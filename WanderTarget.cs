using Godot;
using System;

public partial class WanderTarget : Node2D
{
	Random random;
	Timer timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		random = new Random();
		timer = GetNode<Timer>("Timer");
		timer.Timeout += changeTargetPosition;
		changeTargetPosition();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void changeTargetPosition(){
		GlobalPosition = new Vector2(random.NextSingle()*6000-3000, random.NextSingle()*6000-3000);
	}
}
