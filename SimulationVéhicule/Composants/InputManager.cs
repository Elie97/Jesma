using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace SimulationVéhicule
{
    public class InputManager : Microsoft.Xna.Framework.GameComponent
    {
        Keys[] AnciennesTouches { get; set; }
        Keys[] NouvellesTouches { get; set; }
        KeyboardState ÉtatClavier { get; set; }
        MouseState ÉtatSouris { get; set; }
        MouseState AncienÉtatSouris { get; set; }
        bool EstSourisActive
        {
            get { return EstNouveauClicDroit() || EstNouveauClicGauche(); }
        }

        public InputManager(Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            AnciennesTouches = new Keys[0];
            NouvellesTouches = new Keys[0];
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            AnciennesTouches = NouvellesTouches;
            AncienÉtatSouris = ÉtatSouris;
            ÉtatClavier = Keyboard.GetState();
            ÉtatSouris = Mouse.GetState();
            NouvellesTouches = ÉtatClavier.GetPressedKeys();
            base.Update(gameTime);
        }

        public bool EstClavierActivé
        {
            get { return NouvellesTouches.Length > 0; }
        }

        public bool EstEnfoncée(Keys touche)
        {
            return ÉtatClavier.IsKeyDown(touche);
        }

        public bool EstNouvelleTouche(Keys touche)
        {
            int NbTouches = AnciennesTouches.Length;
            bool EstNouvelleTouche = ÉtatClavier.IsKeyDown(touche);
            int i = 0;
            while (i < NbTouches && EstNouvelleTouche)
            {
                EstNouvelleTouche = AnciennesTouches[i] != touche;
                ++i;
            }
            return EstNouvelleTouche;
        }

        public bool EstAncienClicDroit()
        {
            return AncienÉtatSouris.RightButton == ButtonState.Pressed;
        }

        public bool EstAncienClicGauche()
        {
            return AncienÉtatSouris.LeftButton == ButtonState.Pressed;
        }

        public bool EstNouveauClicDroit()
        {
            bool estNouveauClicDroit = false;
            if (EstAncienClicDroit() == false)
            {
                estNouveauClicDroit = ÉtatSouris.RightButton == ButtonState.Pressed;
            }
            return estNouveauClicDroit;
        }

        public bool EstNouveauClicGauche()
        {
            bool estNouveauClicGauche = false;
            if (EstAncienClicGauche() == false)
            {
                estNouveauClicGauche = ÉtatSouris.LeftButton == ButtonState.Pressed;
            }
            return estNouveauClicGauche;
        }

        public Point GetPositionSouris()
        {
            return new Point(ÉtatSouris.X, ÉtatSouris.Y);
        }
    }
}