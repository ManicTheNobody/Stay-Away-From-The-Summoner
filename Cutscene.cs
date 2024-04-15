using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

public partial class Cutscene : Node2D
{
	private delegate string InjectParams(Dictionary<string, Variant> param);

	public enum CutsceneLineEventType{
		NO_EVENT,
		TAKE_CRYSTAL,
		TRIGGER_UNLOCK
	}

	private struct CutsceneLine{
		public InjectParams line;
		public CutsceneLineEventType eventType;
		public Variant? payload;
		public bool payloadFromParams;
    }

	[Signal]
	public delegate void CutsceneLineEventEventHandler(int type, Variant payload, string line);
	[Signal]
	public delegate void CutsceneCompleteEventHandler(int cutsceneId);

	public enum CutsceneTitle{
		GAME_START,
		TURN_IN_SUCCESS,
		TURN_IN_RETRY,
		TURN_IN_ZERO,


		REQUEST_1,
		REQUEST_2,

		ALL_REQUESTS_FULFILLED
	}

	Dictionary<CutsceneTitle, List<CutsceneLine>> cutscenes = new()
    {
        {CutsceneTitle.GAME_START, new List<CutsceneLine>{
            new() {line = x => $"So, you got yourself mixed up in another contract?"},
			new() {line =x => $"And now [color=red]Hell's Summoner[/color] is after you and you want me to find someone to replace you?"},
			new() {line =x => $"Well of course! You can always trust your old buddy [b][color=orange]BIG MIKE![/color][/b]"},
			new() {line =x => $"It's going to cost you, though."},
			new() {line =x => $"See I'm in serious need of some [wave][color=red]Hell Crystals[/color][/wave], and it's so hard for me to go and get 'em myself."},
			new() {line =x => $"Because I'm so [b][wave][color=orange]BIG[/color][/wave][/b] and all."},
			new() {line =x => $"Go bring me some [wave][color=red]Hell Crystals[/color][/wave] and I'll [i]consider[/i] sending some poor sap in your place."},
			new() {line =x => $"[pulse][color=cyan]Twelve[/color][/pulse] aughta do it for now."},
			new() {line =x => $"Oh, and [rainbow]STAY AWAY FROM THE SUMMONER[/rainbow]. You do [i]not[/i] want him to catch you right now."},
			new() {line =x => $"You still remember how to conjure [wave][color=red]Decoys[/color][/wave], right?", eventType = CutsceneLineEventType.TRIGGER_UNLOCK, payload = "SummonDecoy"}
		}},
		{CutsceneTitle.TURN_IN_SUCCESS, new List<CutsceneLine>{
			new() {line =x => $"Perfect! I'll take those {HelperFunctions.IntToEnglish(x["requiredCount"].AsInt32())} [color=red]Hell Crystals[/color] now."},
			new() {line =x => $"[rainbow]Thank you.[/rainbow]", eventType = CutsceneLineEventType.TAKE_CRYSTAL, payload="requiredCount", payloadFromParams = true}
		}},
		{CutsceneTitle.TURN_IN_RETRY, new List<CutsceneLine>{
			new() {line =x => $"Is there wax in your ears? I certainly hope not."},
			new() {line =x => $"I asked for [color=cyan][wave]{HelperFunctions.IntToEnglish(x["requiredCount"].AsInt32())}[/wave][/color] [color=red]Hell Crystals[/color]."},
			new() {line =x => $"You only have {HelperFunctions.IntToEnglish(x["playerResourceCount"].AsInt32())}! I wanna help you out, but you gotta work with me here."}
		}},
		{CutsceneTitle.TURN_IN_ZERO, new List<CutsceneLine>{
			new() {line =x => $"Are you doing okay, man?"},
			new() {line =x => $"I asked for [color=cyan][wave]{HelperFunctions.IntToEnglish(x["requiredCount"].AsInt32())}[/wave][/color] [color=red]Hell Crystals[/color]."},
			new() {line =x => $"But it seems like you haven't got any at all..."},
			new() {line =x => $"Not even one..."},
			new() {line =x => $"Well good luck..."},
			new() {line =x => $"I guess..."}
		}},
		{CutsceneTitle.REQUEST_1, new List<CutsceneLine>{
			new() {line =x => $"With these, I can afford to spare some of my [color=orange]Lesser Imps[/color] to help you.", eventType = CutsceneLineEventType.TRIGGER_UNLOCK, payload = "SummonWorker"},
			new() {line =x => $"Just conjure 'em up next to some [color=red]Hell Crystals[/color] and they'll chip away at them while you do other things."},
			new() {line =x => $"What's that?"},
			new() {line =x => $"Your contract?"},
			new() {line =x => $"You didn't think that [pulse][color=cyan]{HelperFunctions.IntToEnglish(x["requestAmounts"].AsInt32Array()[x["requestIndex"].AsInt32()-1])}[/color][/pulse] measly [color=red]Hell Crystals[/color] could cover the fees I'm dealing with here?"},
			new() {line =x => $"Bring me [pulse][color=cyan]{HelperFunctions.IntToEnglish(x["requestAmounts"].AsInt32Array()[x["requestIndex"].AsInt32()])}[/color][/pulse] more [wave][color=red]Hell Crystals[/color][/wave] and we'll talk."},
			new() {line =x => $"And watch out..."},
			new() {line =x => $"I think the [color=red]Summoner[/color] is getting faster...", eventType = CutsceneLineEventType.TRIGGER_UNLOCK, payload = "SummonerSpeed"},
		}},
		{CutsceneTitle.REQUEST_2, new List<CutsceneLine>{
			new() {line =x => $"I see my imp friends are serving you well."},
			new() {line =x => $"That's good, because I have another request. And it's a big one."},
			new() {line =x => $"Please bring me [color=cyan][wave]{HelperFunctions.IntToEnglish(x["requestAmounts"].AsInt32Array()[x["requestIndex"].AsInt32()])}[/wave][/color] [color=red]Hell Crystals[/color]."},
			new() {line =x => $"Say again?"},
			new() {line =x => $"You thought the [color=cyan][wave]{HelperFunctions.IntToEnglish(x["requestAmounts"].AsInt32Array()[x["requestIndex"].AsInt32()-1])}[/wave][/color] [color=red]Hell Crystals[/color] you just brought would do the job?"},
			new() {line =x => $"It was more like a [wave]down payment[/wave]."},
			new() {line =x => $"This one'll do it though. Promise."},
			new() {line =x => $"Yup, just need those [color=cyan][wave]{HelperFunctions.IntToEnglish(x["requestAmounts"].AsInt32Array()[x["requestIndex"].AsInt32()])}[/wave][/color] [color=red]Hell Crystals[/color]..."},
		}},
		{CutsceneTitle.ALL_REQUESTS_FULFILLED, new List<CutsceneLine>{
			new() {line =x => $"I told you you could count on [b][color=orange]BIG MIKE![/color][/b]"},
			new() {line =x => $"Yessiree, I've got your replacement all lined up. It sure took a lot of [wave][color=red]Hell Crystals[/color][/wave], huh?"},
			new() {line =x => $"I hope the [color=red]Summoner[/color] didn't give you too much trouble?"},
			new() {line =x => $"Well anyhow, he's got nothing on you now. Get on out there and enjoy your contract-free life."},
			new() {line =x => $"[wave][rainbow]ANOTHER ONE?![/rainbow][/wave]"},
			new() {line =x => $"Oh... haha... You really got me good."},
			new() {line =x => $"Just..."},
			new() {line =x => $"Try not to go signing any more contracts."},
			new() {line =x => $"I'm going to bed."},
		}},
	};

	private int cutsceneIndex = 0;
	private CutsceneTitle activeCutscene;

	private Dictionary<string, Variant> cutsceneParameters;

	RichTextLabel cutsceneArea;
	AudioStreamPlayer2D nextDialogueSFX;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cutsceneParameters = new Dictionary<string, Variant>();
		cutsceneArea = GetNode<RichTextLabel>("Panel/MarginContainer/CutsceneArea");
		nextDialogueSFX = GetNode<AudioStreamPlayer2D>("NextDialogueSFX");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("Interact")){
			cutsceneIndex++;
			if(!cutscenes.ContainsKey(activeCutscene) || cutsceneIndex >= cutscenes[activeCutscene].Count){
				Visible = false;
				GetTree().Paused = false;
				EmitSignal(SignalName.CutsceneComplete, (int)activeCutscene);
				return;
			}
			nextDialogueSFX.PitchScale = (float)GD.RandRange(0.8,1.2);
			nextDialogueSFX.Play();
			var line = cutscenes[activeCutscene][cutsceneIndex];
			cutsceneArea.Text = $"[shake]{line.line(cutsceneParameters)}[/shake]";
			if(line.eventType != CutsceneLineEventType.NO_EVENT){
				var payload = line.payloadFromParams ? cutsceneParameters[line.payload.Value.ToString()]: line.payload.Value;
				EmitSignal(SignalName.CutsceneLineEvent, (int)line.eventType, payload, cutsceneArea.Text);
			}

		}
	}

	public void TriggerCutscene(CutsceneTitle title, Dictionary<string, Variant> parameters = null){
		GetTree().Paused = true;
		cutsceneIndex = -1;
		Visible = true;
		activeCutscene = title;
		if(parameters == null){
			cutsceneParameters.Clear();
		}else{
			cutsceneParameters = parameters;
		}
	}
}
