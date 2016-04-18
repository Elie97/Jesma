using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace SimulationVéhicule
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;

        const int KILOMÈTRE = 10000;
        const int MÈTRE = 10;

        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        SpriteBatch GestionSprites { get; set; }

        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        RessourcesManager<Effect> GestionnaireDeShaders { get; set; }
        RessourcesManager<SoundEffect> GestionnaireDeSon { get; set; }
        RessourcesManager<Song> GestionnaireDeMusique { get; set; }

        CaméraSubjective CaméraJeu { get; set; }
        bool CaméraMobile { get; set; }

        Voiture Mustang { get; set; }
        Voiture AI { get; set; }
        LeTerrain2 Carte { get; set; }
        public List<Sol> LaPiste { get; set; }
        List<Voiture> ListeVoiture { get; set; }

        Vector3 PositionCaméra { get; set; }
        float CibleYCaméra { get; set; }
        int VueArrière { get; set; }
        Vector3[] TableauPositionCaméra { get; set; }
        int IndexPositionCaméra { get; set; }

        float TempsÉcouléDepuisMAJ { get; set; }

        public InputManager GestionInput { get; private set; }

        GUI Interface { get; set; }
        Course LaCourse { get; set; }

        //Course
        int PositionUtilisateur { get; set; }
        int NbVoiture { get; set; }
        int NbTours { get; set; }
        int Piste { get; set; }

        int IDVoitureUtilisateur { get; set; }

        int ModeDeJeu { get; set; }

        Client LeClient { get; set; }
        int NbJoueurConnectés { get; set; }

        bool CourseActive { get; set; }
        bool MenuActif { get; set; }
        bool ImageToucheActive { get; set; }
        bool CoursePeutCommencer { get; set; }

        Texture2D Accueil { get; set; }
        Texture2D InputClavier { get; set; }
        Texture2D InputManette { get; set; }
        SpriteFont Bebas { get; set; }

        public Game1()
        {
            PériphériqueGraphique = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            PériphériqueGraphique.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            DebugShapeRenderer.Initialize(GraphicsDevice);
            PériphériqueGraphique.IsFullScreen = false;
            PériphériqueGraphique.PreferredBackBufferWidth = 900;
            PériphériqueGraphique.PreferredBackBufferHeight = 600;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
            PériphériqueGraphique.ApplyChanges();

            //Menu
            MenuActif = false;
            Accueil = Content.Load<Texture2D>("Textures/Accueil");
            InputClavier = Content.Load<Texture2D>("Textures/input");
            InputManette = Content.Load<Texture2D>("Textures/inputManette");
            Bebas = Content.Load<SpriteFont>("Fonts/Bebas");
            CoursePeutCommencer = true;

            //Course
            ImageToucheActive = false;
            CourseActive = false;
            CibleYCaméra = 0;
            VueArrière = 1;
            TableauPositionCaméra = new Vector3[6];
            IndexPositionCaméra = 0;
            Vector3 positionCaméra = new Vector3(0, 20, -5070);
            PositionCaméra = new Vector3(-80, 20, -80);

            Vector3 cibleCaméra = new Vector3(0, 0, 0);
            CaméraJeu = new CaméraSubjective(this, positionCaméra, cibleCaméra, new Vector3(0, 1, 0), INTERVALLE_MAJ_STANDARD, CaméraMobile);

            Components.Add(CaméraJeu);


            ModeDeJeu = 1;
            CréerUneCourse(2, 0);
            Interface = new GUI(this, INTERVALLE_MAJ_STANDARD, "aiguille2", "speedometer3", LaCourse.NbVoiture, LaCourse.NbTours, IDVoitureUtilisateur, new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height), ModeDeJeu);
            Components.Add(Interface);
            //if (CourseActive)
            //{
            //    CréerUneCourse(2, 0);
            //}
            if (ModeDeJeu == 1)
            {
                LeClient = new Client(this, ListeVoiture);
                Components.Add(LeClient);
            }

            GestionInput = new InputManager(this);

            Components.Add(GestionInput);

            Services.AddService(typeof(Caméra), CaméraJeu);
            Services.AddService(typeof(GUI), Interface);

            GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
            GestionnaireDeModèles = new RessourcesManager<Model>(this, "Models");
            GestionnaireDeShaders = new RessourcesManager<Effect>(this, "Effects");
            GestionnaireDeSon = new RessourcesManager<SoundEffect>(this, "Sounds");
            GestionnaireDeMusique = new RessourcesManager<Song>(this, "Songs");

            Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeModèles);
            Services.AddService(typeof(RessourcesManager<Effect>), GestionnaireDeShaders);
            Services.AddService(typeof(RessourcesManager<SoundEffect>), GestionnaireDeSon);
            Services.AddService(typeof(RessourcesManager<Song>), GestionnaireDeMusique);
            GestionSprites = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), GestionSprites);
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(int), IDVoitureUtilisateur);
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
            GérerClavier();
            TempsÉcouléDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (TempsÉcouléDepuisMAJ >= INTERVALLE_MAJ_STANDARD)
            {
                if (ModeDeJeu == 1 && NbJoueurConnectés != 2)
                {
                    CoursePeutCommencer = false;
                }
                CaméraJeu.CréerPointDeVue();

                if (CourseActive && !ImageToucheActive && LaCourse != null)
                {
                    Interface.CourseActive = true;
                    GestionOrientationCaméra();//Si course est activée
                }

                TempsÉcouléDepuisMAJ = 0;
            }


            if (MenuActif && !ImageToucheActive && GestionInput.EstEnfoncée(Keys.G))
            {
                MenuActif = false;
                ImageToucheActive = true;
            }
            if (ImageToucheActive && !CourseActive && GestionInput.EstEnfoncée(Keys.H) && CoursePeutCommencer)
            {
                ImageToucheActive = false;
                //Interface.CourseActive = true;
                CourseActive = true;
                Carte.CourseActive = true;
                foreach (Voiture x in ListeVoiture)
                {
                    x.CourseActive = true;
                }
                foreach (Sol x in LaCourse.LaPiste)
                {
                    x.CourseActive = true;
                }
                LaCourse.CourseActive = true;
            }


            if (ModeDeJeu == 1)
            {
                LeClient.EnvoiInfoDéplacement(IDVoitureUtilisateur);
                NbJoueurConnectés = LeClient.NbJoueursConnectés;
                Interface.NbJoueursConnectés = LeClient.NbJoueursConnectés;
                LaCourse.NbFranchis[1] = LeClient.NbFranchisAdversaire;
            }

            Window.Title = NbJoueurConnectés.ToString() + " - " + MenuActif.ToString();
            //Window.Title = LaCourse.NbFranchis[0].ToString() + " - " + LaCourse.NbFranchis[1].ToString() + " - " + (LaPiste.Count() * NbTours * 2).ToString() + " - " + LaCourse.CourseTerminée.ToString();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);
            GestionSprites.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            DebugShapeRenderer.Draw(gameTime, CaméraJeu.Vue, CaméraJeu.Projection);
            if (!CourseActive && !MenuActif)
            {
                GestionSprites.Draw(Accueil, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), new Vector2(Window.ClientBounds.Width / (float)Accueil.Width, Window.ClientBounds.Height
                / (float)Accueil.Height), SpriteEffects.None, 0);
                if (Keyboard.GetState().GetPressedKeys().Length > 0)
                {
                    CourseActive = false;
                    MenuActif = true;
                }
            }

            if (ImageToucheActive)
            {
                if (true)
                {
                    GestionSprites.Draw(InputManette, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), new Vector2(Window.ClientBounds.Width / (float)InputManette.Width, Window.ClientBounds.Height
                / (float)InputManette.Height), SpriteEffects.None, 0);
                }
                else
                {
                    GestionSprites.Draw(InputClavier, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), new Vector2(Window.ClientBounds.Width / (float)InputClavier.Width, Window.ClientBounds.Height
                / (float)InputClavier.Height), SpriteEffects.None, 0);
                }

                GestionSprites.DrawString(Bebas, GetMessage(0), new Vector2(30, Window.ClientBounds.Height - 100), new Color(24,93,114), 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
            }
            base.Draw(gameTime);
            GestionSprites.End();
        }

        private void GérerClavier()
        {
            if (GestionInput.EstEnfoncée(Keys.Escape))
            {
                Exit();
            }
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            Interface.UpdateScreenSize(new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height));          
        }

        void CréerUneCourse(int nbTours, int piste)
        {
            CaméraMobile = false;
            //CourseActive = true; 

            PositionUtilisateur = 1;
            IDVoitureUtilisateur = 0;
            NbTours = 1;
            Piste = piste;
            if (Piste == 0)
            {
                //Carte = new Terrain(this, 1f, Vector3.Zero, new Vector3(0, -1258 / 2f, 0), new Vector3(25600 / 2f, 1000 / 2f, 25600 / 2f), "CarteTest3", "grass", 5, INTERVALLE_MAJ_STANDARD, CaméraJeu);
                //Carte = new LeTerrain(this, 1f, Vector3.Zero, new Vector3(0, -1285 / 2f, 0), new Vector3(25600 / 2f, 1000 / 2f, 25600 / 2f), "CarteTest3", "DétailsTerrain2", 5, INTERVALLE_MAJ_STANDARD);
                Carte = new LeTerrain2(this, 1f, Vector3.Zero, new Vector3(0, -630, 0), new Vector3(25600 / 2f, 1000 / 2f, 25600 / 2f), "Canyon2", "DétailsTerrain2", 7, INTERVALLE_MAJ_STANDARD);

            }
            LaPiste = new List<Sol>();

            ListeVoiture = new List<Voiture>();
            Mustang = new Voiture(this, "MustangGT500SansRoue", 0.0088f, new Vector3(0, 0, 0), new Vector3(-1875, 0, 1100), INTERVALLE_MAJ_STANDARD, true, true);
            AI = new Voiture(this, "MustangGT500SansRoue", 0.0088f, new Vector3(0, 0, 0), new Vector3(-1915, 0, 1100), INTERVALLE_MAJ_STANDARD, false, true);
            ListeVoiture.Add(Mustang);
            ListeVoiture.Add(AI);
            NbVoiture = ListeVoiture.Count();

            TempsÉcouléDepuisMAJ = 0;

            Components.Add(new Afficheur3D(this));
            Components.Add(new AfficheurFPS(this, INTERVALLE_CALCUL_FPS));
            Components.Add(Carte);

            LaCourse = new Course(this, NbTours, NbVoiture, LaPiste, ListeVoiture, CaméraJeu, INTERVALLE_MAJ_STANDARD, Piste, ModeDeJeu);
            LaCourse.CréationPiste();
            foreach (Sol x in LaCourse.LaPiste)
            {
                Components.Add(x);
            }

            foreach (Voiture x in ListeVoiture)
            {
                Components.Add(x);
            }

            Components.Add(LaCourse);

            //Interface = new GUI(this, INTERVALLE_MAJ_STANDARD, "aiguille2", "speedometer3", LaCourse.NbVoiture, LaCourse.NbTours, IDVoitureUtilisateur, new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height), ModeDeJeu);
            //Components.Add(Interface);

            //Services.AddService(typeof(GUI), Interface);
            //Services.AddService(typeof(int), IDVoitureUtilisateur);
        }

        void GestionOrientationCaméra()
        {
            if (!CaméraJeu.CaméraMobile)
            {
                float orientationX = (float)Math.Sin(ListeVoiture[IDVoitureUtilisateur].Rotation.Y);
                float orientationZ = (float)Math.Cos(ListeVoiture[IDVoitureUtilisateur].Rotation.Y);
                Vector3 cible = new Vector3(VueArrière * orientationX, VueArrière * CaméraJeu.Direction.Y, VueArrière * orientationZ);
                Vector3 ciblePosition = ListeVoiture[IDVoitureUtilisateur].Position +
                    PositionCaméra * new Vector3((float)Math.Sin(ListeVoiture[IDVoitureUtilisateur].Rotation.Y), 1, (float)Math.Cos(ListeVoiture[IDVoitureUtilisateur].Rotation.Y));

                CaméraJeu.Direction = Vector3.Lerp(CaméraJeu.Direction, cible, 0.1f);
                CaméraJeu.Position = new Vector3(Vector3.Lerp(CaméraJeu.Position, ciblePosition, 0.1f).X, Vector3.Lerp(CaméraJeu.Position, ciblePosition, 1.0f).Y,
                    Vector3.Lerp(CaméraJeu.Position, ciblePosition, 0.1f).Z);

                PositionCaméra = new Vector3(LaCourse.TableauPositionCaméra[LaCourse.IndexPositionCaméra].X + 50 * (ListeVoiture[IDVoitureUtilisateur].PixelToKMH(ListeVoiture[IDVoitureUtilisateur].Vitesse) / 100.0f), LaCourse.TableauPositionCaméra[LaCourse.IndexPositionCaméra].Y, 
                    LaCourse.TableauPositionCaméra[LaCourse.IndexPositionCaméra].Z + 50 * (ListeVoiture[IDVoitureUtilisateur].PixelToKMH(ListeVoiture[IDVoitureUtilisateur].Vitesse) / 100.0f));

            }
        }

        string GetMessage(int id)
        {
            string msg = "";
            if (id == 0)
            {
                msg = "Appuyez sur une touche";

                if (ModeDeJeu == 1 && !CoursePeutCommencer)
                {
                    msg = "En attente de " + (2-NbJoueurConnectés).ToString() + " autre joueur";
                }
            }

            return msg;
        }
    }

}
