using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace YanivUbuntu
{
    public enum CardDrawing { DECK, LEFT, RIGHT, NONE }
    public enum CardState { LIFT, PUT_DOWN, HOVER_UP, HOVER_DOWN, NONE}
    public class Game1 : Game
    {
        private SpriteFont sumOfCardsFont;
        private SpriteFont scoreTitle;

        private readonly Sprite background;
        private readonly Sprite backgroundEmpty;
        private readonly Sprite startupScreen;
        private readonly Sprite chooseButtonSmall;
        private readonly Sprite chooseButtonBig;
        private readonly Sprite deckSprite;
        private readonly Sprite player1Title;
        private readonly Sprite player2Title;
        private readonly Sprite yaniv;
        private readonly Sprite youWon;
        private readonly Sprite again;
        private readonly Sprite cantTakeCard;
        private readonly Sprite cardLeftPlayer;
        private readonly Sprite cardRightPlayer;
        private Sprite callIt;
        private readonly Sprite tookCard;
        private Sprite sortButton;
        private readonly Sprite throwCardSign;
        private Sprite backButton;


        private Texture2D player1TitleBold,
            player2TitleBold,
            assafTexture,
            player1Won,
            player2Won;

        private readonly List<Vector2[]> playersCardsVectors;
        private readonly Vector2[] playersScoresVectors;
        
        private Vector2 rotationVector;

        private readonly byte[] deck;
        private readonly List<Card> tableCards;
        private readonly Random random;
        private float currentTime;
        private readonly List<CardDrawing> playersCardDrawings;
        private bool sort, startGame;

        private int j1;
        private int i1;
        private int numOfPlayers, roundNumber;
        private float rotation;

        private bool assaf;
        private int randomIndex, shape, value, winner, turnCounter;
        private MouseState mouseCurrent;
        private MouseState mousePrevious;
        private List<Card> thrownCards;
        private Card deckCard;
        private SpriteBatch spriteBatch;
        private readonly List<Player> players;

        private SoundEffect cardShuffle;
        private SoundEffect cardBeingThrown;
        private bool openingCardShufflePlayed;

        public Game1()
        {
            var graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 700;
            graphics.PreferredBackBufferHeight = 700;
            graphics.ApplyChanges();
            this.IsMouseVisible = true;
            deck = new byte[54];
            /*players[1] = new Player(1, "Sharon");
            players[2] = new Player(2, "Sharon");
            players[0] = new Player(0, "Sharon");*/
            players = new List<Player>
            {
                new Player(0, ""),
                new Player(1, ""),
                new Player(2, "")
            };
            random = new Random();
            tableCards = new List<Card>();
            thrownCards = new List<Card>();
            playersCardsVectors = new List<Vector2[]>();
            playersCardsVectors.Add(new Vector2[7]);
            playersCardsVectors.Add(new Vector2[7]);
            /*playersCardsVectors[0] = ;
            playersCardsVectors[0] = new Vector2[7];*/
            startGame = false;
            roundNumber = 0;
            numOfPlayers = 0;
            background = new Sprite();
            backgroundEmpty = new Sprite();                                         
            startupScreen = new Sprite();
            chooseButtonSmall = new Sprite();
            chooseButtonBig = new Sprite();
            deckSprite = new Sprite();
            player1Title = new Sprite();
            player2Title = new Sprite();
            yaniv = new Sprite();
            youWon = new Sprite();
            again = new Sprite();
            cantTakeCard = new Sprite();
            cardLeftPlayer = new Sprite();
            cardRightPlayer = new Sprite();
            tookCard = new Sprite();
            throwCardSign = new Sprite();
            backButton = new Sprite();
            playersCardDrawings = new List<CardDrawing> {CardDrawing.NONE, CardDrawing.NONE, CardDrawing.NONE};
            for (var index = 0; index < playersCardsVectors.Count; index++)
            {
                for (i1 = 0; i1 < 7; i1++)
                {
                    playersCardsVectors[index][i1] = new Vector2(30 + index * 530, 165 + 25 * i1);
                }
            }
            playersScoresVectors = new[]
            {
                new Vector2(485, 410),
                new Vector2(50, 100),
                new Vector2(605, 100)
            };
        }

        protected override void Initialize()
        {            
            randomIndex = 53;            
            tableCards.Clear();
            thrownCards.Clear();
            for (byte i = 0; i < 54; i++) deck[i] = i;
            foreach (var player in players) Deal(player);
            PlaceTableCard();
            PlaceCardsOnMat();
            if (winner != 4 && !startGame)
                turnCounter = winner;
            else turnCounter = 0;
            winner = 4;
            assaf = false;
            currentTime = 0f;
            i1 = 0;
            if(numOfPlayers == 0) numOfPlayers = 3;
            openingCardShufflePlayed = false;
            base.Initialize();
        }

        private void PlaceTableCard()
        {
            tableCards.Add(GenerateCard());
            tableCards[0].spriteVector.X = 310;
            tableCards[0].spriteVector.Y = 220;
        }

        private Card GenerateCard()
        {
            var index = random.Next(0, randomIndex);
            shape = deck[index] / 13;
            value = deck[index] % 13;
            deck[index] = deck[randomIndex];
            var card = new Card((Shapes)shape, value, this.Content.Load<Texture2D>("card"));
            randomIndex--;
            return card;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sumOfCardsFont = Content.Load<SpriteFont>("amount");
            scoreTitle = Content.Load<SpriteFont>("scoreTitle");
            background.SpriteTexture = Content.Load<Texture2D>("backgroundLight");
            background.spriteRectangle = new Rectangle(0, 0, 700, 700);
            backgroundEmpty.SpriteTexture = Content.Load<Texture2D>("backgroundEmpty");
            startupScreen.SpriteTexture = Content.Load<Texture2D>("startupScreen");
            chooseButtonSmall.SpriteTexture = Content.Load<Texture2D>("chooseCircleSmall");
            chooseButtonSmall.spriteVector = new Vector2(433, 265);
            chooseButtonBig.SpriteTexture = Content.Load<Texture2D>("chooseCircleSmall");
            chooseButtonBig.spriteVector = new Vector2(387, 400);
            deckCard = new Card(0, 0, Content.Load<Texture2D>("card"));
            deckSprite.SpriteTexture = Content.Load<Texture2D>("deckRedBack");
            deckSprite.spriteVector = new Vector2(300, 60);
            deckSprite.spriteRectangle = new Rectangle(0, 0, 98, 122);
            player1Title.SpriteTexture = Content.Load<Texture2D>("player1");
            player1Title.spriteVector = new Vector2(10, 60);
            player1Title.spriteRectangle = new Rectangle(0, 0, 164, 36);
            player2Title.SpriteTexture = Content.Load<Texture2D>("player2");
            player2Title.spriteVector = new Vector2(530, 60);
            player1TitleBold = Content.Load<Texture2D>("player1Turn");
            player2TitleBold = Content.Load<Texture2D>("player2Turn");
            throwCardSign.SpriteTexture = Content.Load<Texture2D>("throwCard");
            throwCardSign.spriteVector = new Vector2(235, 460);
            yaniv.SpriteTexture = Content.Load<Texture2D>("yaniv");
            yaniv.spriteVector = new Vector2(150, 200);
            yaniv.spriteRectangle = new Rectangle(0, 0, 402, 121);
            assafTexture = Content.Load<Texture2D>("assaf");
            youWon.SpriteTexture = Content.Load<Texture2D>("youWon");
            youWon.spriteVector = new Vector2(200, 330);
            youWon.spriteRectangle = new Rectangle(0, 0, 289, 70);
            player1Won = Content.Load<Texture2D>("player1Won");
            player2Won = Content.Load<Texture2D>("player2Won");
            again.SpriteTexture = Content.Load<Texture2D>("again");
            again.spriteVector = new Vector2(210, 520);
            again.spriteRectangle = new Rectangle(0, 0, 283, 73);
            cantTakeCard.SpriteTexture = Content.Load<Texture2D>("cantTakeCard");
            cantTakeCard.spriteRectangle = new Rectangle(0, 0, 79, 123);
            cardLeftPlayer.SpriteTexture = Content.Load<Texture2D>("cardLeftPlayer");
            cardRightPlayer.SpriteTexture = Content.Load<Texture2D>("cardRightPlayer");
            cardLeftPlayer.spriteRectangle = new Rectangle(0, 0, 111, 79);
            cardRightPlayer.spriteRectangle = new Rectangle(0, 0, 111, 79);
            /*myScoreVector = new Vector2(500, 410);
            sumCardVector = new Vector2(330, 410);
            leftScoreVector = new Vector2(65, 100);
            rightScoreVector = new Vector2(620, 100);*/
            callIt = new Sprite(Content.Load<Texture2D>("callIt"), new Vector2(140, 390));
            tookCard.SpriteTexture = Content.Load<Texture2D>("tookCard");
            tookCard.spriteVector = deckSprite.spriteVector;
            tookCard.spriteVector.Y = playersCardsVectors[0][0].Y + 50;
            tookCard.spriteRectangle = new Rectangle(0, 0, 79, 111);
            sortButton = new Sprite(Content.Load<Texture2D>("sort"),
            new Vector2(312, 638));
            rotationVector = new Vector2((float)79 / 2, (float)123 / 2);
            backButton = new Sprite(Content.Load<Texture2D>("backButton"), new Vector2(600, 600));
            cardShuffle = Content.Load<SoundEffect>("CardsShuffle");
            cardBeingThrown = Content.Load<SoundEffect>("CardThrown");
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            
            if (startGame)
            {
                // Background | Players Titles | Buttons | Scores Sign
                spriteBatch.Draw(background.SpriteTexture, background.spriteRectangle, Color.White);
                spriteBatch.Draw(deckSprite.SpriteTexture, deckSprite.spriteVector, deckSprite.spriteRectangle,
                    Color.White);
                spriteBatch.Draw(player1Title.SpriteTexture, player1Title.spriteVector,
                    player1Title.spriteRectangle, Color.White);
                spriteBatch.Draw(player2Title.SpriteTexture, player2Title.spriteVector,
                    player1Title.spriteRectangle, Color.White);
                spriteBatch.Draw(backButton.SpriteTexture, backButton.spriteVector, Color.White);
                spriteBatch.Draw(sortButton.SpriteTexture, sortButton.spriteVector,
                    sortButton.spriteRectangle, Color.White);
                foreach (var vector in playersScoresVectors) {
                    spriteBatch.DrawString(scoreTitle, "SCORE",
                        vector, Color.White);
                }
                
                // Highlighting a player's title on his turn
                switch (turnCounter % numOfPlayers) {
                    case 1:
                        spriteBatch.Draw(player1TitleBold, player1Title.spriteVector,
                            player1Title.spriteRectangle, Color.White);
                        break;
                    case 2:
                        spriteBatch.Draw(player2TitleBold, player2Title.spriteVector,
                            player1Title.spriteRectangle, Color.White);
                        break;
                }

                foreach (var player in players) {
                    spriteBatch.DrawString(sumOfCardsFont, player.Score.ToString(),
                        playersScoresVectors[player.PlayerNumber] +
                        Vector2.UnitY * 20 + Vector2.UnitX * 20, Color.Black);
                }
                
                // Call it button
                if (players[0].CardSum() <= 7 && turnCounter % numOfPlayers == 0)
                    spriteBatch.Draw(callIt.SpriteTexture, callIt.spriteVector,
                        callIt.spriteRectangle, Color.White);

                // CARD PRINTING //
                i1 = 0;
                foreach (Card tableCard in tableCards)
                {
                    spriteBatch.Draw(tableCard.SpriteTexture, tableCard.spriteVector, tableCard.spriteRectangle,
                        Color.White);
                    if (i1 != 0 && i1 != tableCards.Count - 1)
                        spriteBatch.Draw(cantTakeCard.SpriteTexture, tableCard.spriteVector,
                            cantTakeCard.spriteRectangle, Color.White);
                    i1++;
                }

                // Players cards (including animation)
                if (playersCardDrawings[0] != CardDrawing.NONE) {
                    spriteBatch.Draw(deckCard.SpriteTexture, tookCard.spriteVector,
                        tookCard.spriteRectangle, Color.White);
                    if (tookCard.spriteVector.Y + 14.5f < 500)
                        tookCard.spriteVector.Y += 14.5f;
                    else {
                        tookCard.spriteVector.Y = 60;
                        playersCardDrawings[0] = CardDrawing.NONE;
                    }

                    for (var j = 0; j < players[0].Cards.Count - 1; j++)
                    {
                        spriteBatch.Draw(players[0].Cards[j].SpriteTexture, players[0].Cards[j].spriteVector,
                            players[0].Cards[j].spriteRectangle, Color.White);
                    }

                } else
                {
                    foreach (var card in players[0].Cards)
                    {
                        switch (card.CardState)
                        {
                            case CardState.LIFT:
                                if (card.spriteVector.Y > 450)
                                    card.spriteVector.Y -= 10;
                                break;
                            case CardState.PUT_DOWN:
                                if (card.spriteVector.Y < 500)
                                    card.spriteVector.Y += 10;
                                else card.CardState = CardState.NONE;
                                break;
                            case CardState.HOVER_UP:
                                if (card.spriteVector.Y > 490)
                                    card.spriteVector.Y -= 5;
                                break;
                            case CardState.HOVER_DOWN:
                                if (card.spriteVector.Y < 500)
                                    card.spriteVector.Y += 5;
                                else card.CardState = CardState.NONE;
                                break;
                        }

                        spriteBatch.Draw(card.SpriteTexture, card.spriteVector, card.spriteRectangle, Color.White);
                    }
                }

                for (var i = 1; i < playersCardDrawings.Count; i++)
                {
                    var rightSprite = i == 1 ? cardLeftPlayer : cardRightPlayer;
                    var rightDirection = i == 1 ? -1 : 1;
                    switch (playersCardDrawings[i])
                    {
                        case CardDrawing.DECK:
                        {
                            if (j1 < 30)
                            {
                                spriteBatch.Draw(tookCard.SpriteTexture, tookCard.spriteVector,
                                    tookCard.spriteRectangle, Color.White, rotation,
                                    rotationVector, 1.0f, SpriteEffects.None, 1.0f);
                                rotation += rightDirection * 0.06f;
                                j1++;
                            } else
                            {
                                tookCard.spriteVector.Y = playersCardsVectors[i - 1][0].Y;
                                spriteBatch.Draw(rightSprite.SpriteTexture, tookCard.spriteVector,
                                    tookCard.spriteRectangle, Color.White);
                                if (tookCard.spriteVector.X > playersCardsVectors[i - 1][0].X)
                                    tookCard.spriteVector.X += rightDirection * 14.5f;
                                else
                                {
                                    tookCard.spriteVector.X = deckSprite.spriteVector.X;
                                    playersCardDrawings[1] = CardDrawing.NONE;
                                    rotation = 0;
                                    j1 = 0;
                                }
                            }

                            for (i1 = players[i].Cards.Count - 1; i1 > 0; i1--)
                                spriteBatch.Draw(rightSprite.SpriteTexture, playersCardsVectors[i - 1][i1],
                                    rightSprite.spriteRectangle, Color.White);
                            break;
                        }
                        case CardDrawing.RIGHT:
                        case CardDrawing.LEFT:
                            // Rotation of card
                            if (j1 < 27)
                            {
                                spriteBatch.Draw(tookCard.SpriteTexture, tookCard.spriteVector,
                                    tookCard.spriteRectangle, Color.White, rotation,
                                    rotationVector, 1.0f, SpriteEffects.None, 1.0f);
                                rotation += rightDirection * 0.06f;
                                j1++;
                            } else
                            {
                                // Arriving to players cards 
                                if (!(tookCard.spriteVector.X <=
                                      (playersCardsVectors[i - 1][players[i].Cards.Count - 1].X +
                                       rightSprite.spriteRectangle.Height)) && rightDirection == -1 ||
                                    !(tookCard.spriteVector.X >=
                                      (playersCardsVectors[i - 1][players[i].Cards.Count - 1].X +
                                       rightSprite.spriteRectangle.Height)) && rightDirection == 1)
                                {
                                    tookCard.spriteVector.X += rightDirection * 10.5f;
                                    spriteBatch.Draw(tookCard.SpriteTexture, tookCard.spriteVector,
                                        tookCard.spriteRectangle, Color.White, rightDirection * 29.85f,
                                        rotationVector,
                                        1.0f, SpriteEffects.None, 1.0f);
                                } else
                                {
                                    playersCardDrawings[i] = CardDrawing.NONE;
                                    tookCard.spriteVector.X = deckSprite.spriteVector.X;
                                    j1 = 0;
                                    rotation = 0;
                                }
                            }
                            // Drawing all of players card excluding the last one -
                            // as the animation arrives there. 
                            for (i1 = players[i].Cards.Count - 2; i1 >= 0; i1--)
                                spriteBatch.Draw(rightSprite.SpriteTexture, playersCardsVectors[i - 1][i1],
                                    rightSprite.spriteRectangle, Color.White);

                            break;
                        default:
                        {
                            // Drawing players cards.
                            for (i1 = players[i].Cards.Count - 1; i1 >= 0; i1--)
                                spriteBatch.Draw(rightSprite.SpriteTexture, playersCardsVectors[i - 1][i1],
                                    rightSprite.spriteRectangle, Color.White);
                            break;
                        }
                    }
                }
                /*switch (playersCardDrawings[1])
                {
                    case CardDrawing.DECK:
                        {
                            if (j1 < 30)
                            {
                                spriteBatch.Draw(tookCard.SpriteTexture, tookCard.spriteVector,
                                                 null, Color.White, rotation,
                                                 rotationVector, 1.0f, SpriteEffects.None, 1.0f);
                                rotation += 0.06f;
                                j1++;
                            }
                            else
                            {
                                tookCard.spriteVector.Y = playersCardsVectors[0][0].Y;
                                spriteBatch.Draw(cardLeftPlayer.SpriteTexture, tookCard.spriteVector,
                                                 null, Color.White);
                                if (tookCard.spriteVector.X > playersCardsVectors[0][0].X)
                                    tookCard.spriteVector.X -= 14.5f;
                                else
                                {
                                    tookCard.spriteVector.X = deckSprite.spriteVector.X;
                                    playersCardDrawings[1] = CardDrawing.NONE;
                                    rotation = 0;
                                    j1 = 0;
                                }
                            }
                            for (i1 = players[1].Cards.Count - 1; i1 > 0; i1--)
                                spriteBatch.Draw(cardLeftPlayer.SpriteTexture, playersCardsVectors[0][i1],
                                                cardLeftPlayer.spriteRectangle, Color.White);
                            break;
                        }
                    case CardDrawing.RIGHT:
                    case CardDrawing.LEFT:
                        {
                            if (j1 < 27)
                            {
                                spriteBatch.Draw(tookCard.SpriteTexture, tookCard.spriteVector,
                                                 tookCard.spriteRectangle, Color.White, rotation,
                                                 rotationVector, 1.0f, SpriteEffects.None, 1.0f);
                                rotation += 0.06f;
                                j1++;
                            }
                            else
                            if (tookCard.spriteVector.X >
                                 playersCardsVectors[0][players[1].Cards.Count - 1].X)
                            {
                                if (tookCard.spriteVector.X <= deckSprite.spriteVector.X -
                                    playersCardsVectors[0][players[1].Cards.Count - 1].X / 2)
                                {
                                    tookCard.SpriteTexture = cardLeftPlayer.SpriteTexture;
                                    tookCard.spriteRectangle = cardLeftPlayer.spriteRectangle;
                                    tookCard.spriteVector.Y =
                                        playersCardsVectors[0][players[1].Cards.Count - 1].Y;
                                    spriteBatch.Draw(tookCard.SpriteTexture, tookCard.spriteVector,
                                        tookCard.spriteRectangle, Color.White);
                                }
                                else
                                {
                                    spriteBatch.Draw(tookCard.SpriteTexture, tookCard.spriteVector,
                                    tookCard.spriteRectangle, Color.White, 29.85f,
                                    rotationVector,
                                    1.0f, SpriteEffects.None, 1.0f);
                                }
                                tookCard.spriteVector.X -= 14.5f;
                            }
                            else
                            {
                                tookCard.spriteVector.X = deckSprite.spriteVector.X;
                                playersCardDrawings[1] = CardDrawing.NONE;
                                j1 = 0;
                                rotation = 0;
                            }
                            for (i1 = players[1].Cards.Count - 2; i1 >= 0; i1--)
                                spriteBatch.Draw(cardLeftPlayer.SpriteTexture, playersCardsVectors[0][i1],
                                                cardLeftPlayer.spriteRectangle, Color.White);
                            break;
                        }
                    default:
                        {
                            for (i1 = players[1].Cards.Count - 1; i1 >= 0; i1--)
                                spriteBatch.Draw(cardRightPlayer.SpriteTexture, playersCardsVectors[0][i1],
                                                cardLeftPlayer.spriteRectangle, Color.White);
                            break;
                        }
                }

                if (numOfPlayers == 3)
                {
                    switch (playersCardDrawings[2])
                    {
                        case CardDrawing.DECK:
                            {
                                if (j1 < 30)
                                {
                                    spriteBatch.Draw(tookCard.SpriteTexture, tookCard.spriteVector,
                                                     null, Color.White, rotation,
                                                     rotationVector, 1.0f, SpriteEffects.None, 1.0f);
                                    rotation -= 0.06f;
                                    j1++;
                                }
                                else
                                {
                                    tookCard.spriteVector.Y = playersCardsVectors[0][0].Y;
                                    spriteBatch.Draw(cardRightPlayer.SpriteTexture, tookCard.spriteVector,
                                                     null, Color.White);
                                    if (tookCard.spriteVector.X < playersCardsVectors[0][0].X)
                                        tookCard.spriteVector.X += 14.5f;
                                    else
                                    {
                                        tookCard.spriteVector.X = deckSprite.spriteVector.X;
                                        playersCardDrawings[2] = CardDrawing.NONE;
                                        rotation = 0;
                                        j1 = 0;
                                    }
                                }
                                for (i1 = players[2].Cards.Count - 1; i1 > 0; i1--)
                                    spriteBatch.Draw(cardRightPlayer.SpriteTexture, playersCardsVectors[0][i1],
                                                    null, Color.White);
                                break;
                            }

                        case CardDrawing.LEFT:
                        case CardDrawing.RIGHT:
                            if (j1 < 27)
                            {
                                spriteBatch.Draw(tookCard.SpriteTexture, tookCard.spriteVector,
                                                 tookCard.spriteRectangle, Color.White, rotation,
                                                 rotationVector, 1.0f, SpriteEffects.None, 1.0f);
                                rotation -= 0.06f;
                                j1++;
                            }
                            else
                            {
                                if (tookCard.spriteVector.X <
                                     playersCardsVectors[0][players[2].Cards.Count - 1].X)
                                {
                                    if (tookCard.spriteVector.X >= 60 +
                                        playersCardsVectors[0][players[2].Cards.Count - 1].X / 2)
                                    {
                                        tookCard.SpriteTexture = cardRightPlayer.SpriteTexture;
                                        tookCard.spriteRectangle = cardLeftPlayer.spriteRectangle;
                                        tookCard.spriteVector.Y =
                                            playersCardsVectors[0][players[2].Cards.Count - 1].Y;
                                        spriteBatch.Draw(tookCard.SpriteTexture, tookCard.spriteVector,
                                            tookCard.spriteRectangle, Color.White);
                                    }
                                    else
                                    {
                                        spriteBatch.Draw(tookCard.SpriteTexture, tookCard.spriteVector,
                                        tookCard.spriteRectangle, Color.White, -29.85f,
                                        new Vector2((float)tableCards[tableCards.Count - 1].spriteRectangle.Width / 2,
                                            (float)tableCards[tableCards.Count - 1].spriteRectangle.Height / 2),
                                        1.0f, SpriteEffects.None, 1.0f);
                                    }
                                    tookCard.spriteVector.X += 14.5f;
                                }
                                else
                                {
                                    tookCard.spriteVector.X = deckSprite.spriteVector.X;
                                    playersCardDrawings[2] = CardDrawing.NONE;
                                    j1 = 0;
                                    rotation = 0;
                                }
                            }

                            for (i1 = players[2].Cards.Count - 2; i1 >= 0; i1--)
                                spriteBatch.Draw(cardRightPlayer.SpriteTexture, playersCardsVectors[0][i1],
                                                cardLeftPlayer.spriteRectangle, Color.White);
                            break;

                        default:
                            {
                                for (i1 = players[2].Cards.Count - 1; i1 >= 0; i1--)
                                    spriteBatch.Draw(cardRightPlayer.SpriteTexture, playersCardsVectors[0][i1],
                                                    cardLeftPlayer.spriteRectangle, Color.White);
                                break;
                            }
                    }
                }*/
                
                if (winner < 4)
                {
                    if (!assaf)
                    {
                        spriteBatch.Draw(yaniv.SpriteTexture, yaniv.spriteVector,
                            yaniv.spriteRectangle, Color.White);
                        switch (winner)
                        {
                            case 0:
                                spriteBatch.Draw(youWon.SpriteTexture, youWon.spriteVector,
                                    youWon.spriteRectangle, Color.White);
                                break;
                            case 1:
                                spriteBatch.Draw(player1Won, youWon.spriteVector,
                                    youWon.spriteRectangle, Color.White);
                                break;
                            case 2:
                                spriteBatch.Draw(player2Won, youWon.spriteVector,
                                    youWon.spriteRectangle, Color.White);
                                break;

                        }
                    } else
                    {
                        if (currentTime < 2.5f)
                        {
                            spriteBatch.Draw(yaniv.SpriteTexture, yaniv.spriteVector,
                                yaniv.spriteRectangle, Color.White);
                        } else
                        {
                            spriteBatch.Draw(assafTexture, yaniv.spriteVector,
                                yaniv.spriteRectangle, Color.White);
                            switch (winner)
                            {
                                case 0:
                                    spriteBatch.Draw(youWon.SpriteTexture, youWon.spriteVector,
                                        youWon.spriteRectangle, Color.White);
                                    break;
                                case 1:
                                    spriteBatch.Draw(player1Won, youWon.spriteVector,
                                        youWon.spriteRectangle, Color.White);
                                    break;
                                case 2:
                                    spriteBatch.Draw(player2Won, youWon.spriteVector,
                                        youWon.spriteRectangle, Color.White);
                                    break;

                            }
                        }

                    }

                    if (currentTime > 5f)
                    {
                        spriteBatch.End();
                        base.Draw(gameTime);
                        roundNumber--;
                        if (roundNumber <= 0)
                        {
                            startGame = false;
                            return;
                        }

                        currentTime = 0f;
                        players[0].ResetPlayer();
                        players[1].ResetPlayer();
                        players[2].ResetPlayer();
                        Initialize();
                        return;
                    }
                }
            } else
            {
                // Empty background
                spriteBatch.Draw(backgroundEmpty.SpriteTexture, background.spriteRectangle, Color.White);

                // Displaying setting for game background
                spriteBatch.Draw(startupScreen.SpriteTexture, background.spriteRectangle, Color.White);
                spriteBatch.Draw(chooseButtonSmall.SpriteTexture, chooseButtonSmall.spriteVector
                    , null, Color.White);
                spriteBatch.Draw(chooseButtonBig.SpriteTexture, chooseButtonBig.spriteVector,
                    null, Color.White);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            currentTime += (float) gameTime.ElapsedGameTime.TotalSeconds;
            mousePrevious = mouseCurrent;
            mouseCurrent = Mouse.GetState();
            if (startGame)
            {
                // Play the card shuffle sound at the start of the game
                if (!openingCardShufflePlayed)
                {
                    cardShuffle.Play();
                    openingCardShufflePlayed = true;
                }

                if (backButton.MouseTouched(mouseCurrent, mousePrevious))
                {
                    startGame = false;
                    base.Update(gameTime);
                    return;
                }

                // If there is a winner for this game, Start a new one or close game
                if (winner < 4)
                {
                    base.Update(gameTime);
                    return;
                }

                // Cards mouse hover animation
                foreach (Card card in players[0].Cards)
                {
                    if (card.MouseHovered(mouseCurrent) && card.CardState != CardState.LIFT)
                    {
                        card.CardState = CardState.HOVER_UP;
                    } else if (mouseCurrent != mousePrevious && card.CardState == CardState.HOVER_UP)
                        card.CardState = CardState.HOVER_DOWN;
                }

                if (turnCounter % numOfPlayers == 0)
                {
                    thrownCards.Clear();
                    if (sortButton.MouseTouched(mouseCurrent, mousePrevious))
                    {
                        sort = true;
                        PlaceCardsOnMat();
                    }

                    if (players[0].CardSum() <= 7 &&
                        callIt.MouseTouched(mouseCurrent, mousePrevious))
                    {
                        CheckWhoIsTheWinner(players[0]);
                        base.Update(gameTime);
                        return;
                    }

                    // Clicking and picking cards to throw
                    foreach (var c in players[0].Cards.Where(c => c.MouseTouched(mouseCurrent, mousePrevious)))
                        players[0].PickCard(c);

                    if (!CheckIfLegal(players[0]))
                    {
                        base.Update(gameTime);
                        return;
                    }

                    if (tableCards[0].MouseTouched(mouseCurrent, mousePrevious) ||
                        tableCards[tableCards.Count - 1].MouseTouched(mouseCurrent, mousePrevious))
                    {

                        var index = tableCards[0].MouseTouched(mouseCurrent, mousePrevious) &&
                                    tableCards[tableCards.Count - 1].MouseTouched(mouseCurrent, mousePrevious)
                            ? tableCards.Count - 1
                            : 0;
                        thrownCards = players[0].Play(new Card(tableCards[index]));
                        tookCard.spriteRectangle = tableCards[0].spriteRectangle;
                        playersCardDrawings[0] = index == 0 ? CardDrawing.LEFT : CardDrawing.RIGHT;
                        cardBeingThrown.CreateInstance().Play();
                        ReplaceTableCards(thrownCards);
                        PlaceCardsOnMat();
                        turnCounter++;
                        currentTime = 0;
                    } else if (deckSprite.MouseTouched(mouseCurrent, mousePrevious))
                    {
                        var cardToTake = GenerateCard();
                        thrownCards = players[0].Play(cardToTake);
                        tookCard.spriteRectangle = cardToTake.spriteRectangle;
                        playersCardDrawings[0] = CardDrawing.DECK;
                        cardBeingThrown.CreateInstance().Play();
                        ReplaceTableCards(thrownCards);
                        PlaceCardsOnMat();
                        turnCounter++;
                        currentTime = 0;
                    }
                } else
                {
                    if (currentTime > 3.5f)
                    {
                        ComputerStrategyCardTake(players[turnCounter % numOfPlayers]);
                        turnCounter++;
                        currentTime = 0f;
                    }
                }
            } else
            {
                if (mouseCurrent.X >= 348 &&
                    mouseCurrent.X < 405 &&
                    mouseCurrent.Y >= 275 &&
                    mouseCurrent.Y < 332)
                {
                    chooseButtonSmall.spriteVector.X = 345;
                    numOfPlayers = 2;
                }

                if (mouseCurrent.X >= 442 &&
                    mouseCurrent.X < 500 &&
                    mouseCurrent.Y >= 264 &&
                    mouseCurrent.Y < 320)
                {
                    chooseButtonSmall.spriteVector.X = 433;
                    numOfPlayers = 3;

                }

                if (mouseCurrent.X >= 200 &&
                    mouseCurrent.X < 257 &&
                    mouseCurrent.Y >= 400 &&
                    mouseCurrent.Y < 457)
                {
                    chooseButtonBig.spriteVector.X = 200;
                    roundNumber = 1;


                }

                if (mouseCurrent.X >= 295 &&
                    mouseCurrent.X < 295 + 57 &&
                    mouseCurrent.Y >= 400 &&
                    mouseCurrent.Y < 457)
                {
                    chooseButtonBig.spriteVector.X = 295;
                    roundNumber = 3;

                }

                if (mouseCurrent.X >= 390 &&
                    mouseCurrent.X < 490 &&
                    mouseCurrent.Y >= 420 &&
                    mouseCurrent.Y < 467)
                {
                    chooseButtonBig.spriteVector.X = 387;
                    roundNumber = 5;

                }

                if (mousePrevious.LeftButton == ButtonState.Pressed && mouseCurrent.LeftButton == ButtonState.Released)
                {
                    if (mouseCurrent.X >= 229 &&
                        mouseCurrent.X < 500 &&
                        mouseCurrent.Y >= 558 &&
                        mouseCurrent.Y < 645)
                    {
                        players[0].ResetPlayer();
                        players[1].ResetPlayer();
                        players[2].ResetPlayer();
                        Initialize();
                        startGame = true;
                    }
                }
            }

            base.Update(gameTime);
        }

        private bool CheckIfLegal(Player player)
        {
            if (player.PickedCards.Count == 0) return false;
            if (player.PickedCards.Count == 1) return true;
            var sameValue = false;

            var series = CheckSeries(player.PickedCards).Count >= 3;
            for (var index = 0; index < player.PickedCards.Count - 1; index++)
            {
                if (player.PickedCards[index].CardValue == player.PickedCards[index + 1].CardValue)
                {
                    sameValue = true;
                }
            }
            return sameValue ^ series;

        }
        private void ReplaceTableCards(List<Card> cards)
        {
            var arrayTableCards = cards.ToArray();
            var middle = cards.Count % 2 == 0 ? cards.Count / 2 : (cards.Count - 1) / 2;
            arrayTableCards[middle].spriteVector.X = 310;
            arrayTableCards[middle].spriteVector.Y = 220;

            for (var i = 1; i <= middle; i++)
            {
                arrayTableCards[middle - i].spriteVector =
                    arrayTableCards[middle].spriteVector - Vector2.UnitX * 30 * i;
                if (middle + i >= arrayTableCards.Length) continue;
                arrayTableCards[middle + i].spriteVector = 
                    arrayTableCards[middle].spriteVector + Vector2.UnitX * 30 * i;
            }
            tableCards.Clear();
            tableCards.AddRange(arrayTableCards);
        }

        private void ComputerStrategy(Player player, Card cardToTake, bool chosenFromDeck)
        {
            var series = new List<Card>();
            var sameValueCards = new List<Card>();
           
            // Checking if a series exists
            List<Card>[] shapes = {player.ClubsCards, player.DiamondCards, player.HeartsCards, player.SpadesCards}; 
            foreach (var t in shapes)
            {
                var demoCards = new List<Card>(t);
                demoCards.AddRange(player.JokerCards);
                if (demoCards.Count < 3) continue;
                if(series.Count < 3)
                    series = CheckSeries(demoCards);
                else
                {
                    var betterSeries = CheckSeries(demoCards);
                    if (Card.CardSum(series) < Card.CardSum(betterSeries))
                        series = betterSeries;
                }
            }
            /*
            if (player.DiamondCards.Count >= 3 || player.JokerCards.Count > 0 && player.DiamondCards.Count >= 2)
            {
                if(series.Count < 3)
                    series = CheckSeries(player.DiamondCards, player.JokerCards);
                else
                {
                    List<Card> betterSeries = CheckSeries(player.DiamondCards, player.JokerCards);
                    if (Card.CardSum(series) < Card.CardSum(betterSeries))
                        series = betterSeries;
                }
            }
            if (player.HeartsCards.Count >= 3 || player.JokerCards.Count > 0 && player.HeartsCards.Count >= 2)
            {
                if(series.Count < 3)
                    series = CheckSeries(player.HeartsCards, player.JokerCards);
                else
                {
                    List<Card> betterSeries = CheckSeries(player.HeartsCards, player.JokerCards);
                    if (Card.CardSum(series) < Card.CardSum(betterSeries))
                        series = betterSeries;
                }

            }
            if (player.SpadesCards.Count >= 3 || player.JokerCards.Count > 0 && player.SpadesCards.Count >= 2)
            {
                if(series.Count < 3)
                    series = CheckSeries(player.SpadesCards, player.JokerCards);
                else
                {
                    List<Card> betterSeries = CheckSeries(player.SpadesCards, player.JokerCards);
                    if (Card.CardSum(series) < Card.CardSum(betterSeries))
                        series = betterSeries;
                }
            }*/

            // Checking if two cards or more have the same value 
            player.Cards.Sort();
            var sameCardValue = -1;
            Card largestCardThatNotThrown = null;
            for (var i =  player.Cards.Count - 1; i > 0; i--)
            {
                if (!chosenFromDeck)
                {
                    if (largestCardThatNotThrown == null && player.Cards[i].CardValue != cardToTake.CardValue)
                        largestCardThatNotThrown = player.Cards[i];
                } else
                {
                    if (largestCardThatNotThrown == null && player.Cards[i].CardShape != Shapes.JOKER)
                        largestCardThatNotThrown = player.Cards[i];
                }


                // If there is two or more cards with the same value
                if (player.Cards[i].CardValue != player.Cards[i - 1].CardValue ||
                    player.Cards[i].CardShape == Shapes.JOKER || chosenFromDeck ||
                    player.Cards[i].CardValue == cardToTake.CardValue) continue;
                if (sameValueCards.Count == 0)
                {
                    sameValueCards.Add(player.Cards[i]);
                    sameValueCards.Add(player.Cards[i - 1]);
                    sameCardValue = player.Cards[i].CardValue;
                }
                // To avoid two cases of equality. 
                else if (player.Cards[i].CardValue == sameCardValue)
                    sameValueCards.Add(player.Cards[i]);

                // If player chose a table card he wouldn't throw cards with
                // it's value so he would throw them on the next round.
                // If player chose a deck card he would throw it anyway. 
            }

            // Check value of largest card as opposed to the finding above
            var seriesValue = series.Count >= 3 ? Card.CardSum(series) : 0;
            var sameValueCardsValue = Card.CardSum(sameValueCards);
            
            if (seriesValue > player.Cards[player.Cards.Count - 1].CardValue &&
                 seriesValue > sameValueCardsValue)
                player.PickedCards = series;
            else if (sameValueCardsValue > player.Cards[player.Cards.Count - 1].CardValue &&
                     sameValueCardsValue > seriesValue)
                player.PickedCards = sameValueCards;
            else
            {
                player.PickedCards = largestCardThatNotThrown != null ? new List<Card> {largestCardThatNotThrown} :
                    new List<Card> {player.Cards[player.Cards.Count - 1]};
            }
            
            player.PickCards();
            playersCardDrawings[player.PlayerNumber] = player.PlayerNumber == 1 ? CardDrawing.LEFT : CardDrawing.RIGHT;
            GetTableCardLocationForAnimation();
            ReplaceTableCards(player.Play(new Card(cardToTake)));
            cardBeingThrown.CreateInstance().Play();
        }

        private void ComputerStrategyCardTake(Player player)
        {
            Card cardToTake = null;
            var chosenFromDeck = false;

            if (player.CardSum() <= 7)
            {
                CheckWhoIsTheWinner(player);
                return;
            }

            // If table card is a low value card or the player has another card in his cards with the same value
            // He will take it for the next round
            // If the card on the table isn't helpful, the player will take a deck card   
            /*if(tableCards[0].CardShape == Shapes.JOKER)
            {
                if (player.PlayerNumber == 1) playersCardDrawings[1] = CardDrawing.LEFT;
                else playersCardDrawings[2] = CardDrawing.LEFT;
                cardToTake = new Card(tableCards[0]);
            }
            else if (tableCards[tableCards.Count - 1].CardShape == Shapes.JOKER)
            {
                if (player.PlayerNumber == 1) playersCardDrawings[1] = CardDrawing.RIGHT;
                else playersCardDrawings[2] = CardDrawing.RIGHT;
                cardToTake = new Card(tableCards[tableCards.Count - 1]);
            }
            else if (tableCards[0].CardValue < 3 ||
                     player.Cards.Exists(card => card.CardValue == tableCards[0].CardValue))
            {
                if (player.PlayerNumber == 1) playersCardDrawings[1] = CardDrawing.LEFT;
                else playersCardDrawings[2] = CardDrawing.LEFT;
                cardToTake = new Card(tableCards[0]);
            }
            else if (tableCards[tableCards.Count - 1].CardValue < 3 ||
                     player.Cards.Exists(card => card.CardValue == tableCards[tableCards.Count - 1].CardValue))
            {
                if (player.PlayerNumber == 1) playersCardDrawings[1] = CardDrawing.RIGHT;
                else playersCardDrawings[2] = CardDrawing.RIGHT;
                cardToTake = new Card(tableCards[tableCards.Count - 1]);
            }
            else
            {
                if (player.PlayerNumber == 1) playersCardDrawings[1] = CardDrawing.DECK;
                else playersCardDrawings[2] = CardDrawing.DECK;
                cardToTake = GenerateCard();
                deckCard1 = true;
            }*/

            Card[] allowedToTake = {tableCards[0], tableCards[tableCards.Count - 1]};
            var seriesSum = 0;
            // Check if one of the table cards complete a series
            for (var i = 0; i < 2 && cardToTake == null; i++)
            {
                var shapeCards = allowedToTake[i].CardShape == Shapes.CLUBS ? new List<Card>(player.ClubsCards) :
                    allowedToTake[i].CardShape == Shapes.HEARTS ? new List<Card>(player.HeartsCards)  :
                    allowedToTake[i].CardShape == Shapes.SPADES ? new List<Card>(player.SpadesCards) :
                    new List<Card>(player.DiamondCards);
                
                shapeCards.AddRange(player.JokerCards);
                shapeCards.Add(allowedToTake[i]);
                List<Card> series;
                if ((series = CheckSeries(shapeCards)).Count < 3) continue;
                seriesSum = Card.CardSum(series);
                cardToTake = new Card(allowedToTake[i]);
            }

            // Check if one of the table cards exists in players cards
            // Compare its sum to the potential series, is found earlier.
            var saveValueSum = 0;
            for (var i = 0; i < 2; i++)
            {
                if (!player.Cards.Exists(card => card.CardValue == allowedToTake[i].CardValue) ||
                    allowedToTake[i].CardValue * 2 <= seriesSum || allowedToTake[i].CardValue * 2 <= saveValueSum) continue;
                cardToTake = new Card(allowedToTake[i]);
                saveValueSum = allowedToTake[i].CardValue * 2;
            }

            // If the cards on the table don't fit a double throw or a series
            // if one of the table cards has a small value
            for (var i = 0; i < 2 && cardToTake == null; i++) {
                if (allowedToTake[i].CardValue < 3)
                    cardToTake = new Card(allowedToTake[i]);
            }

            if (cardToTake == null) {
                chosenFromDeck = true;
                cardToTake = GenerateCard();
            }
            
            ComputerStrategy(player, cardToTake, chosenFromDeck);
        }
        private void CheckWhoIsTheWinner(Player player)
        {
            var minSum = int.MaxValue;
            var winnerNumber = 0;
            foreach (var p in players.Where(p => minSum > p.CardSum()))
            {
                minSum = p.CardSum();
                winnerNumber = p.PlayerNumber;
            }

            if (minSum < player.CardSum())
            {
                assaf = true;
            }
            winner = winnerNumber;
            foreach (var p in players)
            {
                p.ScorePlayer(p == player && assaf);
            }
        }
        private List<Card> CheckSeries(List<Card> cards)
        {
            var series = new List<Card>();
            cards.Sort();
            var jokers = new List<Card>(cards);
            jokers.RemoveAll(c => c.CardShape != Shapes.JOKER);
            var pickedCardsWnj = new List<Card>(cards);
            pickedCardsWnj.RemoveAll(c => c.CardShape == Shapes.JOKER);
            var lastSeriesValue = -1;
            for (var i = 0; i < pickedCardsWnj.Count - 1; i++)
            {
                if ((lastSeriesValue != -1 && lastSeriesValue == pickedCardsWnj[i].CardValue) &&
                    pickedCardsWnj[i].CardValue + 1 == pickedCardsWnj[i + 1].CardValue || 
                    (lastSeriesValue == -1 && pickedCardsWnj[i].CardValue + 1 == pickedCardsWnj[i + 1].CardValue))
                {
                    if(lastSeriesValue != pickedCardsWnj[i].CardValue)
                        series.Add(pickedCardsWnj[i]);
                    series.Add(pickedCardsWnj[i + 1]);
                    lastSeriesValue = pickedCardsWnj[i + 1].CardValue;
                } else if ((lastSeriesValue != -1 && lastSeriesValue == pickedCardsWnj[i].CardValue &&
                           pickedCardsWnj[i].CardValue + jokers.Count + 1 == pickedCardsWnj[i + 1].CardValue) ||
                           (lastSeriesValue == -1  &&
                           pickedCardsWnj[i].CardValue + jokers.Count + 1 == pickedCardsWnj[i + 1].CardValue))
                {
                    if(lastSeriesValue != pickedCardsWnj[i].CardValue)
                        series.Add(pickedCardsWnj[i]);
                    for (var j = 0; j < pickedCardsWnj[i + 1].CardValue - pickedCardsWnj[i].CardValue - 1; j++)
                    {
                        var joker = jokers[jokers.Count - 1 - j];
                        joker.CardValue = pickedCardsWnj[i].CardValue + jokers.Count;
                        series.Add(joker);
                    }
                    series.Add(pickedCardsWnj[i + 1]);
                    lastSeriesValue = pickedCardsWnj[i + 1].CardValue;
                }
            }
            return series;
        }
        
        private void Deal(Player player)
        {
            var playersDeck = new List<Card>();
            for (int i = 0; i < 7; i++)
            {
                playersDeck.Add(GenerateCard());
            }
            player.SetCards(playersDeck);
        }

        private void PlaceCardsOnMat()
        {
            Card[] cards = players[0].Cards.ToArray();
            int middle, addition = 0;
            if (sort)
            {
                Array.Sort(cards);
            }
            if (cards.Length % 2 == 1)
                middle = (cards.Length - 1) / 2;
            else
            {
                middle = cards.Length / 2;
                addition = 40;
            }

            cards[middle].spriteVector.X = 320 + addition;
            cards[middle].spriteVector.Y = 500;
            
            tookCard.spriteVector.X = cards[middle].spriteVector.X;

            for (int i = 1; i <= middle; i++)
            {
                cards[middle - i].spriteVector.X = 320 - (85 * i) + addition;
                cards[middle - i].spriteVector.Y = 500;
                if (middle + i < cards.Length)
                {
                    cards[middle + i].spriteVector.X = 320 + (85 * i) + addition;
                    cards[middle + i].spriteVector.Y = 500;
                }
            }
            if (!sort)
            {
                var temp = cards[middle].spriteVector;
                cards[middle].spriteVector = cards[cards.Length - 1].spriteVector;
                cards[cards.Length - 1].spriteVector = temp;
            }
            else
            {
                players[0].FixList(cards);
                sort = false;
            }

        }

        private void GetTableCardLocationForAnimation()
        {
            switch (playersCardDrawings[0])
            {
                case CardDrawing.DECK:                    
                    tookCard.spriteRectangle = deckCard.spriteRectangle;
                    tookCard.spriteVector.Y = deckSprite.spriteVector.Y;
                    return;
                case CardDrawing.LEFT:                   
                    tookCard.spriteRectangle = tableCards[0].spriteRectangle;
                    tookCard.spriteVector.Y = tableCards[0].spriteVector.Y + 30;
                    break;
                case CardDrawing.RIGHT:
                    tookCard.SpriteTexture = deckCard.SpriteTexture;
                    tookCard.spriteRectangle = tableCards[tableCards.Count - 1].spriteRectangle;
                    tookCard.spriteVector.Y = tableCards[0].spriteVector.Y + 30;
                    break;
            }
            
            for (var index = 1; index < playersCardDrawings.Count; index++)
            {
                var cardDrawing = playersCardDrawings[index];
                foreach (var cardsVector in playersCardsVectors)
                {
                    var tableCardIndex = cardDrawing == CardDrawing.LEFT
                        ? 0
                        : tableCards.Count - 1;
                    switch (cardDrawing)
                    {
                        case CardDrawing.DECK:
                            tookCard.SpriteTexture = Content.Load<Texture2D>("tookCard");
                            tookCard.spriteVector.X = deckSprite.spriteVector.X;
                            tookCard.spriteVector.Y = cardsVector[index - 1].Y + 50;
                            break;
                        case CardDrawing.LEFT:
                        case CardDrawing.RIGHT:
                            tookCard.SpriteTexture = tableCards[tableCardIndex].SpriteTexture;
                            tookCard.spriteRectangle = tableCards[tableCardIndex].spriteRectangle;
                            tookCard.spriteVector.Y = cardsVector[players[index].Cards.Count - 1].Y + 25;
                            tookCard.spriteVector.X = deckSprite.spriteVector.X;
                            break;
                        case CardDrawing.NONE:
                            break;
                    }
                }
            }

            /*switch (playersCardDrawings[1])
            {
                case CardDrawing.DECK:
                    tookCard.SpriteTexture = Content.Load<Texture2D>("tookCard");
                    tookCard.spriteVector.X = deckSprite.spriteVector.X;
                    tookCard.spriteVector.Y = playersCardsVectors[0][0].Y + 50;
                    break;
                case CardDrawing.RIGHT:
                    tookCard.SpriteTexture = tableCards[tableCards.Count - 1].SpriteTexture;
                    tookCard.spriteRectangle = tableCards[tableCards.Count - 1].spriteRectangle;
                    tookCard.spriteVector.Y = playersCardsVectors[0][players[1].Cards.Count - 1].Y;
                    tookCard.spriteVector.X = deckSprite.spriteVector.X;
                    break;
                case CardDrawing.LEFT:
                    tookCard.SpriteTexture = tableCards[0].SpriteTexture;
                    tookCard.spriteRectangle = tableCards[0].spriteRectangle;
                    tookCard.spriteVector.Y = playersCardsVectors[0][players[1].Cards.Count - 1].Y;
                    tookCard.spriteVector.X = deckSprite.spriteVector.X;
                    break;
                case CardDrawing.NONE:
                    break;
            }

            switch (playersCardDrawings[2])
            {
                case CardDrawing.DECK:
                    tookCard.SpriteTexture = Content.Load<Texture2D>("tookCard");
                    tookCard.spriteVector.X = deckSprite.spriteVector.X;
                    tookCard.spriteVector.Y = playersCardsVectors[0][0].Y + 50;
                    break;
                case CardDrawing.LEFT:
                    tookCard.SpriteTexture = tableCards[0].SpriteTexture;
                    tookCard.spriteRectangle = tableCards[0].spriteRectangle;
                    tookCard.spriteVector.X = deckSprite.spriteVector.X;
                    tookCard.spriteVector.Y =
                            playersCardsVectors[0][players[2].Cards.Count - 1].Y + 50;
                    break;
                case CardDrawing.RIGHT:
                    tookCard.SpriteTexture = tableCards[tableCards.Count - 1].SpriteTexture;
                    tookCard.spriteRectangle = tableCards[tableCards.Count - 1].spriteRectangle;
                    tookCard.spriteVector.X = deckSprite.spriteVector.X;
                    tookCard.spriteVector.Y =
                            playersCardsVectors[0][players[2].Cards.Count - 1].Y + 50;
                    break;
            }*/
        }
    }
}