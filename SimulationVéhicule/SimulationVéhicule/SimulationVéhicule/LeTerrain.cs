using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SimulationVéhicule
{
    public class LeTerrain : PrimitiveDeBaseAnimée
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

        int[] indices;
        VertexPositionNormalTexture[] terrainVertices;
        VertexPositionNormalTexture[] VerticesRoche;
        Texture2D sandTexture;
        Texture2D rockTexture;
        Texture2D snowTexture;

        Model Ciel { get; set; }
        Matrix[] TransformationsModèle { get; set; }
        Texture2D TextureCiel { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }

        int Index { get; set; }


        public LeTerrain(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
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
            sandTexture = GestionnaireDeTextures.Find("sand");
            rockTexture = GestionnaireDeTextures.Find("rock");
            snowTexture = GestionnaireDeTextures.Find("snow");
            InitialiserDonnéesCarte();
            InitialiserDonnéesTexture();
            Origine = new Vector3(-Étendue.X / 2 + 1, 0, Étendue.Z / 2);
            AllouerTableaux();
            CréerTableauPoints();
            Index = 0;
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
            Sommets = new VertexPositionNormalTexture[(NbColonnes) * (NbRangées) * 6];
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            EffetDeBase = new BasicEffect(GraphicsDevice);
            InitialiserParamètresEffetDeBase();

            GestionnaireDeModèles = Game.Services.GetService(typeof(RessourcesManager<Model>)) as RessourcesManager<Model>;
            Ciel = GestionnaireDeModèles.Find("Ciel2");
            TransformationsModèle = new Matrix[Ciel.Bones.Count];
            Ciel.CopyAbsoluteBoneTransformsTo(TransformationsModèle);


            terrainVertices = SetUpTerrainVertices();
            SetUpIndices();
            CalculateNormals();
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
                    //Sommets[++NoSommet] = new VertexPositionNormalTexture(PtsSommets[(i) + j * NbColonnes], PtsTexture[0, ObtenirPtTexture(i, j)]);
                    //Sommets[++NoSommet] = new VertexPositionNormalTexture(PtsSommets[(i) + (j + 1) * NbColonnes], PtsTexture[0, ObtenirPtTexture(i, j) + 1]);
                    //Sommets[++NoSommet] = new VertexPositionNormalTexture(PtsSommets[(i + 1) + j * NbColonnes], PtsTexture[1, ObtenirPtTexture(i, j)]);
                    //Sommets[++NoSommet] = new VertexPositionNormalTexture(PtsSommets[(i + 1) + j * NbColonnes], PtsTexture[1, ObtenirPtTexture(i, j)]);
                    //Sommets[++NoSommet] = new VertexPositionNormalTexture(PtsSommets[(i) + (j + 1) * NbColonnes], PtsTexture[0, ObtenirPtTexture(i, j) + 1]);
                    //Sommets[++NoSommet] = new VertexPositionNormalTexture(PtsSommets[(i + 1) + (j + 1) * NbColonnes], PtsTexture[1, ObtenirPtTexture(i, j) + 1]);

                    Sommets[++NoSommet].Position = PtsSommets[(i) + j * NbColonnes];
                    Sommets[NoSommet].TextureCoordinate.X = PtsTexture[0, ObtenirPtTexture(i, j)].X;
                    Sommets[NoSommet].TextureCoordinate.Y = PtsTexture[0, ObtenirPtTexture(i, j)].Y;

                    Sommets[++NoSommet].Position = PtsSommets[(i) + (j + 1) * NbColonnes];
                    Sommets[NoSommet].TextureCoordinate.X = PtsTexture[0, ObtenirPtTexture(i, j) + 1].X;
                    Sommets[NoSommet].TextureCoordinate.Y = PtsTexture[0, ObtenirPtTexture(i, j) + 1].Y;

                    Sommets[++NoSommet].Position = PtsSommets[(i + 1) + j * NbColonnes];
                    Sommets[NoSommet].TextureCoordinate.X = PtsTexture[1, ObtenirPtTexture(i, j)].X;
                    Sommets[NoSommet].TextureCoordinate.Y = PtsTexture[1, ObtenirPtTexture(i, j)].Y;

                    Sommets[++NoSommet].Position = PtsSommets[(i + 1) + j * NbColonnes];
                    Sommets[NoSommet].TextureCoordinate.X = PtsTexture[1, ObtenirPtTexture(i, j)].X;
                    Sommets[NoSommet].TextureCoordinate.Y = PtsTexture[1, ObtenirPtTexture(i, j)].Y;

                    Sommets[++NoSommet].Position = (PtsSommets[(i) + (j + 1) * NbColonnes]);
                    Sommets[NoSommet].TextureCoordinate.X = PtsTexture[0, ObtenirPtTexture(i, j) + 1].X;
                    Sommets[NoSommet].TextureCoordinate.Y = PtsTexture[0, ObtenirPtTexture(i, j) + 1].Y;

                    Sommets[++NoSommet].Position = PtsSommets[(i + 1) + (j + 1) * NbColonnes];
                    Sommets[NoSommet].TextureCoordinate.X = PtsTexture[1, ObtenirPtTexture(i, j) + 1].X;
                    Sommets[NoSommet].TextureCoordinate.Y = PtsTexture[1, ObtenirPtTexture(i, j) + 1].Y;
                }
            }
        }

        public int ObtenirPtTexture(int i, int j)
        {
            int ptTexture = 0;
            if (PtsSommets[i + j * NbColonnes].Y == 0)
            {
                ptTexture = 0;
            }
            else if (PtsSommets[i + j * NbColonnes].Y > 0 && PtsSommets[i + j * NbColonnes].Y <= 12)
            {
                ptTexture = 1;
            }
            else if (PtsSommets[i + j * NbColonnes].Y > 12 && PtsSommets[i + j * NbColonnes].Y <= 18)
            {
                ptTexture = 2;
            }
            else if (PtsSommets[i + j * NbColonnes].Y > 18 && PtsSommets[i + j * NbColonnes].Y <= 24)
            {
                ptTexture = 3;
            }
            else if (PtsSommets[i + j * NbColonnes].Y > 24)
            {
                ptTexture = 4;
            }
            return ptTexture;
        }



        public override void Draw(GameTime gameTime)
        {
            GestionMatriceMonde();
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;
            EffetDeBase.LightingEnabled = true;
            EffetDeBase.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
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
                if (Index >= 100 && Index <= 900)
                {
                    //EffetDeBase.Texture = ObtenirTexture();
                }
                else
                {
                    //EffetDeBase.Texture = snowTexture;
                }
                //GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, Sommets, 0, NbTriangles);
                //GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, Sommets, 0, Sommets.Length, PtsTexture, 0, PtsTexture.Length / 3, VertexPositionNormalTexture.VertexDeclaration);
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, terrainVertices, 0, terrainVertices.Length, indices, 0, indices.Length / 3);
                //GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, Sommets, 0, NbTriangles,);
                    //GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, );
                //GraphicsDevice.Draw
                Index++;
            }
            //Game.Window.Title = Index.ToString()
            base.Draw(gameTime);
        }

        public void GestionMatriceMonde()
        {
            Monde = Matrix.Identity *
                        Matrix.CreateScale(HomothétieInitiale) *
                        Matrix.CreateFromYawPitchRoll(RotationInitiale.Y - (float)Math.PI / 2, RotationInitiale.X, RotationInitiale.Z) *
                        Matrix.CreateTranslation(PositionInitiale);
        }

        private void SetUpIndices()
        {
            indices = new int[(NbColonnes) * (NbRangées) * 6];
            int counter = 0;
            for (int y = 0; y < NbRangées - 1; y++)
            {
                for (int x = 0; x < NbColonnes - 1; x++)
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

        private VertexPositionNormalTexture[] SetUpTerrainVertices()
        {
            VertexPositionNormalTexture[] terrainVertices = new VertexPositionNormalTexture[NbColonnes * NbRangées];
            //VertexPositionNormalTexture a = new VertexPositionNormalTexture()
            for (int x = 0; x < NbColonnes; x++)
            {
                for (int y = 0; y < NbRangées; y++)
                {
                    terrainVertices[x + y * NbColonnes].Position = new Vector3(PtsSommets[x + y * NbColonnes].X, Hauteur[x, y] * 2f, PtsSommets[x + y * NbColonnes].Z);
                    terrainVertices[x + y * NbColonnes].TextureCoordinate.X = x;
                    terrainVertices[x + y * NbColonnes].TextureCoordinate.Y = y;
                }
            }

            return terrainVertices;
        }

        private void CalculateNormals()
        {
            for (int i = 0; i < terrainVertices.Length; i++)
                terrainVertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = terrainVertices[index1].Position - terrainVertices[index3].Position;
                Vector3 side2 = terrainVertices[index1].Position - terrainVertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                terrainVertices[index1].Normal += normal;
                terrainVertices[index2].Normal += normal;
                terrainVertices[index3].Normal += normal;
            }

            for (int i = 0; i < terrainVertices.Length; i++)
                terrainVertices[i].Normal.Normalize();
        }

        public int ObtenirPtTexture2(int i, int j)
        {
            int ptTexture = 0;
            if (terrainVertices[i + j * NbColonnes].Position.Y == 0)
            {
                ptTexture = 0;
            }
            return ptTexture;
        }

        Texture2D ObtenirTexture()
        {
            Texture2D texture;
            if (true)
            {
                texture = rockTexture;
            }
            return texture;
        }
    }
}
