using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JeuDeShadi
{
    public class Game1 : Game
    {
        // IMPORTANT POUR LA PISTE BLEUE :
        // Vous n'avez pas besoin de comprendre la classe Game1 ni la programmation orientée objet.
        // Le cadre MonoGame est fourni. Concentrez-vous uniquement sur la zone élève.

        // Valeurs utilisées dans le tableau plateauDeJeu.
        // Ces constantes sont fournies.
        const int VIDE = 0;
        const int BLANC = 1;
        const int NOIR = 2;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // --- Configuration visuelle ---
        private const int TailleGrille = 10;
        private const int TailleCase = 64;
        private const int MargeFenetre = 40;
        private const int LargeurPanneauAide = 340;

        private Texture2D _pixel;      // texture 1x1 réutilisée pour tout dessiner (rectangles, cercles)
        private Texture2D _texturePion; // cercle plein généré procéduralement
        private Texture2D _textureAnneau; // anneau pour les surlignages

        // --- Données principales du jeu ---
        // Ces variables sont fournies. Vous allez les UTILISER, pas les recréer.
        // plateauDeJeu[x, y] : x = colonne, y = ligne. (0,0) = en haut à gauche.
        int[,] plateauDeJeu = new int[TailleGrille, TailleGrille];

        int joueurCourant = BLANC;

        // Variable interne fournie : elle mémorise les coups possibles.
        // Vous n'avez pas à la manipuler directement dans la piste bleue.
        List<Coup> coupsLegaux = new List<Coup>();

        // Sélection en cours (pion cliqué en attente d'une destination)
        private bool _unPionEstSelectionne = false;
        private int _pionSelectionneX = -1;
        private int _pionSelectionneY = -1;
        private List<Coup> _destinationsPossiblesPourSelection = new List<Coup>();

        private int _finDePartie = 0; // 0 = en cours, 1 = blanc gagne, 2 = noir gagne, -1 = match nul / blocage

        private MouseState _etatSourisPrecedent;
        private KeyboardState _etatClavierPrecedent;

        // Outils de vérification fournis pour la piste bleue.
        // Ils permettent de tester visuellement les étapes sans connaître MonoGame.
        private bool _afficherTestEtape4 = false;
        private List<Point> _casesTrouveesPourTest = new List<Point>();
        private string[] _resultatTest = { "AUCUN TEST LANCE" };

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            int largeur = MargeFenetre * 2 + TailleGrille * TailleCase + LargeurPanneauAide;
            int hauteur = MargeFenetre * 2 + TailleGrille * TailleCase;
            _graphics.PreferredBackBufferWidth = largeur;
            _graphics.PreferredBackBufferHeight = hauteur;
            _graphics.ApplyChanges();

            InitialiserPlateau();
            RecalculerCoupsLegaux();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Texture 1x1 blanche, sert de base à tous les dessins (rectangles colorés via tint)
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _texturePion = CreerTextureCercle(TailleCase - 16, plein: true);
            _textureAnneau = CreerTextureCercle(TailleCase - 8, plein: false);

        }

        /// <summary>
        /// Génère procéduralement une texture circulaire (cercle plein ou anneau)
        /// pour ne dépendre d'aucune image externe.
        /// </summary>
        private Texture2D CreerTextureCercle(int diametre, bool plein)
        {
            var texture = new Texture2D(GraphicsDevice, diametre, diametre);
            var donnees = new Color[diametre * diametre];
            float rayon = diametre / 2f;
            Vector2 centre = new Vector2(rayon, rayon);
            float epaisseurAnneau = 4f;

            for (int y = 0; y < diametre; y++)
            {
                for (int x = 0; x < diametre; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), centre);
                    bool dansLeCercle = distance <= rayon;
                    bool dansLAnneau = dansLeCercle && distance >= rayon - epaisseurAnneau;

                    bool visible = plein ? dansLeCercle : dansLAnneau;
                    donnees[y * diametre + x] = visible ? Color.White : Color.Transparent;
                }
            }

            texture.SetData(donnees);
            return texture;
        }

        // =====================================================================
        // ZONE ÉLÈVE — PISTE BLEUE
        // =====================================================================
        // Progression de l'accompagnement :
        // - étapes 1 et 2 : très guidées ;
        // - étapes 3 et 4 : guidées ;
        // - à partir de l'étape 5 : autonomie accompagnée.
        //
        // Dans toutes les étapes, les méthodes et outils mis à disposition sont
        // expliqués. En revanche, plus vous avancez, moins l'algorithme est donné.
        //
        // Vous devez vous concentrer sur les notions déjà étudiées : variables,
        // conditions, boucles, tableaux et méthodes/fonctions.
        // =====================================================================

        /// <summary>
        /// ÉTAPE 1 - TRÈS GUIDÉE
        /// Place les pions dans leur position de départ.
        ///
        /// Rappels :
        /// - plateauDeJeu[x, y] représente une case du plateau ;
        /// - VIDE = 0, BLANC = 1, NOIR = 2 ;
        /// - seules les cases où (x + y) est impair sont jouables ;
        /// - les Noirs occupent les lignes 0 à 3 ;
        /// - les Blancs occupent les lignes 6 à 9.
        ///
        /// Aucune méthode d'aide n'est nécessaire ici : vous travaillez directement
        /// avec le tableau plateauDeJeu et les constantes fournies.
        /// </summary>
        void InitialiserPlateau()
        {
            plateauDeJeu = new int[TailleGrille, TailleGrille];

            // TODO ÉLÈVE :
            // 1. Parcourir les lignes avec une boucle.
            // 2. Pour chaque ligne, parcourir les colonnes avec une deuxième boucle.
            // 3. Vérifier si la case est jouable avec (x + y) % 2.
            // 4. Si la ligne est comprise entre 0 et 3, placer un pion NOIR.
            // 5. Si la ligne est comprise entre 6 et 9, placer un pion BLANC.
            //
            // Exemple de structure attendue :
            // for (...)
            // {
            //     for (...)
            //     {
            //         if (...)
            //         {
            //             ...
            //         }
            //     }
            // }


            // CODE FOURNI : remet l'état général de la partie à zéro.
            ReinitialiserEtatPartie();
        }

        /// <summary>
        /// ÉTAPE 2 - TRÈS GUIDÉE
        /// Retourne la direction dans laquelle avance un pion.
        /// Blanc : -1 (vers le haut).
        /// Noir  : +1 (vers le bas).
        ///
        /// Aucune méthode d'aide n'est nécessaire ici.
        /// </summary>
        int DirectionAvant(int couleur)
        {
            // TODO ÉLÈVE :
            // - si couleur vaut BLANC, retourner -1 ;
            // - sinon, retourner +1.
            // Utilisez if / else et return.
            return 0;
        }

        /// <summary>
        /// ÉTAPE 3 - GUIDÉE
        /// Retourne true uniquement si la position (x, y) appartient au plateau.
        ///
        /// Outil disponible : TailleGrille.
        /// Cette constante représente la taille du plateau et permet d'éviter
        /// d'écrire directement la valeur 10 dans la condition.
        ///
        /// Cette méthode sera réutilisée plus tard avant d'accéder à une case
        /// calculée du tableau.
        /// </summary>
        bool DansLesLimites(int x, int y)
        {
            // TODO ÉLÈVE :
            // Construisez une condition qui vérifie que x et y sont tous les deux
            // des indices valides du tableau. Vous pouvez utiliser &&.
            return false;
        }

        /// <summary>
        /// ÉTAPE 4 - GUIDÉE
        /// Recherche tous les coups possibles du joueur courant.
        ///
        /// Méthodes mises à disposition :
        ///
        /// CommencerRechercheCoups()
        ///   Prépare une nouvelle recherche et efface les résultats précédents.
        ///   Elle doit être appelée avant de parcourir le plateau.
        ///
        /// MarquerPionTrouvePourTest(x, y)
        ///   Sert uniquement au test visuel de l'étape 4. Elle surligne le pion
        ///   trouvé pour vous permettre de contrôler votre boucle.
        ///
        /// GenererCoupsPourPion(x, y)
        ///   Recherche les coups possibles pour le pion situé en (x, y).
        ///   Elle doit être appelée pour chaque pion du joueur courant.
        ///
        /// TerminerRechercheCoups()
        ///   Termine la recherche et applique les règles générales liées aux coups
        ///   trouvés, notamment la priorité des prises.
        /// </summary>
        void RecalculerCoupsLegaux()
        {
            CommencerRechercheCoups();

            // TODO ÉLÈVE :
            // Parcourez tout le plateau avec deux boucles.
            // Lorsqu'une case contient un pion de joueurCourant :
            // - appelez MarquerPionTrouvePourTest(x, y) ;
            // - appelez GenererCoupsPourPion(x, y).


            TerminerRechercheCoups();
        }

        /// <summary>
        /// ÉTAPE 5 - AUTONOMIE ACCOMPAGNÉE
        /// Recherche tous les déplacements et toutes les prises possibles pour
        /// le pion situé en (x, y).
        ///
        /// Les règles et les outils disponibles sont expliqués ci-dessous, mais
        /// vous devez construire vous-même l'algorithme.
        ///
        /// Méthodes déjà construites ou fournies :
        ///
        /// DirectionAvant(couleur)
        ///   Retourne le sens d'avancement du pion : -1 pour BLANC, +1 pour NOIR.
        ///
        /// DansLesLimites(x, y)
        ///   Retourne true si la position existe dans le plateau. Utilisez-la avant
        ///   de lire une case dont vous venez de calculer les coordonnées.
        ///
        /// AjouterDeplacement(xDepart, yDepart, xArrivee, yArrivee)
        ///   Enregistre un déplacement simple que vous avez déterminé comme valide.
        ///   La liste interne des coups est gérée par le code fourni.
        ///
        /// AjouterPrise(xDepart, yDepart, xArrivee, yArrivee)
        ///   Enregistre une prise que vous avez déterminée comme valide.
        ///
        /// EstUnAdversaire(x, y, couleurPion)
        ///   Retourne true si la case (x, y) contient un pion adverse à couleurPion.
        /// </summary>
        void GenererCoupsPourPion(int x, int y)
        {
            // Informations déjà préparées pour vous :
            int couleurPion = plateauDeJeu[x, y];
            int avant = DirectionAvant(couleurPion);

            // Les deux directions horizontales possibles : gauche et droite.
            int[] decalagesX = { -1, 1 };

            // TODO ÉLÈVE - A : DÉPLACEMENTS SIMPLES
            // Un pion peut avancer d'une case en diagonale vers l'avant.
            // Pour chaque direction horizontale, déterminez si une destination
            // valide et libre existe. Si oui, utilisez AjouterDeplacement(...).
            //
            // À vous de déterminer :
            // - les coordonnées à calculer ;
            // - l'ordre des vérifications ;
            // - la boucle et les conditions nécessaires.


            // Pour une prise, le pion peut sauter vers l'avant ou vers l'arrière.
            int[] directionsY = { avant, -avant };

            // TODO ÉLÈVE - B : PRISES
            // Une prise est possible lorsqu'un pion adverse se trouve sur la case
            // intermédiaire et que la case située juste derrière est libre.
            //
            // Vous devez examiner les combinaisons :
            // - gauche / droite ;
            // - avant / arrière.
            //
            // Utilisez DansLesLimites(...), EstUnAdversaire(...) et
            // AjouterPrise(...). À vous de construire les calculs, les boucles
            // et les conditions nécessaires.
        }

        /// <summary>
        /// ÉTAPE 6 - AUTONOMIE ACCOMPAGNÉE
        /// Applique au plateau le coup choisi par le joueur.
        ///
        /// Le programme vous transmet déjà :
        /// - la case de départ ;
        /// - la case d'arrivée ;
        /// - estUnePrise, qui indique si un pion adverse doit être supprimé.
        ///
        /// Méthodes fournies :
        ///
        /// DeclarerVictoire(couleurPion)
        ///   Termine la partie lorsqu'un pion vient d'atteindre la rangée de victoire.
        ///
        /// TerminerTour(couleurPion)
        ///   À utiliser lorsqu'il n'y a pas de victoire. Cette méthode change de
        ///   joueur, recalcule les coups possibles et vérifie si le joueur suivant
        ///   peut encore jouer.
        ///
        /// Le coup reçu a déjà été validé : vous n'avez normalement pas besoin
        /// d'appeler DirectionAvant(...) ou DansLesLimites(...) ici.
        /// </summary>
        void AppliquerCoup(
            int xDepart,
            int yDepart,
            int xArrivee,
            int yArrivee,
            bool estUnePrise)
        {
            // TODO ÉLÈVE :
            // Votre méthode doit :
            // - conserver la couleur du pion avant de modifier sa case de départ ;
            // - déplacer le pion dans plateauDeJeu ;
            // - supprimer le pion adverse si estUnePrise vaut true ;
            // - détecter si le pion vient de gagner ;
            // - sinon appeler TerminerTour(couleurPion).
            //
            // À vous de déterminer l'ordre des traitements et les calculs utiles.
        }

        // =====================================================================
        // FIN DE LA ZONE ÉLÈVE PRINCIPALE

        // =====================================================================


        // =====================================================================
        // MÉTHODES D'AIDE FOURNIES — NE PAS MODIFIER
        // =====================================================================
        // Ces méthodes cachent les notions qui ne font pas partie des objectifs
        // de la piste bleue : listes, struct, construction d'objets et détails
        // internes du moteur graphique.
        // =====================================================================

        private List<Coup> _deplacementsTrouves = new List<Coup>();
        private List<Coup> _prisesTrouvees = new List<Coup>();

        private int CouleurAdverse(int couleur)
        {
            if (couleur == BLANC)
            {
                return NOIR;
            }

            return BLANC;
        }

        private void ReinitialiserEtatPartie()
        {
            joueurCourant = BLANC;
            _finDePartie = 0;
            _unPionEstSelectionne = false;
            _destinationsPossiblesPourSelection.Clear();
        }

        private void CommencerRechercheCoups()
        {
            _deplacementsTrouves.Clear();
            _prisesTrouvees.Clear();
            _casesTrouveesPourTest.Clear();
        }

        // À appeler dans l'étape 4 lorsque vous avez trouvé un pion du joueur courant.
        // Cette méthode ne change pas les règles du jeu. Elle sert uniquement à afficher
        // un surlignage lorsque le mode de test de l'étape 4 est activé avec F4.
        private void MarquerPionTrouvePourTest(int x, int y)
        {
            if (_afficherTestEtape4)
            {
                _casesTrouveesPourTest.Add(new Point(x, y));
            }
        }

        // Test automatique fourni pour l'étape 2.
        private void TesterDirectionAvant()
        {
            bool blancCorrect = DirectionAvant(BLANC) == -1;
            bool noirCorrect = DirectionAvant(NOIR) == 1;

            _resultatTest = new[]
            {
                "TEST ETAPE 2",
                "BLANC : " + DirectionAvant(BLANC) + "  ATTENDU : -1",
                "NOIR  : " + DirectionAvant(NOIR) + "  ATTENDU : +1",
                blancCorrect && noirCorrect ? "RESULTAT : OK" : "RESULTAT : A CORRIGER"
            };
        }

        // Test automatique fourni pour l'étape 3.
        private void TesterDansLesLimites()
        {
            bool test1 = DansLesLimites(0, 0);
            bool test2 = DansLesLimites(9, 9);
            bool test3 = !DansLesLimites(-1, 3);
            bool test4 = !DansLesLimites(10, 4);

            _resultatTest = new[]
            {
                "TEST ETAPE 3",
                "(0,0)   ATTENDU VRAI : " + (test1 ? "OK" : "ERREUR"),
                "(9,9)   ATTENDU VRAI : " + (test2 ? "OK" : "ERREUR"),
                "(-1,3)  ATTENDU FAUX : " + (test3 ? "OK" : "ERREUR"),
                "(10,4)  ATTENDU FAUX : " + (test4 ? "OK" : "ERREUR"),
                test1 && test2 && test3 && test4 ? "RESULTAT : OK" : "RESULTAT : A CORRIGER"
            };
        }

        private void AjouterDeplacement(int xDepart, int yDepart, int xArrivee, int yArrivee)
        {
            _deplacementsTrouves.Add(
                new Coup(xDepart, yDepart, xArrivee, yArrivee, false));
        }

        private void AjouterPrise(int xDepart, int yDepart, int xArrivee, int yArrivee)
        {
            _prisesTrouvees.Add(
                new Coup(xDepart, yDepart, xArrivee, yArrivee, true));
        }

        private void TerminerRechercheCoups()
        {
            if (_prisesTrouvees.Count > 0)
            {
                coupsLegaux = new List<Coup>(_prisesTrouvees);
            }
            else
            {
                coupsLegaux = new List<Coup>(_deplacementsTrouves);
            }
        }

        private bool EstUnAdversaire(int x, int y, int couleur)
        {
            if (!DansLesLimites(x, y))
            {
                return false;
            }

            if (plateauDeJeu[x, y] == VIDE)
            {
                return false;
            }

            return plateauDeJeu[x, y] != couleur;
        }

        private void DeclarerVictoire(int couleur)
        {
            _finDePartie = couleur;
        }

        private void TerminerTour(int couleurQuiVientDeJouer)
        {
            joueurCourant = CouleurAdverse(couleurQuiVientDeJouer);
            RecalculerCoupsLegaux();

            if (coupsLegaux.Count == 0)
            {
                _finDePartie = CouleurAdverse(joueurCourant);
            }
        }

        // Situation fournie pour tester les étapes 5B et 6.
        // F5 place un pion blanc pouvant capturer un pion noir.
        private void ChargerSituationTestPrise()
        {
            plateauDeJeu = new int[TailleGrille, TailleGrille];
            plateauDeJeu[3, 6] = BLANC;
            plateauDeJeu[4, 5] = NOIR;
            joueurCourant = BLANC;
            _finDePartie = 0;
            _unPionEstSelectionne = false;
            _destinationsPossiblesPourSelection.Clear();
            _afficherTestEtape4 = false;
            RecalculerCoupsLegaux();
            _resultatTest = new[]
            {
                "TEST PRISE",
                "CLIQUEZ LE PION BLANC",
                "EN (3,6)",
                "LA PRISE DOIT ALLER",
                "VERS (5,4)"
            };
        }

        // Adaptateur entre la partie graphique (qui utilise Coup) et la zone élève
        // (qui ne travaille qu'avec des int et un bool).
        private void JouerCoup(Coup coup)
        {
            AppliquerCoup(
                coup.XDepart,
                coup.YDepart,
                coup.XArrivee,
                coup.YArrivee,
                coup.EstUnePrise);
        }

        // =====================================================================
        // CODE FOURNI : AFFICHAGE ET INTERACTIONS
        // Ne modifiez pas cette partie sauf indication de l'enseignant.
        // =====================================================================

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var etatSouris = Mouse.GetState();
            var etatClavier = Keyboard.GetState();
            bool clicGaucheRelache = _etatSourisPrecedent.LeftButton == ButtonState.Pressed
                                      && etatSouris.LeftButton == ButtonState.Released;

            bool toucheF2 = etatClavier.IsKeyDown(Keys.F2) && !_etatClavierPrecedent.IsKeyDown(Keys.F2);
            bool toucheF3 = etatClavier.IsKeyDown(Keys.F3) && !_etatClavierPrecedent.IsKeyDown(Keys.F3);
            bool toucheF4 = etatClavier.IsKeyDown(Keys.F4) && !_etatClavierPrecedent.IsKeyDown(Keys.F4);
            bool toucheF5 = etatClavier.IsKeyDown(Keys.F5) && !_etatClavierPrecedent.IsKeyDown(Keys.F5);

            if (toucheF2)
            {
                TesterDirectionAvant();
            }

            if (toucheF3)
            {
                TesterDansLesLimites();
            }

            if (toucheF4)
            {
                _afficherTestEtape4 = !_afficherTestEtape4;
                RecalculerCoupsLegaux();
                if (_afficherTestEtape4)
                {
                    _resultatTest = new[]
                    {
                        "TEST ETAPE 4",
                        "PIONS TROUVES : " + _casesTrouveesPourTest.Count,
                        "ATTENDU AU DEPART : 20",
                        "LES PIONS BLANCS DOIVENT",
                        "ETRE SURLIGNES EN BLEU"
                    };
                }
                else
                {
                    _resultatTest = new[] { "TEST ETAPE 4 MASQUE" };
                }
            }

            if (toucheF5)
            {
                ChargerSituationTestPrise();
            }

            // Touche R pour relancer une partie rapidement pendant les tests.
            if (etatClavier.IsKeyDown(Keys.R))
            {
                InitialiserPlateau();
                RecalculerCoupsLegaux();
                _resultatTest = new[] { "POSITION INITIALE RECHARGEE" };
            }

            if (_finDePartie == 0 && clicGaucheRelache)
            {
                GererClic(etatSouris.X, etatSouris.Y);
            }

            _etatSourisPrecedent = etatSouris;
            _etatClavierPrecedent = etatClavier;

            base.Update(gameTime);
        }

        /// <summary>
        /// Convertit une position écran en coordonnées de case, puis gère la sélection
        /// d'un pion ou l'exécution d'un coup si une destination valide est cliquée.
        /// </summary>
        private void GererClic(int ecranX, int ecranY)
        {
            int caseX = (ecranX - MargeFenetre) / TailleCase;
            int caseY = (ecranY - MargeFenetre) / TailleCase;

            if (!DansLesLimites(caseX, caseY)) return;

            if (_unPionEstSelectionne)
            {
                // Le joueur clique sur une destination déjà proposée -> on joue le coup.
                foreach (var coup in _destinationsPossiblesPourSelection)
                {
                    if (coup.XArrivee == caseX && coup.YArrivee == caseY)
                    {
                        JouerCoup(coup);
                        _unPionEstSelectionne = false;
                        _destinationsPossiblesPourSelection.Clear();
                        return;
                    }
                }

                // Clic ailleurs : si c'est un autre pion jouable, on change la sélection ; sinon on désélectionne.
                if (plateauDeJeu[caseX, caseY] == joueurCourant)
                {
                    SelectionnerPion(caseX, caseY);
                }
                else
                {
                    _unPionEstSelectionne = false;
                    _destinationsPossiblesPourSelection.Clear();
                }
            }
            else
            {
                if (plateauDeJeu[caseX, caseY] == joueurCourant)
                {
                    SelectionnerPion(caseX, caseY);
                }
            }
        }

        /// <summary>
        /// Sélectionne un pion et filtre, parmi les coups légaux globaux, ceux qui partent de ce pion.
        /// Si la prise est obligatoire, seuls les pions ayant une prise possible deviennent sélectionnables
        /// avec des destinations (les autres pions n'auront aucune destination affichée).
        /// </summary>
        private void SelectionnerPion(int x, int y)
        {
            var destinations = new List<Coup>();
            foreach (var coup in coupsLegaux)
            {
                if (coup.XDepart == x && coup.YDepart == y)
                    destinations.Add(coup);
            }

            if (destinations.Count == 0)
            {
                // Ce pion n'a aucun coup légal actuellement (probablement parce qu'une prise est
                // obligatoire ailleurs) : on ne sélectionne rien.
                _unPionEstSelectionne = false;
                _destinationsPossiblesPourSelection.Clear();
                return;
            }

            _unPionEstSelectionne = true;
            _pionSelectionneX = x;
            _pionSelectionneY = y;
            _destinationsPossiblesPourSelection = destinations;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(30, 30, 30));

            _spriteBatch.Begin();

            DessinerDamier();
            DessinerSurlignages();
            DessinerPions();
            DessinerPanneauAide();

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DessinerRectangle(int x, int y, int largeur, int hauteur, Color couleur)
        {
            _spriteBatch.Draw(_pixel, new Rectangle(x, y, largeur, hauteur), couleur);
        }

        private void DessinerDamier()
        {
            Color caseClaire = new Color(230, 220, 200);
            Color caseSombre = new Color(120, 90, 60);

            for (int y = 0; y < TailleGrille; y++)
            {
                for (int x = 0; x < TailleGrille; x++)
                {
                    bool sombre = (x + y) % 2 == 1;
                    Color couleurCase = sombre ? caseSombre : caseClaire;

                    DessinerRectangle(
                        MargeFenetre + x * TailleCase,
                        MargeFenetre + y * TailleCase,
                        TailleCase,
                        TailleCase,
                        couleurCase);
                }
            }

            // Bordure globale du plateau.
            int tailleTotale = TailleGrille * TailleCase;
            DessinerContour(MargeFenetre, MargeFenetre, tailleTotale, tailleTotale, Color.Black, 2);
        }

        private void DessinerContour(int x, int y, int largeur, int hauteur, Color couleur, int epaisseur)
        {
            DessinerRectangle(x, y, largeur, epaisseur, couleur);
            DessinerRectangle(x, y + hauteur - epaisseur, largeur, epaisseur, couleur);
            DessinerRectangle(x, y, epaisseur, hauteur, couleur);
            DessinerRectangle(x + largeur - epaisseur, y, epaisseur, hauteur, couleur);
        }

        private void DessinerSurlignages()
        {
            // TEST ETAPE 4 : cases des pions trouvés par le code de l'élève.
            if (_afficherTestEtape4)
            {
                Color couleurTest = new Color(0, 180, 255, 110);
                foreach (Point caseTrouvee in _casesTrouveesPourTest)
                {
                    DessinerRectangle(
                        MargeFenetre + caseTrouvee.X * TailleCase,
                        MargeFenetre + caseTrouvee.Y * TailleCase,
                        TailleCase, TailleCase, couleurTest);
                }
            }

            if (!_unPionEstSelectionne) return;

            // Surlignage du pion sélectionné.
            Color surlignageSelection = new Color(255, 220, 0, 120);
            DessinerRectangle(
                MargeFenetre + _pionSelectionneX * TailleCase,
                MargeFenetre + _pionSelectionneY * TailleCase,
                TailleCase, TailleCase, surlignageSelection);

            // Anneaux verts sur les destinations possibles.
            foreach (var coup in _destinationsPossiblesPourSelection)
            {
                Color couleurAnneau = coup.EstUnePrise ? Color.OrangeRed : Color.LimeGreen;
                int cx = MargeFenetre + coup.XArrivee * TailleCase + TailleCase / 2 - _textureAnneau.Width / 2;
                int cy = MargeFenetre + coup.YArrivee * TailleCase + TailleCase / 2 - _textureAnneau.Height / 2;
                _spriteBatch.Draw(_textureAnneau, new Vector2(cx, cy), couleurAnneau);
            }
        }

        private void DessinerPions()
        {
            for (int y = 0; y < TailleGrille; y++)
            {
                for (int x = 0; x < TailleGrille; x++)
                {
                    int couleurPion = plateauDeJeu[x, y];
                    if (couleurPion == VIDE) continue;

                    Color couleurDessin = couleurPion == BLANC ? Color.White : new Color(20, 20, 20);
                    Color couleurContourPion = couleurPion == BLANC ? Color.Black : Color.White;

                    int px = MargeFenetre + x * TailleCase + TailleCase / 2 - _texturePion.Width / 2;
                    int py = MargeFenetre + y * TailleCase + TailleCase / 2 - _texturePion.Height / 2;

                    // Léger contour : on dessine d'abord un cercle légèrement plus grand de la couleur de contour.
                    _spriteBatch.Draw(_texturePion, new Vector2(px - 1, py - 1),
                        null, couleurContourPion, 0f, Vector2.Zero, 1.06f, SpriteEffects.None, 0f);

                    _spriteBatch.Draw(_texturePion, new Vector2(px, py), couleurDessin);
                }
            }
        }

        // ---------------------------------------------------------------------
        // CODE FOURNI : panneau d'aide et petite police bitmap.
        // Aucun fichier de police MonoGame n'est nécessaire.
        // Vous n'avez pas à comprendre ni modifier cette partie.
        // ---------------------------------------------------------------------
        private void DessinerPanneauAide()
        {
            int xPanneau = MargeFenetre + TailleGrille * TailleCase + 25;
            int yPanneau = MargeFenetre;
            int largeur = LargeurPanneauAide - 45;
            int hauteur = TailleGrille * TailleCase;

            DessinerRectangle(xPanneau - 12, yPanneau - 12, largeur + 24, hauteur + 24, new Color(45, 45, 45));
            DessinerContour(xPanneau - 12, yPanneau - 12, largeur + 24, hauteur + 24, new Color(110, 110, 110), 2);

            DessinerTexteBitmap("JEU DE SHADI", xPanneau, yPanneau, 2, Color.White);

            string joueur = joueurCourant == BLANC ? "BLANCS" : "NOIRS";
            DessinerTexteBitmap("JOUEUR : " + joueur, xPanneau, yPanneau + 34, 2, Color.White);

            if (_finDePartie == BLANC)
                DessinerTexteBitmap("VICTOIRE DES BLANCS", xPanneau, yPanneau + 64, 2, Color.White);
            else if (_finDePartie == NOIR)
                DessinerTexteBitmap("VICTOIRE DES NOIRS", xPanneau, yPanneau + 64, 2, Color.White);
            else if (PriseObligatoireEnCours())
                DessinerTexteBitmap("PRISE OBLIGATOIRE", xPanneau, yPanneau + 64, 2, Color.OrangeRed);

            int yTouches = yPanneau + 110;
            DessinerTexteBitmap("TOUCHES", xPanneau, yTouches, 2, Color.White);
            DessinerTexteBitmap("F2 : DIRECTION", xPanneau, yTouches + 28, 2, Color.LightGray);
            DessinerTexteBitmap("F3 : LIMITES", xPanneau, yTouches + 52, 2, Color.LightGray);
            DessinerTexteBitmap("F4 : PIONS TROUVES", xPanneau, yTouches + 76, 2, Color.LightGray);
            DessinerTexteBitmap("F5 : TEST PRISE", xPanneau, yTouches + 100, 2, Color.LightGray);
            DessinerTexteBitmap("R  : REINITIALISER", xPanneau, yTouches + 124, 2, Color.LightGray);

            int yResultat = yPanneau + 285;
            DessinerTexteBitmap("RESULTAT DU TEST", xPanneau, yResultat, 2, Color.White);

            for (int i = 0; i < _resultatTest.Length; i++)
            {
                DessinerTexteBitmap(_resultatTest[i], xPanneau, yResultat + 30 + i * 25, 2, Color.LightGreen);
            }
        }

        private void DessinerTexteBitmap(string texte, int x, int y, int echelle, Color couleur)
        {
            int xDepart = x;
            texte = texte.ToUpperInvariant();

            foreach (char caractere in texte)
            {
                if (caractere == '\n')
                {
                    x = xDepart;
                    y += 8 * echelle;
                    continue;
                }

                string[] motif = ObtenirMotifCaractere(caractere);
                if (motif != null)
                {
                    for (int ligne = 0; ligne < motif.Length; ligne++)
                    {
                        for (int colonne = 0; colonne < motif[ligne].Length; colonne++)
                        {
                            if (motif[ligne][colonne] == '1')
                            {
                                DessinerRectangle(
                                    x + colonne * echelle,
                                    y + ligne * echelle,
                                    echelle,
                                    echelle,
                                    couleur);
                            }
                        }
                    }
                }

                x += 6 * echelle;
            }
        }

        private string[] ObtenirMotifCaractere(char c)
        {
            switch (c)
            {
                case 'A': return new[] { "01110", "10001", "10001", "11111", "10001", "10001", "10001" };
                case 'B': return new[] { "11110", "10001", "10001", "11110", "10001", "10001", "11110" };
                case 'C': return new[] { "01111", "10000", "10000", "10000", "10000", "10000", "01111" };
                case 'D': return new[] { "11110", "10001", "10001", "10001", "10001", "10001", "11110" };
                case 'E': return new[] { "11111", "10000", "10000", "11110", "10000", "10000", "11111" };
                case 'F': return new[] { "11111", "10000", "10000", "11110", "10000", "10000", "10000" };
                case 'G': return new[] { "01111", "10000", "10000", "10111", "10001", "10001", "01111" };
                case 'H': return new[] { "10001", "10001", "10001", "11111", "10001", "10001", "10001" };
                case 'I': return new[] { "11111", "00100", "00100", "00100", "00100", "00100", "11111" };
                case 'J': return new[] { "00111", "00010", "00010", "00010", "10010", "10010", "01100" };
                case 'K': return new[] { "10001", "10010", "10100", "11000", "10100", "10010", "10001" };
                case 'L': return new[] { "10000", "10000", "10000", "10000", "10000", "10000", "11111" };
                case 'M': return new[] { "10001", "11011", "10101", "10101", "10001", "10001", "10001" };
                case 'N': return new[] { "10001", "11001", "10101", "10011", "10001", "10001", "10001" };
                case 'O': return new[] { "01110", "10001", "10001", "10001", "10001", "10001", "01110" };
                case 'P': return new[] { "11110", "10001", "10001", "11110", "10000", "10000", "10000" };
                case 'Q': return new[] { "01110", "10001", "10001", "10001", "10101", "10010", "01101" };
                case 'R': return new[] { "11110", "10001", "10001", "11110", "10100", "10010", "10001" };
                case 'S': return new[] { "01111", "10000", "10000", "01110", "00001", "00001", "11110" };
                case 'T': return new[] { "11111", "00100", "00100", "00100", "00100", "00100", "00100" };
                case 'U': return new[] { "10001", "10001", "10001", "10001", "10001", "10001", "01110" };
                case 'V': return new[] { "10001", "10001", "10001", "10001", "10001", "01010", "00100" };
                case 'W': return new[] { "10001", "10001", "10001", "10101", "10101", "11011", "10001" };
                case 'X': return new[] { "10001", "10001", "01010", "00100", "01010", "10001", "10001" };
                case 'Y': return new[] { "10001", "10001", "01010", "00100", "00100", "00100", "00100" };
                case 'Z': return new[] { "11111", "00001", "00010", "00100", "01000", "10000", "11111" };
                case '0': return new[] { "01110", "10001", "10011", "10101", "11001", "10001", "01110" };
                case '1': return new[] { "00100", "01100", "00100", "00100", "00100", "00100", "01110" };
                case '2': return new[] { "01110", "10001", "00001", "00010", "00100", "01000", "11111" };
                case '3': return new[] { "11110", "00001", "00001", "01110", "00001", "00001", "11110" };
                case '4': return new[] { "00010", "00110", "01010", "10010", "11111", "00010", "00010" };
                case '5': return new[] { "11111", "10000", "10000", "11110", "00001", "00001", "11110" };
                case '6': return new[] { "01110", "10000", "10000", "11110", "10001", "10001", "01110" };
                case '7': return new[] { "11111", "00001", "00010", "00100", "01000", "01000", "01000" };
                case '8': return new[] { "01110", "10001", "10001", "01110", "10001", "10001", "01110" };
                case '9': return new[] { "01110", "10001", "10001", "01111", "00001", "00001", "01110" };
                case ':': return new[] { "00000", "00100", "00100", "00000", "00100", "00100", "00000" };
                case '-': return new[] { "00000", "00000", "00000", "11111", "00000", "00000", "00000" };
                case '+': return new[] { "00000", "00100", "00100", "11111", "00100", "00100", "00000" };
                case '(': return new[] { "00010", "00100", "01000", "01000", "01000", "00100", "00010" };
                case ')': return new[] { "01000", "00100", "00010", "00010", "00010", "00100", "01000" };
                case ',': return new[] { "00000", "00000", "00000", "00000", "00100", "00100", "01000" };
                case '.': return new[] { "00000", "00000", "00000", "00000", "00000", "00100", "00100" };
                case '/': return new[] { "00001", "00010", "00100", "01000", "10000", "00000", "00000" };
                case ' ': return new[] { "00000", "00000", "00000", "00000", "00000", "00000", "00000" };
                default:  return new[] { "00000", "00000", "00000", "00000", "00000", "00000", "00000" };
            }
        }

        // CODE FOURNI : permet uniquement d'afficher le message « prise obligatoire ».
        // Cette méthode utilise la liste de coups, une notion qui ne fait pas partie
        // des objectifs de la piste bleue. Vous n'avez pas à la modifier.
        private bool PriseObligatoireEnCours()
        {
            foreach (Coup coup in coupsLegaux)
            {
                if (coup.EstUnePrise)
                {
                    return true;
                }
            }

            return false;
        }
    }
}