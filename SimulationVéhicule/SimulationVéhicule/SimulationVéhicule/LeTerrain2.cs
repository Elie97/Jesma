using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SimulationVéhicule
{
    public class LeTerrain2 : PrimitiveDeBaseAnimée
    {
        const int NB_TRIANGLES_PAR_TUILE = 2;
        const int NB_SOMMETS_PAR_TRIANGLE = 3;
        const int NB_SOMMETS_PAR_TUILE = 4;
        const float MAX_COULEUR = 255f;

        Vector3 Étendue { get; set; }
        string NomCarteTerrain { get; set; }
        string NomTextureTerrain { get; set; }
        int NbNiveauTexture { get; set; }

        BasicEffect EffetDeBase { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        Texture2D CarteTerrain { get; set; }
        Texture2D TextureTerrain { get; set; }
        Vector3 Origine { get; set; }

        Vector3 Delta { get; set; }
        int NbColonnes { get; set; }
        int NbRangées { get; set; }
        Color[] DataTexture { get; set; }
        float DeltaTexture { get; set; }
        Vector3[] PtsSommets { get; set; }
        Vector2[,] PtsTexture { get; set; }
        VertexPositionNormalTexture[] Sommets { get; set; }
        float[,] Hauteur { get; set; }
        Vector3[] Normale { get; set; }
        int[] indices { get; set; }

        Model Ciel { get; set; }
        Matrix[] TransformationsModèle { get; set; }
        Texture2D TextureCiel { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        Effect effect;

        public LeTerrain2(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                       Vector3 étendue, string nomCarteTerrain, string nomTextureTerrain, int nbNiveauxTexture, float intervalleMAJ)
            : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Étendue = étendue;
            NomCarteTerrain = nomCarteTerrain;
            NomTextureTerrain = nomTextureTerrain;
            NbNiveauTexture = nbNiveauxTexture;
        }

        public override void Initialize()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            CarteTerrain = GestionnaireDeTextures.Find(NomCarteTerrain);
            TextureTerrain = GestionnaireDeTextures.Find(NomTextureTerrain);
            SetUpIndices();
            InitialiserDonnéesCarte();
            InitialiserDonnéesTexture();
            Origine = new Vector3(-Étendue.X / 2 + 1, 0, Étendue.Z / 2);
            AllouerTableaux();
            base.Initialize();
        }

        void InitialiserDonnéesCarte()
        {
            NbColonnes = CarteTerrain.Width;
            NbRangées = CarteTerrain.Height;
            Delta = new Vector3(Étendue.X / NbColonnes, Étendue.Y / MAX_COULEUR, Étendue.Z / NbRangées);

            DataTexture = new Color[NbColonnes * NbRangées];
            CarteTerrain.GetData<Color>(DataTexture);

            Hauteur = new float[NbColonnes, NbRangées];
            for (int i = 0; i < NbColonnes; i++)
            {
                for (int j = 0; j < NbRangées; j++)
                {
                    Hauteur[j, i] = DataTexture[i + j * (NbColonnes)].R * 5;
                }
            }

            NbTriangles = NbColonnes * NbRangées * 2;
            NbSommets = NbTriangles * 2;
        }

        void InitialiserDonnéesTexture()
        {
            PtsTexture = new Vector2[2, NbNiveauTexture + 1];//Niveaux? pas niveau
            DeltaTexture = 1f / NbNiveauTexture;

            for (int j = 0; j < PtsTexture.GetLength(1); j++)
            {
                for (int i = 0; i < PtsTexture.GetLength(0); i++)
                {
                    PtsTexture[i, j] = new Vector2(i, DeltaTexture * j);
                }
            }
        }

        void AllouerTableaux()
        {
            PtsSommets = new Vector3[NbColonnes * NbRangées];
            Normale = new Vector3[NbColonnes * NbRangées];
            Sommets = new VertexPositionNormalTexture[(NbColonnes) * (NbRangées) * 6];
            CréerTableauPoints();
            //CalculateNormals();

        }

        protected override void LoadContent()
        {
            base.LoadContent();
            EffetDeBase = new BasicEffect(GraphicsDevice);
            effect = Game.Content.Load<Effect>("effects");

            GestionnaireDeModèles = Game.Services.GetService(typeof(RessourcesManager<Model>)) as RessourcesManager<Model>;
            Ciel = GestionnaireDeModèles.Find("Ciel2");
            TransformationsModèle = new Matrix[Ciel.Bones.Count];
            Ciel.CopyAbsoluteBoneTransformsTo(TransformationsModèle);

            InitialiserParamètresEffetDeBase();
        }

        void InitialiserParamètresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TextureTerrain;
        }

        private void CréerTableauPoints()
        {
            for (int j = 0; j < NbRangées; j++)
            {
                for (int i = 0; i < NbColonnes; i++)
                {
                    PtsSommets[i + j * NbColonnes] = new Vector3(Origine.X + Delta.X * i, Origine.Y + Hauteur[i, j] * Delta.Y, (Origine.Z - Delta.Z * j));
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
                    Sommets[++NoSommet] = new VertexPositionNormalTexture(PtsSommets[(i) + j * NbColonnes], new Vector3(0,0,0), PtsTexture[0, ObtenirPtTexture(i, j)]);
                    Sommets[++NoSommet] = new VertexPositionNormalTexture(PtsSommets[(i) + (j + 1) * NbColonnes], new Vector3(0, 0, 0), PtsTexture[0, ObtenirPtTexture(i, j) + 1]);
                    Sommets[++NoSommet] = new VertexPositionNormalTexture(PtsSommets[(i + 1) + j * NbColonnes], new Vector3(0, 0, 0), PtsTexture[1, ObtenirPtTexture(i, j)]);

                    Sommets[++NoSommet] = new VertexPositionNormalTexture(PtsSommets[(i + 1) + j * NbColonnes], new Vector3(0, 0, 0), PtsTexture[1, ObtenirPtTexture(i, j)]);
                    Sommets[++NoSommet] = new VertexPositionNormalTexture(PtsSommets[(i) + (j + 1) * NbColonnes], new Vector3(0, 0, 0), PtsTexture[0, ObtenirPtTexture(i, j) + 1]);
                    Sommets[++NoSommet] = new VertexPositionNormalTexture(PtsSommets[(i + 1) + (j + 1) * NbColonnes], new Vector3(0, 0, 0), PtsTexture[1, ObtenirPtTexture(i, j) + 1]);
                }
            }

            CalculateNormals();
        }

        public int ObtenirPtTexture(int i, int j)
        {
            int ptTexture = 6;
            if (PtsSommets[i + j * NbColonnes].Y < 850)
            {
                ptTexture = 0;
            }
            else if (PtsSommets[i + j * NbColonnes].Y >= 850 && PtsSommets[i + j * NbColonnes].Y < 950)
            {
                ptTexture = 1;
            }
            else if (PtsSommets[i + j * NbColonnes].Y >= 950 && PtsSommets[i + j * NbColonnes].Y < 1050)
            {
                ptTexture = 2;
            }
            else if (PtsSommets[i + j * NbColonnes].Y >= 1050 && PtsSommets[i + j * NbColonnes].Y < 1150)
            {
                ptTexture = 3;
            }
            else if (PtsSommets[i + j * NbColonnes].Y >= 1150 && PtsSommets[i + j * NbColonnes].Y < 2000)
            {
                ptTexture = 4;
            }
            else if (PtsSommets[i + j * NbColonnes].Y >= 2000 && PtsSommets[i + j * NbColonnes].Y < 2100)
            {
                ptTexture = 5;
            }

            //if (PtsSommets[i + j * NbColonnes].Y - PositionInitiale.Y >= 2000)
            //{
            //    ptTexture = 2;
            //}
            //else if (PtsSommets[i + j * NbColonnes].Y - PositionInitiale.Y > 0 && PtsSommets[i + j * NbColonnes].Y - PositionInitiale.Y <= 12)
            //{
            //    ptTexture = 1;
            //}
            //else if (PtsSommets[i + j * NbColonnes].Y > 12 / 5f && PtsSommets[i + j * NbColonnes].Y <= 18 / 5f)
            //{
            //    ptTexture = 2;
            //}
            //else if (PtsSommets[i + j * NbColonnes].Y > 18 / 5f && PtsSommets[i + j * NbColonnes].Y <= 24 / 5f)
            //{
            //    ptTexture = 3;
            //}
            //else if (PtsSommets[i + j * NbColonnes].Y > 24 / 5f)
            //{
            //    ptTexture = 4;
            //}
            return ptTexture;
        }

        public override void Draw(GameTime gameTime)
        {
            GestionMatriceMonde();
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;
            EffetDeBase.LightingEnabled = true;
            EffetDeBase.AmbientLightColor = new Vector3(0.25f, 0.25f, 0.25f);

            foreach (ModelMesh Mesh in Ciel.Meshes)
            {
                foreach (BasicEffect Effect in Mesh.Effects)
                {
                    Effect.Projection = CaméraJeu.Projection;
                    Effect.View = CaméraJeu.Vue;
                    Effect.World = TransformationsModèle[Mesh.ParentBone.Index] * Matrix.CreateScale(20000) * Matrix.CreateTranslation(new Vector3(CaméraJeu.Position.X, -9500, CaméraJeu.Position.Z));
                }
                Mesh.Draw();
            }


            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, Sommets, 0, NbTriangles);
            }
            base.Draw(gameTime);
        }

        public void GestionMatriceMonde()
        {
            Monde = Matrix.Identity *
                        Matrix.CreateScale(HomothétieInitiale) *
                        Matrix.CreateFromYawPitchRoll(RotationInitiale.Y - (float)Math.PI / 2, RotationInitiale.X, RotationInitiale.Z) *
                        Matrix.CreateTranslation(PositionInitiale);
        }

        private void CalculateNormals()
        {
            for (int j = 0; j < NbRangées; j++)
            {
                for (int i = 0; i < NbColonnes; i++)
                {
                    Normale[i + j * NbColonnes] = new Vector3(0, 0, 0);
                }
            }

            for (int i = 0; i < NbTriangles; i++)
            {
                int index1 = i * 3;
                int index2 = i * 3 + 1;
                int index3 = i * 3 + 2;

                Vector3 side1 = Sommets[index1].Position - Sommets[index3].Position;
                Vector3 side2 = Sommets[index1].Position - Sommets[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                Sommets[index1].Normal += normal;
                Sommets[index2].Normal += normal;
                Sommets[index3].Normal += normal;

            }

            for (int i = 0; i < Normale.Length; i++)
            {
                Normale[i].Normalize();
            }

        }

        private void SetUpIndices()
        {
            indices = new int[(NbColonnes) * (NbRangées) * 6];
            int counter = 0;
            for (int y = 0; y < NbRangées; y++)
            {
                for (int x = 0; x < NbColonnes; x++)
                {
                    int lowerLeft = x + y * NbColonnes;
                    int lowerRight = (x + 1) + y * NbColonnes;
                    int topLeft = x + (y + 1) * NbColonnes;
                    int topRight = (x + 1) + (y + 1) * NbColonnes;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }
        }
    }
}
