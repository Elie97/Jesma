using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SimulationVéhicule
{
    public class CaméraSubjective : Caméra
    {
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        const float ACCÉLÉRATION = 0.001f;
        const float VITESSE_INITIALE_ROTATION = 5f;
        const float VITESSE_INITIALE_TRANSLATION = 10f;
        const float DELTA_LACET = MathHelper.Pi / 180; // 1 degré à la fois
        const float DELTA_TANGAGE = MathHelper.Pi / 180; // 1 degré à la fois
        const float DELTA_ROULIS = MathHelper.Pi / 180; // 1 degré à la fois
        const float RAYON_COLLISION = 1f;

        public Vector3 Direction { get; set; }
        public Vector3 Latéral { get; set; }
        float VitesseTranslation { get; set; }
        float VitesseRotation { get; set; }
        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        InputManager GestionInput { get; set; }

        public bool CaméraMobile { get; set; }

        bool estEnZoom;
        bool EstEnZoom
        {
            get { return estEnZoom; }
            set
            {
                float ratioAffichage = Game.GraphicsDevice.Viewport.AspectRatio;
                estEnZoom = value;
                if (estEnZoom)
                {
                    CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF / 2, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
                }
                else
                {
                    CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
                }
            }
        }

        public CaméraSubjective(Game jeu, Vector3 positionCaméra, Vector3 cible, Vector3 orientation, float intervalleMAJ, bool caméraMobile)
            : base(jeu)
        {
            IntervalleMAJ = intervalleMAJ;
            CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
            CréerPointDeVue(positionCaméra, cible, orientation);
            EstEnZoom = false;
            CaméraMobile = caméraMobile;
        }

        public override void Initialize()
        {
            VitesseRotation = VITESSE_INITIALE_ROTATION;
            VitesseTranslation = VITESSE_INITIALE_TRANSLATION;
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
        }

        public override void CréerPointDeVue()
        {
            Direction = Vector3.Normalize(Direction);
            OrientationVerticale = Vector3.Normalize(OrientationVerticale);
            Vue = Matrix.CreateLookAt(Position, Position + Direction, OrientationVerticale);
            GénérerFrustum();
        }

        protected override void CréerPointDeVue(Vector3 position, Vector3 cible, Vector3 orientation)
        {
            Position = position;
            OrientationVerticale = orientation;
            Cible = cible;
            Direction = cible - Position;
            Latéral = Vector3.Cross(OrientationVerticale, Direction);
            OrientationVerticale = Vector3.Cross(Direction, Latéral);
            CréerPointDeVue();
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            GestionClavier();
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                if (CaméraMobile)
                {
                    if (GestionInput.EstEnfoncée(Keys.LeftShift) || GestionInput.EstEnfoncée(Keys.RightShift))
                    {
                        GérerAccélération();
                        GérerDéplacement();
                        GérerRotation();
                        CréerPointDeVue();
                    }
                    CréerPointDeVue();   
                }
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        private int GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? 1 : 0;
        }

        private void GérerAccélération()
        {
            int valAccélération = (GérerTouche(Keys.Subtract) + GérerTouche(Keys.OemMinus)) - (GérerTouche(Keys.Add) + GérerTouche(Keys.OemPlus));
            if (valAccélération != 0)
            {
                IntervalleMAJ += ACCÉLÉRATION * valAccélération;
                IntervalleMAJ = MathHelper.Max(INTERVALLE_MAJ_STANDARD, IntervalleMAJ);
            }
        }

        private void GérerDéplacement()
        {
            Vector3 nouvellePosition = Position;
            float déplacementDirection = (GérerTouche(Keys.W) - GérerTouche(Keys.S)) * VitesseTranslation;
            float déplacementLatéral = (GérerTouche(Keys.A) - GérerTouche(Keys.D)) * VitesseTranslation;

            if (GestionInput.EstEnfoncée(Keys.W))
            {
                Position = déplacementDirection * Direction + Position;
            }
            if (GestionInput.EstEnfoncée(Keys.S))
            {
                Position = Position - Direction * -déplacementDirection;
            }

            Latéral = Vector3.Cross(Direction, OrientationVerticale);
            if (GestionInput.EstEnfoncée(Keys.D))
            {
                Position = -déplacementLatéral * Latéral + Position;
            }
            if (GestionInput.EstEnfoncée(Keys.A))
            {
                Position = Position - Latéral * déplacementLatéral;
            }
        }

        private void GérerRotation()
        {
            if (GestionInput.EstEnfoncée(Keys.Left) || GestionInput.EstEnfoncée(Keys.Right))
            {
                GérerLacet();
            }
            if (GestionInput.EstEnfoncée(Keys.PageUp) || GestionInput.EstEnfoncée(Keys.PageDown))
            {
                GérerRoulis();
            }
            if (GestionInput.EstEnfoncée(Keys.Up) || GestionInput.EstEnfoncée(Keys.Down))
            {
                GérerTangage();
            }
        }

        private void GérerLacet()
        {
            Matrix lacet = new Matrix();
            if (GestionInput.EstEnfoncée(Keys.Left))
            {
                lacet = Matrix.CreateFromAxisAngle(OrientationVerticale * VitesseRotation, DELTA_LACET);
            }
            if (GestionInput.EstEnfoncée(Keys.Right))
            {
                lacet = Matrix.CreateFromAxisAngle(-OrientationVerticale * VitesseRotation, DELTA_LACET);
            }
            Direction = Vector3.Transform(Direction, lacet);
        }

        private void GérerTangage()
        {
            Matrix tangage = new Matrix();
            if (GestionInput.EstEnfoncée(Keys.Down))
            {
                Latéral = Vector3.Cross(Direction, OrientationVerticale) * VitesseRotation;
            }
            if (GestionInput.EstEnfoncée(Keys.Up))
            {
                Latéral = Vector3.Cross(OrientationVerticale, Direction) * VitesseRotation;
            }
            tangage = Matrix.CreateFromAxisAngle(Latéral, DELTA_TANGAGE);
            Direction = Vector3.Transform(Direction, tangage);
            tangage = Matrix.CreateFromAxisAngle(Latéral, DELTA_ROULIS);
            OrientationVerticale = Vector3.Transform(Vector3.Normalize(OrientationVerticale), tangage);
        }

        private void GérerRoulis()
        {
            Matrix roulis = new Matrix();
            if (GestionInput.EstEnfoncée(Keys.PageUp))
            {
                roulis = Matrix.CreateFromAxisAngle(Direction * VitesseRotation, DELTA_ROULIS);
            }
            if (GestionInput.EstEnfoncée(Keys.PageDown))
            {
                roulis = Matrix.CreateFromAxisAngle(-Direction * VitesseRotation, DELTA_ROULIS);
            }
            OrientationVerticale = Vector3.Transform(OrientationVerticale, roulis);
        }

        private void GestionClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Z))
            {
                EstEnZoom = !EstEnZoom;
            }
        }

        public void ChangerTypeCaméra()
        {
            CaméraMobile = !CaméraMobile;
        }

        public void ChangerTypeCaméra(bool type)
        {
            CaméraMobile = type;
        }
    }
}
