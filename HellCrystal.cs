using Godot;
using System;

public partial class HellCrystal : StaticBody2D
{
	//Signals
	[Signal]
	public delegate void ResourceHarvestCompletedEventHandler(bool wasSuccess);
	[Signal]
	public delegate void CrystalDepletedEventHandler();
	//Internal variables
	bool isBeingMined = false;
	int resourceCount = 8;
	//External references
	SkillCheck skillCheck;
	RichTextLabel tooltip;
	Area2D interactableTriggerArea;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		skillCheck = GetNode<SkillCheck>("SkillCheck");
		skillCheck.SkillCheckCompleted += handleSkillCheckResult;

		tooltip = GetNode<RichTextLabel>("Tooltip");

		interactableTriggerArea = GetNode<Area2D>("HellCrystalInteractableArea");
		interactableTriggerArea.BodyEntered += handleBodyEntered;
		interactableTriggerArea.BodyExited += handleBodyExited;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		interactableTriggerArea.GetNode<CollisionShape2D>("HellCrystalTriggerZone").Disabled = isBeingMined;

		if(isBeingMined){
			tooltip.Visible = false;
		}
		if(resourceCount <= 5){
			skillCheck.difficulty = 1;
		}
		if(resourceCount <= 2){
			skillCheck.difficulty = 2;
		}
		if(resourceCount<=0){
			EmitSignal(SignalName.CrystalDepleted);
			QueueFree();
		}
	}

	public bool startHarvesting(Node2D harvester){
		if(isBeingMined) return false;
		isBeingMined = true;
		if(harvester.GetMeta("IsPlayer", false).AsBool()){
			skillCheck.Visible = true;
			skillCheck.ProcessMode = ProcessModeEnum.Inherit;
		}
		return true;
	}

	private void handleSkillCheckResult(bool wasSuccess){
		isBeingMined = false;
		tooltip.Visible = true;
		resourceCount--;
		if(!wasSuccess){
			resourceCount -= Math.Abs((int)GD.Randi()) % 3;
		}
		EmitSignal(SignalName.ResourceHarvestCompleted, wasSuccess);
	}

	private void handleMinerHarvest(){
		isBeingMined = true;
		resourceCount--;
		
		var n = GetNode<Node2D>("OffScreenMarker");
		if(n!=null){
			n.QueueFree();
		}
	}

	private void handleBodyEntered(Node2D body){
		if(body.GetMeta("IsPlayer", false).AsBool() && !isBeingMined){
			tooltip.Visible = true;
			((Player)body).mineable = this;
		}
		else if(body.GetMeta("IsWorker", false).AsBool()){
			isBeingMined = true;
			var workerBody = ((Worker)body);
			workerBody.ResourceMineStarted += handleMinerHarvest;
			CrystalDepleted += workerBody.HandleResourceDepleted;
			
		}
	}
	private void handleBodyExited(Node2D body){
		if(body.GetMeta("IsPlayer", false).AsBool()){
			tooltip.Visible = false;
			((Player)body).mineable = null;
		}
	}
}
