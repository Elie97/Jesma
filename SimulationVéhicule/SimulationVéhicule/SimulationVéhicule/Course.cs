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


namespace SimulationVéhicule
{
    public class Course : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public int NbTours { get; set; }
        public int NbVoiture { get; set; }
        public int[] ToursFait { get; set; }
        public List<Sol> LaPiste { get; set; }
        public List<Voiture> ListeVoiture { get; set; }
        public List<bool[]> CheckPointParVoiture { get; set; }
        public List<int[]> FranchiParVoiture { get; set; }
        public int[] NbFranchis { get; set; }
        List<bool>[] ListeCheckPoint { get; set; }
        List<int[]> PositionVoiture { get; set; }
        public int PositionUtilisateur { get; set; }
        List<bool[]> CheckPointFranchiParVoiture { get; set; }
        CaméraSubjective CaméraJeu { get; set; }
        int Piste { get; set; }

        public List<int> Position { get; set; }

        //Collision à l'extérieur de la piste
        List<BoundingBox> ListeLimitePiste { get; set; }
        List<bool>[] ListeDansLesLimites { get; set; }
        bool[] VitesseMiseÀZéro { get; set; }
        Sol[] SolEnCollision { get; set; }
        float[] DeltaRotation { get; set; }
        Vector3[] DeltaPosition { get; set; }

        //Cinématique
        List<bool> ListeCinématiqueTerminée { get; set; }
        bool Initialization { get; set; }
        bool InitializationCourseCinématique { get; set; }
        float TempsDécompte { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float IntervalleMAJ { get; set; }
        float TempsÉcoulé { get; set; }
        InputManager GestionInput { get; set; }
        bool ForcerArrêt { get; set; }

        Vector3[] ÉtapeDirection { get; set; }
        Vector3[] ÉtapePosition { get; set; }
        bool[] ÉtapeDirectionComplet { get; set; }
        bool[] ÉtapePositionComplet { get; set; }

        RessourcesManager<SoundEffect> GestionnaireDeSon { get; set; }
        SoundEffectInstance Introduction { get; set; }
        SoundEffect BeepGrave { get; set; }
        SoundEffect BeepAigu { get; set; }
        SoundEffect Victoire { get; set; }
        SoundEffect Défaite { get; set; }
        bool[] EstSonJoué { get; set; }

        GUI Interface { get; set; }

        bool AfficherTexte { get; set; }
        SpriteFont Bebas { get; set; }
        SpriteFont Bebas120 { get; set; }
        SpriteBatch GestionSprites { get; set; }

        Vector2 PositionNomCarte { get; set; }

        bool DépartCourse { get; set; }

        bool AfficherDécompte { get; set; }
        Texture2D CountdownImage { get; set; }
        Texture2D CountdownImage1 { get; set; }
        Texture2D CountdownImage2 { get; set; }
        Texture2D CountdownImage3 { get; set; }
        Texture2D CountdownImage4 { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }

        int IDVoitureUtilisateur { get; set; }

        Texture2D ÉcranNoir { get; set; }
        Texture2D Confettis { get; set; }
        Texture2D Trophée { get; set; }
        bool AfficherÉcranNoir { get; set; }
        bool AfficherConfettis { get; set; }
        float ÉchelleConfettis { get; set; }

        RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        Model Pont { get; set; }
        Matrix[] TransformationsModèle { get; set; }

        bool[,] ÉtatCollisionPiste { get; set; }
        //Caméra
        int VueArrière { get; set; }
        Vector3 PositionCaméra { get; set; }
        public Vector3[] TableauPositionCaméra { get; set; }
        public int IndexPositionCaméra { get; set; }

        int ModeDeJeu { get; set; }//Couse Contre Montre
        float TempsAdversaire { get; set; }
        float TempsParCheckPointAdversaire { get; set; }
        int Difficulté { get; set; }

        public bool CourseTerminée { get; set; }



        public Course(Game game, int nbTours, int nbVoiture, List<Sol> laPiste, List<Voiture> listeVoiture, CaméraSubjective caméra, float intervalle, int piste, int modeDeJeu)
            : base(game)
        {
            NbTours = nbTours;
            NbVoiture = nbVoiture;
            LaPiste = laPiste;
            ListeVoiture = listeVoiture;
            CaméraJeu = caméra;
            IntervalleMAJ = intervalle;
            Piste = piste;
            ModeDeJeu = modeDeJeu;
        }

        protected override void LoadContent()
        {
            GestionnaireDeSon = Game.Services.GetService(typeof(RessourcesManager<SoundEffect>)) as RessourcesManager<SoundEffect>;
            Interface = Game.Services.GetService(typeof(GUI)) as GUI;
            Bebas = Game.Content.Load<SpriteFont>("Fonts/Bebas");
            Bebas120 = Game.Content.Load<SpriteFont>("Fonts/Bebas120");
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            IDVoitureUtilisateur = (int)Game.Services.GetService(typeof(int));
            GestionnaireDeModèles = Game.Services.GetService(typeof(RessourcesManager<Model>)) as RessourcesManager<Model>;
            //CaméraJeu = Game.Services.GetService(typeof(Caméra)) as CaméraSubjective;



            Introduction = GestionnaireDeSon.Find("TimeIntro").CreateInstance();
            BeepGrave = GestionnaireDeSon.Find("beepGrave");
            BeepAigu = GestionnaireDeSon.Find("beepAigu");
            Victoire = GestionnaireDeSon.Find("Victoire");
            Défaite = GestionnaireDeSon.Find("Defaite");
            CountdownImage = CountdownImage1 = GestionnaireDeTextures.Find("décompte1");
            CountdownImage2 = GestionnaireDeTextures.Find("décompte2");
            CountdownImage3 = GestionnaireDeTextures.Find("décompte3");
            CountdownImage4 = GestionnaireDeTextures.Find("décompte4");
            ÉcranNoir = GestionnaireDeTextures.Find("ÉcranNoir");
            Confettis = GestionnaireDeTextures.Find("confettis");
            Trophée = GestionnaireDeTextures.Find("trophée");
        }

        public override void Initialize()
        {
            ListeCinématiqueTerminée = new List<bool>(2);
            ListeCinématiqueTerminée.Add(false);
            Initialization = true;
            InitializationCourseCinématique = true;
            TempsDécompte = 0;
            TempsÉcouléDepuisMAJ = 0;
            ÉtapeDirection = new Vector3[] { new Vector3(-0.1272521f, 0.4382029f, 0.8898231f), new Vector3(-0.9342267f, -0.2819609f, -0.2184461f),
                                             new Vector3(-0.8982279f, -0.2436537f, 0.3658135f), new Vector3(0.4710512f,-0.3949662f,0.7887411f), new Vector3(0.08072881f, -0.1129913f, -0.990311f)};
            ÉtapeDirectionComplet = new bool[] { false, false, false, false, false };
            ÉtapePositionComplet = new bool[] { false, false, false, false, false };
            ÉtapePosition = new Vector3[] { new Vector3(4700, 1750, 3200), new Vector3(3750, 1625, -1180), new Vector3(570, 1390, -1201), new Vector3(-1900, 8, 1369), new Vector3(-1900, 8, 719) };
            AfficherTexte = true;
            ForcerArrêt = false;

            DépartCourse = false;
            AfficherDécompte = false;
            PositionNomCarte = new Vector2(30, Game.Window.ClientBounds.Height + 50);

            PositionUtilisateur = NbVoiture;
            NbFranchis = new int[NbVoiture];
            ListeCheckPoint = new List<bool>[2];
            ToursFait = new int[NbVoiture];
            CheckPointParVoiture = new List<bool[]>(NbVoiture);
            CheckPointFranchiParVoiture = new List<bool[]>(NbVoiture);
            FranchiParVoiture = new List<int[]>(NbVoiture);
            PositionVoiture = new List<int[]>();

            for (int i = 0; i < NbVoiture; i++)
            {
                NbFranchis[i] = 0;
                ToursFait[i] = 0;
                CheckPointParVoiture.Add(new bool[LaPiste.Count()]);
                FranchiParVoiture.Add(new int[2]);
            }

            EstSonJoué = new bool[] { false, false, false, false, false, false };
            AfficherÉcranNoir = false;
            AfficherConfettis = false;

            ÉchelleConfettis = 0;

            ListeLimitePiste = new List<BoundingBox>();
            foreach (Sol x in LaPiste)
            {
                ListeLimitePiste.Add(x.BoxComplet);
            }
            ListeDansLesLimites = new List<bool>[NbVoiture];

            VitesseMiseÀZéro = new bool[NbVoiture];
            for (int i = 0; i < NbVoiture; i++)
            {
                VitesseMiseÀZéro[i] = false;
            }

            SolEnCollision = new Sol[NbVoiture];
            for (int v = 0; v < NbVoiture; v++)
            {
                SolEnCollision[v] = LaPiste[0];
            }

            DeltaRotation = new float[NbVoiture];
            DeltaPosition = new Vector3[NbVoiture];

            ÉtatCollisionPiste = new bool[LaPiste.Count(), NbVoiture];

            //Caméra
            VueArrière = 1;
            TableauPositionCaméra = new Vector3[6];
            IndexPositionCaméra = 0;
            PositionCaméra = new Vector3(-80, 20, -80);

            if (ModeDeJeu == 0)
            {
                Difficulté = 2;//plus facile
                if (Difficulté == 0)
                {
                    TempsAdversaire = 5000;
                }
                else if(Difficulté == 1)
                {
                    TempsAdversaire = 3000;
                }
                else if(Difficulté == 2)
                {
                    TempsAdversaire = 1000;
                }
                TempsParCheckPointAdversaire = TempsAdversaire / (LaPiste.Count() * NbTours * 2);
            }

            CourseTerminée = false;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            GérerClavier();
            TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds; //Mettre partout?
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                if (ModeDeJeu == 1 || Cinématique())
                {
                    Introduction.Stop();
                    Interface.Afficher = true;
                    CaméraJeu.ChangerTypeCaméra(false);
                    AfficherTexte = false;

                    if (DépartCourse)
                    {
                        ListeVoiture[IDVoitureUtilisateur].Controle = true;
                        //ListeVoiture[1].AI();

                    }
                    else
                    {
                        AfficherDécompte = true;
                        TempsDécompte++;
                        //TempsÉcouléDepuisMAJ = 0;
                        if (TempsDécompte >= 450)
                        {
                            DépartCourse = true;
                        }
                    }

                    GestionAnimationFinCourse();

                    if (TempsDécompte >= 600)
                    {
                        AfficherDécompte = false;
                    }


                    //Gestion Des Collision
                    //for (int i = 1; i < ListeVoiture.Count(); i++)
                    //{
                    //    //ListeVoiture[IDVoitureUtilisateur].GestionCollisionVoiture(ListeVoiture[i]);
                    //}
                    ListeVoiture[IDVoitureUtilisateur].GestionCollisionVoiture(ListeVoiture[1]);

                    //Gestion de la caméra
                    //GestionOrientationCaméra(); //Dans un counter

                    //Gestion des checks points
                    GestionDesCheckPoints();

                    int nbCheckPointParTour = LaPiste.Count() * Sol.NB_CHECK_POINT;
                    PositionVoiture = new List<int[]>();

                }
                else
                {
                    ListeVoiture[IDVoitureUtilisateur].Controle = false;
                    Interface.Afficher = false;
                    AfficherTexte = true;
                    PositionNomCarte = AnimationTexteVersHaut(Game.Window.ClientBounds.Height - 100, PositionNomCarte, 2f);
                }

                GestionCheckPoints();

                PositionVoiture = PositionVoiture.OrderByDescending(x => x[1]).ToList();

                GestionPositionÉgale();

                string d = "";
                foreach (bool x in ListeCheckPoint[IDVoitureUtilisateur])
                {
                    d += " - " + x;
                }

                for (int i = 0; i < LaPiste.Count(); i++)
                {
                    for (int j = 0; j < LaPiste[i].BoxÉtape.Count(); j++)
                    {
                        if (ListeCheckPoint[IDVoitureUtilisateur][i + i + j] == true)
                        {
                            LaPiste[i].CouleurCheckPoint = Color.Green;
                        }
                    }
                }


                GestionCollisionAvecPiste();

                GestionCourseContreLaMontre();

                Interface.UpdateGUI((int)ListeVoiture[IDVoitureUtilisateur].PixelToKMH(ListeVoiture[IDVoitureUtilisateur].Vitesse),
                        PositionUtilisateur, CheckPointParVoiture[IDVoitureUtilisateur],
                        ToursFait[IDVoitureUtilisateur]);

                TempsÉcouléDepuisMAJ = 0;
            }

            if (GestionInput.EstNouvelleTouche(Keys.Space) && !DépartCourse)
            {
                ForcerArrêt = true;
                CaméraJeu.Direction = new Vector3(0, 0, 0) - CaméraJeu.Position;
            }

            Interface.GetDépartCourse(DépartCourse);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (AfficherTexte)
            {
                GestionSprites.DrawString(Bebas, GetMessage(0), PositionNomCarte, Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                GestionSprites.DrawString(Bebas, GetMessage(1), PositionNomCarte + new Vector2(0, 50), Color.White, 0, new Vector2(0, 0), 0.3f, SpriteEffects.None, 0);
            }
            //afficher différence temps pour course contre la montre
            if (AfficherDécompte)
            {
                Décompte();
                GestionSprites.Draw(CountdownImage, new Vector2(Game.Window.ClientBounds.Width/2, Game.Window.ClientBounds.Height/4), null, Color.White, 0,new Vector2(CountdownImage.Width/2, CountdownImage.Height/2), 0.75f, SpriteEffects.None, 0);
            }

            if (AfficherÉcranNoir)
            {
                GestionSprites.Draw(ÉcranNoir, new Vector2(0,0), null, new Color(255, 255, 255, 0.5f), 0, new Vector2(0,0), 100f, SpriteEffects.None, 0);
                if (AfficherConfettis)
                {
                    if (ÉchelleConfettis < 1f)
                    {
                        ÉchelleConfettis += 0.01f;
                    }
                    GestionSprites.Draw(Confettis, new Vector2(Game.Window.ClientBounds.Width/2, Game.Window.ClientBounds.Height/2), null, Color.White, 0,new Vector2(Confettis.Width/2, Confettis.Height/2), ÉchelleConfettis, SpriteEffects.None, 0);
                    GestionSprites.Draw(Trophée, new Vector2(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2) + new Vector2(0, 50), null, Color.White, 0, new Vector2(Trophée.Width / 2, Trophée.Height / 2), 1.0f, SpriteEffects.None, 0);
                    GestionSprites.DrawString(Bebas120, "VICTOIRE!", new Vector2(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 4), Color.White, 0, Bebas120.MeasureString("VICTOIRE!") / 2f, 1.0f, SpriteEffects.None, 0);

                }
                else
                {
                    GestionSprites.DrawString(Bebas120, "DÉFAITE!", new Vector2(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 4), Color.White, 0, Bebas120.MeasureString("VICTOIRE!") / 2f, 1.0f, SpriteEffects.None, 0);
                }
            }
            base.Draw(gameTime);
        }

        bool ÉgaleUnePosition(int IDUtilisateur)
        {
            bool égaleUnePosition = false;
            for (int i = 1; i < NbVoiture; i++)
            {
                égaleUnePosition = false;
                if (NbFranchis[IDUtilisateur] == NbFranchis[i])
                {
                    égaleUnePosition = true;
                }
            }
            return égaleUnePosition;
        }

        float GetSensDeltaRotationSol(float rotationVoiture, float rotationSol, bool courbe)
        {
            float deltaRotation = 0;
            float facteurVitesse = 1;
            float sensRotation = 0;
            if (rotationSol == MathHelper.Pi)
            {
                deltaRotation = (float)Math.Cos(rotationVoiture);
            }
            if (rotationSol == (MathHelper.PiOver2))
            {
                deltaRotation = (float)-Math.Sin(rotationVoiture);
            }
            if (rotationSol == 0)
            {
                deltaRotation = (float)-Math.Cos(rotationVoiture);
            }
            if (rotationSol == -MathHelper.PiOver2)
            {
                deltaRotation = (float)Math.Sin(rotationVoiture);
            }
            if (ListeVoiture[0].Vitesse != 0)
            {
                facteurVitesse = (ListeVoiture[0].Vitesse / Math.Abs(ListeVoiture[0].Vitesse));
            }
            sensRotation = deltaRotation * facteurVitesse;
            if (courbe)
            {
                sensRotation = 1;
            }
            return sensRotation;
        }

        bool Cinématique()
        {
            bool terminé = false;
            if (!CaméraJeu.CaméraMobile)
            {
                CaméraJeu.ChangerTypeCaméra(true);
            }

            if (!ForcerArrêt)
            {
                ListeCinématiqueTerminée[0] = CinématiqueCarte();
                terminé = ListeCinématiqueTerminée.TrueForAll(x => x);
            }
            else
            {
                terminé = true;
            }
            return terminé;
        }

        bool CinématiqueCarte()
        {
            Introduction.Play();
            Introduction.Volume = 1.0f;
            bool terminé = false;

            if (ForcerArrêt)
            {
                terminé = true;
            }

            if (Initialization)
            {
                CaméraJeu.Position = new Vector3(4700, 1750, 3200);
                CaméraJeu.Direction = ÉtapeDirection[0];
                Initialization = false;
            }

            if (!ÉtapeDirectionComplet[0])
            {
                DéplacementDirection(0.010f, ÉtapeDirection[1], 0, terminé);
            }
            else if (ÉtapeDirectionComplet[0] && (!ÉtapePositionComplet[0] || !ÉtapeDirectionComplet[1]))
            {
                DéplacementPosition(0.008f, ÉtapePosition[1], 0, terminé);//selon fps
                DéplacementDirection(0.010f, ÉtapeDirection[2], 1, terminé);
            }
            else if (ÉtapeDirectionComplet[1] && ÉtapePositionComplet[0] && (!ÉtapePositionComplet[1] || !ÉtapeDirectionComplet[2]))
            {
                DéplacementPosition(0.008f, ÉtapePosition[2], 1, terminé);
                DéplacementDirection(0.01f, ÉtapeDirection[3], 2, terminé);
            }
            else if (ÉtapePositionComplet[1] && ÉtapeDirectionComplet[2] && (!ÉtapePositionComplet[2]))
            {
                if (InitializationCourseCinématique)
                {
                    CaméraJeu.Position = ÉtapePosition[3];
                    CaméraJeu.Direction = ÉtapeDirection[4];
                    InitializationCourseCinématique = false;
                }
                DéplacementPosition(0.002f, ÉtapePosition[4], 3, terminé);
            }
           
            return terminé;
        }


        void DéplacementDirection(float vitesse, Vector3 cible, int idDirection, bool continuer)
        {
            Vector3 delta = CaméraJeu.Direction - cible;
            if (!ForcerArrêt)
            {
                float distance = (float)Math.Sqrt(Math.Pow(cible.X - CaméraJeu.Direction.X, 2) + Math.Pow(cible.Y - CaméraJeu.Direction.Y, 2) + Math.Pow(cible.Z - CaméraJeu.Direction.Z, 2));
                if (distance >= 0.10f)
                {
                    CaméraJeu.Direction = Vector3.Lerp(CaméraJeu.Direction, cible, vitesse);
                }
                else
                {
                    ÉtapeDirectionComplet[idDirection] = true;
                }
            }
        }

        void DéplacementPosition(float vitesse, Vector3 cible, int idPosition, bool continuer)
        {
            if (!ForcerArrêt)
            {
                Vector3 delta = CaméraJeu.Position - cible;
                float distance = (float)Math.Sqrt(Math.Pow(cible.X - CaméraJeu.Position.X, 2) + Math.Pow(cible.Y - CaméraJeu.Position.Y, 2) + Math.Pow(cible.Z - CaméraJeu.Position.Z, 2));
                if (distance >= 300f)
                {
                    CaméraJeu.Position = Vector3.Lerp(CaméraJeu.Position, cible, vitesse);
                }
                else
                {
                    ÉtapePositionComplet[idPosition] = true;
                }
            }
        }

        string GetMessage(int message)
        {
            string msg = "";
            if (message == 0)
            {
                //nom de la carte
                msg = "Aoraki Mountains";
            }
            else if (message == 1)
            {
                //nom de la carte
                msg = "\"Espace\" pour continuer";
            }
            return msg;
        }

        Vector2 AnimationTexteVersHaut(float hauteurMax, Vector2 position, float vitesse)
        {
            if (position.Y > hauteurMax)
            {
                position = new Vector2(position.X, position.Y - vitesse);
            }
            else
            {
                position = new Vector2(position.X, hauteurMax);
            }
            return position;
        }

        void Décompte()
        {
            if (!EstSonJoué[0])
            {
                BeepGrave.Play();
                EstSonJoué[0] = true;
            }
            CountdownImage = CountdownImage1;
            TempsDécompte++;
            if (TempsDécompte > 150 && TempsDécompte <= 300)
            {
                if (!EstSonJoué[1])
                {
                    BeepGrave.Play();
                    EstSonJoué[1] = true;
                }
                CountdownImage = CountdownImage2;
            }
            else if (TempsDécompte > 300 && TempsDécompte <= 450)
            {
                if (!EstSonJoué[2])
                {
                    BeepGrave.Play();
                    EstSonJoué[2] = true;
                }
                CountdownImage = CountdownImage3;
            }
            else if (TempsDécompte > 450)
            {
                if (!EstSonJoué[3])
                {
                    BeepAigu.Play();
                    EstSonJoué[3] = true;
                }
                CountdownImage = CountdownImage4;
                Interface.DébuterTemps = true;
            }
        }

        void GestionCollisionBordure(Sol sol, Vector3 deltaPosition)
        {
            if (SolEnCollision[0].RotationInitiale.Y == MathHelper.Pi || SolEnCollision[0].RotationInitiale.Y == 0)
            {
                if (DeltaPosition[0].X < 0)
                {
                    if (Math.Sin(ListeVoiture[0].Rotation.Y) > 0)
                    {
                        ListeVoiture[0].AvancePossiblePiste = false;
                    }
                    else
                    {
                          ListeVoiture[0].ReculePossiblePiste = false;
                    }
                }
                else
                {
                    if (Math.Sin(ListeVoiture[0].Rotation.Y) > 0)
                    {
                        ListeVoiture[0].ReculePossiblePiste = false;
                    }
                    else
                    {
                        ListeVoiture[0].AvancePossiblePiste = false;
                    }
                }
            }
            else if (Math.Abs(SolEnCollision[0].RotationInitiale.Y) == MathHelper.PiOver2)
            {
                if (DeltaPosition[0].Z < 0)
                {
                    if (Math.Cos(ListeVoiture[0].Rotation.Y) < 0)
                    {
                        ListeVoiture[0].ReculePossiblePiste = false;
                    }
                    else
                    {
                        ListeVoiture[0].AvancePossiblePiste = false;
                    }
                }
                else
                {
                    if (Math.Cos(ListeVoiture[0].Rotation.Y) > 0)
                    {
                        ListeVoiture[0].ReculePossiblePiste = false;
                    }
                    else
                    {
                        ListeVoiture[0].AvancePossiblePiste = false;
                    }
                }
            }
        } //voiture utilisateur ??

        void GestionAnimationFinCourse()
        {

            if (NbFranchis[0] >= LaPiste.Count() * NbTours * 2 || NbFranchis[1] >= LaPiste.Count() * NbTours * 2)
            {
                CourseTerminée = true;
            }

            if (CourseTerminée)
            {
                CaméraJeu.Position = LaPiste[0].Position + new Vector3(100, 25, 0);
                Interface.Afficher = false;
                CaméraJeu.Direction = ListeVoiture[IDVoitureUtilisateur].Position;
                ListeVoiture[IDVoitureUtilisateur].Controle = false;
                CaméraJeu.ChangerTypeCaméra(true);
                TempsÉcouléDepuisMAJ += TempsÉcoulé;
                if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
                {
                    if (ListeVoiture[IDVoitureUtilisateur].Vitesse >= 0)
                    {
                        ListeVoiture[IDVoitureUtilisateur].Avance();
                        ListeVoiture[IDVoitureUtilisateur].Vitesse -= (Voiture.ACCÉLÉRATION_FREIN) / (60f * 2);
                    }
                    else
                    {
                        ListeVoiture[IDVoitureUtilisateur].Vitesse = 0;
                    }
                    TempsÉcouléDepuisMAJ = 0;
                }
                if (Interface.PositionUtilisateur == 1)//Si c'est une victoire!
                {
                    AfficherÉcranNoir = true;
                    AfficherConfettis = true;
                    if (!EstSonJoué[4])
                    {
                        Victoire.Play();
                        EstSonJoué[4] = true;
                    }
                }
                else//Si c'est une défaite!
                {
                    AfficherÉcranNoir = true;
                    if (!EstSonJoué[5])
                    {
                        Défaite.Play();
                        EstSonJoué[5] = true;
                    }
                }
            }

            if (TempsDécompte >= 600)
            {
                AfficherDécompte = false;
            }
        }

        void GestionCourseContreLaMontre()
        {
            if (ModeDeJeu == 0)
            {
                //Game.Window.Title = (NbFranchis[IDVoitureUtilisateur] * TempsParCheckPointAdversaire).ToString() + " - " + Interface.TempsMilliSeconde.ToString() + " - " + (TempsAdversaire/(LaPiste.Count() * 1)).ToString();
                if (Interface.TempsMilliSeconde > NbFranchis[IDVoitureUtilisateur] * TempsParCheckPointAdversaire)
                {
                    PositionUtilisateur = 2;
                }
                else
                {
                    PositionUtilisateur = 1;
                }
            }
        }

        void GestionCheckPoints()
        {
            for (int v = 0; v < NbVoiture; v++)
            {
                //ini liste de tous les checks points
                ListeCheckPoint[v] = new List<bool>();
                for (int i = 0; i < LaPiste.Count(); i++)
                {
                    foreach (bool x in LaPiste[i].ListeFranchiParVoiture[v])
                    {
                        ListeCheckPoint[v].Add(x);
                    }
                }

                if (ToursFait[v] < NbTours)
                {
                    //retour checkpont à false si tour complet
                    if (ListeCheckPoint[v].TrueForAll(x => x))
                    {
                        for (int i = 0; i < LaPiste.Count(); i++)
                        {
                            for (int j = 0; j < Sol.NB_CHECK_POINT; j++)
                            {
                                LaPiste[i].ListeFranchiParVoiture[v][j] = false;
                            }
                        }
                        ToursFait[v]++;
                    }
                }
                if (!ListeVoiture[v].BoxVoiture.Intersects(LaPiste[LaPiste.Count() - 1].BoxÉtape[Sol.NB_CHECK_POINT - 1]))
                {
                    LaPiste[LaPiste.Count() - 1].ListeFranchiParVoiture[v][1] = false;
                }

                //détection collision check point
                for (int i = 0; i < LaPiste.Count(); i++)
                {
                    for (int j = 0; j < LaPiste[i].BoxÉtape.Count(); j++)
                    {
                        if (ListeVoiture[v].BoxVoiture.Intersects(LaPiste[i].BoxÉtape[j]))
                        {
                            if (i != 0 && j == 1 && LaPiste[i].ListeFranchiParVoiture[v][j - 1] == true)
                            {
                                if (GetSensDeltaRotationSol(ListeVoiture[v].Rotation.Y, LaPiste[i].RotationInitiale.Y, LaPiste[i].Courbe) >= 0)
                                {
                                    LaPiste[i].ListeFranchiParVoiture[v][j] = true;
                                }
                                if (GetSensDeltaRotationSol(ListeVoiture[v].Rotation.Y, LaPiste[i].RotationInitiale.Y, LaPiste[i].Courbe) <= 0)
                                {
                                    LaPiste[i].ListeFranchiParVoiture[v][j] = false;
                                }
                            }
                            if (i != 0 && j == 0 && LaPiste[i - 1].ListeFranchiParVoiture[v][j + 1] == true)
                            {
                                if (GetSensDeltaRotationSol(ListeVoiture[v].Rotation.Y, LaPiste[i].RotationInitiale.Y, LaPiste[i].Courbe) >= 0)
                                {
                                    LaPiste[i].ListeFranchiParVoiture[v][j] = true;
                                }
                                if (GetSensDeltaRotationSol(ListeVoiture[v].Rotation.Y, LaPiste[i].RotationInitiale.Y, LaPiste[i].Courbe) <= 0)
                                {
                                    LaPiste[i].ListeFranchiParVoiture[v][j] = false;
                                }
                            }
                            if (i == 0)
                            {
                                if (GetSensDeltaRotationSol(ListeVoiture[v].Rotation.Y, LaPiste[i].RotationInitiale.Y, LaPiste[i].Courbe) >= 0)
                                {
                                    LaPiste[i].ListeFranchiParVoiture[v][j] = true;
                                }
                                if (GetSensDeltaRotationSol(ListeVoiture[v].Rotation.Y, LaPiste[i].RotationInitiale.Y, LaPiste[i].Courbe) <= 0)
                                {
                                    LaPiste[i].ListeFranchiParVoiture[v][j] = false;
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < LaPiste.Count(); i++)
                {
                    for (int j = 0; j < LaPiste[i].BoxÉtape.Count(); j++)
                    {
                        ListeCheckPoint[v][i + i + j] = LaPiste[i].ListeFranchiParVoiture[v][j];
                    }
                }

                NbFranchis[v] = ListeCheckPoint[v].Where(x => x).Count() + (ToursFait[v] * LaPiste.Count() * Sol.NB_CHECK_POINT);

                PositionVoiture.Add(new int[] { v, NbFranchis[v] });
            }

            //NbFranchis[1] = 2;
            ListeVoiture[0].NbFranchis = NbFranchis[0];
            ListeVoiture[1].NbFranchis = NbFranchis[1];
            PositionVoiture[0] = new int[] { 0, ListeVoiture[0].NbFranchis };
            PositionVoiture[1] = new int[] { 1, ListeVoiture[1].NbFranchis };
        }

        public void CréationPiste()
        {
            if (Piste == 0)
            {
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1850, 0, 1100), new Vector2(100, 200), new Vector2(2, 2), "routeDépart", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1850, 0, 1300), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1850, 0, 1500), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1850, 0, 1700), new Vector2(100, 200), new Vector2(2, 2), "route", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1850, 0, 1900), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1850, 0, 2100), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1850, 0, 2300), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1850, 0, 2500), new Vector2(100, 200), new Vector2(2, 20), "routeSale", 0, 0, true, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(-1850, 0, 2500), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(-1650, 0, 2500), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(-1450, 0, 2500), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(-1250, 0, 2500), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(-1050, 0, 2500), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(-850, 0, 2500), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(-650, 0, 2600), new Vector2(100, 200), new Vector2(2, 20), "routeSale", 0, 0, true, MathHelper.PiOver2, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-550, 0, 2600), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-550, 0, 2800), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-550, 0, 3000), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-550, 0, 3200), new Vector2(100, 200), new Vector2(2, 20), "routeSale", 0, 0, true, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(-550, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(-350, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(-150, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(-50, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(50, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(150, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(350, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(550, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(750, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(950, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(1150, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(1350, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(1550, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(1750, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(1950, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(2150, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(2350, 0, 3200), new Vector2(100, 200), new Vector2(2, 20), "routeSale", 0, 0, true, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(MathHelper.Pi / 32f, 0, 0), new Vector3(2350, 0, 3200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 3000), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 2800), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 2600), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 2400), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 2200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 2000), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 1800), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 1600), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 1400), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 1200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 1000), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 800), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 600), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 400), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, 0), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, -200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, -400), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, -600), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, -800), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2350, 0, -1000), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2450, 0, -1200), new Vector2(100, 200), new Vector2(2, 20), "routeSale", 0, 0, true, MathHelper.PiOver2, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2450, 0, -1300), new Vector2(100, 200), new Vector2(2, 20), "routeSale", 0, 0, true, -MathHelper.PiOver2, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2450, 0, -1300), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2450, 0, -1500), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2450, 0, -1700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2450, 0, -1900), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2450, 0, -2100), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2450, 0, -2300), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(2450, 0, -2500), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(2450, 0, -2700), new Vector2(100, 200), new Vector2(2, 20), "routeSale", 0, 0, true, MathHelper.PiOver2, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(2450, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(2250, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(2050, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(1850, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(1650, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(1450, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(1250, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(1050, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(850, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(650, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(450, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(250, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(50, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-50, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-250, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-450, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-650, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-850, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-1050, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-1250, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-1450, 0, -2700), new Vector2(100, 200), new Vector2(2, 20), "routeSale", 0, 0, true, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, -2700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, -2500), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, -2300), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, -2100), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, -1900), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, -1700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, -1500), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, -1300), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, -1100), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, -900), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, -700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, -500), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, -300), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, -100), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, 100), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1450, 0, 200), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, 0, 0), new Vector3(-1550, 0, 400), new Vector2(100, 200), new Vector2(2, 20), "routeSale", 0, 0, true, -MathHelper.PiOver2, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-1550, 0, 500), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-1750, 0, 500), new Vector2(100, 100), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-1850, 0, 500), new Vector2(100, 200), new Vector2(2, 20), "routeSale", 0, 0, true, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1850, 0, 500), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1850, 0, 700), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));
                LaPiste.Add(new Sol(Game, 1.0f, new Vector3(0, MathHelper.Pi, 0), new Vector3(-1850, 0, 900), new Vector2(100, 200), new Vector2(2, 2), "routeSale", 0, 0, false, 0, NbVoiture));


            }
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
                //fonctionne pas??????
                PositionCaméra = new Vector3(TableauPositionCaméra[IndexPositionCaméra].X + 50 * (ListeVoiture[IDVoitureUtilisateur].PixelToKMH(ListeVoiture[IDVoitureUtilisateur].Vitesse) / 200f), TableauPositionCaméra[IndexPositionCaméra].Y, TableauPositionCaméra[IndexPositionCaméra].Z + 50 * (ListeVoiture[IDVoitureUtilisateur].PixelToKMH(ListeVoiture[IDVoitureUtilisateur].Vitesse) / 200f));
                //PositionCaméra = new Vector3(TableauPositionCaméra[IndexPositionCaméra].X + 55, TableauPositionCaméra[IndexPositionCaméra].Y, TableauPositionCaméra[IndexPositionCaméra].Z + 55);

            }
        }

        void GérerClavier()
        {
            Vector3 arrièreRapproché = new Vector3(-80, 20, -80);
            Vector3 arrièreMoyen = new Vector3(-90, 25, -90);
            Vector3 arrièreLoin = new Vector3(-100, 30, -100);
            Vector3 intérieurConducteur = new Vector3(-20, 11, -20);
            Vector3 capo = new Vector3(-10, 12, -10);
            Vector3 vueArrière = new Vector3(70, 20, 70);
            TableauPositionCaméra[0] = arrièreRapproché;
            TableauPositionCaméra[1] = arrièreMoyen;
            TableauPositionCaméra[2] = arrièreLoin;
            TableauPositionCaméra[3] = intérieurConducteur;
            TableauPositionCaméra[4] = capo;
            TableauPositionCaméra[5] = vueArrière;

            if (GestionInput.EstNouvelleTouche(Keys.NumPad1))
            {
                PositionCaméra = arrièreRapproché;
                VueArrière = 1;
                IndexPositionCaméra = 0;
            }
            else if (GestionInput.EstNouvelleTouche(Keys.NumPad2))
            {
                PositionCaméra = arrièreMoyen;
                VueArrière = 1;
                IndexPositionCaméra = 1;
            }
            else if (GestionInput.EstNouvelleTouche(Keys.NumPad3))
            {
                PositionCaméra = arrièreLoin;
                VueArrière = 1;
                IndexPositionCaméra = 2;
            }
            else if (GestionInput.EstNouvelleTouche(Keys.NumPad4))
            {
                PositionCaméra = intérieurConducteur;
                VueArrière = 1;
                IndexPositionCaméra = 3;
            }
            else if (GestionInput.EstNouvelleTouche(Keys.NumPad5))
            {
                PositionCaméra = capo;
                VueArrière = 1;
                IndexPositionCaméra = 4;
            }
            else if (GestionInput.EstNouvelleTouche(Keys.NumPad6))
            {
                PositionCaméra = vueArrière;
                VueArrière = -1;
                IndexPositionCaméra = 5;
            }

            if (GestionInput.EstEnfoncée(Keys.R))
            {
                if (NbFranchis[IDVoitureUtilisateur] >= 2)
                {
                    ListeVoiture[IDVoitureUtilisateur].Position = LaPiste[(int)(NbFranchis[IDVoitureUtilisateur] / 2) - (ToursFait[IDVoitureUtilisateur] * LaPiste.Count())].Position + new Vector3((float)Math.Cos(LaPiste[(int)(NbFranchis[IDVoitureUtilisateur] / 2) - (ToursFait[IDVoitureUtilisateur] * LaPiste.Count())].RotationInitiale.Y) * 50, 0, (float)Math.Sin(LaPiste[(int)(NbFranchis[IDVoitureUtilisateur] / 2) - (ToursFait[IDVoitureUtilisateur] * LaPiste.Count())].RotationInitiale.Y) * -50);
                }
                else
                {
                    ListeVoiture[IDVoitureUtilisateur].Position = LaPiste[0].Position + new Vector3((float)Math.Cos(LaPiste[0].RotationInitiale.Y) * 50, 0, (float)Math.Sin(LaPiste[0].RotationInitiale.Y) * -50);
                }
            }
        }

        void GestionCollisionAvecPiste()
        {
            for (int v = 0; v < NbVoiture; v++)
            {
                bool collisionTouteFausse = true;
                for (int i = 0; i < LaPiste.Count(); i++)
                {
                    ÉtatCollisionPiste[i, v] = ListeVoiture[v].GestionCollisionPiste(LaPiste[i]);
                }
                for (int i = 0; i < ÉtatCollisionPiste.GetLength(0) && collisionTouteFausse; i++)
                {
                    if (ÉtatCollisionPiste[i, v])
                    {
                        collisionTouteFausse = false;
                    }
                }
                if (collisionTouteFausse)
                {
                    ListeVoiture[v].GetHauteur();
                    //ListeVoiture[IDVoitureUtilisateur].VariationInclinaison();
                }
            }
        }

        void GestionPositionÉgale()
        {
            for (int i = 0; i < PositionVoiture.Count(); i++)
            {
                if (!ÉgaleUnePosition(0))
                {
                    if (0 == PositionVoiture[i][IDVoitureUtilisateur])//id voiture utilisateur
                    {
                        PositionUtilisateur = i + 1;
                    }
                }
            }
        }

        void GestionDesCheckPoints()
        {
            for (int v = 0; v < NbVoiture; v++)
            {
                ListeDansLesLimites[v] = new List<bool>();

                for (int i = 0; i < ListeLimitePiste.Count(); i++)
                {
                    ListeDansLesLimites[v].Add(ListeVoiture[v].SphereRouteAvant.Intersects(LaPiste[i].BoxComplet));
                }

                if (ListeDansLesLimites[v].TrueForAll(x => !x))
                {
                    for (int i = 0; i < ListeLimitePiste.Count(); i++)
                    {
                        if (ListeVoiture[v].BoxVoiture.Intersects(LaPiste[i].BoxComplet))//Pour empêcher d'avancer ou de reculer selon l'orientation
                        {
                            SolEnCollision[v] = LaPiste[i];
                            DeltaPosition[v] = SolEnCollision[v].Position - ListeVoiture[v].Position;
                            GestionCollisionBordure(SolEnCollision[v], DeltaPosition[v]);
                        }
                    }
                    if (!VitesseMiseÀZéro[v])
                    {
                        ListeVoiture[v].Vitesse = 0;
                        VitesseMiseÀZéro[v] = true;
                    }
                }
                else
                {
                    VitesseMiseÀZéro[v] = false;
                    ListeVoiture[v].AvancePossiblePiste = true;
                    ListeVoiture[v].ReculePossiblePiste = true;
                }
            }
        }

    }
}
