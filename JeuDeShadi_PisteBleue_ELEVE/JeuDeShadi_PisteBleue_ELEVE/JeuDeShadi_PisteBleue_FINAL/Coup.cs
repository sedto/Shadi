namespace JeuDeShadi
{
    // CODE FOURNI — PISTE BLEUE
    // Vous n'avez pas besoin de comprendre ou de modifier ce fichier.
    // Il sert au code interne pour mémoriser un déplacement possible.
    public readonly struct Coup
    {
        public readonly int XDepart;
        public readonly int YDepart;
        public readonly int XArrivee;
        public readonly int YArrivee;
        public readonly bool EstUnePrise;

        public Coup(int xDepart, int yDepart, int xArrivee, int yArrivee, bool estUnePrise)
        {
            XDepart = xDepart;
            YDepart = yDepart;
            XArrivee = xArrivee;
            YArrivee = yArrivee;
            EstUnePrise = estUnePrise;
        }
    }
}
