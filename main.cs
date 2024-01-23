using Godot;
using System;
using System.Diagnostics;
using System.IO;
using Underworld;
using System.Text.Json;
using System.Collections;
using Peaky.Coroutines;

/// <summary>
/// Node to initialise the game
/// </summary>
public partial class main : Node3D
{

	public static bool blockinput
	{
		get
		{
			return
			 ConversationVM.InConversation
			 ||
			 uimanager.InAutomap;
			
			; //TODO and other menu modes that will stop input
		}
	}
	public static Node3D instance;

	// Called when the node enters the scene tree for the first time.
	[Export] public Camera3D cam;
	[Export] public AudioStreamPlayer audioplayer;
	[Export] public RichTextLabel lblPositionDebug;
	[Export] public uimanager uwUI;

	double gameRefreshTimer = 0f;
	double cycletime = 0;

	public override void _Ready()
	{
		instance = this;
		var appfolder = OS.GetExecutablePath();
		appfolder = Path.GetDirectoryName(appfolder);
		var settingsfile = Path.Combine(appfolder, "uwsettings.json");

		if (!File.Exists(settingsfile))
		{
			OS.Alert("missing file uwsettings.json at " + settingsfile);
			return;
		}
		var gamesettings = JsonSerializer.Deserialize<uwsettings>(File.ReadAllText(settingsfile));
		uwsettings.instance = gamesettings;
		cam.Fov = Math.Max(50, uwsettings.instance.FOV);

		UWClass._RES = gamesettings.gametoload;
		switch (UWClass._RES)
		{
			case UWClass.GAME_UW1:
				UWClass.BasePath = gamesettings.pathuw1; break;
			case UWClass.GAME_UW2:
				UWClass.BasePath = gamesettings.pathuw2; break;
			default:
				throw new InvalidOperationException("Invalid Game Selected");
		}

		switch (UWClass._RES)
		{
			case UWClass.GAME_UW2:
				cam.Position = new Vector3(-23f, 4.3f, 58.2f);

				break;
			default:
				cam.Position = new Vector3(-38f, 4.2f, 2.2f);
				//cam.Position = new Vector3(-14.9f, 0.78f, 5.3f);
				break;
		}
		cam.Rotate(Vector3.Up, (float)Math.PI);

		GRLoader gr = new GRLoader(GRLoader.OBJECTS_GR, GRLoader.GRShaderMode.BillboardSpriteShader);
		uwUI.InitUI();
		messageScroll.AddString(GameStrings.GetString(1, 13));


		if (uwsettings.instance.levarkfolder.ToUpper() != "DATA")
        {
            //load player dat from a save file
            playerdat.Load(uwsettings.instance.levarkfolder);  
            Debug.Print($"You are at x:{playerdat.X} y:{playerdat.Y} z:{playerdat.Z}");
            Debug.Print($"You are at x:{playerdat.tileX} {playerdat.xpos} y:{playerdat.tileY} {playerdat.ypos} z:{playerdat.zpos}");
            cam.Position = uwObject.GetCoordinate(playerdat.tileX, playerdat.tileY, playerdat.xpos, playerdat.ypos, playerdat.camerazpos);
            Debug.Print($"Player Heading is {playerdat.heading}");
            cam.Rotation = Vector3.Zero;
            cam.Rotate(Vector3.Up, (float)(Math.PI));//align to the north.
            cam.Rotate(Vector3.Up, (float)(-playerdat.heading / 127f * Math.PI));
            for (int i = 0; i < 8; i++)
            {//Init the backpack indices
                playerdat.SetBackPackIndex(i, playerdat.BackPackObject(i));
            }
            RenderingServer.GlobalShaderParameterSet("cutoffdistance", shade.GetViewingDistance(uwsettings.instance.lightlevel));
            DrawPlayerPositionSprite(gr);
        }
        else
		{
			//Random r = new Random();
			playerdat.InitEmptyPlayer();
			playerdat.tileX = -(int)(cam.Position.X / 1.2f);
			playerdat.tileY = (int)(cam.Position.Z / 1.2f);
			playerdat.dungeon_level = uwsettings.instance.level + 1;

			var isFemale = Rng.r.Next(0, 2) == 1;
			playerdat.isFemale = isFemale;
			uimanager.SetHelm(isFemale, -1);
			uimanager.SetArmour(isFemale, -1);
			uimanager.SetBoots(isFemale, -1);
			uimanager.SetLeggings(isFemale, -1);
			uimanager.SetGloves(isFemale, -1);
			uimanager.SetRightShoulder(-1);
			uimanager.SetLeftShoulder(-1);
			uimanager.SetRightHand(-1);
			uimanager.SetLeftHand(-1);
			for (int i = 0; i < 8; i++)
			{
				uimanager.SetBackPackArt(i, -1);
			}
			playerdat.Body = Rng.r.Next(0, 4);
			playerdat.CharName= "GRONK";
		}

		playerdat.CharNameStringNo = GameStrings.AddString(0x125,playerdat.CharName);

		messageScroll.AddString("Welcome to the underworld");
		//Common launch actions
		_ = Coroutine.Run(
		LoadTileMap(playerdat.dungeon_level - 1, gr), main.instance);


		//Load bablglobals
		bglobal.LoadGlobals(uwsettings.instance.levarkfolder);

		//Draw UI
		uimanager.SetBody(playerdat.Body, playerdat.isFemale);
		uimanager.RedrawSelectedRuneSlots();
        uimanager.RefreshHealthFlask();
        uimanager.RefreshManaFlask();
		//set paperdoll
        uimanager.UpdateInventoryDisplay();
		//Load rune slots
		for (int i = 0; i < 24; i++)
		{
			uimanager.SetRuneInBag(i, playerdat.GetRune(i));
		}

		//Set the playerlight level;
		uwsettings.instance.lightlevel = light.BrightestLight();
	}

    private void DrawPlayerPositionSprite(GRLoader gr)
    {
        int spriteNo = 127;
        var a_sprite = new MeshInstance3D(); //new Sprite3D();
        a_sprite.Name = "player";
        a_sprite.Mesh = new QuadMesh();
        Vector2 NewSize;
        var img = gr.LoadImageAt(spriteNo);
        if (img != null)
        {
            a_sprite.Mesh.SurfaceSetMaterial(0, gr.GetMaterial(spriteNo));
            NewSize = new Vector2(
                    ArtLoader.SpriteScale * img.GetWidth(),
                    ArtLoader.SpriteScale * img.GetHeight()
                    );
            a_sprite.Mesh.Set("size", NewSize);
            Node3D worldobjects = GetNode<Node3D>("/root/Underworld/worldobjects");
            worldobjects.AddChild(a_sprite);
            a_sprite.Position = cam.Position;
        }
    }


    public static IEnumerator LoadTileMap(int newLevelNo, GRLoader grObjects)
	{
		grObjects.UseRedChannel = true;

		Node3D worldobjects = instance.GetNode<Node3D>("/root/Underworld/worldobjects");
		Node3D the_tiles = instance.GetNode<Node3D>("/root/Underworld/tilemap");

		LevArkLoader.LoadLevArkFileData(folder: uwsettings.instance.levarkfolder);
		Underworld.TileMap.current_tilemap = new(newLevelNo);

		Underworld.TileMap.current_tilemap.BuildTileMapUW(newLevelNo, Underworld.TileMap.current_tilemap.lev_ark_block, Underworld.TileMap.current_tilemap.tex_ark_block, Underworld.TileMap.current_tilemap.ovl_ark_block);
        ObjectCreator.GenerateObjects(worldobjects, Underworld.TileMap.current_tilemap.LevelObjects, grObjects, Underworld.TileMap.current_tilemap);
		the_tiles.Position = new Vector3(0f, 0f, 0f);
		tileMapRender.GenerateLevelFromTileMap(the_tiles, worldobjects, UWClass._RES, Underworld.TileMap.current_tilemap, Underworld.TileMap.current_tilemap.LevelObjects, false);

		switch (UWClass._RES)
		{
			case UWClass.GAME_UW2:
				automap.automaps = new automap[80]; break;
			default:
				automap.automaps = new automap[9]; break;
		}
		automap.automaps[newLevelNo] = new automap(newLevelNo);
		uwsettings.instance.lightlevel = light.BrightestLight();
		Debug.Print($"{Underworld.TileMap.current_tilemap.uw}");
		yield return null;
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		int tileX = -(int)(cam.Position.X / 1.2f);
		int tileY = (int)(cam.Position.Z / 1.2f);

		lblPositionDebug.Text = $"{cam.Position.ToString()}\n{tileX} {tileY}\n{uimanager.instance.uwsubviewport.GetMousePosition()}";

		if ((tileX < 64) && (tileX >= 0) && (tileY < 64) && (tileY >= 0))
		{
			// 	//automap.currentautomap.tiles[tileX,tileX].visited = true;
			if ((playerdat.tileX != tileX) || (playerdat.tileY != tileY))
			{
				playerdat.tileX = tileX;
				playerdat.tileY = tileY;
				uwsettings.instance.lightlevel = light.BrightestLight();
			}
		}


		//RenderingServer.GlobalShaderParameterSet("cameraPos", cam.Position);
		cycletime += delta;
		if (cycletime > 0.2)
		{
			cycletime = 0;
			PaletteLoader.UpdatePaletteCycles();
		}
		gameRefreshTimer += delta;
		if (gameRefreshTimer >= 0.3)
		{
			gameRefreshTimer = 0;
			npc.UpdateNPCs();
			AnimationOverlay.UpdateAnimationOverlays();
		}
	}

	public override void _Input(InputEvent @event)
	{
		float RayLength = 3.0f;
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
		{
			if (!blockinput)
			{
				if (uimanager.IsMouseInViewPort())
				{
					//var camera3D = GetNode<Camera3D>("Camera3D");
					var from = cam.ProjectRayOrigin(eventMouseButton.Position);
					var mousepos = uimanager.instance.uwsubviewport.GetMousePosition(); //eventMouseButton.Position
					var to = from + cam.ProjectRayNormal(mousepos) * RayLength;
					var query = PhysicsRayQueryParameters3D.Create(from, to);
					var spaceState = GetWorld3D().DirectSpaceState;
					var result = spaceState.IntersectRay(query);
					if (result != null)
					{
						if (result.ContainsKey("collider"))
						{
							var obj = (StaticBody3D)result["collider"];
							Debug.Print(obj.Name);
							messageScroll.AddString(obj.Name);
							string[] vals = obj.Name.ToString().Split("_");
							if (int.TryParse(vals[0], out int objindex))
							{
								switch (uimanager.InteractionMode)
								{
									case uimanager.InteractionModes.ModeTalk:
										talk.Talk(objindex, Underworld.TileMap.current_tilemap.LevelObjects, true);
										break;
									case uimanager.InteractionModes.ModeLook:
										//Do a look interaction with the object
										look.LookAt(objindex, Underworld.TileMap.current_tilemap.LevelObjects, true);
										break;
									case uimanager.InteractionModes.ModeUse:
										//do a use interaction with the object.
										use.Use(objindex, Underworld.TileMap.current_tilemap.LevelObjects, true);
										break;
								}
							}
						}
					}
				}
			}
		}

		if (ConversationVM.WaitingForInput)
		{
			if (@event is InputEventKey keyinput)
			{
				Debug.Print(keyinput.Keycode.ToString());
				if (int.TryParse(keyinput.AsText(), out int result))
				{
					if ((result > 0) && (result <= ConversationVM.MaxAnswer))
					{
						ConversationVM.PlayerNumericAnswer = result;
						ConversationVM.WaitingForInput = false;
					}
				}
			}
		}
	}
}
