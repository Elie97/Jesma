using Microsoft.Xna.Framework;

namespace SimulationVéhicule
{
    public class CaméraFixe : Caméra
    {
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        float TempsÉcouléDepuisMAJ { get; set; }
        public CaméraFixe(Game jeu, Vector3 positionCaméra, Vector3 cible, Vector3 orientation)
            : base(jeu)
        {
            CréerPointDeVue(positionCaméra, cible, orientation); // Création de la matrice de vue
            CréerVolumeDeVisualisation(Caméra.OUVERTURE_OBJECTIF, Caméra.DISTANCE_PLAN_RAPPROCHÉ, Caméra.DISTANCE_PLAN_ÉLOIGNÉ); // Création de la matrice de projection (volume de visualisation)
        }

        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= INTERVALLE_MAJ_STANDARD)
            {
                //if (GestionInput.EstEnfoncée(Keys.O))
                //{
                //    GérerLacet();
                //}
                CréerPointDeVue();
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
    }
}
