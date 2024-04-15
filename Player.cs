using Godot;
using System;

public partial class Player : CharacterBody2D
{

	//Directly modifiable variables
	public HellCrystal mineable = null;
	public bool isDecoyUnlocked = false;
	public bool isWorkerUnlocked = false;

	//Signals
	[Signal]
	public delegate void DecoySummonedEventHandler(Decoy decoy);
	[Signal]
	public delegate void WorkerSummonedEventHandler(Worker decoy);
	[Signal]
	public delegate void ResourceCountChangedEventHandler(int newCount, int delta);
	[Signal]
	public delegate void SummonDecoyCooldownProgressEventHandler(double percentRemaining, double secondsRemaining);
	[Signal]
	public delegate void SummonWorkerCooldownProgressEventHandler(double percentRemaining, double secondsRemaining);

	//Internal variables
	int speed = 600;
	bool isMining = false;
	[Export]
	int summonRadius = 550;

	int _resourceCount = 0;
	public int ResourceCount {get {return _resourceCount;} set {EmitSignal(SignalName.ResourceCountChanged, value, value - _resourceCount); _resourceCount = value; } }


	//External references
	PackedScene decoyScene;
	PackedScene workerScene;
	Timer decoyCooldown;
	Timer workerCooldown;
	Level level;
	AudioStreamPlayer2D summonDecoySFX;
	AudioStreamPlayer2D summonWorkerSFX;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		decoyScene = GD.Load<PackedScene>("res://Decoy.tscn");
		workerScene = GD.Load<PackedScene>("res://Worker.tscn");
		decoyCooldown = GetNode<Timer>("SummonDecoyCooldown");
		workerCooldown = GetNode<Timer>("SummonWorkerCooldown");
		level = GetParent<Level>();
		summonDecoySFX = GetNode<AudioStreamPlayer2D>("SummonDecoySFXPlayer");
		summonWorkerSFX = GetNode<AudioStreamPlayer2D>("SummonWorkerSFXPlayer");
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
		if(!decoyCooldown.IsStopped()){
			EmitSignal(SignalName.SummonDecoyCooldownProgress, 1 - (decoyCooldown.TimeLeft / decoyCooldown.WaitTime), decoyCooldown.TimeLeft);
		}
		if(!workerCooldown.IsStopped()){
			EmitSignal(SignalName.SummonWorkerCooldownProgress, 1 - (workerCooldown.TimeLeft / workerCooldown.WaitTime), workerCooldown.TimeLeft);
		}
		if(isMining){

			return;
		}
		var globalMousePos = GetGlobalMousePosition();
		var mouseInStage = level.top + 60 <= globalMousePos.Y;
        if(isDecoyUnlocked && Input.IsActionJustPressed("SummonDecoy") && decoyCooldown.IsStopped() && mouseInStage){
			decoyCooldown.Start();
			if(!summonDecoySFX.Playing){
				summonDecoySFX.Play();
			}
			var decoy = decoyScene.Instantiate<Decoy>();
			decoy.Position = globalMousePos;
			decoy.direction = Position.DirectionTo(decoy.Position);
			if(decoy.Position.DistanceTo(Position) > summonRadius){
				decoy.Position = Position + decoy.direction * summonRadius;
			}
			GetParent().AddChild(decoy);
			EmitSignal(SignalName.DecoySummoned, decoy);
		}
		if(isWorkerUnlocked && Input.IsActionJustPressed("SummonWorker") && workerCooldown.IsStopped() && mouseInStage){
			var physicsParams = new PhysicsPointQueryParameters2D(){CollideWithAreas=true,CollisionMask=1<<4,Position=globalMousePos};
			if(globalMousePos.DistanceSquaredTo(Position) < Math.Pow(summonRadius,2) && PhysicsServer2D.SpaceGetDirectState(GetWorld2D().Space).IntersectPoint(physicsParams).Count>0){
				workerCooldown.Start();
				if(!summonWorkerSFX.Playing){
					summonWorkerSFX.Play();
				}
				var worker = workerScene.Instantiate<Worker>();
				worker.Position = globalMousePos;
				worker.ResourceMineStarted += handleResourceHarvestByMiner;

				GetParent().AddChild(worker);
				EmitSignal(SignalName.WorkerSummoned, worker);

			}
			
		}
		if(Input.IsActionJustPressed("Interact") && mineable != null && mineable.startHarvesting(this)){
			isMining = true;
			if(!mineable.IsConnected("ResourceHarvestCompleted", Callable.From<bool>(handleResourceHarvestCompleted))){
				mineable.Connect("ResourceHarvestCompleted", Callable.From<bool>(handleResourceHarvestCompleted));
			}
		}
    }
    public override void _PhysicsProcess(double delta)
	{
		if(isMining){
			Velocity = Vector2.Zero;
		}
		else{
			var direction = Input.GetVector("MoveLeft", "MoveRight","MoveUp","MoveDown");
			Velocity = direction * speed;
		}
		MoveAndSlide();
	}

	private void handleResourceHarvestCompleted(bool wasSuccess){
		isMining=false;
		if(wasSuccess){
			ResourceCount++;
		}
	}
	private void handleResourceHarvestByMiner(){
		ResourceCount++;
	}
}
