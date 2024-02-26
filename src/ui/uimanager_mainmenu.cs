using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Godot;
using Peaky.Coroutines;

namespace Underworld
{
    public partial class uimanager : Node2D
    {
        [ExportGroup("MainMenu")]
        [Export]
        public Panel PanelMainMenu;
        [Export]
        public TextureRect MainMenuBG;

        [Export]
        public TextureRect[] MainMenuButtons = new TextureRect[4];

        [Export] public Label LoadingLabel;

        [Export] public Label[] SaveGamesNames = new Label[4];

        private void InitMainMenu()
        {

            MainMenuBG.Texture = bitmaps.LoadImageAt(BytLoader.OPSCR_BYT);
            //MainMenuBG.Material = bitmaps.GetMaterial(BytLoader.OPSCR_BYT);
            LoadingLabel.Text = "";
            TurnButtonsOff();
            HideSaves();

        }

        private void TurnButtonsOff()
        {
            for (int i = 0; i < 4; i++)
            {
                MainMenuButtons[i].Texture = grOptbtn.LoadImageAt(i * 2);
            }
        }

        private void HideButtons()
        {
            for (int i = 0; i < 4; i++)
            {
                EnableDisable(MainMenuButtons[i], false);
            }
        }


        private void HideSaves()
        {
            for (int i = 0; i < 4; i++)
            {
                EnableDisable(SaveGamesNames[i], false);
            }
        }

        private void _on_introduction_mouse_entered()
        {
            TurnButtonsOff();
            MainMenuButtons[0].Texture = grOptbtn.LoadImageAt(1);
        }


        private void _on_introduction_mouse_exited()
        {
            TurnButtonsOff();
            MainMenuButtons[0].Texture = grOptbtn.LoadImageAt(0);
        }


        private void _on_create_character_mouse_entered()
        {
            TurnButtonsOff();
            MainMenuButtons[1].Texture = grOptbtn.LoadImageAt(3);
        }


        private void _on_create_character_mouse_exited()
        {
            TurnButtonsOff();
            MainMenuButtons[1].Texture = grOptbtn.LoadImageAt(2);
        }


        private void _on_acknowledgements_mouse_entered()
        {
            TurnButtonsOff();
            MainMenuButtons[2].Texture = grOptbtn.LoadImageAt(5);
        }


        private void _on_acknowledgements_mouse_exited()
        {
            TurnButtonsOff();
            MainMenuButtons[2].Texture = grOptbtn.LoadImageAt(4);
        }


        private void _on_journey_onwards_mouse_entered()
        {
            TurnButtonsOff();
            MainMenuButtons[3].Texture = grOptbtn.LoadImageAt(7);
        }


        private void _on_journey_onwards_mouse_exited()
        {
            TurnButtonsOff();
            MainMenuButtons[3].Texture = grOptbtn.LoadImageAt(6);
        }


        /// <summary>
        /// Load the save game specified in the config file
        /// </summary>
        /// <param name="event"></param>
        private void _on_journey_onwards_gui_input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                HideButtons();
                for (int i = 1; i <= 4; i++)
                {
                    var path = Path.Combine(UWClass.BasePath, $"SAVE{i}", "DESC");
                    if (File.Exists(path))
                    {
                        var savename = File.ReadAllText(path);
                        EnableDisable(SaveGamesNames[i - 1], true);
                        SaveGamesNames[i - 1].Text = savename;
                    }
                    else
                    {
                        EnableDisable(SaveGamesNames[i - 1], false);
                    }
                }

            }
        }

        private IEnumerator ClearMainMenu()
        {
            HideButtons();
            if (UWClass._RES == UWClass.GAME_UW2)
            {
                LoadingLabel.Text = GameStrings.GetString(1, 273);
            }
            else
            {
                LoadingLabel.Text = GameStrings.GetString(1, 257);
            }
            yield return 0;
        }

        private IEnumerator JourneyOnwards(string folder)
        {

            playerdat.LoadPlayerDat(datafolder: folder);

            // //Common launch actions            
            yield return UWTileMap.LoadTileMap(
                    newLevelNo: playerdat.dungeon_level - 1,
                    datafolder: folder,
                    fromMainMenu: true);

            yield return null;

        }

        private void _on_create_character_gui_input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                _ = Coroutine.Run(
                   ClearMainMenu()
                   , main.instance);

                _ = Coroutine.Run(
                    JourneyOnwards("DATA")
                    , main.instance);
            }
        }



        private void _on_save_1_gui_input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                _ = Coroutine.Run(
                   ClearMainMenu()
                   , main.instance);

                _ = Coroutine.Run(
                    JourneyOnwards("SAVE1")
                    , main.instance);
            }
        }

        private void _on_save_2_gui_input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                _ = Coroutine.Run(
                   ClearMainMenu()
                   , main.instance);

                _ = Coroutine.Run(
                    JourneyOnwards("SAVE2")
                    , main.instance);
            }
        }


        private void _on_save_3_gui_input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                _ = Coroutine.Run(
                   ClearMainMenu()
                   , main.instance);

                _ = Coroutine.Run(
                    JourneyOnwards("SAVE3")
                    , main.instance);
            }
        }


        private void _on_save_4_gui_input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                _ = Coroutine.Run(
                   ClearMainMenu()
                   , main.instance);

                _ = Coroutine.Run(
                    JourneyOnwards("SAVE4")
                    , main.instance);
            }
        }

    }//end class
}//end namespace