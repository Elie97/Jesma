using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SimulationVéhicule
{
    public class RessourcesManager<T>
    {
        Game Jeu { get; set; }
        string RépertoireDesRessources { get; set; }
        List<RessourceDeBase<T>> ListeRessources { get; set; }

        public RessourcesManager(Game jeu, string répertoireDesTextures)
        {
            Jeu = jeu;
            RépertoireDesRessources = répertoireDesTextures;
            ListeRessources = new List<RessourceDeBase<T>>();
        }

        public void Add(string nom, T textureAjouter)
        {
            RessourceDeBase<T> textureÀAjouter = new RessourceDeBase<T>(nom, textureAjouter);
            if (!ListeRessources.Contains(textureÀAjouter))
            {
                ListeRessources.Add(textureÀAjouter);
            }
            //else 
            //{
            //   // lever une exception...
            //}
        }

        void Add(RessourceDeBase<T> textureÀAjouter)
        {
            textureÀAjouter.Load();
            ListeRessources.Add(textureÀAjouter);
        }

        public T Find(string nomTexture)
        {
            const int TEXTURE_PAS_TROUVÉE = -1;
            RessourceDeBase<T> textureÀRechercher = new RessourceDeBase<T>(Jeu.Content, RépertoireDesRessources, nomTexture);
            int indexTexture = ListeRessources.IndexOf(textureÀRechercher);
            if (indexTexture == TEXTURE_PAS_TROUVÉE)
            {
                Add(textureÀRechercher);
                indexTexture = ListeRessources.Count - 1;
            }
            return ListeRessources[indexTexture].Ressource;
        }
    }
}
