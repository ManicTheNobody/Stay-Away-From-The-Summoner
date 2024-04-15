using Godot;
using System;
using System.Collections.Generic;

public partial class BigMike : StaticBody2D
{
	private bool leftCutsceneArea = false;
	private bool enableNextCutscene = false;
	private bool hasNextCutscene = true;

	int[] requiredCount = new int[]{12, 50, 300};
	int requestIndex = 0;
	Cutscene cutsceneNode;
	Area2D turnInZone;
	RichTextLabel prompt;
	Player player;
	Summoner summoner;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<Player>("/root/Level/Player");
		summoner = GetNode<Summoner>("/root/Level/Summoner");

		turnInZone = GetNode<Area2D>("TurnInZone");
		turnInZone.BodyEntered += handleBodyEntered;
		turnInZone.BodyExited += handleBodyExited;
		prompt = GetNode<RichTextLabel>("Prompt");
		cutsceneNode = GetNode<Cutscene>("Cutscene");
		cutsceneNode.CutsceneLineEvent += handleCutsceneLineEvent;
		cutsceneNode.CutsceneComplete += handleCutsceneComplete;
		SetMeta("LastPlayed", Cutscene.CutsceneTitle.GAME_START.ToString());
		cutsceneNode.TriggerCutscene(Cutscene.CutsceneTitle.GAME_START);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("Interact") && enableNextCutscene && requestIndex < requiredCount.Length){
			Cutscene.CutsceneTitle? nextCutscene = null;
			Dictionary<string, Variant> parameters = new Dictionary<string, Variant>();
			if(requestIndex < requiredCount.Length){
				parameters["requiredCount"] = requiredCount[requestIndex];
				if(player.ResourceCount >= requiredCount[requestIndex]){
					nextCutscene = Cutscene.CutsceneTitle.TURN_IN_SUCCESS;
					requestIndex++;
				}
				else if(player.ResourceCount == 0){
					nextCutscene = Cutscene.CutsceneTitle.TURN_IN_ZERO;
				}
				else{
					parameters["playerResourceCount"] = player.ResourceCount;
					nextCutscene = Cutscene.CutsceneTitle.TURN_IN_RETRY;
				}
			}
			if(nextCutscene.HasValue){
				SetMeta("LastPlayed", nextCutscene.ToString());
				leftCutsceneArea = false;
				prompt.Visible = false;
				cutsceneNode.TriggerCutscene(nextCutscene.Value, parameters);
			}
		}
	}

	private void handleBodyEntered(Node2D body){
		if(body.GetMeta("IsPlayer", false).AsBool() && leftCutsceneArea && hasNextCutscene){
			prompt.Visible = true;
			enableNextCutscene = true;
		}
	}
	private void handleBodyExited(Node2D body){
		if(body.GetMeta("IsPlayer", false).AsBool()){
			prompt.Visible = false;
			leftCutsceneArea = true;
			enableNextCutscene = false;
		}
	}

	private void handleCutsceneLineEvent(int type, Variant payload, string line){
		GD.Print(line);
		switch(type){
			case (int)Cutscene.CutsceneLineEventType.TAKE_CRYSTAL:
				GD.Print($"Taking {payload} crystals");
				player.ResourceCount -= payload.AsInt32();
				break;
			case (int)Cutscene.CutsceneLineEventType.TRIGGER_UNLOCK:
				GD.Print($"Unlocking {payload}");
				handleUnlock(payload);
				break;
		}
	}

	private void handleCutsceneComplete(int cutsceneId){
		if(cutsceneId == (int)Cutscene.CutsceneTitle.TURN_IN_SUCCESS){
			Cutscene.CutsceneTitle requestCutscene = requestIndex switch{
				1 => Cutscene.CutsceneTitle.REQUEST_1,
				2 => Cutscene.CutsceneTitle.REQUEST_2,
				_ => Cutscene.CutsceneTitle.ALL_REQUESTS_FULFILLED
			};
			Dictionary<string, Variant> parameters = new Dictionary<string, Variant>();
			parameters["requestAmounts"] = requiredCount;
			parameters["requestIndex"] = requestIndex;
			SetMeta("LastPlayed", requestCutscene.ToString());
			leftCutsceneArea = false;
			prompt.Visible = false;
			cutsceneNode.TriggerCutscene(requestCutscene, parameters);
		}
		if(cutsceneId == (int)Cutscene.CutsceneTitle.ALL_REQUESTS_FULFILLED){
			hasNextCutscene = false;
			GetTree().ChangeSceneToFile("res://Victory.tscn");
		}
	}
	private void handleUnlock(Variant payload){
		switch(payload.AsString()){
			case "SummonDecoy":
				player.isDecoyUnlocked = true;
				break;
			case "SummonWorker":
				player.isWorkerUnlocked = true;
				break;
			case "SummonerSpeed":
				summoner.beginSpeedup();
				break;
		}
	}
}
