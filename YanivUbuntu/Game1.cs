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
        // SPRITES
        private SpriteBatch spriteBatch;
        private SpriteFont sumOfCardsFont, scoreTitle;

        private Sprite gameScreen,
            startupScreen,
            deckSprite,
            player1Title,
            player2Title,
            yanivSign,
            youWon,
            callIt,
            tookCard,
            sortButton,
            textBox,
            startButton;
        
        // TEXTURES
        private Texture2D player1TitleBold,
            player2TitleBold,
            assafTexture,
            player1Won,
            player2Won,
            cantTakeCard,
            cardLeftPlayer,
            cardRightPlayer,
            startButtonGlowTexture,
            textLetters;
        
        // LISTS
        private List<Player> players;
        private List<Vector2[]> playersCardsVectors;
        private List<CardDrawing> playersCardDrawings;
        private readonly List<Card> tableCards;
        private List<Sprite> typedName;

        // VECTORS
        private readonly Vector2[] playersScoresVectors;
        private readonly Vector2 rotationVector;
        
        // BOOLEANS
        private bool sort,
            startGame,
            openingCardShufflePlayed,
            startButtonGlow,
            assaf;
        
        // INTEGERS
        private int[] deck;

        private int j1, i1,
            roundNumber = 3,
            randomIndex,
            shape,
            value,
            winner,
            turnCounter,
            textIndex;

        // OTHERS
        private readonly Random random;
        private float currentTime, rotation;
        private MouseState mouseCurrent, mousePrevious;
        private KeyboardState keyboardStateCurrent, keyboardStatePrevious;
        private Card deckCard;
        private SoundEffect cardShuffle, cardBeingThrown;
        
        public Game1() {
            var graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 
                graphics.PreferredBackBufferHeight = 700;
            graphics.ApplyChanges();
            IsMouseVisible = true;
            
            deck = new int[54];
            random = new Random();

            // LISTS
            tableCards = new List<Card>();
            players = new List<Player>();
            playersCardDrawings = new List<CardDrawing>();
            playersCardsVectors = new List<Vector2[]>();
            typedName = new List<Sprite>();
            
            // VECTORS
            rotationVector = new Vector2((float)79 / 2, (float)123 / 2);
            playersScoresVectors = new[] {
                new Vector2(485, 410),
                new Vector2(50, 100),
                new Vector2(605, 100)
            };

            for (var i = 0; i < 3; i++) {
                players.Add(new Player(i, ""));
                playersCardDrawings.Add(CardDrawing.NONE);
            }
            
            for (var i = 0; i < 2; i++) {
                playersCardsVectors.Add(new Vector2[7]);
                for (var k = 0; k < 7; k++)
                    playersCardsVectors[i][k] = new Vector2(30 + i * 530, 165 + 25 * k);
            }
            tookCard = new Sprite(Content.Load<Texture2D>("tookCard"),new Vector2(300, 60));
            tookCard.spriteRectangle.Y = 215;
        }

        private void PlaceTableCard() {
            var tableCard = GenerateDeckCard();
            tableCard.spriteVector = Vector2.UnitX * 310 + Vector2.UnitY * 220;
            tableCards.Add(tableCard);
        }

        private Card GenerateDeckCard() {
            var index = random.Next(0, randomIndex);
            shape = deck[index] / 13;
            value = deck[index] % 13;
            deck[index] = deck[randomIndex];
            var card = new Card((Shapes) shape, value, Content.Load<Texture2D>("card"));
            randomIndex--;
            return card;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // --- STARTUP SCREEN ---
            startupScreen = new Sprite(Content.Load<Texture2D>("startupScreen"), Vector2.Zero);
            textBox = new Sprite(Content.Load<Texture2D>("textBox"),new Vector2(244, 365));
            textLetters = Content.Load<Texture2D>("textLetters");
            startButton = new Sprite(Content.Load<Texture2D>("startButton"), new Vector2(213, 500));
            startButtonGlowTexture = Content.Load<Texture2D>("startButtonGlowing");
            textIndex = (int)textBox.spriteVector.X - 25;
            
            // --- GAME SPRITES, TEXTURES AND SOUND ---
            gameScreen = new Sprite(Content.Load<Texture2D>("gameScreen"), Vector2.Zero);
            
            // Cards
            deckSprite = new Sprite(Content.Load<Texture2D>("deckRedBack"), new Vector2(300, 60));
            deckCard = new Card(0, 0, Content.Load<Texture2D>("card"));
            cantTakeCard = Content.Load<Texture2D>("cantTakeCard");
            cardLeftPlayer = Content.Load<Texture2D>("cardLeftPlayer");
            cardRightPlayer = Content.Load<Texture2D>("cardRightPlayer");
            cardShuffle = Content.Load<SoundEffect>("CardsShuffle");
            cardBeingThrown = Content.Load<SoundEffect>("CardThrown");
            
            // Signs
            sumOfCardsFont = Content.Load<SpriteFont>("amount");
            scoreTitle = Content.Load<SpriteFont>("scoreTitle");
            player1Title = new Sprite(Content.Load<Texture2D>("player1"), new Vector2(10, 60));
            player2Title = new Sprite(Content.Load<Texture2D>("player2"),new Vector2(530, 60));
            player1TitleBold = Content.Load<Texture2D>("player1Turn");
            player2TitleBold = Content.Load<Texture2D>("player2Turn");
            yanivSign = new Sprite(Content.Load<Texture2D>("yaniv"),new Vector2(150, 200));
            assafTexture = Content.Load<Texture2D>("assaf");
            youWon = new Sprite(Content.Load<Texture2D>("youWon"), new Vector2(200, 330));
            player1Won = Content.Load<Texture2D>("player1Won");
            player2Won = Content.Load<Texture2D>("player2Won");
            callIt = new Sprite(Content.Load<Texture2D>("callIt"), new Vector2(140, 390));
            
            // Buttons
            sortButton = new Sprite(Content.Load<Texture2D>("sort"), new Vector2(312, 638)); }

        protected override void Initialize() {             
            // SET STARTING VALUES 
            turnCounter = 0;
            winner = 4;
            assaf = false;
            currentTime = 0f;
            i1 = 0;
            openingCardShufflePlayed = false;
            
            // CARDS & PLAYERS
            randomIndex = 53;
            for (var i = 0; i < 54; i++) deck[i] = i;
            tableCards.Clear();
            PlaceTableCard();
            foreach (var player in players) {
                player.ResetPlayer();
                Deal(player);
            } 
            PlaceCardsOnMat();
            base.Initialize(); 

        /*// DEBUG
        players[1].ResetPlayer();
        players[1].SetCards(new List<Card>() {
            new Card(Shapes.CLUBS, 3, Content.Load<Texture2D>("card")),
            new Card(Shapes.CLUBS, 5, Content.Load<Texture2D>("card")),
            new Card(Shapes.DIAMONDS, 5, Content.Load<Texture2D>("card"))
        });

        players[0].Cards[0].CardValue = 4;
        players[0].Cards[0].CardShape = Shapes.CLUBS;*/
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            
            if (startGame) {
                // gameScreen | Players Titles | Buttons | Scores Sign
                spriteBatch.Draw(gameScreen.SpriteTexture, gameScreen.spriteRectangle, Color.White);
                spriteBatch.Draw(deckSprite.SpriteTexture, deckSprite.spriteVector, deckSprite.spriteRectangle,
                    Color.White);
                spriteBatch.Draw(player1Title.SpriteTexture, player1Title.spriteVector,
                    player1Title.spriteRectangle, Color.White);
                spriteBatch.Draw(player2Title.SpriteTexture, player2Title.spriteVector,
                    player1Title.spriteRectangle, Color.White);
                spriteBatch.Draw(sortButton.SpriteTexture, sortButton.spriteVector,
                    sortButton.spriteRectangle, Color.White);
                foreach (var vector in playersScoresVectors) {
                    spriteBatch.DrawString(scoreTitle, "SCORE",
                        vector, Color.White);
                }

                // Drawing player's name
                var loc = (5 * players[0].PlayerName.Length) / 2;
                spriteBatch.DrawString(scoreTitle,"NAME",
                    playersScoresVectors[0] - Vector2.UnitX * 120 + Vector2.UnitX * loc, Color.White);
                spriteBatch.DrawString(sumOfCardsFont, players[0].PlayerName,
                    playersScoresVectors[0] - Vector2.UnitX * 120 + Vector2.UnitY * 20 , Color.Black);
                
                // Highlighting a player's title on his turn
                switch (turnCounter % 3) {
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
                if (players[0].CardSum() <= 7 && turnCounter % 3 == 0 && winner == 4)
                    spriteBatch.Draw(callIt.SpriteTexture, callIt.spriteVector,
                        callIt.spriteRectangle, Color.White);

                // CARD PRINTING //
                i1 = 0;
                foreach (var tableCard in tableCards) {
                    spriteBatch.Draw(tableCard.SpriteTexture, tableCard.spriteVector, tableCard.spriteRectangle,
                        Color.White);
                    if (i1 != 0 && i1 != tableCards.Count - 1)
                        spriteBatch.Draw(cantTakeCard, tableCard.spriteVector,
                            null, Color.White);
                    i1++;
                }

                // Players cards (including animation)
                for (var i = 0; i < 3; i++) {
                    if (i == 0) {
                        if (playersCardDrawings[i] != CardDrawing.NONE) {
                            spriteBatch.Draw(deckCard.SpriteTexture, tookCard.spriteVector,
                                tookCard.spriteRectangle, Color.White);
                            if (tookCard.spriteVector.Y + 14.5f < 500)
                                tookCard.spriteVector.Y += 14.5f;
                            else {
                                tookCard.spriteVector.Y = 60;
                                playersCardDrawings[i] = CardDrawing.NONE;
                            }

                            for (var j = 0; j < players[i].Cards.Count - 1; j++) 
                                spriteBatch.Draw(players[i].Cards[j].SpriteTexture, players[i].Cards[j].spriteVector,
                                    players[i].Cards[j].spriteRectangle, Color.White);
                        } else {
                            foreach (var card in players[i].Cards) {
                                switch (card.CardState) {
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
                                spriteBatch.Draw(card.SpriteTexture, card.spriteVector,
                                    card.spriteRectangle, Color.White);
                            }
                        }
                    } else {
                        var rightTexture = i == 1 ? cardLeftPlayer : cardRightPlayer;
                        var rightDirection = i == 1 ? -1 : 1;
                        switch (playersCardDrawings[i]) {
                            case CardDrawing.DECK: {
                                if (j1 < 30) {
                                    spriteBatch.Draw(tookCard.SpriteTexture, tookCard.spriteVector,
                                        tookCard.spriteRectangle, Color.White, rotation,
                                        rotationVector, 1.0f, SpriteEffects.None, 1.0f);
                                    rotation += rightDirection * 0.06f;
                                    j1++;
                                } else {
                                    tookCard.spriteVector.Y = playersCardsVectors[i - 1][0].Y;
                                    spriteBatch.Draw(rightTexture, tookCard.spriteVector,
                                        null, Color.White);
                                    if (rightDirection == 1 && tookCard.spriteVector.X < playersCardsVectors[i - 1][0].X || 
                                        rightDirection == -1 && tookCard.spriteVector.X > playersCardsVectors[i - 1][0].X)
                                        tookCard.spriteVector.X += rightDirection * 14.5f;
                                    else {
                                        tookCard.spriteVector.X = deckSprite.spriteVector.X;
                                        playersCardDrawings[i] = CardDrawing.NONE;
                                        rotation = 0;
                                        j1 = 0;
                                    }
                                }
                                for (i1 = players[i].Cards.Count - 1; i1 > 0; i1--)
                                    spriteBatch.Draw(rightTexture, playersCardsVectors[i - 1][i1],
                                        null, Color.White);
                                break;
                            }
                            case CardDrawing.RIGHT:
                            case CardDrawing.LEFT:
                                // Rotation of card
                                if (j1 < 27) {
                                    spriteBatch.Draw(tookCard.SpriteTexture, tookCard.spriteVector,
                                        tookCard.spriteRectangle, Color.White, rotation,
                                        rotationVector, 1.0f, SpriteEffects.None, 1.0f);
                                    rotation += rightDirection * 0.06f;
                                    j1++;
                                } else {
                                    // Arriving to players cards 
                                    if (!(tookCard.spriteVector.X <=
                                          (playersCardsVectors[i - 1][players[i].Cards.Count - 1].X +
                                           rightTexture.Height)) && rightDirection == -1 ||
                                        !(tookCard.spriteVector.X >=
                                          (playersCardsVectors[i - 1][players[i].Cards.Count - 1].X +
                                           rightTexture.Height)) && rightDirection == 1)
                                    {
                                        tookCard.spriteVector.X += rightDirection * 10.5f;
                                        spriteBatch.Draw(tookCard.SpriteTexture, tookCard.spriteVector,
                                            tookCard.spriteRectangle, Color.White, rightDirection * 29.85f,
                                            rotationVector,
                                            1.0f, SpriteEffects.None, 1.0f);
                                    } else {
                                        playersCardDrawings[i] = CardDrawing.NONE;
                                        tookCard.spriteVector.X = deckSprite.spriteVector.X;
                                        j1 = 0;
                                        rotation = 0;
                                    }
                                }
                                // Drawing all of players card excluding the last one -
                                // as the animation arrives there. 
                                for (i1 = players[i].Cards.Count - 2; i1 >= 0; i1--)
                                    spriteBatch.Draw(rightTexture, playersCardsVectors[i - 1][i1],
                                        null, Color.White);
                                
                                break;
                            default: {
                                if(winner < 4) break;
                                // Drawing players cards.
                                for (i1 = players[i].Cards.Count - 1; i1 >= 0; i1--)
                                    spriteBatch.Draw(rightTexture, playersCardsVectors[i - 1][i1],
                                        null, Color.White);

                                // Debug
                                for (i1 = players[i].Cards.Count - 1; i1 >= 0; i1--)
                                    spriteBatch.DrawString(scoreTitle, players[i].Cards[i1].ToString(),
                                        playersCardsVectors[i - 1][i1], Color.White);

                                break;
                            }
                        }
                    }
                }

                if (winner < 4) {
                    if (!assaf) {
                        spriteBatch.Draw(yanivSign.SpriteTexture, yanivSign.spriteVector,
                            yanivSign.spriteRectangle, Color.White);
                        switch (winner) {
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
                    } else {
                        if (currentTime < 2.5f) {
                            spriteBatch.Draw(yanivSign.SpriteTexture, yanivSign.spriteVector,
                                null, Color.White);
                        } else {
                            spriteBatch.Draw(assafTexture, yanivSign.spriteVector,
                                null, Color.White);
                            switch (winner) {
                                case 0:
                                    spriteBatch.Draw(youWon.SpriteTexture, youWon.spriteVector,
                                        null, Color.White);
                                    break;
                                case 1:
                                    spriteBatch.Draw(player1Won, youWon.spriteVector,
                                        null, Color.White);
                                    break;
                                case 2:
                                    spriteBatch.Draw(player2Won, youWon.spriteVector,
                                        null, Color.White);
                                    break;
                            }
                        }

                    }

                    if (currentTime > 5f) {
                        roundNumber--;
                        if (roundNumber <= 0) {
                            Exit();
                        }
                        spriteBatch.End();
                        base.Draw(gameTime);
                        Initialize();
                        return;
                    }

                    // Displaying players card after a yaniv has been called
                    for (var j = 0; j < players[1].Cards.Count; j++)
                        spriteBatch.Draw(players[1].Cards[j].SpriteTexture,
                            playersCardsVectors[0][j] + (Vector2.UnitY * 35) + (Vector2.UnitX * 45),
                            players[1].Cards[j].spriteRectangle, Color.White, MathHelper.PiOver2,
                            rotationVector, 1, SpriteEffects.None, 0);

                    if (players.Count > 2) {
                        for (var j = players[2].Cards.Count - 1; j >= 0; j--)
                            spriteBatch.Draw(players[2].Cards[j].SpriteTexture,
                                playersCardsVectors[1][j] + (Vector2.UnitY * 35) + (Vector2.UnitX * 55),
                                players[2].Cards[j].spriteRectangle, Color.White, MathHelper.PiOver2,
                                rotationVector, 1, SpriteEffects.None, 0);
                    }
                }
            } else {
                // Displaying setting for game gameScreen
                spriteBatch.Draw(startupScreen.SpriteTexture, Vector2.Zero, null, Color.White);
                spriteBatch.Draw(textBox.SpriteTexture, textBox.spriteVector, null, Color.White);
                spriteBatch.Draw(startButtonGlow ? startButtonGlowTexture : startButton.SpriteTexture,
                    startButton.spriteVector, null, Color.White);
                foreach (var letter in typedName) {
                    spriteBatch.Draw(letter.SpriteTexture, letter.spriteVector, letter.spriteRectangle, Color.White);
                }
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime) {
            currentTime += (float) gameTime.ElapsedGameTime.TotalSeconds;
            mousePrevious = mouseCurrent;
            mouseCurrent = Mouse.GetState();
            keyboardStatePrevious = keyboardStateCurrent;
            keyboardStateCurrent = Keyboard.GetState();
            if (startGame) {
                if (players[0].PlayerName.Equals(string.Empty))
                    players[0].PlayerName = new string(typedName
                        .Select(spriteLetter => (char) ((spriteLetter.spriteRectangle.X / 40) + 65)).ToArray());

                // Play the card shuffle sound at the start of the game
                if (!openingCardShufflePlayed) {
                    cardShuffle.Play();
                    openingCardShufflePlayed = true;
                }

                // If there is a winner for this game, Start a new one or close game
                if (winner < 4) {
                    base.Update(gameTime);
                    return;
                }

                // Cards mouse hover animation
                foreach (var card in players[0].Cards) {
                    if (card.MouseHovered(mouseCurrent) && card.CardState != CardState.LIFT) {
                        card.CardState = CardState.HOVER_UP;
                    } else if (mouseCurrent != mousePrevious && card.CardState == CardState.HOVER_UP)
                        card.CardState = CardState.HOVER_DOWN;
                }

                // Player's turn
                if (turnCounter % 3 == 0) {
                    if (sortButton.MouseTouched(mouseCurrent, mousePrevious)) {
                        sort = true;
                        PlaceCardsOnMat();
                    }

                    if (players[0].CardSum() <= 7 &&
                        callIt.MouseTouched(mouseCurrent, mousePrevious)) {
                        CheckWhoIsTheWinner(players[0]);
                        base.Update(gameTime);
                        return;
                    }

                    // Clicking and picking cards to throw
                    foreach (var c in players[0].Cards.Where(c => c.MouseTouched(mouseCurrent, mousePrevious)))
                        players[0].PickCard(c);

                    // Clicking on one of the cards on the table
                    if (tableCards[0].MouseTouched(mouseCurrent, mousePrevious) ||
                        tableCards[tableCards.Count - 1].MouseTouched(mouseCurrent, mousePrevious) ||
                        deckSprite.MouseTouched(mouseCurrent, mousePrevious)) {
                        if (!CheckIfLegal(players[0].PickedCards)) {
                            base.Update(gameTime);
                            return;
                        }

                        var index = tableCards[0].MouseTouched(mouseCurrent, mousePrevious) &&
                                    !tableCards[tableCards.Count - 1].MouseTouched(mouseCurrent, mousePrevious)
                            ? 0
                            : tableCards[0].MouseTouched(mouseCurrent, mousePrevious) &&
                              tableCards[tableCards.Count - 1].MouseTouched(mouseCurrent, mousePrevious)
                                ? tableCards.Count - 1
                                : !tableCards[0].MouseTouched(mouseCurrent, mousePrevious) &&
                                  tableCards[tableCards.Count - 1].MouseTouched(mouseCurrent, mousePrevious)
                                    ? tableCards.Count - 1
                                    : -1;

                        var cardToTake = index == -1 ? GenerateDeckCard() : tableCards[index];
                        tookCard.spriteRectangle = cardToTake.spriteRectangle;
                        playersCardDrawings[0] = index == -1 ? CardDrawing.DECK :
                            index == 0 ? CardDrawing.LEFT : CardDrawing.RIGHT;
                        cardBeingThrown.CreateInstance().Play();
                        ReplaceTableCards(players[0].Play(new Card(cardToTake)));
                        PlaceCardsOnMat();
                        turnCounter++;
                        currentTime = 0;

                    }

                    // Computer's turn
                } else {
                    if (currentTime > 3.5f) {
                        ComputerStrategy(players[turnCounter % 3]);
                        turnCounter++;
                        currentTime = 0f;
                    }
                }
            } else {
                startButtonGlow = startButton.MouseHovered(mouseCurrent);
                startGame = startButton.MouseTouched(mouseCurrent, mousePrevious);
                for (var i = 0; i < 27; i++) {
                    var key = (Keys) (i + 65);
                    if (keyboardStatePrevious.IsKeyDown(key) && keyboardStateCurrent.IsKeyUp(key) &&
                        textIndex < textBox.spriteRectangle.Width + textBox.spriteVector.X - 80)
                        typedName.Add(new Sprite(textLetters, new Vector2(textIndex += 40, textBox.spriteVector.Y + 12),
                            new Rectangle(i * 40, 0, 40, 45)));
                }

                if (keyboardStatePrevious.IsKeyDown(Keys.Back) && keyboardStateCurrent.IsKeyUp(Keys.Back) &&
                    typedName.Count > 0) {
                    typedName.RemoveAt(typedName.Count - 1);
                    textIndex -= 40;
                }
            }

            base.Update(gameTime);
        }

        private bool CheckIfLegal(List<Card> cards) {
            if (cards.Count == 0) return false;
            if (cards.Count == 1) return true;
            
            var series = true;
            var sameValue = true;

            /*foreach (var card in cards)
            {
                if (shape == null && card.CardShape != Shapes.JOKER)
                    shape = card.CardShape;
                if (shape == card.CardShape || card.CardShape == Shapes.JOKER) continue;
                series = false;
                break;

            }
            if(series)*/
            series = CheckSeries(cards).Count >= 3;

            for (var i = 1; i < cards.Count - 1 && sameValue; i++)
                if (cards[i].CardValue != cards[i + 1].CardValue)
                    sameValue = false;

            return sameValue ^ series;
        }

        private void ReplaceTableCards(List<Card> cards) {
            if (cards.Count == 0) return;
            cards.Sort();

            var jokers = cards.FindAll(card => card.CardShape == Shapes.JOKER);
            foreach (var joker in jokers)
                joker.CardValue = joker.spriteRectangle.X / 79;

            var middle = cards.Count % 2 == 0 ? cards.Count / 2 : (cards.Count - 1) / 2;
            cards[middle].spriteVector.X = 310;
            cards[middle].spriteVector.Y = 220;

            for (var i = 1; i <= middle; i++) {
                cards[middle - i].spriteVector =
                    cards[middle].spriteVector - Vector2.UnitX * 30 * i;
                if (middle + i >= cards.Count) continue;
                cards[middle + i].spriteVector =
                    cards[middle].spriteVector + Vector2.UnitX * 30 * i;
            }

            tableCards.Clear();
            tableCards.AddRange(cards);
        }
        
        private void ComputerStrategy(Player player) {
            
            if (player.CardSum() <= 7) {
                CheckWhoIsTheWinner(player);
                return;
            }
            
            Card cardToTake = null;
            var seriesWithoutTableCardSum = 0;
            var seriesWithTableCardSum = 0;

            var doNotThrow = new List<Card>();
            var doThrow = new List<Card>();
            var optionalThrow = new List<Card>();

            player.Cards.Sort();
            player.Cards.Reverse();
            
            //    Throwing cards without relation to the table cards
            //---------------------------------------------------------
            
            // Check if we can throw a series
            foreach (var shapeCards in 
                player.OrganizedCards.Where(shapeCard => shapeCard.Count > 1)) {
                var series = new List<Card>(shapeCards);
                series.AddRange(player.JokerCards);
                series = CheckSeries(series);
                var currentSum = Card.CardSum(series);
                if (currentSum > seriesWithoutTableCardSum) seriesWithoutTableCardSum = currentSum;
                else continue;
                /*doThrow.Clear();
                doThrow.AddRange(series);*/
                doThrow = series;
            }
            
            // Check double cards to throw
            var sameCardValue = -1;
            var sameCardSum = -1;
            for (var i = 0; i < player.Cards.Count - 1; i++) {
                if (player.Cards[i].CompareTo(player.Cards[i + 1]) != 0 ||
                    player.Cards[i].CardShape == Shapes.JOKER ||
                    player.Cards[i + 1].CardShape == Shapes.JOKER) continue;

                if (sameCardValue != -1 && sameCardValue != player.Cards[i].CardValue) break;

                if (sameCardValue == -1)
                    sameCardSum = sameCardValue = player.Cards[i].CardValue;
                
                
                // If the player can throw a series too, check with cards 
                // he should throw based on the value. 
                sameCardSum += player.Cards[i].CardValue;

                if (seriesWithoutTableCardSum >= sameCardSum)
                    continue;
                
                if (!optionalThrow.Contains(player.Cards[i]))
                    optionalThrow.Add(player.Cards[i]);
                optionalThrow.Add(player.Cards[i + 1]);
            }
            
            //    Check if there is a throw on the next round based on the table cards
            //--------------------------------------------------------------------------
            var allowedToTake = new List<Card>() {tableCards[0]};
            if (tableCards.Count > 1)
                allowedToTake.Add(tableCards[tableCards.Count - 1]);

            // Check if one of the table cards complete a series
            foreach (var card in allowedToTake) {

                if (card.CardShape == Shapes.JOKER) {
                    cardToTake = card;
                    break;
                }
                
                var shapeCards = card.CardShape switch {
                    Shapes.CLUBS => new List<Card>(player.ClubsCards),
                    Shapes.HEARTS => new List<Card>(player.HeartsCards),
                    Shapes.SPADES => new List<Card>(player.SpadesCards),
                    _ => new List<Card>(player.DiamondCards)
                };

                shapeCards.AddRange(player.JokerCards);
                shapeCards.Add(card);
                shapeCards = CheckSeries(shapeCards);
                seriesWithTableCardSum = Card.CardSum(shapeCards);
                if (!shapeCards.Contains(card) || seriesWithTableCardSum <= seriesWithoutTableCardSum) continue;
                // Throwing series next turn with the new card
                /*doNotThrow.Clear();
                doNotThrow.AddRange(shapeCards);*/
                cardToTake = card;
                doNotThrow = shapeCards;
                /*// Check if there is a series after removing cards that
                // are going to be thrown at the next round
                doThrow.RemoveAll(tCard => shapeCards.Contains(tCard));
                doThrow = CheckSeries(doThrow);
                // Check if there are cards in the optional throwing
                optionalThrow.RemoveAll(tCard => shapeCards.Contains(tCard));*/
            }
            
            // Check if one of the table cards exists in players cards.
            //if (cardToTake == null) {
                foreach (var tableCard in allowedToTake) {
                    var allAppearances = player.Cards.FindAll(card => card.CompareTo(tableCard) == 0);
                    /*if (allAppearances.Count * tableCard.CardValue > seriesWithoutTableCardSum) {
                        // Check if there is a series after removing cards that
                        // are going to be thrown at the next round
                        doThrow.RemoveAll(card => allAppearances.Contains(card));
                        doThrow = CheckSeries(doThrow);
                        // Remove the optionalThrow if it contains the cards that 
                        // are going to be thrown at the next round
                        cardToTake = tableCard;
                    } 
                    // Check if the player should keep the card for series or the same-value cards
                    if (optionalThrow.Exists(card => card.CompareTo(tableCard) == 0) &&
                        allAppearances.Count * tableCard.CardValue > seriesWithTableCardSum) {
                        doNotThrow.Clear();
                        doNotThrow.AddRange(allAppearances);
                        optionalThrow.Clear();
                        cardToTake = tableCard;
                    }*/
                    if (allAppearances.Count * tableCard.CardValue > seriesWithoutTableCardSum ||
                        (optionalThrow.Exists(card => card.CompareTo(tableCard) == 0) &&
                         allAppearances.Count * tableCard.CardValue > seriesWithTableCardSum)) {
                        doNotThrow = allAppearances;
                        cardToTake = tableCard;
                    }
                }
           // }
            
            // If the cards on the table don't fit a double throw or a series completion
            // check if one of the table cards has a small value
            if (cardToTake == null) 
                foreach (var tableCard in allowedToTake.Where(tableCard => tableCard.CardValue <= 2).Where(tableCard =>
                    cardToTake == null || cardToTake.CardValue > tableCard.CardValue)) 
                    cardToTake = tableCard;

            // Take from deck
            if (cardToTake == null) {
                playersCardDrawings[player.PlayerNumber] = CardDrawing.DECK;
                cardToTake = GenerateDeckCard();
            }

            // Fix options after all the changes.
            foreach (var card in doNotThrow) {
                doThrow.Remove(card);
                optionalThrow.Remove(card);
            }
            doThrow = CheckSeries(doThrow);

            
            // If no cards got picked, pick the biggest card
            var theEmptyOption = doThrow.Count == 0 ? ref doThrow : ref optionalThrow;
            if(theEmptyOption.Count == 0)
                theEmptyOption.Add(player.Cards.Find(card => 
                    !tableCards.Exists(c => c.CompareTo(card) == 0) &&
                                                             !doNotThrow.Contains(card)));
            
            // if there are two options, Choose the better one, or if the plans changed.
            if (Card.CardSum(doThrow) < Card.CardSum(optionalThrow))
                doThrow = optionalThrow;
            
            player.PickCards(doThrow);
            if(playersCardDrawings[player.PlayerNumber] != CardDrawing.DECK) 
                playersCardDrawings[player.PlayerNumber] = cardToTake == tableCards[0] ? CardDrawing.LEFT : CardDrawing.RIGHT;
            GetTableCardLocationForAnimation();
            ReplaceTableCards(player.Play(new Card(cardToTake)));
            cardBeingThrown.CreateInstance().Play();
        }

        private void CheckWhoIsTheWinner(Player player) {
            var minSum = int.MaxValue;
            var winnerNumber = 0;
            foreach (var p in players.Where(p => minSum > p.CardSum())) {
                minSum = p.CardSum();
                winnerNumber = p.PlayerNumber;
            }

            if (minSum < player.CardSum()) {
                assaf = true;
            }

            winner = winnerNumber;
            foreach (var p in players) {
                p.ScorePlayer(p == player && assaf, p == player);
            }
        }

        private List<Card> CheckSeries(List<Card> cards) {
            if (cards.Count == 0) return new List<Card>();
            var series = new List<Card>();
            var jokers = new List<Card>(cards);
            jokers.RemoveAll(c => c.CardShape != Shapes.JOKER);
            var pickedCardsWnj = new List<Card>(cards);
            pickedCardsWnj.RemoveAll(c => c.CardShape == Shapes.JOKER);
            
            pickedCardsWnj.Sort();
            pickedCardsWnj.Reverse();
            
            // Check if all of the card has the same shape
            if (pickedCardsWnj.Count > 0 &&
                !pickedCardsWnj.TrueForAll(card => card.CardShape == pickedCardsWnj[0].CardShape)) return series;
            
            var lastSeriesValue = -1;
            for (var i = 0; i < pickedCardsWnj.Count - 1; i++) {
                if (pickedCardsWnj[i].CardValue - 1 == pickedCardsWnj[i + 1].CardValue && 
                    (lastSeriesValue == -1 || lastSeriesValue == pickedCardsWnj[i].CardValue)) {
                    if (lastSeriesValue != pickedCardsWnj[i].CardValue)
                        series.Add(pickedCardsWnj[i]);
                    series.Add(pickedCardsWnj[i + 1]);
                    lastSeriesValue = pickedCardsWnj[i + 1].CardValue;
                } else if (pickedCardsWnj[i].CardValue - jokers.Count - 1 == pickedCardsWnj[i + 1].CardValue 
                           && (lastSeriesValue == -1 || lastSeriesValue == pickedCardsWnj[i].CardValue)) {
                    if (lastSeriesValue != pickedCardsWnj[i].CardValue)
                        series.Add(pickedCardsWnj[i]);
                    lastSeriesValue = pickedCardsWnj[i].CardValue;
                    for (var j = 0;
                        j < pickedCardsWnj[i].CardValue - pickedCardsWnj[i + 1].CardValue && jokers.Count > 0;
                        j++) {
                        var joker = jokers[0];
                        joker.CardValue = --lastSeriesValue;
                        series.Add(joker);
                        jokers.RemoveAt(0);
                    }
                    series.Add(pickedCardsWnj[i + 1]);
                }
            }

            if (series.Count == 2 && jokers.Count > 0) {
                var joker = jokers[0];
                joker.CardValue = series[1].CardValue + 1 < 13 ? series[0].CardValue + 1 : series[1].CardValue - 1; 
                    series.Add(joker);
                jokers.RemoveAt(0);
            }
            return series.Count >= 3 ? series : new List<Card>();
        }

        private void Deal(Player player) {
            var playersDeck = new List<Card>();
            for (var i = 0; i < 7; i++) 
                playersDeck.Add(GenerateDeckCard());
            player.SetCards(playersDeck);
        }

        private void PlaceCardsOnMat() {
            var cards = players[0].Cards;
            
            // Fix jokers value
            var jokers = cards.FindAll(card => card.CardShape == Shapes.JOKER);
            foreach (var joker in jokers)
                joker.CardValue = joker.spriteRectangle.X / 79;

            int middle, addition = 0;
            
            if (sort) 
                cards.Sort();
            
            if (cards.Count % 2 == 1)
                middle = (cards.Count - 1) / 2;
            else {
                middle = cards.Count / 2;
                addition = 40;
            }

            cards[middle].spriteVector.X = 320 + addition;
            cards[middle].spriteVector.Y = 500;

            tookCard.spriteVector.X = cards[middle].spriteVector.X;

            for (var i = 1; i <= middle; i++) {
                cards[middle - i].spriteVector.X = 320 - (85 * i) + addition;
                cards[middle - i].spriteVector.Y = 500;
                if (middle + i >= cards.Count) continue;
                cards[middle + i].spriteVector.X = 320 + (85 * i) + addition;
                cards[middle + i].spriteVector.Y = 500;
            }

            if (!sort) {
                var temp = cards[middle].spriteVector;
                cards[middle].spriteVector = cards[cards.Count - 1].spriteVector;
                cards[cards.Count - 1].spriteVector = temp;
            } 
            sort = false;
        }

        private void GetTableCardLocationForAnimation() {
            for (var i = 0; i < players.Count; i++) {
                if (i == 0) {
                    switch (playersCardDrawings[i]) {
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
                } else {
                    foreach (var cardsVector in playersCardsVectors) {
                        var tableCardIndex = playersCardDrawings[i] == CardDrawing.LEFT ? 0 : tableCards.Count - 1;
                        switch (playersCardDrawings[i]) {
                            case CardDrawing.DECK:
                                tookCard.SpriteTexture = Content.Load<Texture2D>("tookCard");
                                tookCard.spriteRectangle = new Rectangle(0, 0, 79, 111);
                                tookCard.spriteVector.X = deckSprite.spriteVector.X;
                                tookCard.spriteVector.Y = cardsVector[i - 1].Y + 50;
                                break;
                            case CardDrawing.LEFT:
                            case CardDrawing.RIGHT:
                                tookCard.SpriteTexture = tableCards[tableCardIndex].SpriteTexture;
                                tookCard.spriteRectangle = tableCards[tableCardIndex].spriteRectangle;
                                tookCard.spriteVector.Y = cardsVector[players[i].Cards.Count - 1].Y + 25;
                                tookCard.spriteVector.X = deckSprite.spriteVector.X;
                                break;
                            case CardDrawing.NONE:
                                break;
                        }
                    }
                }
            }
        }
    }
}