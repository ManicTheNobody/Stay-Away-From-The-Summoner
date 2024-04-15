using Godot;
using System;

public partial class HUD : CanvasLayer
{
	HellCrystalSpawner spawner;
	Player player;
	Summoner summoner;
	Control decoyCooldown;
	TextureProgressBar decoyCooldownProgressBar;
	Label decoyCooldownSeconds;
	Control workerCooldown;
	TextureProgressBar workerCooldownProgressBar;
	Label workerCooldownSeconds;

	RichTextLabel resources;
	RichTextLabel spawnTimeRemaining;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<Player>("/root/Level/Player");
		
		summoner = GetNode<Summoner>("/root/Level/Summoner");

		spawner = GetNode<HellCrystalSpawner>("/root/Level/HellCrystalSpawner");

		decoyCooldown = GetNode<Control>("MarginContainer/Control/Cooldowns/SummonDecoyCooldown/VerticalLayout");
		decoyCooldownProgressBar = decoyCooldown.GetNode<TextureProgressBar>("Container/SummonDecoyCooldownProgress");
		decoyCooldownSeconds = decoyCooldown.GetNode<Label>("Container/SummonDecoyCooldownSeconds");
		
		workerCooldown = GetNode<Control>("MarginContainer/Control/Cooldowns/SummonWorkerCooldown/VerticalLayout");
		workerCooldownProgressBar = workerCooldown.GetNode<TextureProgressBar>("Container/SummonWorkerCooldownProgress");
		workerCooldownSeconds = workerCooldown.GetNode<Label>("Container/SummonWorkerCooldownSeconds");
		
		resources = GetNode<RichTextLabel>("MarginContainer/Control/Resources/HellCrystals/AmountLabel");
		spawnTimeRemaining = GetNode<RichTextLabel>("MarginContainer/Control/Resources/TimeToSpawn");
		
		player.SummonDecoyCooldownProgress += handleDecoyCooldownProgress;
		player.SummonWorkerCooldownProgress += handleWorkerCooldownProgress;
		player.ResourceCountChanged += handleResourceCountChanged;

		spawner.SpawnTimerProgress += handleSpawnTimerProgress;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		decoyCooldown.Visible = player.isDecoyUnlocked;
		workerCooldown.Visible = player.isWorkerUnlocked;
	}
	private void handleDecoyCooldownProgress(double percentRemaining, double secondsRemaining){
		decoyCooldownProgressBar.Value = 1 - percentRemaining;
		decoyCooldownSeconds.Text = $"{(int)secondsRemaining}";
		decoyCooldownSeconds.Visible = (int)secondsRemaining>0;
	}
	private void handleWorkerCooldownProgress(double percentRemaining, double secondsRemaining){
		workerCooldownProgressBar.Value = 1 - percentRemaining;
		if(percentRemaining > .96) workerCooldownProgressBar.Value = 0;
		workerCooldownSeconds.Text = $"{(int)secondsRemaining}";
		workerCooldownSeconds.Visible = (int)secondsRemaining>0;
	}
	private void handleSpawnTimerProgress(double percentRemaining, double secondsRemaining){
		if(secondsRemaining <= 7.5){
			spawnTimeRemaining.Text = $"[center][font_size=38]New crystal emerging [wave][rainbow]soon[/rainbow][/wave][/font_size][/center]";
		}
		else{
			spawnTimeRemaining.Text = "";
		}
	}
	private void handleResourceCountChanged(int newCount, int delta){
		resources.Text = $"[center][font_size=96][color=green]x{newCount}[/color][/font_size][/center]";
	}
}
