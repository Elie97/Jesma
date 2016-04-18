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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GUI : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch GestionSprites { get; set; }
        SpriteFont ArialFont { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }

        Texture2D Aiguille { get; set; }
        string NomAiguille { get; set; }
        float RotationAiguille { get; set; }

        Texture2D Accéléromètre { get; set; }
        string NomAccéléromètre { get; set; }

        Texture2D Notification { get; set; }

        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }

        string Vitesse { get; set; }
        Vector2 DimensionVitesse { get; set; }
        Vector2 PositionVitesse { get; set; }
        Vector2 PositionAccéléromètre { get; set; }

        int NbVoiture { get; set; }
        public int PositionUtilisateur { get; set; }
        Vector2 PositionDePosition { get; set; }
        Vector2 DimensionPosition { get; set; }

        public int ModeDeJeu { get; set; }

        int Temps { get; set; }
        int[] TableauTemps { get; set; }
        Vector2 PositionTemps { get; set; }
        Vector2 DimensionTemps { get; set; }
        bool AfficherTemps { get; set; }
        public int TempsMilliSeconde { get; set; }

        string ÉtatDépart { get; set; }
        Vector2 PositionDépart { get; set; }
        Vector2 DimensionDépart { get; set; }
        bool AfficherNotification { get; set; }
        float TransparenceNotification { get; set; }
        bool AfficherMessageChangementTour { get; set; }
        int TempsDépart { get; set; }

        bool[] CheckPoint { get; set; }
        int PourcentageCourse { get; set; }
        Vector2 PositionTour { get; set; }
        Vector2 DimensionTour { get; set; }
        int NbTours { get; set; }
        int ToursFait { get; set; }
        int ToursInitial { get; set; }
        bool ChangementTour { get; set; }
        int TempsChangementTour { get; set; }
        public int NbCheckPointFranchis { get; set; }
        Vector2 DimensionÉcran { get; set; }
        int IDVoitureUtilisateur { get; set; }


        //Notification
        RessourcesManager<SoundEffect> GestionnaireDeSon { get; set; }
        SoundEffectInstance SoundNotif { get; set; }
        float ÉchelleNotif { get; set; }
        int TempsAffichageNotif { get; set; }
        bool AffichageContinu { get; set; }
        float TransparenceNotif { get; set; }
        Vector2 DimensionNotif { get; set; }
        Vector2 PositionNotif { get; set; }
        Vector2 DimensionMessageNotif { get; set; }
        Vector2 PositionMessageNotif { get; set; }
        Texture2D[] TableauNotification { get; set; }
        int TypeNotification { get; set; }

        public bool Afficher { get; set; }
        public bool DébuterTemps { get; set; }

        public int NbJoueursConnectés { get; set; }

        bool DépartCourse { get; set; }

        public bool CourseActive { get; set; }

        public GUI(Game game, float intervalleMAJ, string aiguille, string accéléromètre, int nbVoiture, int nbTours, int idVoitureUtilisateur, Vector2 dimensionÉcran, int modeDeJeu)
            : base(game)
        {
            IntervalleMAJ = intervalleMAJ;
            NomAiguille = aiguille;
            NomAccéléromètre = accéléromètre;
            NbVoiture = nbVoiture;
            NbTours = nbTours;
            IDVoitureUtilisateur = idVoitureUtilisateur;
            DimensionÉcran = dimensionÉcran;
            ModeDeJeu = modeDeJeu;
        }


        protected override void LoadContent()
        {
            ArialFont = Game.Content.Load<SpriteFont>("Fonts/Bebas");
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionnaireDeSon = Game.Services.GetService(typeof(RessourcesManager<SoundEffect>)) as RessourcesManager<SoundEffect>;
            Aiguille = GestionnaireDeTextures.Find(NomAiguille);
            Accéléromètre = GestionnaireDeTextures.Find(NomAccéléromètre);
            //Notification = GestionnaireDeTextures.Find("notif");
            TableauNotification = new Texture2D[4] { GestionnaireDeTextures.Find("notif"), GestionnaireDeTextures.Find("notifTour"), GestionnaireDeTextures.Find("notifFinCourse"), GestionnaireDeTextures.Find("notifPerteConnection") };

            SoundNotif = GestionnaireDeSon.Find("notif").CreateInstance();

            base.LoadContent();
        }

        public override void Initialize()
        {
            CourseActive = false;
            TempsÉcouléDepuisMAJ = 0;
            RotationAiguille = 0;
            PourcentageCourse = 0;
            PositionUtilisateur = 0;
            TableauTemps = new int[3];
            TableauTemps[0] = 0;
            TableauTemps[1] = 0;
            TableauTemps[2] = 0;
            NbCheckPointFranchis = 0;
            Temps = 0;
            ToursInitial = 0;
            AfficherTemps = true;
            Vitesse = "200";

            Afficher = true;
            DébuterTemps = false;

            PositionAccéléromètre = new Vector2(103, Game.Window.ClientBounds.Height - 130);
            PositionVitesse = new Vector2(54, Game.Window.ClientBounds.Height - 42);
            PositionDePosition = new Vector2(Percent(5, true), Percent(10, false));
            PositionTemps = new Vector2(Percent(5, true), Percent(15, false));
            PositionTour = new Vector2(Percent(95, true), Percent(10, false));

            ÉtatDépart = "";
            AfficherNotification = false;
            AfficherMessageChangementTour = false;
            TempsDépart = 0;
            TransparenceNotification = 0;

            ChangementTour = false;
            TempsChangementTour = 0;

            //Notif
            ÉchelleNotif = 0f;
            TempsAffichageNotif = 0;
            AffichageContinu = false;
            TransparenceNotif = 0f;
            PositionNotif = new Vector2(Game.Window.ClientBounds.Width / 2f - DimensionNotif.X, 25);
            TypeNotification = 0;

            if (ModeDeJeu == 0)
            {
                NbVoiture++;
            }

            DépartCourse = false;

            base.Initialize();
        }


        public override void Update(GameTime gameTime)
        {
            //Game.Window.Title = Afficher.ToString();
            if (CourseActive)
            {
                if (Afficher)
                {
                    TempsÉcouléDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
                    {
                        if (TempsDépart / 60f <= 4)
                        {
                            TempsDépart++;
                        }
                        //GestionDépart();
                        if (ToursFait != NbTours && TempsDépart / 60f > 2)
                        {
                            Temps++;
                        }

                        AfficherNotif(VérificationChangementTour());

                        NotificationTour(VérificationChangementTour());


                        PositionAccéléromètre = new Vector2(103, Game.Window.ClientBounds.Height - 130);
                        PositionVitesse = new Vector2(54, Game.Window.ClientBounds.Height - 42);
                        PositionDePosition = new Vector2(Percent(5, true), Percent(10, false));
                        PositionTemps = new Vector2(Percent(5, true), Percent(15, false));
                        PositionTour = new Vector2(Percent(95, true), Percent(10, false));
                        PositionDépart = new Vector2(Percent(50, true), Percent(50, false));

                        DimensionDépart = ArialFont.MeasureString(ÉtatDépart);
                        DimensionVitesse = ArialFont.MeasureString(Vitesse);
                        DimensionPosition = ArialFont.MeasureString(PositionUtilisateur.ToString() + "/" + NbVoiture.ToString());
                        DimensionTemps = ArialFont.MeasureString(TableauTemps[2].ToString("00") + ":" + TableauTemps[1].ToString("00") + "." + TableauTemps[0].ToString("00").ToString());
                        DimensionTour = ArialFont.MeasureString(PourcentageCourse.ToString("0/0"));
                        DimensionNotif = new Vector2(300 * ÉchelleNotif, 50 * ÉchelleNotif);
                        PositionNotif = new Vector2(Game.Window.ClientBounds.Width / 2f - DimensionNotif.X / 2f, PositionNotif.Y);
                        DimensionMessageNotif = ArialFont.MeasureString(GetMessage(0));
                        PositionMessageNotif = PositionNotif + new Vector2(150 * ÉchelleNotif, DimensionNotif.Y / 2f);
                        RotationAiguille = ((Math.Abs(Convert.ToInt32(Vitesse) / Voiture.VITESSE_MAX)) * (float)2.2f) + 0.85f;//Vitesse max!
                        if (DébuterTemps)
                        {
                            GestionTemps();
                        }
                        TempsMilliSeconde = TableauTemps[0] + TableauTemps[1] * 60 + TableauTemps[2] * 3600;
                        TempsÉcouléDepuisMAJ = 0;
                    }
                
            }

                base.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (CourseActive)
            {
                if (Afficher)
                {
                    GestionSprites.Draw(Accéléromètre, PositionAccéléromètre, null, Color.White, 0, new Vector2(167, 27), 0.40f, SpriteEffects.None, 0);
                    //GestionSprites.DrawString(ArialFont, Vitesse, PositionVitesse, new Color(17, 83, 133), 0f, new Vector2(DimensionVitesse.X / 2, DimensionVitesse.Y / 2), 0.375f, SpriteEffects.None, 0);
                    //GestionSprites.Draw(Aiguille, new Vector2(103, Game.Window.ClientBounds.Height - 70), null, Color.White, RotationAiguille, new Vector2(167, 27), 0.35f, SpriteEffects.None, 0);

                    GestionSprites.DrawString(ArialFont, Vitesse, PositionVitesse + new Vector2(1, 1), Color.Black, 0f, new Vector2(DimensionVitesse.X / 2, DimensionVitesse.Y / 2), 0.70f, SpriteEffects.None, 0);
                    GestionSprites.DrawString(ArialFont, Vitesse, PositionVitesse, Color.White, 0f, new Vector2(DimensionVitesse.X / 2, DimensionVitesse.Y / 2), 0.70f, SpriteEffects.None, 0);
                    GestionSprites.Draw(Aiguille, new Vector2(90, Game.Window.ClientBounds.Height - 24), null, Color.White, RotationAiguille, new Vector2(267, 27), 0.35f, SpriteEffects.None, 0);

                    GestionSprites.DrawString(ArialFont, ("POSITION"), PositionDePosition - new Vector2(12, 25) + new Vector2(1, 1), Color.Black, 0f, new Vector2(DimensionPosition.X / 2, DimensionPosition.Y / 2), 0.35f, SpriteEffects.None, 0);
                    GestionSprites.DrawString(ArialFont, ("POSITION"), PositionDePosition - new Vector2(12, 25), Color.White, 0f, new Vector2(DimensionPosition.X / 2, DimensionPosition.Y / 2), 0.35f, SpriteEffects.None, 0);

                    GestionSprites.DrawString(ArialFont, (PositionUtilisateur.ToString() + "/" + NbVoiture.ToString()), PositionDePosition + new Vector2(1, 1), Color.Black, 0f, new Vector2(DimensionPosition.X / 2, DimensionPosition.Y / 2), 0.625f, SpriteEffects.None, 0);
                    GestionSprites.DrawString(ArialFont, (PositionUtilisateur.ToString() + "/" + NbVoiture.ToString()), PositionDePosition, Color.White, 0f, new Vector2(DimensionPosition.X / 2, DimensionPosition.Y / 2), 0.625f, SpriteEffects.None, 0);

                    if (AfficherTemps)
                    {
                        GestionSprites.DrawString(ArialFont, (TableauTemps[2].ToString("00") + ":" + TableauTemps[1].ToString("00") + "." + TableauTemps[0].ToString("00").ToString()), PositionTemps + new Vector2(1, 1), Color.Black, 0f, new Vector2(DimensionTemps.X / 2, DimensionTemps.Y / 2), 0.375f, SpriteEffects.None, 0);
                        GestionSprites.DrawString(ArialFont, (TableauTemps[2].ToString("00") + ":" + TableauTemps[1].ToString("00") + "." + TableauTemps[0].ToString("00").ToString()), PositionTemps, Color.White, 0f, new Vector2(DimensionTemps.X / 2, DimensionTemps.Y / 2), 0.375f, SpriteEffects.None, 0);

                    }

                    GestionSprites.DrawString(ArialFont, ("TOUR"), PositionTour - new Vector2(5, 25) + new Vector2(1, 1), Color.Black, 0f, new Vector2(DimensionTour.X / 2, DimensionTour.Y / 2), new Vector2(0.40f, 0.35f), SpriteEffects.None, 0);
                    GestionSprites.DrawString(ArialFont, ("TOUR"), PositionTour - new Vector2(5, 25), Color.White, 0f, new Vector2(DimensionTour.X / 2, DimensionTour.Y / 2), new Vector2(0.40f, 0.35f), SpriteEffects.None, 0);


                    GestionSprites.DrawString(ArialFont, (ToursFait + "/" + NbTours), PositionTour + new Vector2(1, 1), Color.Black, 0f, new Vector2(DimensionTour.X / 2, DimensionTour.Y / 2), 0.625f, SpriteEffects.None, 0);
                    GestionSprites.DrawString(ArialFont, (ToursFait + "/" + NbTours), PositionTour, Color.White, 0f, new Vector2(DimensionTour.X / 2, DimensionTour.Y / 2), 0.625f, SpriteEffects.None, 0);

                    GestionSprites.Draw(TableauNotification[TypeNotification], PositionNotif, null, new Color(255, 255, 255, TransparenceNotif), 0, DimensionNotif, ÉchelleNotif, SpriteEffects.None, 0);
                    GestionSprites.DrawString(ArialFont, GetMessage(0), PositionMessageNotif, new Color(0, 0, 0, TransparenceNotif), 0, DimensionNotif, ÉchelleNotif, SpriteEffects.None, 0);

                    //Afficher notification si un joueur se déconnecte lors d'une partie en lan
                    if (ModeDeJeu == 1 && DépartCourse && NbJoueursConnectés == 1)// true est si la course est commencée
                    {//besoin d'optimisation
                        GestionSprites.Draw(TableauNotification[3], new Vector2(Game.Window.ClientBounds.Width / 2f - (TableauNotification[3].Width * 0.4f / 2f), 25), null, Color.White, 0, DimensionNotif, 0.4f, SpriteEffects.None, 0);
                        GestionSprites.DrawString(ArialFont, GetMessage(4), new Vector2(Game.Window.ClientBounds.Width / 2f - (TableauNotification[3].Width * 0.4f / 2f), 35) + new Vector2(150 * 0.4f, DimensionNotif.Y / 2f),
                            Color.Black, 0, DimensionNotif, 0.4f, SpriteEffects.None, 0);

                    }

                    GestionSprites.DrawString(ArialFont, ÉtatDépart, PositionDépart + new Vector2(1, 1), Color.Black, 0f, new Vector2(DimensionDépart.X / 2, DimensionDépart.Y / 2), 1f, SpriteEffects.None, 0);
                    GestionSprites.DrawString(ArialFont, ÉtatDépart, PositionDépart, Color.White, 0f, new Vector2(DimensionDépart.X / 2, DimensionDépart.Y / 2), 1f, SpriteEffects.None, 0);

                    //if (AfficherMessageChangementTour)
                    //{
                    //    if (NbTours - ToursFait != 0)
                    //    {
                    //        GestionSprites.DrawString(ArialFont, GetMessage(0), PositionDépart + new Vector2(0, -50) + new Vector2(1, 1), Color.Black, 0f, ArialFont.MeasureString(GetMessage(0)) * new Vector2(0.5f, 0.5f), 1f, SpriteEffects.None, 0);
                    //        GestionSprites.DrawString(ArialFont, GetMessage(0), PositionDépart + new Vector2(0, -50), Color.White, 0f, ArialFont.MeasureString(GetMessage(0)) * new Vector2(0.5f, 0.5f), 1f, SpriteEffects.None, 0);

                    //        GestionSprites.DrawString(ArialFont, GetMessage(1), PositionDépart + new Vector2(0, 10) + new Vector2(1, 1), Color.Black, 0f, ArialFont.MeasureString(GetMessage(1)) * new Vector2(0.5f, 0.5f), 0.5f, SpriteEffects.None, 0);
                    //        GestionSprites.DrawString(ArialFont, GetMessage(1), PositionDépart + new Vector2(0, 10), Color.White, 0f, ArialFont.MeasureString(GetMessage(1)) * new Vector2(0.5f, 0.5f), 0.5f, SpriteEffects.None, 0);    
                    //    }
                    //    else
                    //    {
                    //        GestionSprites.DrawString(ArialFont, GetMessage(2), PositionDépart + new Vector2(0, -50) + new Vector2(1, 1), Color.Black, 0f, ArialFont.MeasureString(GetMessage(2)) * new Vector2(0.5f, 0.5f), 1f, SpriteEffects.None, 0);
                    //        GestionSprites.DrawString(ArialFont, GetMessage(2), PositionDépart + new Vector2(0, -50), Color.White, 0f, ArialFont.MeasureString(GetMessage(2)) * new Vector2(0.5f, 0.5f), 1f, SpriteEffects.None, 0);

                    //        GestionSprites.DrawString(ArialFont, GetMessage(3), PositionDépart + new Vector2(0, 10) + new Vector2(1, 1), Color.Black, 0f, ArialFont.MeasureString(GetMessage(3)) * new Vector2(0.5f, 0.5f), 0.5f, SpriteEffects.None, 0);
                    //        GestionSprites.DrawString(ArialFont, GetMessage(3), PositionDépart + new Vector2(0, 10), Color.White, 0f, ArialFont.MeasureString(GetMessage(3)) * new Vector2(0.5f, 0.5f), 0.5f, SpriteEffects.None, 0);    
                    //    }


                    //}
                }
            }

            base.Draw(gameTime);
        }

        public void UpdateGUI(int vitesse, int positionUtilisateur, bool[] checkPoint, int toursFait)
        {
            Vitesse = Math.Abs(vitesse).ToString();
            PositionUtilisateur = positionUtilisateur;
            CheckPoint = checkPoint;
            ToursFait = toursFait;
        }

        public void GetDépartCourse(bool départ)
        {
            DépartCourse = départ;
        }

        public void GestionTemps()
        {
            TableauTemps[0] = Temps;
            if (TableauTemps[0] > 60)
            {
                Temps = 0;
                TableauTemps[1]++;
            }
            if (TableauTemps[1] > 60)
            {
                Temps = 0;
                TableauTemps[1] = 0;
                TableauTemps[2]++;
            }
        }

        public void UpdateNbCheckPointFranchis()
        {
            NbCheckPointFranchis++;
        }

        float Percent(float percent, bool x)
        {
            float pixel;
            if (x)
            {
                pixel = (percent / 100f) * DimensionÉcran.X;
            }
            else
            {
                pixel = (percent / 100f) * DimensionÉcran.Y;
            }
            return pixel;
        }

        public void UpdateScreenSize(Vector2 dimension)
        {
            DimensionÉcran = dimension;
        }

        void GestionDépart()
        {
            if (TempsDépart / 60f <= 1f)
            {
                AfficherNotification = true;
                ÉtatDépart = "Prêt";                
            }
            else if (TempsDépart / 60f > 1f && TempsDépart / 60f <= 1.5f)
            {
                AfficherNotification = true;
                ÉtatDépart = "";
            }
            else if (TempsDépart / 60f > 1.5f && TempsDépart / 60f <= 2)
            {
                AfficherNotification = true;
                ÉtatDépart = "Partez!";
            }
            else if (TempsDépart / 60f > 2)
            {
                AfficherNotification = false;
                ÉtatDépart = "";
            }

            if (AfficherNotification)
            {
                if (TransparenceNotification < 1)
	            {
                    TransparenceNotification = TransparenceNotification + 0.02f;
	            }
            }
            else
            {
                TransparenceNotification = Variation(TransparenceNotification, -0.02f);
            }
        }

        void AfficherNotif(bool afficher)
        {
            if (afficher || AffichageContinu)
            {
                TempsAffichageNotif++;
                AffichageContinu = true;
                if (afficher)
                {
                    SoundNotif.Play();
                }
            }

            if (TempsAffichageNotif / 60f <= 1f && TempsAffichageNotif / 60f > 0)
            {
                if (TransparenceNotif < 1f)
                {
                    TransparenceNotif += 0.02f;
                    ÉchelleNotif += (0.5f / 60f);
                }
                else
                {
                    TransparenceNotif = 1;
                    if (ÉchelleNotif > 0.35f)
                    {
                        ÉchelleNotif -= (0.5f / 60f);
                    }
                }
            }

            if (TempsAffichageNotif / 60f >= 4f)
            {
                if (PositionNotif.Y >= -DimensionNotif.Y*2)
                {
                    PositionNotif = new Vector2(PositionNotif.X, PositionNotif.Y - 1f);                    
                }
                else
                {
                   // TransparenceNotif = 0;
                   // PositionNotif = new Vector2(PositionNotif.X, 25);
                }
            }

            if (TempsAffichageNotif / 60f >= 5)
            {
                AffichageContinu = false;
                TempsAffichageNotif = 0;
                TransparenceNotif = 0;
                ÉchelleNotif = 0;
                PositionNotif = new Vector2(PositionNotif.X, 25);
            }
        }

        float Variation(float valeur, float décrémentation)
        {
            if (valeur > 0)
            {
                valeur += décrémentation;
            }
            return valeur;
        }

        void NotificationTour(bool afficher)
        {
            bool affichageContinue = false;
            if (afficher || affichageContinue)
            {
                affichageContinue = true;
                AfficherNotification = true;
                TransparenceNotification = 1.0f;
                AfficherMessageChangementTour = true;
            }
            if (!AfficherNotification)
            {
                AfficherMessageChangementTour = false;
            }
        }
        
        bool VérificationChangementTour()
        {
            if (ChangementTour)
            {
                TempsChangementTour++;
            }

            if (ToursInitial != ToursFait)
            {
                ToursInitial = ToursFait;
                ChangementTour = true;
            }

            if (TempsChangementTour / 60f >= 1)
            {
                TempsChangementTour = 0;
                ChangementTour = false;
            }

            if (ToursFait == NbTours)
            {
                TypeNotification = 2;
            }
            else
            {
                TypeNotification = 1;
            }

            return ChangementTour;
        }

        string GetMessage(int message)
        {
            string msg = "";

            if (message == 0)
            {
                msg = "Tour Complet!";
            }
            else if (message == 1)
            {
                string tour = "tour";
                if (NbTours - ToursFait != 1)
                {
                    tour = "tours";
                }
                msg = "Plus que " + (NbTours - ToursFait).ToString() + " " + tour + " à faire!";
   
            }
            else if(message == 2)
            {
                msg = "Victoire!";
                if (PositionUtilisateur != 1)
                {
                    msg = "Défaite!";
                }
            }
            else if (message == 3)
            {
                msg = "Vous avez fini " + PositionUtilisateur + " sur " + NbVoiture + " en un temps de " +(TableauTemps[2].ToString("00") + ":" + TableauTemps[1].ToString("00") + "." + TableauTemps[0].ToString("00").ToString()); 
            }
            else if(message == 4)
            {
                msg = "Perte de connection avec l'adversaire";
            }

            return msg;
        }
    }
}
