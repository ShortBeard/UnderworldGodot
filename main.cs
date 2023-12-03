using Godot;
using System;
using System.Diagnostics;
using System.IO;
using Underworld;
using System.Text.Json;



/// <summary>
/// Node to initialise the game
/// </summary>
public partial class main : Node3D
{

	// Called when the node enters the scene tree for the first time.
	[Export] public Camera3D cam;
	[Export] public AudioStreamPlayer audioplayer;
	[Export] public RichTextLabel lblPositionDebug;
	[Export] public uimanager uwUI;

	double gameRefreshTimer = 0f;
	double cycletime = 0;

	public override void _Ready()
	{
		var appfolder = OS.GetExecutablePath();
		appfolder = System.IO.Path.GetDirectoryName(appfolder);
		var settingsfile = System.IO.Path.Combine(appfolder, "uwsettings.json");

		if (!System.IO.File.Exists(settingsfile))
		{
			OS.Alert("missing file uwsettings.json at " + settingsfile);
			return;
		}
		var gamesettings = JsonSerializer.Deserialize<uwsettings>(File.ReadAllText(settingsfile));
		uwsettings.instance = gamesettings;

		//shade.getFarDist(0);
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

		//playerdat.Load("SAVE1");
		//Debug.Print(playerdat.CharName);
		// Voc file loading. 
		// var vocfiles = System.IO.Directory.GetFiles(System.IO.Path.Combine(UWClass.BasePath, "SOUND"), "sp18.voc");
		// foreach (var vocfile in vocfiles)
		// {
		// 	var voc = vocLoader.Load(vocfile);
		// 	if (voc != null)
		// 	{
		// 		audioplayer.Stream = voc.toWav();
		// 		audioplayer.Play();
		// 		while (audioplayer.Playing)
		// 		{
		// 			System.Threading.Thread.Sleep(1000);
		// 		}
		// 	}
		// }


		// var mdl = modelloader.DecodeModel(1);
		// //File.WriteAllText("c:\\temp\\mdl.txt",mdl.commands);
		// int vindex=0;
		// var nd = GetNode<Node3D>("/root/Node3D");
		// string code="";
		// foreach (var v in mdl.verts)
		// {
		// 	if (vindex<= mdl.NoOfVerts)
		// 	{
		// 		Label3D obj_lbl = new();
		// 		obj_lbl.Text = $"{vindex}";
		// 		obj_lbl.Position = new Vector3(v.X,v.Z,v.Y) * new Vector3(8,8,8);
		// 		code+= $"v[{vindex}] = new Vector3({v.X}f, {v.Z}f, {v.Y}f);\n";
		// 		//obj_lbl.Font = font;
		// 		obj_lbl.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
		// 		nd.CallDeferred("add_child",obj_lbl);
		// 	}
		// 	vindex++;
		// }
		// //Debug.Print (code);
		// //File.WriteAllText("c:\\temp\\mdlcode.txt",code);
		// cam.Position= Vector3.Zero;

		//Random rnd = new Random();
		//var index = rnd.Next(8);
		//Debug.Print(index.ToString());

		//grey.Texture = shade.shadesdata[gamesettings.lightlevel].FullShadingImage();

		//grey.Texture = PaletteLoader.AllLightMaps(PaletteLoader.light); //PaletteLoader.light[5].toImage();
		//var textureloader = new TextureLoader();
		//var a_texture = textureloader.LoadImageAt(index);
		//grey.Texture=a_texture;
		//var bytloader = new Underworld.BytLoader();
		//var a_bitmap = bytloader.LoadImageAt(index);

		// create the texture for the mesh
		//ImageTexture textureForMesh=new();
		//textureForMesh.SetImage(a_texture);
		//textureForMesh.SetImage(a_bitmap);

		// CreateMesh(a_texture);

		//update the mesh with a new material.
		//var material = mesh.GetActiveMaterial(0) as StandardMaterial3D; // or your shader...
		//material!.AlbedoTexture = a_bitmap; // shader parameter, etc.
		//material.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;

		//Load a sprte and apply it to the 2d and 3d sprties
		GRLoader gr = new GRLoader(GRLoader.OBJECTS_GR, GRLoader.GRShaderMode.BillboardSpriteShader);
		//var a_sprite = gr.LoadImageAt(index);
		//sprite_3d.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;
		//sprite_2d.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;

		//sprite_2d.Texture = a_sprite;
		//sprite_3d.Texture = a_sprite;

		//Load strings
		//GetTree().Root.GetNode("Node3D").GetNode<Button>("Button").Text = StringLoader.GetString(1, index);

		//test object combinations
		// for (int obj_a=0; obj_a<463; obj_a++)
		// {
		// 	for (int obj_b=0; obj_b<463; obj_b++)
		// 	{
		// 		var cmb = objectCombination.GetCombination(obj_a,obj_b);
		// 		if (cmb!=null)
		// 		{
		// 			Debug.Print(StringLoader.GetString(4,obj_a) + " + " + StringLoader.GetString(4,obj_b) + " = " + StringLoader.GetString(4,cmb.Obj_Out));					
		// 		}
		// 	}
		// }

		// for (int o = 0; o<463;o++)
		// {
		// 	Debug.Print( StringLoader.GetObjectNounUW(o) + " Height" + commonObjDat.height(o) + " radius " + commonObjDat.radius(o) + " monetary " + commonObjDat.monetaryvalue(o) );
		// }


		// var weaponloader = new WeaponsLoader(0);
		// var a_weapon = weaponloader.LoadImageAt(index);
		// weapon_2d.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
		// weapon_2d.Texture = a_weapon;

		// var critloader = new CritLoader(index);
		// var a_critter = critloader.critter.AnimInfo.animSprites[index];
		// critter_3d.Texture = a_critter;
		// critter_3d.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;

		//var main_windowgr = new GRLoader(GRLoader.ThreeDWIN_GR);
		// var uielem = GetNode<TextureRect>("/root/Node3D/UI/3DWin");
		// var mainIndex = BytLoader.MAIN_BYT;
		// if (UWClass._RES == UWClass.GAME_UW2)
		// {
		// 	mainIndex = BytLoader.UW2ThreeDWin_BYT;
		// }
		// var ThreeDWinImg = bytloader.LoadImageAt(mainIndex, true);

		//uielem.Texture = ThreeDWinImg;
		//uielem.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;


		// var cuts = new CutsLoader(Path.Combine(UWClass.BasePath,"CUTS","CS000.N02"));
		// var cutimg = cuts.ImageCache[index];
		// // var cutstex = new ImageTexture();
		// // cutstex.SetImage(cutimg);
		// uielem.Texture=cutimg;
		// uielem.TextureFilter=CanvasItem.TextureFilterEnum.Nearest;



		uwUI.InitUI();
		messageScroll.AddString(GameStrings.GetString(1, 13));


		if (uwsettings.instance.levarkfolder.ToUpper() != "DATA")
        {
            //load player dat
            playerdat.Load(uwsettings.instance.levarkfolder);
            Debug.Print($"Your name is {playerdat.CharName}");
            LoadTileMap(playerdat.dungeon_level - 1, gr);

            Debug.Print($"You are at x:{playerdat.X} y:{playerdat.Y} z:{playerdat.Z}");
            Debug.Print($"You are at x:{playerdat.tileX} {playerdat.xpos} y:{playerdat.tileY} {playerdat.ypos} z:{playerdat.zpos}");
            cam.Position = uwObject.GetCoordinate(playerdat.tileX, playerdat.tileY, playerdat.xpos, playerdat.ypos, playerdat.camerazpos);
            Debug.Print($"Player Heading is {playerdat.heading}");
            cam.Rotation = Vector3.Zero;
            cam.Rotate(Vector3.Up, (float)(Math.PI));//align to the north.
            cam.Rotate(Vector3.Up, (float)(-playerdat.heading / 127f * Math.PI));
            uimanager.SetBody(playerdat.Body, playerdat.isFemale);
            for (int i = 0; i < 8; i++)
            {//Init the backpack indices
                playerdat.SetBackPackIndex(i, playerdat.BackPackObject(i));
            }
			
            //set paperdoll
            uimanager.UpdateInventoryDisplay();

            //Load rune slots
            for (int i = 0; i < 24; i++)
            {
                uimanager.SetRuneInBag(i, playerdat.GetRune(i));
            }
            uimanager.RedrawSelectedRuneSlots();

            //Set the playerlight level;
            uwsettings.instance.lightlevel = light.BrightestLight();

            RenderingServer.GlobalShaderParameterSet("cutoffdistance", shade.GetViewingDistance(uwsettings.instance.lightlevel));
            RenderingServer.GlobalShaderParameterSet("shades", shade.shadesdata[uwsettings.instance.lightlevel].ToImage());
        }
        else
		{
			Random r = new Random();
			playerdat.InitEmptyPlayer();
			playerdat.tileX = -(int)(cam.Position.X/1.2f);
			playerdat.tileY = (int)(cam.Position.Z/1.2f);

			playerdat.dungeon_level = uwsettings.instance.level+1;
			var isFemale = r.Next(0, 2) == 1;
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

			playerdat.Body = r.Next(0, 4);
			uimanager.SetBody(playerdat.Body, playerdat.isFemale);
			

			

			LoadTileMap(gamesettings.level, gr);
		}
	}



    public void LoadTileMap(int newLevelNo, GRLoader grObjects)
	{
		grObjects.UseRedChannel = true;

		Node3D worldobjects = GetNode<Node3D>("/root/Underworld/worldobjects");
		Node3D the_tiles = GetNode<Node3D>("/root/Underworld/tilemap");
		//tilemap/Tile_30_00
		LevArkLoader.LoadLevArkFileData(folder: uwsettings.instance.levarkfolder);
		Underworld.TileMap.current_tilemap = new(newLevelNo);

		Underworld.TileMap.current_tilemap.BuildTileMapUW(newLevelNo, Underworld.TileMap.current_tilemap.lev_ark_block, Underworld.TileMap.current_tilemap.tex_ark_block, Underworld.TileMap.current_tilemap.ovl_ark_block);
		Underworld.ObjectCreator.GenerateObjects(worldobjects, Underworld.TileMap.current_tilemap.LevelObjects, grObjects, Underworld.TileMap.current_tilemap);
		the_tiles.Position = new Vector3(0f, 0f, 0f);
		tileMapRender.GenerateLevelFromTileMap(the_tiles, worldobjects, UWClass._RES, Underworld.TileMap.current_tilemap, Underworld.TileMap.current_tilemap.LevelObjects, false);
		
		switch (UWClass._RES)
		{
			case UWClass.GAME_UW2:	
				automap.automaps = new automap[80];break;		
			default:
				automap.automaps = new automap[9];break;
		}
		automap.automaps[newLevelNo] = new automap(newLevelNo);
		string auto="";
		for (int y=63; y>=0; y--)
		{
			for (int x=0; x<64; x++)
			{
				auto+=automap.automaps[newLevelNo].tiles[x,y].tileType.ToString("0#") + ",";
			}
			auto+="\n";
		}
		//Debug.Print(auto);
		//File.WriteAllText("c:\\temp\\automap.txt", auto);
		uwsettings.instance.lightlevel = light.BrightestLight();
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		int tileX = -(int)(cam.Position.X/1.2f);
		int tileY = (int)(cam.Position.Z/1.2f);

		lblPositionDebug.Text = $"{cam.Position.ToString()} {tileX} {tileY}";
		 if ((tileX <64) && (tileX>=0) && (tileY <64) && (tileY>=0)) 
		 {
		// 	//automap.currentautomap.tiles[tileX,tileX].visited = true;
			if ((playerdat.tileX!=tileX) || (playerdat.tileY!=tileY))
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
			//var camera3D = GetNode<Camera3D>("Camera3D");
			var from = cam.ProjectRayOrigin(eventMouseButton.Position);
			var to = from + cam.ProjectRayNormal(eventMouseButton.Position) * RayLength;
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
							case uimanager.InteractionModes.ModeLook:
								//Do a look interaction with the object
								look.LookAt(objindex, Underworld.TileMap.current_tilemap.LevelObjects);
								break;
							case uimanager.InteractionModes.ModeUse:
								//do a use interaction with the object.
								use.Use(objindex, Underworld.TileMap.current_tilemap.LevelObjects);
								break;
						}
					}
				}
			}
			// foreach (var item in result)
			// {
			// 	Debug.Print(item.ToString());
			// }
		}
	}

}
