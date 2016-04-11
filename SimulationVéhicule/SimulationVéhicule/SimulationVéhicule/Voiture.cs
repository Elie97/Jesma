using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;


namespace SimulationVéhicule
{
    public class Voiture : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const float DÉCÉLÉRATION = 0.02f;// en décimètre
        public const float VITESSE_MAX = 200;// Km/H
        const float VITESSE_MAX_RECULONS = -30.0f;// Km/H
        const float ACCÉLÉRATION = 4.0f;// Temps en seconde pour atteindre 100 km/h 4.0f!!!
        const float ACCÉLÉRATION_RECULONS = -2.0f;// Temps en seconde pour atteindre 100 km/h
        const float MASSE_VOITURE = 1000.0f;// En Kilogramme
        public const float ACCÉLÉRATION_FREIN = 3.0f;// Km/h Valeur pas correcte
        const float ROTATION_MAXIMALE_ROUE = (float)Math.PI / 80f;
        const float ROTATION_MAX_MODELE_ROUE = 0.8f;
        Vector3 DIMENSION = new Vector3(20, 25, 40);


        string NomModèle { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        RessourcesManager<SoundEffect> GestionnaireDeSon { get; set; }
        RessourcesManager<Song> GestionnaireDeMusique { get; set; }
        Caméra CaméraJeu { get; set; }
        protected float Échelle { get; set; }
        protected float ÉchelleInitiale { get; set; }
        public Vector3 Rotation { get; set; }
        protected Vector3 RotationInitiale { get; set; }
        public Vector3 Position { get; set; }
        public Model Modèle { get; private set; }

        Model[] Roues { get; set; }
        List<Matrix[]> TransformationsRoues { get; set; }
        Matrix[] MondeRoue { get; set; }
        Vector3 RotationRoue { get; set; }
        float RotationModèleRoue { get; set; }

        protected Matrix[] TransformationsModèle { get; private set; }
        protected Matrix Monde { get; set; }
        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        InputManager GestionInput { get; set; }
        bool EnAvant { get; set; }
        Vector3 Dimension { get; set; }
        public float Vitesse { get; set; }
        int CPT { get; set; }

        int Déplacement { get; set; }
        float RotationMaximaleDéplacement { get; set; }
        float Temps { get; set; }
        float RotationVoiture { get; set; }

        float FacteurEngine { get; set; }

        SoundEffectInstance SoundAcceleration { get; set; }
        SoundEffectInstance SoundBrake { get; set; }
        SoundEffectInstance SoundChangementVitesse { get; set; }
        SoundEffectInstance SoundCourbe { get; set; }
        SoundEffectInstance BornToBeWild { get; set; }
        SoundEffectInstance ChaseAndStatus { get; set; }
        List<SoundEffectInstance> ListeChanson { get; set; }

        bool User { get; set; }

        public bool EnContactVoiture { get; set; }
        bool AvanceCollision { get; set; }
        bool AvancePossible { get; set; }
        public bool AvancePossiblePiste { get; set; }
        public bool ReculePossiblePiste { get; set; }
        bool AccélérationPossible { get; set; }
        bool CollisionPrête { get; set; }
        int Collision { get; set; }
        float RotationEnCollision { get; set; }
        float VitesseRotation { get; set; }
        bool DernièreCollisionEstAvant { get; set; }
        bool Translation { get; set; }
        public bool ChuteLibrePossible { get; set; }


        public BoundingBox BoxVoiture { get; set; }
        public BoundingSphere SphereVoitureAvant { get; set; }
        BoundingSphere SphereVoitureArrière { get; set; }
        BoundingSphere SphereVoitureMillieu { get; set; }
        public BoundingSphere SphereRouteAvant { get; set; }

        BoundingBox BoxVoitureAvant { get; set; }
        BoundingBox BoxVoitureArrière { get; set; }
        BoundingBox BoxVoitureMillieu { get; set; }

        SpriteBatch GestionSprites { get; set; }//ToDELETE
        SpriteFont ArialFont { get; set; }//ToDELETE

        public bool Controle { get; set; }

        public bool Afficher { get; set; }

        public int NbFranchis { get; set; }

        bool CourseActive { get; set; }

        public Voiture(Game jeu, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, bool user, bool afficher)
            : base(jeu)
        {
            NomModèle = nomModèle;
            Position = positionInitiale;
            ÉchelleInitiale = échelleInitiale;
            Échelle = échelleInitiale;
            Rotation = rotationInitiale;
            RotationInitiale = rotationInitiale;
            IntervalleMAJ = intervalleMAJ;
            User = user;
            Afficher = afficher;
        }

        public override void Initialize()
        {
            CalculerMonde();
            Controle = true;
            NbFranchis = 0;
            Dimension = new Vector3(20, 25, 40);
            EnAvant = true;
            Vitesse = 0;
            CPT = 0;
            Déplacement = 0;
            Temps = 0;
            EnContactVoiture = false;
            FacteurEngine = -0.5f;
            AvanceCollision = true;
            AvancePossiblePiste = true;
            AccélérationPossible = true;
            ReculePossiblePiste = true;
            CollisionPrête = false;
            DernièreCollisionEstAvant = false;
            Translation = false;
            ChuteLibrePossible = true;
            AvancePossible = true;
            Collision = 0;
            RotationEnCollision = 0;
            VitesseRotation = 0;
            Roues = new Model[4];
            MondeRoue = new Matrix[4];
            TransformationsRoues = new List<Matrix[]>(4);
            for (int i = 0; i < MondeRoue.Length; i++)
            {
                MondeRoue[i] = Matrix.Identity * Matrix.CreateScale(Échelle);
            }
            RotationRoue = new Vector3(0, 0, 0);
            RotationModèleRoue = 0;
            base.Initialize();
        }

        private void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        protected override void LoadContent()
        {
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;//ToDELETE
            ArialFont = Game.Content.Load<SpriteFont>("Fonts/Arial");//ToDELETE
            CaméraJeu = Game.Services.GetService(typeof(Caméra)) as Caméra;
            GestionnaireDeModèles = Game.Services.GetService(typeof(RessourcesManager<Model>)) as RessourcesManager<Model>;
            GestionnaireDeSon = Game.Services.GetService(typeof(RessourcesManager<SoundEffect>)) as RessourcesManager<SoundEffect>;
            GestionnaireDeMusique = Game.Services.GetService(typeof(RessourcesManager<Song>)) as RessourcesManager<Song>;
            Modèle = GestionnaireDeModèles.Find(NomModèle);

            Roues[0] = GestionnaireDeModèles.Find("Pneu");
            Roues[1] = GestionnaireDeModèles.Find("Pneu");
            Roues[2] = GestionnaireDeModèles.Find("Pneu");
            Roues[3] = GestionnaireDeModèles.Find("Pneu");

            TransformationsRoues.Add(new Matrix[Roues[0].Bones.Count]);
            TransformationsRoues.Add(new Matrix[Roues[1].Bones.Count]);
            TransformationsRoues.Add(new Matrix[Roues[2].Bones.Count]);
            TransformationsRoues.Add(new Matrix[Roues[3].Bones.Count]);

            TransformationsModèle = new Matrix[Modèle.Bones.Count];
            Modèle.CopyAbsoluteBoneTransformsTo(TransformationsModèle);
            for (int i = 0; i < Roues.Length; i++)
            {
                Roues[i].CopyAbsoluteBoneTransformsTo(TransformationsRoues[i]);
            }
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            SoundAcceleration = GestionnaireDeSon.Find("Gear1").CreateInstance();
            SoundBrake = GestionnaireDeSon.Find("brakeEffect").CreateInstance();
            SoundCourbe = GestionnaireDeSon.Find("BrakeCurveMajor").CreateInstance();
            BornToBeWild = GestionnaireDeSon.Find("BornToBeWild").CreateInstance();
            ChaseAndStatus = GestionnaireDeSon.Find("ChaseAndStatus").CreateInstance();

            ListeChanson = new List<SoundEffectInstance>();
            ListeChanson.Add(BornToBeWild);
            ListeChanson.Add(ChaseAndStatus);
        }

        public override void Draw(GameTime gameTime)
        {
            if (CourseActive)
            {
                if (Afficher)
                {
                    foreach (ModelMesh Mesh in Modèle.Meshes)
                    {
                        foreach (BasicEffect Effect in Mesh.Effects)
                        {
                            Effect.Projection = CaméraJeu.Projection;
                            Effect.View = CaméraJeu.Vue;
                            Effect.World = TransformationsModèle[Mesh.ParentBone.Index] * GetMonde();
                            Effect.LightingEnabled = true;
                            Effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
                        }
                        Mesh.Draw();
                    }

                    for (int i = 0; i < Roues.Length; i++)
                    {
                        foreach (ModelMesh Mesh in Roues[i].Meshes)
                        {
                            foreach (BasicEffect Effect in Mesh.Effects)
                            {
                                Effect.Projection = CaméraJeu.Projection;
                                Effect.View = CaméraJeu.Vue;
                                Effect.World = TransformationsRoues[i][Mesh.ParentBone.Index] * GetMondeRoues(i);
                                Effect.EnableDefaultLighting();
                            }
                            Mesh.Draw();
                        }
                    }

                    if (GestionInput.EstEnfoncée(Keys.C))
                    {
                        Info();//ToDELETE
                    }
                } 
            }


            // DebugShapeRenderer.AddBoundingBox(BoxVoiture, Color.Red);
            //DebugShapeRenderer.AddBoundingBox(BoxVoitureAvant, Color.Green);
            //DebugShapeRenderer.AddBoundingBox(BoxVoitureMillieu, Color.Red);
            //DebugShapeRenderer.AddBoundingBox(BoxVoitureArrière, Color.Yellow);
            //DebugShapeRenderer.AddBoundingSphere(SphereRouteAvant, Color.Yellow);
            //DebugShapeRenderer.AddBoundingSphere((SphereVoitureAvant), Color.Green);
            //DebugShapeRenderer.AddBoundingSphere((SphereVoitureMillieu), Color.Red);
            //DebugShapeRenderer.AddBoundingSphere((SphereVoitureArrière), Color.Yellow);

        }

        public override void Update(GameTime gameTime)
        {
            CourseActive = (bool)Game.Services.GetService(typeof(bool));
            if (CourseActive)
            {
                TempsÉcouléDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
                {

                    if (User)
                    {
                        //Game.Window.Title = Rotation.ToString();
                        if (Controle)
                        {
                            if (GestionInput.EstEnfoncée(Keys.W) && Vitesse >= 0 && AvancePossible && AvancePossiblePiste)
                            {
                                if (GestionInput.EstEnfoncée(Keys.E))
                                {
                                    EnAvant = true;
                                    if (AccélérationPossible)
                                    {
                                        Accélération(EnAvant);
                                    }
                                }
                                Avance();
                            }
                            else if (GestionInput.EstEnfoncée(Keys.S) && Vitesse <= 0 && ReculePossiblePiste)
                            {
                                if (GestionInput.EstEnfoncée(Keys.E))
                                {
                                    EnAvant = false;
                                    if (AccélérationPossible)
                                    {
                                        Accélération(EnAvant);
                                    }
                                }
                                Avance();
                            }
                            else
                            {
                                Décélération(EnAvant);
                                Avance();
                            }
                            if (Vitesse != 0)
                            {
                                GestionRotationVoiture();
                            }
                            if (GestionInput.EstEnfoncée(Keys.Tab))
                            {
                                Freinage();
                            }

                            if (Position.Y <= 0)
                            {
                                Temps = 0;
                                Position = new Vector3(Position.X, 0, Position.Z);
                            }
                            //else
                            //{
                            //    GetHauteur();
                            //}

                            PitchAndSound();
                        }
                    }

                    CalculerMonde();
                    CreateBoundingBox();
                    TempsÉcouléDepuisMAJ = 0;
                }
                Radio();    
            } 
            base.Update(gameTime);
        }

        public virtual Matrix GetMonde()
        {
            return Monde;
        }

        Matrix GetMondeRoues(int roue)
        {
            GetRotationModèleRoue();
            Matrix monde = Matrix.Identity;
            if (roue == 0)
            {
                RotationRoue = new Vector3(RotationRoue.X + 0.0005f * PixelToKMH(Vitesse), RotationRoue.Y, RotationRoue.Z);
                MondeRoue[0] = Matrix.Identity;
                MondeRoue[0] *= Matrix.CreateScale(Échelle);
                MondeRoue[0] *= Matrix.CreateFromYawPitchRoll(Rotation.Y, RotationRoue.X, Rotation.Z);
                MondeRoue[0] *= Matrix.CreateTranslation(new Vector3(Position.X - 27 * (float)Math.Sin(Rotation.Y) - 8 * (float)Math.Cos(Rotation.Y), Position.Y + 4 + ((float)Math.Sin(Rotation.X) * 27), Position.Z - 27 * (float)Math.Cos(Rotation.Y) + 8 * (float)Math.Sin(Rotation.Y)));
                monde = MondeRoue[0];
            }
            else if (roue == 1)
            {
                RotationRoue = new Vector3(RotationRoue.X + 0.0005f * PixelToKMH(Vitesse), RotationRoue.Y, RotationRoue.Z);
                MondeRoue[1] = Matrix.Identity;
                MondeRoue[1] *= Matrix.CreateScale(Échelle);
                MondeRoue[1] *= Matrix.CreateFromYawPitchRoll(Rotation.Y + RotationModèleRoue, RotationRoue.X, Rotation.Z);
                MondeRoue[1] *= Matrix.CreateTranslation(new Vector3(Position.X - 8 * (float)Math.Cos(Rotation.Y), Position.Y + 4, Position.Z + 8 * (float)Math.Sin(Rotation.Y)));
                monde = MondeRoue[1];
            }
            else if (roue == 2)
            {
                RotationRoue = new Vector3(RotationRoue.X + 0.0005f * PixelToKMH(Vitesse), RotationRoue.Y, RotationRoue.Z);
                MondeRoue[2] = Matrix.Identity;
                MondeRoue[2] *= Matrix.CreateScale(Échelle);
                MondeRoue[2] *= Matrix.CreateFromYawPitchRoll(Rotation.Y, RotationRoue.X, MathHelper.Pi);
                MondeRoue[2] *= Matrix.CreateTranslation(new Vector3(Position.X - 27 * (float)Math.Sin(Rotation.Y) + 8 * (float)Math.Cos(Rotation.Y), Position.Y + 4 + ((float)Math.Sin(Rotation.X) * 27), Position.Z - 27 * (float)Math.Cos(Rotation.Y) - 8 * (float)Math.Sin(Rotation.Y)));
                monde = MondeRoue[2];
            }
            else if (roue == 3)
            {
                RotationRoue = new Vector3(RotationRoue.X + 0.0005f * PixelToKMH(Vitesse), RotationRoue.Y, RotationRoue.Z);
                MondeRoue[3] = Matrix.Identity;
                MondeRoue[3] *= Matrix.CreateScale(Échelle);
                MondeRoue[3] *= Matrix.CreateFromYawPitchRoll(Rotation.Y + RotationModèleRoue, RotationRoue.X, MathHelper.Pi);
                MondeRoue[3] *= Matrix.CreateTranslation(new Vector3(Position.X + 8 * (float)Math.Cos(Rotation.Y), Position.Y + 4, Position.Z - 8 * (float)Math.Sin(Rotation.Y)));
                monde = MondeRoue[3];
            }
            return monde;
        }

        void GetRotationModèleRoue()
        {
            if (GestionInput.EstEnfoncée(Keys.Left) || GestionInput.EstEnfoncée(Keys.Right))
            {
                if (RotationModèleRoue < ROTATION_MAX_MODELE_ROUE && GestionInput.EstEnfoncée(Keys.Left))
                {
                    RotationModèleRoue += 0.0016f;
                }
                if (RotationModèleRoue > -ROTATION_MAX_MODELE_ROUE && GestionInput.EstEnfoncée(Keys.Right))
                {
                    RotationModèleRoue -= 0.0016f;
                }
            }
            else
            {
                if (RotationModèleRoue > 0)
                {
                    RotationModèleRoue -= 0.0016f;
                }
                else if (RotationModèleRoue < 0)
                {
                    RotationModèleRoue += 0.0016f;
                }
            }
        }

        public bool GestionToucheActive(Keys key, bool estActif)
        {
            if (GestionInput.EstNouvelleTouche(key))
            {
                estActif = !estActif;
            }
            return estActif;
        }

        public void Avance()
        {
            Position = new Vector3(Position.X + (Vitesse * (float)Math.Sin(Rotation.Y)), Position.Y, Position.Z + (Vitesse * (float)Math.Cos(Rotation.Y)));

        }

        void TranslationCollision(float vitesse)
        {
            Position = new Vector3(Position.X + (vitesse * (float)Math.Cos(Rotation.Y)), Position.Y, Position.Z + (vitesse * (float)Math.Sin(Rotation.Y)));
        }

        public void Décélération(bool négative)
        {
            if (négative)
            {
                if (Vitesse > 0)
                {
                    Vitesse -= DÉCÉLÉRATION;
                }
                else
                {
                    Vitesse = 0;
                }
            }
            else
            {
                if (Vitesse < 0)
                {
                    Vitesse += DÉCÉLÉRATION;
                }
                else
                {
                    Vitesse = 0;
                }
            }
        }

        void Accélération(bool positive)
        {
            if (positive)
            {
                if (Vitesse < KMHtoPixel(VITESSE_MAX))
                {
                    Vitesse += ((KMHtoPixel(100) / ACCÉLÉRATION) / 60f);//BUG!!!
                }
                else
                {
                    Vitesse = KMHtoPixel(VITESSE_MAX);
                }
            }
            else
            {
                if (Vitesse > KMHtoPixel(VITESSE_MAX_RECULONS))
                {
                    Vitesse -= ((KMHtoPixel(VITESSE_MAX_RECULONS) / ACCÉLÉRATION_RECULONS) / 60f);
                }
            }
        }

        void Freinage()
        {
            if (Vitesse > 0)
            {
                Vitesse -= (ACCÉLÉRATION_FREIN) / 60f;
            }
            else if (Vitesse < 0)
            {
                Vitesse += (ACCÉLÉRATION_FREIN) / 60f;
            }
            else
            {
                Vitesse = 0;
            }
        }

        float KMHtoPixel(float kmh)
        {
            return (kmh / 3.6f) / 6;
        }

        public float PixelToKMH(float pixel)
        {
            return (pixel * 6) * 3.6f;
        }

        void GestionRotationVoiture()
        {
            float rotation = 0;
            if (Vitesse >= KMHtoPixel(Math.Abs(VITESSE_MAX_RECULONS)))
            {
                RotationVoiture = ROTATION_MAXIMALE_ROUE;
            }
            else
            {
                RotationVoiture = ROTATION_MAXIMALE_ROUE * (Vitesse / KMHtoPixel(Math.Abs(VITESSE_MAX_RECULONS)));
            }

            if ((Rotation.Y + RotationVoiture) >= (Math.PI * 2))
            {
                rotation -= (float)Math.PI * 2;
            }
            else if ((Rotation.Y - RotationVoiture) <= (Math.PI * -2))
            {
                rotation -= (float)Math.PI * 2;
            }

            if (GestionInput.EstEnfoncée(Keys.Left))
            {
                Rotation = new Vector3(Rotation.X, Rotation.Y + RotationVoiture, Rotation.Z);

            }
            if (GestionInput.EstEnfoncée(Keys.Right))
            {
                Rotation = new Vector3(Rotation.X, Rotation.Y - RotationVoiture, Rotation.Z);
            }
        }

        public void GetHauteur()
        {
            Temps++; // 1/60s
            float g = 98.1f / (60f * 60f);
            float vitesse = g * Temps;
            Position = new Vector3(Position.X, Position.Y - vitesse, Position.Z);
        }

        void PitchAndSound()
        {
            SoundAcceleration.Play();
            SoundAcceleration.Volume = 1.0f;
            if (!GestionInput.EstEnfoncée(Keys.Tab))
            {
                if (SoundCourbe.Volume - 0.05f >= 0)
                {
                    SoundCourbe.Volume = SoundCourbe.Volume - 0.05f;
                }
                //if (Vitesse >= KMHtoPixel(0) && Vitesse < KMHtoPixel(30))
                //{
                    SoundAcceleration.Pitch = (Math.Abs(PixelToKMH(Vitesse)) / VITESSE_MAX) - 0.1f;
                //}
                //else if (Vitesse >= KMHtoPixel(30) && Vitesse < KMHtoPixel(60))
                //{
                //    //SoundChangementVitesse.Play();
                //    SoundAcceleration.Pitch = (Math.Abs(PixelToKMH(Vitesse)) / VITESSE_MAX) - 0.3f;
                //}
                //else if (Vitesse >= KMHtoPixel(60) && Vitesse < KMHtoPixel(100))
                //{
                //    SoundAcceleration.Pitch = (Math.Abs(PixelToKMH(Vitesse)) / VITESSE_MAX) - 0.5f;
                //}
                //else if (Vitesse >= KMHtoPixel(100) && Vitesse < KMHtoPixel(150))
                //{
                //    SoundAcceleration.Pitch = (Math.Abs(PixelToKMH(Vitesse)) / VITESSE_MAX) - 0.7f;
                //}
                //else if (Vitesse >= KMHtoPixel(150))
                //{
                //    SoundAcceleration.Pitch = (Math.Abs(PixelToKMH(Vitesse)) / VITESSE_MAX) - 0.9f;
                //}
            }
            else
            {
                SoundAcceleration.Pitch = (Math.Abs(PixelToKMH(Vitesse)) / VITESSE_MAX) - 0.1f;
                if (Vitesse >= KMHtoPixel(60))
                {
                    SoundCourbe.Play();
                    SoundCourbe.Volume = 1.0f;
                }
            }

            if ((GestionInput.EstEnfoncée(Keys.Left) || GestionInput.EstEnfoncée(Keys.Right)) && Vitesse >= KMHtoPixel(70))
            {
                SoundCourbe.Play();
                SoundCourbe.Volume = 0.5f;
            }
            else
            {
                SoundCourbe.Stop();
            }

            if (Vitesse == 0)
            {
                SoundAcceleration.Volume = 0.5f;
            }
        }

        public void GestionCollisionVoiture(Voiture voiture)
        {
            float deltaRotation = NormalizeRotation(Rotation.Y) - NormalizeRotation(voiture.Rotation.Y);
            if (BoxVoitureAvant.Intersects(voiture.BoxVoitureArrière))
            {
                AvanceCollision = true;
                Translation = false;
                DernièreCollisionEstAvant = false;
                RotationEnCollision = (float)Math.Sin(deltaRotation) * ((float)(Math.PI / 8f) * (PixelToKMH(Vitesse) / VITESSE_MAX));
                RotationCollision(voiture, true, RotationEnCollision, DernièreCollisionEstAvant);
                voiture.Vitesse += Vitesse * 0.5f;
                Vitesse -= Vitesse * 0.5f;
                voiture.Avance();
                AvancePossible = false;
            }
            else if (BoxVoitureAvant.Intersects(voiture.BoxVoitureMillieu) && !BoxVoitureAvant.Intersects(voiture.BoxVoitureArrière))
            {
                Translation = true;
                if (PixelToKMH(Vitesse) >= 40.0f)
                {
                    Vitesse /= 2f;
                }
                if (Vitesse > 0)
                {
                    Vitesse -= KMHtoPixel(1.0f);
                }
                if (deltaRotation <= MathHelper.Pi)
                {
                    if (Vitesse >= 0)
                    {
                        voiture.Position = new Vector3(voiture.Position.X + (1.01f * Vitesse * (float)Math.Sin(deltaRotation)), voiture.Position.Y, voiture.Position.Z - (1.01f * Vitesse * (float)Math.Cos(deltaRotation)));
                    }
                }
                else
                {
                    if (Vitesse >= 0)
                    {
                        voiture.Position = new Vector3(voiture.Position.X - (1.01f * Vitesse * (float)Math.Sin(deltaRotation)), voiture.Position.Y, voiture.Position.Z + (1.01f * Vitesse * (float)Math.Cos(deltaRotation)));
                    }
                }
            }
            else if (BoxVoitureAvant.Intersects(voiture.BoxVoitureAvant))
            {
                AvanceCollision = false;
                Translation = false;
                DernièreCollisionEstAvant = true;
                RotationEnCollision = (float)Math.Sin(deltaRotation - MathHelper.Pi) * ((float)(Math.PI / -8f) * (PixelToKMH(Vitesse) / VITESSE_MAX));
                RotationCollision(voiture, true, RotationEnCollision, DernièreCollisionEstAvant);
                voiture.Vitesse += Vitesse * -0.5f;
                Vitesse -= Vitesse * 0.5f;
                voiture.Avance();
            }
            else
            {
                AvancePossible = true;
                if (!Translation)
                {
                    voiture.Décélération(AvanceCollision);
                    voiture.Avance();
                }
            }
            RotationCollision(voiture, false, RotationEnCollision, DernièreCollisionEstAvant);
        }

        public bool GestionCollisionPiste(Sol sol)
        {
            bool enCollision = BoxVoiture.Intersects(sol.Box);//Fonctionne PAS!
            if (enCollision)
            {
                //sol.GetHauteur(Position);
                Position = new Vector3(Position.X, sol.GetHauteur(Position), Position.Z);
                Rotation = new Vector3(-sol.RotationInitiale.X, Rotation.Y, Rotation.Z);

                //ChuteLibrePossible = false;
            }

            //if (BoxVoiture.Intersects(sol.BoxDroite) || BoxVoiture.Intersects(sol.BoxGauche))
            //{
            //    Vitesse = 0;//Pour l'instant
            //    AvancePossible = false;
            //}
            if (GestionInput.EstEnfoncée(Keys.S))
            {
                AvancePossible = true;//?
            }

            return enCollision;
        }

        void CreateBoundingBox()
        {
            SphereVoitureAvant = new BoundingSphere(new Vector3(Position.X, Position.Y + 10, Position.Z), 7f);
            SphereVoitureMillieu = new BoundingSphere(new Vector3(Position.X + (15 * (float)Math.Sin(-Rotation.Y)), Position.Y + 12, Position.Z - (15 * (float)Math.Cos(-Rotation.Y))), 7f);
            SphereVoitureArrière = new BoundingSphere(new Vector3(Position.X + (30 * (float)Math.Sin(-Rotation.Y)), Position.Y + 10, Position.Z - (30 * (float)Math.Cos(-Rotation.Y))), 7f);
            SphereRouteAvant = new BoundingSphere(Position, 1);

            BoxVoitureAvant = BoundingBox.CreateFromSphere(SphereVoitureAvant);
            BoxVoitureMillieu = BoundingBox.CreateFromSphere(SphereVoitureMillieu);
            BoxVoitureArrière = BoundingBox.CreateFromSphere(SphereVoitureArrière);

            BoxVoiture = BoundingBox.CreateMerged(BoundingBox.CreateFromSphere(SphereVoitureAvant), BoundingBox.CreateFromSphere(SphereVoitureMillieu));
            BoxVoiture = BoundingBox.CreateMerged(BoxVoiture, BoundingBox.CreateFromSphere(SphereVoitureArrière));

        }

        public float NormalizeRotation(float rotation)
        {
            if (rotation > 0)
            {
                rotation = rotation % (float)(Math.PI * 2);
            }
            else if (rotation < 0)
            {
                rotation = rotation % (float)(Math.PI * 2);
            }
            return rotation;
        }

        void Info()//ToDelete
        {
            if (User)
            {
                string info = "Vitesse: " + ((int)PixelToKMH(Vitesse)) + "  Position: " + Position + "  Rotation:" + Rotation.Y;

                GestionSprites.DrawString(ArialFont, info, new Vector2(0, 0), Color.White, 0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            }
        }

        void RotationCollision(Voiture voiture, bool enContact, float angleMax, bool avant)
        {
            if (enContact)
            {
                VitesseRotation = (angleMax) / 4f;
            }
            VitesseRotation -= VitesseRotation * 0.1f;
            if (!avant)
            {
                voiture.Rotation = new Vector3(voiture.Rotation.X, voiture.Rotation.Y - (VitesseRotation), voiture.Rotation.Z);
            }
            else
            {
                voiture.Rotation = new Vector3(voiture.Rotation.X, voiture.Rotation.Y + (VitesseRotation * 2), voiture.Rotation.Z);
            }
        }

        public void VariationInclinaison()
        {
            Rotation = new Vector3(Rotation.X + 0.005f, Rotation.Y, Rotation.Z);
        }

        void Radio()
        {
            if (GestionInput.EstNouvelleTouche(Keys.D1))
            {
                foreach (SoundEffectInstance x in ListeChanson)
                {
                    x.Stop();
                }
                ListeChanson[0].Play();
                ListeChanson[0].Volume = 1.0f;
            }
            else if (GestionInput.EstNouvelleTouche(Keys.D2))
            {
                foreach (SoundEffectInstance x in ListeChanson)
                {
                    x.Stop();
                }
                ListeChanson[1].Play();
                ListeChanson[1].Volume = 1.0f;
            }
        }

        public void AI()
        {
            if (Vitesse <= KMHtoPixel(50))
            {
                Accélération(EnAvant);
            }
            Avance();
            //Vitesse = 1.5f;
        }
    }
}