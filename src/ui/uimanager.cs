using System.Diagnostics;
using Godot;

namespace Underworld
{
	public partial class uimanager : Node2D
	{
		public static uimanager instance;

		[Export] public Font Font4X5P;
		[Export] public Font Font5X6I;
		[Export] public Font Font5X6P;
		[Export] public Font FontBig;
		public enum InteractionModes
		{
			ModeOptions = 0,
			ModeTalk = 1,
			ModePickup = 2,
			ModeLook = 3,
			ModeAttack = 4,
			ModeUse = 5
		};

		public static InteractionModes InteractionMode = InteractionModes.ModeUse;

		[Export]
		public Camera3D cam;
		[Export] public Node3D freelook;

		// [Export] public SubViewportContainer uwviewport;
		// [Export] public SubViewport uwsubviewport;

		[Export] public mouseCursor mousecursor;
		[Export] public CanvasLayer uw1UI;
		[Export] public CanvasLayer uw2UI;

		[Export] public TextureRect mainwindowUW1;
		[Export] public TextureRect mainwindowUW2;

		[Export] public Label messageScrollUW1;
		[Export] public Label messageScrollUW2;

		[Export] public TextureRect placeholderuw1;
		[Export] public TextureRect placeholderuw2;

		//Array to store the interaction mode mo
		[Export] public Godot.TextureButton[] InteractionButtonsUW1 = new Godot.TextureButton[6];
		[Export] public Godot.TextureButton[] InteractionButtonsUW2 = new Godot.TextureButton[6];

		public static bool Fullscreen = false;
		public static GRLoader grCursors;
		public static GRLoader grObjects;
		public static GRLoader grLfti;
		public static GRLoader grOptBtns;
		private ImageTexture[] UW2OptBtnsOff;
		private ImageTexture[] UW2OptBtnsOn;

		public static GRLoader grBody;
		public static GRLoader grArmour_F;
		public static GRLoader grArmour_M;

		[Export] public TextureRect Body;
		[Export] public TextureRect Helm;
		[Export] public TextureRect Armour;
		[Export] public TextureRect Leggings;
		[Export] public TextureRect Boots;
		[Export] public TextureRect Gloves;
		[Export] public TextureRect RingLeftUW1;
		[Export] public TextureRect RingRightUW2;
		[Export] public TextureRect RightShoulder;
		[Export] public TextureRect LeftShoulder;
		[Export] public TextureRect RightHand;
		[Export] public TextureRect LeftHand;
		[Export] public TextureRect[] Backpack = new TextureRect[8];

		public static BytLoader byt;

		static uimanager()
		{

		}

		public void InitUI()
		{
			instance = this;
			grCursors = new GRLoader(GRLoader.CURSORS_GR, GRLoader.GRShaderMode.UIShader);
			grObjects = new GRLoader(GRLoader.OBJECTS_GR, GRLoader.GRShaderMode.UIShader);
			grObjects.UseRedChannel = true;
			grLfti = new GRLoader(GRLoader.LFTI_GR, GRLoader.GRShaderMode.UIShader);
			grArmour_F = new GRLoader(GRLoader.ARMOR_F_GR, GRLoader.GRShaderMode.UIShader);
			grArmour_M = new GRLoader(GRLoader.ARMOR_M_GR, GRLoader.GRShaderMode.UIShader);

			// for (int i=0; i<grArmour_F.ImageCache.GetUpperBound(0);i++)
			// {
			//    grArmour_F.LoadImageAt(i).GetImage().SavePng(System.IO.Path.Combine("c:\\temp",$"armourf_{i}.png"));
			// }
			if (UWClass._RES == UWClass.GAME_UW2)
			{
				UW2OptBtnsOff = new ImageTexture[6];
				UW2OptBtnsOn = new ImageTexture[6];
				grOptBtns = new GRLoader(GRLoader.OPTBTNS_GR, GRLoader.GRShaderMode.UIShader);
				var Off = grOptBtns.LoadImageAt(0).GetImage();
				var On = grOptBtns.LoadImageAt(1).GetImage();
				UW2OptBtnsOff[4] = ArtLoader.CropImage(Off, new Rect2I(0, 0, 25, 14)); //attack button off
				UW2OptBtnsOn[4] = ArtLoader.CropImage(On, new Rect2I(0, 0, 25, 14)); //attack button on

				UW2OptBtnsOff[5] = ArtLoader.CropImage(Off, new Rect2I(26, 0, 25, 14)); //use button off
				UW2OptBtnsOn[5] = ArtLoader.CropImage(On, new Rect2I(26, 0, 25, 14)); //use button on

				UW2OptBtnsOff[2] = ArtLoader.CropImage(Off, new Rect2I(52, 0, 25, 14)); //pickup button off
				UW2OptBtnsOn[2] = ArtLoader.CropImage(On, new Rect2I(52, 0, 25, 14)); //pickup button on

				UW2OptBtnsOff[1] = ArtLoader.CropImage(Off, new Rect2I(0, 15, 25, 14)); //talk button off
				UW2OptBtnsOn[1] = ArtLoader.CropImage(On, new Rect2I(0, 15, 25, 14)); //talk button on

				UW2OptBtnsOff[3] = ArtLoader.CropImage(Off, new Rect2I(26, 15, 25, 14)); //look button off
				UW2OptBtnsOn[3] = ArtLoader.CropImage(On, new Rect2I(26, 15, 25, 14)); //look button on

				UW2OptBtnsOff[0] = ArtLoader.CropImage(Off, new Rect2I(52, 15, 25, 14)); //options button off
				UW2OptBtnsOn[0] = ArtLoader.CropImage(On, new Rect2I(52, 15, 25, 14)); //option button on

				//Move paperdoll
				var offset = new Vector2(-8, -13);
				Body.Position += offset;
				Helm.Position += offset;
				Boots.Position += offset;
				Gloves.Position += offset;
				Leggings.Position += offset;
				Armour.Position += offset;


			}
			byt = new BytLoader();

			mousecursor.InitCursor();
			EnableDisable(placeholderuw1, false);
			EnableDisable(placeholderuw2, false);

			EnableDisable(uw1UI, UWClass._RES == UWClass.GAME_UW1);
			EnableDisable(uw2UI, UWClass._RES != UWClass.GAME_UW1);

			//EnableDisable(mainwindowUW1, UWClass._RES == UWClass.GAME_UW1);
			//EnableDisable(mainwindowUW2, UWClass._RES != UWClass.GAME_UW1);  

			//EnableDisable(messageScrollUW1, UWClass._RES == UWClass.GAME_UW1);
			//EnableDisable(messageScrollUW2, UWClass._RES != UWClass.GAME_UW1);  

			switch (UWClass._RES)
			{
				case UWClass.GAME_UW2:
					mainwindowUW2.Texture = byt.LoadImageAt(BytLoader.UW2ThreeDWin_BYT, true);
					if (!Fullscreen)
					{
						// uwviewport.SetSize(new Vector2(840f,512f));
						// uwviewport.Position = new Vector2(62f,62f);
						// uwsubviewport.Size = new Vector2I(840,512);
					}

					for (int i = 0; i <= InteractionButtonsUW2.GetUpperBound(0); i++)
					{
						InteractionButtonsUW2[i].TexturePressed = UW2OptBtnsOn[i]; // grLfti.LoadImageAt(i*2 + 1,false);
						InteractionButtonsUW2[i].TextureNormal = UW2OptBtnsOff[i]; //grLfti.LoadImageAt(i*2,false);  
						InteractionButtonsUW2[i].SetPressedNoSignal((i == (int)InteractionMode));
					}

					break;
				default:
					mainwindowUW1.Texture = byt.LoadImageAt(BytLoader.MAIN_BYT, true);
					if (!Fullscreen)
					{
						// uwviewport.SetSize(new Vector2(700f,456f));
						// uwviewport.Position = new Vector2(200f,72f);
						// uwsubviewport.Size = new Vector2I(700,456);
					}
					//grLfti.ExportImages("c:\\temp\\lfti\\");
					for (int i = 0; i <= InteractionButtonsUW1.GetUpperBound(0); i++)
					{
						InteractionButtonsUW1[i].TexturePressed = grLfti.LoadImageAt(i * 2 + 1, false);
						InteractionButtonsUW1[i].TextureNormal = grLfti.LoadImageAt(i * 2, false);
						InteractionButtonsUW1[i].SetPressedNoSignal((i == (int)InteractionMode));
					}
					break;
			}

		}

		static void EnableDisable(Control ctrl, bool state)
		{
			if (ctrl != null)
			{
				ctrl.Visible = state;
			}
		}

		static void EnableDisable(CanvasLayer ctrl, bool state)
		{
			if (ctrl != null)
			{
				ctrl.Visible = state;
			}
		}


		public void _paperdoll_gui_input(InputEvent inputEvent)
		{
			Debug.Print("HERE");
		}



		public void InteractionModeToggle(InteractionModes index)
		{
			Debug.Print($"Press {index}");

			if (UWClass._RES == UWClass.GAME_UW2)
			{

				for (int i = 0; i <= instance.InteractionButtonsUW2.GetUpperBound(0); i++)
				{
					InteractionButtonsUW2[i].SetPressedNoSignal(i == (int)(index));
					if (i == (int)(index))
					{
						InteractionMode = index;
					}
				}
			}
			else
			{
				for (int i = 0; i <= instance.InteractionButtonsUW1.GetUpperBound(0); i++)
				{
					InteractionButtonsUW1[i].SetPressedNoSignal(i == (int)(index));
					if (i == (int)(index))
					{
						InteractionMode = index;
					}
				}
			}
		}

		public static void SetBody(int body, bool isFemale)
		{
			int MaleOrFemale = 0;
			if (isFemale)
			{
				MaleOrFemale = 1;
			}
			if (grBody == null)
			{
				grBody = new GRLoader(GRLoader.BODIES_GR, GRLoader.GRShaderMode.UIShader);
			}
			instance.Body.Texture = grBody.LoadImageAt(body + (5 * MaleOrFemale));
		}

		public static void SetHelm(bool isFemale, int SpriteNo = -1)
		{
			if (SpriteNo == -1)
			{ //clear the slot
				instance.Helm.Texture = null;
			}
			else
			{
				instance.Helm.Texture = grArmour(isFemale).LoadImageAt(SpriteNo);
			}
		}

		public static void SetArmour(bool isFemale, int SpriteNo = -1)
		{
			if (SpriteNo == -1)
			{ //clear the slot
				instance.Armour.Texture = null;
			}
			else
			{
				instance.Armour.Texture = grArmour(isFemale).LoadImageAt(SpriteNo);
			}
		}

		public static void SetLeggings(bool isFemale, int SpriteNo = -1)
		{
			if (SpriteNo == -1)
			{ //clear the slot
				instance.Leggings.Texture = null;
			}
			else
			{
				instance.Leggings.Texture = grArmour(isFemale).LoadImageAt(SpriteNo);
			}
		}

		public static void SetBoots(bool isFemale, int SpriteNo = -1)
		{
			if (SpriteNo == -1)
			{ //clear the slot
				instance.Boots.Texture = null;
			}
			else
			{
				instance.Boots.Texture = grArmour(isFemale).LoadImageAt(SpriteNo);
			}
		}

		public static void SetGloves(bool isFemale, int SpriteNo = -1)
		{
			if (SpriteNo == -1)
			{ //clear the slot
				instance.Gloves.Texture = null;
			}
			else
			{
				instance.Gloves.Texture = grArmour(isFemale).LoadImageAt(SpriteNo);
			}
		}

		public static void SetRightShoulder(int SpriteNo = -1)
		{
			if (SpriteNo == -1)
			{ //clear the slot
				instance.RightShoulder.Texture = null;
			}
			else
			{
				instance.RightShoulder.Texture = grObjects.LoadImageAt(SpriteNo);
				instance.RightShoulder.Material = grObjects.GetMaterial(SpriteNo);
			}
		}

		public static void SetLeftShoulder(int SpriteNo = -1)
		{
			if (SpriteNo == -1)
			{ //clear the slot
				instance.LeftShoulder.Texture = null;
			}
			else
			{
				instance.LeftShoulder.Texture = grObjects.LoadImageAt(SpriteNo);
				instance.LeftShoulder.Material = grObjects.GetMaterial(SpriteNo);
			}
		}

		public static void SetRightHand(int SpriteNo = -1)
		{
			if (SpriteNo == -1)
			{ //clear the slot
				instance.RightHand.Texture = null;
			}
			else
			{
				instance.RightHand.Texture = grObjects.LoadImageAt(SpriteNo);
				instance.RightHand.Material = grObjects.GetMaterial(SpriteNo);
			}
		}

		public static void SetLeftHand(int SpriteNo = -1)
		{
			if (SpriteNo == -1)
			{ //clear the slot
				instance.LeftHand.Texture = null;
			}
			else
			{
				instance.LeftHand.Texture = grObjects.LoadImageAt(SpriteNo);
				instance.LeftHand.Material = grObjects.GetMaterial(SpriteNo);
			}
		}


		public static void SetBackPack(int slot, int SpriteNo = -1)
		{
			if (SpriteNo == -1)
			{ //clear the slot
				instance.Backpack[slot].Texture = null;
			}
			else
			{
				instance.Backpack[slot].Texture = grObjects.LoadImageAt(SpriteNo);
				instance.Backpack[slot].Material = grObjects.GetMaterial(SpriteNo);
			}
		}

		public static GRLoader grArmour(bool isFemale)
		{
			if (isFemale)
			{
				if (grArmour_F == null)
				{
					grArmour_F = new GRLoader(GRLoader.ARMOR_F_GR, GRLoader.GRShaderMode.UIShader);
				}
				return grArmour_F;
			}
			else
			{
				if (grArmour_M == null)
				{
					grArmour_M = new GRLoader(GRLoader.ARMOR_M_GR, GRLoader.GRShaderMode.UIShader);
				}
				return grArmour_M;
			}
		}

		private void _paperdoll_gui_input(InputEvent @event, string extra_arg_0)
		{
			// Replace with function body.
			if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
			{
				var obj = 0;
				Debug.Print($"{extra_arg_0}");
				switch (extra_arg_0)
				{
					case "LeftShoulder": {obj = playerdat.LeftShoulder;break;}
					case "RightShoulder": {obj = playerdat.RightShoulder;break;}
					case "LeftHand": {obj = playerdat.LeftHand;break;}
					case "RightHand": {obj = playerdat.RightHand;break;}
					case "Back0": {obj = playerdat.BackPack(0);break;}
					case "Back1": {obj = playerdat.BackPack(1);break;}
					case "Back2": {obj = playerdat.BackPack(2);break;}
					case "Back3": {obj = playerdat.BackPack(3);break;}
					case "Back4": {obj = playerdat.BackPack(4);break;}
					case "Back5": {obj = playerdat.BackPack(5);break;}
					case "Back6": {obj = playerdat.BackPack(6);break;}
					case "Back7": {obj = playerdat.BackPack(7);break;}
					default:
						Debug.Print("Unimplemented inventory slot"); break;
				}

				//Do action appropiate to the interaction mode verb. use 
				if (obj != 0)
				{
					switch (InteractionMode)
					{
						case InteractionModes.ModeUse:
							use.Use(obj, playerdat.InventoryObjects); break;
						case InteractionModes.ModeLook:
							look.LookAt(obj, playerdat.InventoryObjects); break;
						default:
							Debug.Print("Unimplemented inventory use verb-object combination"); break;
					}
				}
			}
		}
	} //end class
}   //end namespace
