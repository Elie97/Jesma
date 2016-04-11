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

//Enlever BoundingBox pour les courbe!

namespace SimulationVéhicule
{
    public class Sol : PrimitiveDeBase
    {
        public const int NB_CHECK_POINT = 2;

        Vector2 Étendue { get; set; }
        Vector2 Charpente { get; set; }
        int NbRangées { get; set; }
        int NbColonnes { get; set; }
        String NomTexture { get; set; }
        float Largeur { get; set; }
        float Hauteur { get; set; }
        Vector2 Delta { get; set; }
        Vector2[,] PtsTexture { get; set; }
        Vector3[,] PtsSommets { get; set; }
        VertexPositionTexture[] Sommets { get; set; }
        Texture2D LaTexture { get; set; }
        BasicEffect EffetDeBase { get; set; }
        Vector3 Origine { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        Vector3[] PointsBox { get; set; }
        Vector3[] PointsÉtape { get; set; }
        Vector3[] PointBoxLatéralGauche { get; set; }
        Vector3[] PointBoxLatéralDroit { get; set; }
        Vector3[] PointsBoxComplet { get; set; }

        public Vector3 Position { get; set; }

        public Color CouleurCheckPoint { get; set; }

        float Rayon { get; set; }
        float Angle { get; set; }
        public bool Courbe { get; set; }
        float RotationCourbe { get; set; }

        int NbVoiture { get; set; }

        public bool[,] Franchi { get; set; }
        public List<bool> ListeFranchi { get; set; }
        public List<List<bool>> ListeFranchiParVoiture { get; set; }

        List<Vector3[]> ListePointÉtape { get; set; }

        public BoundingBox Box
        {
            get
            {
                return BoundingBox.CreateFromPoints(PointsBox);
            }
        }

        public BoundingBox BoxComplet
        {
            get
            {
                return BoundingBox.CreateFromPoints(PointsBoxComplet);
            }
        }

        public List<BoundingBox> BoxÉtape
        {
            get
            {
                List<BoundingBox> boxÉtape = new List<BoundingBox>();
                for (int i = 0; i < ListePointÉtape.Count(); i++)
                {
                    boxÉtape.Add(BoundingBox.CreateFromPoints(ListePointÉtape[ListePointÉtape.Count() - i - 1]));
                }
                return boxÉtape;
            }
        }

        //public BoundingBox BoxDroite
        //{
        //    get
        //    {
        //        return BoundingBox.CreateFromPoints(PointBoxLatéralDroit);
        //    }
        //}

        //public BoundingBox BoxGauche
        //{
        //    get
        //    {
        //        return BoundingBox.CreateFromPoints(PointBoxLatéralGauche);
        //    }
        //}

        float PositionRelativeX { get; set; }
        float PositionRelativeZ { get; set; }
        float PositionRelativeY { get; set; }

        public Sol(Game jeu, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue,
            Vector2 charpente, string nomTexture, float rayon, float angle, bool courbe, float rotationCourbe, int nbVoiture)
            : base(jeu, échelleInitiale, rotationInitiale, positionInitiale)
        {
            Étendue = étendue;
            Charpente = charpente;
            NomTexture = nomTexture;
            Rayon = rayon;
            Angle = angle;
            Courbe = courbe;
            RotationCourbe = rotationCourbe;
            NbVoiture = nbVoiture;
            Position = positionInitiale;
        }


        public override void Initialize()
        {
            CouleurCheckPoint = Color.Red;
            ListeFranchi = new List<bool>();//pcq il y a + de 1 check point par piste!
            ListeFranchiParVoiture = new List<List<bool>>();
            Franchi = new bool[NB_CHECK_POINT, NbVoiture];

            for (int j = 0; j < NbVoiture; j++)
            {
                ListeFranchi = new List<bool>();
                for (int i = 0; i < NB_CHECK_POINT; i++)
                {
                    Franchi[i, j] = false;
                    ListeFranchi.Add(false);
                }
                ListeFranchiParVoiture.Add(ListeFranchi);
            }

            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            LaTexture = GestionnaireDeTextures.Find(NomTexture);
            Largeur = Étendue.X;
            Hauteur = Étendue.Y;
            NbColonnes = (int)Charpente.X;
            NbRangées = (int)Charpente.Y;
            PointsBox = new Vector3[NbColonnes * NbRangées];
            PointsBoxComplet = new Vector3[NbColonnes * NbRangées];
            PointsÉtape = new Vector3[2];
            ListePointÉtape = new List<Vector3[]>();
            Delta = new Vector2(Largeur / ((float)NbColonnes - 1), Hauteur / ((float)NbRangées - 1));
            //Origine = new Vector3(Position.X - (Étendue.X / 2) + ((Étendue.X / Charpente.X) / 2), Position.Y, Position.Z);
            Origine = new Vector3(0, 0, 0);
            PtsSommets = new Vector3[NbColonnes, NbRangées];
            PtsTexture = new Vector2[NbColonnes, NbRangées];
            Sommets = new VertexPositionTexture[(NbColonnes) * (NbRangées) * 6];
            NbTriangles = NbColonnes * NbRangées * 2;
            CréerTableauPoints();
            InitialiserDonnéesTexture();
            SetPointBox();
            base.Initialize();
        }

        private void CréerTableauPoints()
        {
            for (int j = 0; j < NbRangées; j++)
            {
                for (int i = 0; i < NbColonnes; i++)
                {
                    if (Courbe)
                    {
                        PtsSommets[i, j] = new Vector3(Origine.X + Delta.X * i, Origine.Y, Origine.Z - 0.5f * j);
                    }
                    else
                    {
                        PtsSommets[i, j] = new Vector3(Origine.X + Delta.X * i, Origine.Y, Origine.Z - Delta.Y * j);
                    }

                }
            }

            if (Courbe)
            {

                for (int j = 0; j < NbRangées; j++)
                {
                    for (int i = 0; i < NbColonnes; i++)
                    {
                        PtsSommets[i, j] = Vector3.Transform(new Vector3(PtsSommets[i, j].X, PtsSommets[i, j].Y, PtsSommets[i, j].Z),
                             Matrix.CreateRotationY((j) / (float)(NbRangées) * MathHelper.PiOver2));
                    }
                }

                for (int j = 0; j < NbRangées; j++)
                {
                    for (int i = 0; i < NbColonnes; i++)
                    {
                        PtsSommets[i, j] = Vector3.Transform(PtsSommets[i, j],
                             Matrix.CreateRotationY(RotationCourbe));
                    }
                }

            }

            for (int j = 0; j < NbRangées; j++)
            {
                for (int i = 0; i < NbColonnes; i++)
                {
                    PtsSommets[i, j] = new Vector3(PtsSommets[i, j].X - (Rayon * (float)Math.Sin(Angle * (j / (float)NbRangées)))
                    , PtsSommets[i, j].Y, PtsSommets[i, j].Z);
                }
            }
        }

        protected override void InitialiserSommets()
        {
            int NoSommet = -1;
            for (int j = 0; j < NbRangées - 1; j++)
            {
                for (int i = 0; i < NbColonnes - 1; i++)
                {
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j], PtsTexture[i, j]);
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i + 1, j], PtsTexture[i + 1, j]);
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i + 1, j], PtsTexture[i + 1, j]);
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i + 1, j + 1], PtsTexture[i + 1, j + 1]);
                    //Sommets[++NoSommet] = new VertexPositionColor(PtsSommets[i, j], Color.Red);
                    //Sommets[++NoSommet] = new VertexPositionColor(PtsSommets[i, j + 1], Color.Red);
                    //Sommets[++NoSommet] = new VertexPositionColor(PtsSommets[i + 1, j], Color.Red);
                    //Sommets[++NoSommet] = new VertexPositionColor(PtsSommets[i + 1, j], Color.Red);
                    //Sommets[++NoSommet] = new VertexPositionColor(PtsSommets[i, j + 1], Color.Red);
                    //Sommets[++NoSommet] = new VertexPositionColor(PtsSommets[i + 1, j + 1], Color.Red);
                }
            }
        }

        void InitialiserDonnéesTexture()
        {
            Vector2 ptsTexture;
            ptsTexture = new Vector2(1f / (NbColonnes - 1), 1f / (NbRangées - 1));
            for (int j = 0; j < NbRangées; j++)
            {
                if (Hauteur / (float)NbRangées == 100)
                {
                    if (j % 2 == 0)
                    {
                        ptsTexture = new Vector2(1f / (NbColonnes - 1), 0);
                    }
                    else
                    {
                        ptsTexture = new Vector2(1f / (NbColonnes - 1), 1);
                    }
                }
                for (int i = 0; i < NbColonnes; i++)
                {
                    PtsTexture[i, j] = new Vector2(i * ptsTexture.X, ptsTexture.Y * j);
                }
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            EffetDeBase = new BasicEffect(GraphicsDevice);
            //EffetDeBase.VertexColorEnabled = true;
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = LaTexture;
        }

        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;
            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Sommets, 0, NbTriangles);

            }
            //DebugShapeRenderer.AddBoundingBox(Box, Color.Wheat);
            foreach (BoundingBox x in BoxÉtape)
            {
                DebugShapeRenderer.AddBoundingBox(x, CouleurCheckPoint);
            }
            //DebugShapeRenderer.AddBoundingBox(BoxComplet, Color.Black);
            //DebugShapeRenderer.AddBoundingBox(BoxDroite, Color.YellowGreen);
            //DebugShapeRenderer.AddBoundingBox(BoxGauche, Color.YellowGreen);
           //DebugShapeRenderer.AddBoundingBox(Box, Color.Wheat);
            //DebugShapeRenderer.AddBoundingBox(Box, Color.Blue);
            //DebugShapeRenderer.AddBoundingSphere(Sphere, Color.Yellow);
            //DebugShapeRenderer.AddBoundingSphere(new BoundingSphere(new Vector3(Position.X, Position.Y, Position.Z), (Étendue.X / 2)), Color.Green);
            base.Draw(gameTime);
        }

        public float GetHauteur(Vector3 positionVoiture)
        {
            float hauteur = PositionInitiale.Y;
            if (RotationInitiale.X != 0)
            {
                float hauteurInitiale = Vector3.Transform(PtsSommets[0, 0], Matrix.CreateFromYawPitchRoll(RotationInitiale.Y, RotationInitiale.X, RotationInitiale.Z)).Y;
                float hauteurFinale = Vector3.Transform(PtsSommets[0, NbRangées - 1], Matrix.CreateFromYawPitchRoll(RotationInitiale.Y, RotationInitiale.X, RotationInitiale.Z)).Y;
                float deltaHauteur = (hauteurFinale - hauteurInitiale) / Hauteur;
                float profondeur;

                if (RotationInitiale.Y == 0 || RotationInitiale.Y == MathHelper.Pi)
                {
                    profondeur = Math.Abs(positionVoiture.Z - PositionInitiale.Z);
                }
                else
                {
                    profondeur = Math.Abs(positionVoiture.X - PositionInitiale.X);
                }

                if (profondeur >= Hauteur)
                {
                    profondeur = Hauteur;
                }
                if (profondeur <= 0)
                {
                    profondeur = 0;
                }

                hauteur = (profondeur * deltaHauteur);
                if (RotationInitiale.X != 0)
                {
                    hauteur += ((profondeur / Hauteur) * (Hauteur * 0.03f));
                }
            }


            return hauteur;
        }

        void SetPointBox()
        {
            for (int i = 0; i < NbColonnes; i++)
            {
                for (int j = 0; j < NbRangées; j++)
                {
                    PointsBox[i + (i * j)] = PtsSommets[i, j];
                }
            }
            PointsBox[0] = new Vector3(PointsBox[0].X, PointsBox[0].Y + 3, PointsBox[0].Z);
            Matrix transformation = Matrix.CreateFromYawPitchRoll(RotationInitiale.Y, RotationInitiale.X, 0) * Matrix.CreateTranslation(PositionInitiale);
            for (int i = 0; i < PointsBox.Length; i++)
            {
                PointsBox[i] = Vector3.Transform(PointsBox[i], transformation);
            }

            for (int i = 0; i < NbColonnes; i++)
            {
                for (int j = 0; j < NbRangées; j++)
                {
                    PointsBoxComplet[i + (i * j)] = PtsSommets[i, j];
                }
            }
            PointsBoxComplet[0] = new Vector3(PointsBoxComplet[0].X- 50, PointsBoxComplet[0].Y, PointsBoxComplet[0].Z);
            PointsBoxComplet[1] = new Vector3(PointsBoxComplet[1].X + 50, PointsBoxComplet[1].Y + 500, PointsBoxComplet[1].Z);
            transformation = Matrix.CreateFromYawPitchRoll(RotationInitiale.Y, RotationInitiale.X, 0) * Matrix.CreateTranslation(PositionInitiale);
            for (int i = 0; i < PointsBoxComplet.Length; i++)
            {
                PointsBoxComplet[i] = Vector3.Transform(PointsBoxComplet[i], transformation);
            }



            //GÉNÉRIQUE?
            for (int k = 0; k < NB_CHECK_POINT; k++)
            {
                PointsÉtape = new Vector3[2];
                if (!Courbe)
                {
                    PointsÉtape[0] = new Vector3(PtsSommets[0, NbRangées - 1].X - 50, PtsSommets[0, NbRangées - 1].Y, PtsSommets[0, NbRangées - 1].Z / (k + 1));
                    PointsÉtape[1] = new Vector3(PtsSommets[NbColonnes - 1, NbRangées - 1].X + 50, PtsSommets[NbColonnes - 1, NbRangées - 1].Y + 100, PtsSommets[NbColonnes - 1, NbRangées - 1].Z / (k + 1));   
                }
                else
                {
                    PointsÉtape[0] = new Vector3(PtsSommets[0, NbRangées - 1].X, PtsSommets[0, NbRangées - 1].Y, PtsSommets[0, NbRangées - 1].Z / (k + 1));
                    PointsÉtape[1] = new Vector3(PtsSommets[NbColonnes - 1, NbRangées - 1].X, PtsSommets[NbColonnes - 1, NbRangées - 1].Y + 100, PtsSommets[NbColonnes - 1, NbRangées - 1].Z / (k + 1));
                }

                for (int i = 0; i < PointsÉtape.Length; i++)
                {
                    PointsÉtape[i] = Vector3.Transform(PointsÉtape[i], transformation);
                }
                ListePointÉtape.Add(PointsÉtape);
            }

            //

            ////PointBoxLatéralDroit = new Vector3[] { PtsSommets[NbColonnes - 1, 0], 
            //  //  new Vector3(PtsSommets[NbColonnes - 1, NbRangées - 1].X, PtsSommets[NbColonnes - 1, NbRangées - 1].Y + 50, PtsSommets[NbColonnes - 1, NbRangées - 1].Z) };
            //PointBoxLatéralDroit = new Vector3[NbRangées];
            //for (int i = 0; i < NbRangées - 1; i++)
            //{
            //    PointBoxLatéralDroit[i] = Vector3.Transform(PtsSommets[NbColonnes - 1, i], Matrix.CreateFromYawPitchRoll(RotationInitiale.Y, RotationInitiale.X, 0));
            //}
            //for (int i = 0; i < PointBoxLatéralDroit.Length; i++)
            //{
            //    //PointBoxLatéralDroit[i] = Vector3.Transform(PointBoxLatéralDroit[i], Matrix.CreateFromYawPitchRoll(RotationInitiale.Y, 0, 0));
            //    //PointBoxLatéralDroit[i] = Vector3.Transform(PointBoxLatéralDroit[i], Matrix.CreateFromYawPitchRoll(0, 0, RotationInitiale.Y));        
            //}

            //PointBoxLatéralGauche = new Vector3[] { PtsSommets[0, 0], 
            //    new Vector3(PtsSommets[0, NbRangées - 1].X, PtsSommets[0, NbRangées - 1].Y + 50, PtsSommets[0, NbRangées - 1].Z) };

            //for (int i = 0; i < PointBoxLatéralGauche.Length; i++)
            //{
            //   PointBoxLatéralGauche[i] = Vector3.Transform(PointBoxLatéralGauche[i], Matrix.CreateFromYawPitchRoll(RotationInitiale.Y, 0, 0));
            //}
        }

    }
}
