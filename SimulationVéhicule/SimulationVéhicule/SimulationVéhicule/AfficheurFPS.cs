using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace SimulationVéhicule
{
    public class AfficheurFPS : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const int MARGE_BAS = 10;
        const int MARGE_DROITE = 15;

        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        int CptFrames { get; set; }
        float ValFPS { get; set; }
        string ChaîneFPS { get; set; }

        SpriteBatch GestionSprites { get; set; }
        SpriteFont ArialFont { get; set; }
        Vector2 PositionDroiteBas { get; set; }
        Vector2 PositionChaîne { get; set; }
        Vector2 Dimension { get; set; }

        public AfficheurFPS(Game game, float intervalleMAJ)
            : base(game)
        {
            IntervalleMAJ = intervalleMAJ;
        }

        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;
            ValFPS = 0;
            CptFrames = 0;
            ChaîneFPS = "";
            PositionDroiteBas = new Vector2(Game.Window.ClientBounds.Width - MARGE_DROITE,
                                            Game.Window.ClientBounds.Height - MARGE_BAS);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            ArialFont = Game.Content.Load<SpriteFont>("Fonts/Bebas");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ++CptFrames;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                float oldValFPS = ValFPS;
                ValFPS = CptFrames / TempsÉcouléDepuisMAJ;
                if (oldValFPS != ValFPS)
                {
                    ChaîneFPS = ValFPS.ToString("0");
                    Dimension = ArialFont.MeasureString(ChaîneFPS);
                    PositionChaîne = PositionDroiteBas - Dimension;
                }
                CptFrames = 0;
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.DrawString(ArialFont, ChaîneFPS, PositionChaîne, Color.Tomato, 0,
                                      Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            base.Draw(gameTime);
        }
    }
}