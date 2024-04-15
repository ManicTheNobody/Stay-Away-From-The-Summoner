using Godot;
using System;
using System.Reflection.Metadata;

public partial class HellCrystalSpawner : Node2D
{

	//Signals
	[Signal]
	public delegate void SpawnTimerProgressEventHandler(double percentRemaining, double secondsRemaining);
	//Internal variables
	Random rand;
	//External references
	Level level;
	Timer spawnTimer;
	PackedScene hellCrystal;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rand = new Random();
		hellCrystal = GD.Load<PackedScene>("res://HellCrystal.tscn");
		level = GetParent<Level>();
		spawnTimer = GetNode<Timer>("SpawnTimer");
		spawnTimer.Timeout += spawnHellCrystal;
		level.Ready += () =>{
			for(int i=0;i<6;i++)spawnHellCrystal();
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!spawnTimer.IsStopped()){
			EmitSignal(SignalName.SpawnTimerProgress, 1 - (spawnTimer.TimeLeft/spawnTimer.WaitTime), spawnTimer.TimeLeft);
		}
	}

	private void spawnHellCrystal(){
		HellCrystal crystal = hellCrystal.Instantiate<HellCrystal>();
		level.AddChild(crystal);
		ShapeCast2D spawnBlocker = crystal.GetNode<ShapeCast2D>("HellCrystalSpawnBlockArea");
		do{
			crystal.Position = new Vector2(rand.Next(level.left+300, level.right-300), rand.Next(level.top+300, level.bottom-300));
			spawnBlocker.ForceShapecastUpdate();
		}while(spawnBlocker.GetCollisionCount()>0);
		spawnBlocker.Enabled = false;
		//level.CallDeferred("add_child", crystal);
	}
}
