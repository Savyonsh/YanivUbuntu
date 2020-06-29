using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace YanivUbuntu
{
    internal class Player
    {
        // Personal Details
        private string playerName;
        public int PlayerNumber { get; }
        public int Score { get; private set; }
        
        // Game Details
        public List<Card> Cards { get; }
        public List<Card> ClubsCards { get; }
        public List<Card> DiamondCards { get; }
        public List<Card> HeartsCards { get; }
        public List<Card> SpadesCards { get; }
        public List<Card> JokerCards { get;}
        public List<Card> PickedCards { get; set; }
        
        public Player(int number, string name)
        { 
            PlayerNumber = number;
            playerName = name;
            Score = 0;
            Cards = new List<Card>();
            ClubsCards = new List<Card>();
            DiamondCards = new List<Card>();
            HeartsCards = new List<Card>();
            SpadesCards = new List<Card>();
            PickedCards = new List<Card>();
            JokerCards = new List<Card>();
        }
        public void SetCards(List<Card> cards)
        {
            foreach (Card card in cards)
            {
                Cards.Add(card);
                card.CardState = CardState.NONE;
                switch(card.CardShape)
                {
                    case Shapes.CLUBS:
                        ClubsCards.Add(card);
                        break;
                    case Shapes.DIAMONDS:
                        DiamondCards.Add(card);
                        break;
                    case Shapes.HEARTS:
                        HeartsCards.Add(card);
                        break;
                    case Shapes.SPADES:
                        SpadesCards.Add(card);
                        break;
                    case Shapes.JOKER:
                        JokerCards.Add(card);
                        break;
                }
            }
        }

        public int CardSum()
        {
            return Card.CardSum(Cards);
        }
        public void PickCard(Card card)
        {
            if (card.Picked)
            {
                card.Picked = false;
                PickedCards.Remove(card);
                PickedCards.Sort();
                card.CardState = CardState.PUT_DOWN;
            }
            else
            {
                card.Picked = true;
                PickedCards.Sort();
                if (card.CardShape == Shapes.JOKER) {
                    if (PickedCards.Count > 0)
                        card.CardValue = PickedCards[PickedCards.Count - 1].CardValue + 1;
                }
                PickedCards.Add(card);
                card.CardState = CardState.LIFT;
            }
        }

        public void PickCards()
        {
            foreach (Card card in PickedCards)
            {
                card.Picked = true;
            }
        }
        public void ResetPlayer()
        {
            Cards.Clear();
            ClubsCards.Clear();
            DiamondCards.Clear();
            HeartsCards.Clear();
            SpadesCards.Clear();
            JokerCards.Clear();
            PickedCards.Clear();
        }

        public List<Card> Play(Card newCard)
        {
            /*/*PickedCards.Sort();
            
            for (var i = 0; i < pickedCardsWnj.Count - 1; i++)
            {
                if (pickedCardsWnj[i].CardValue + 1 == pickedCardsWnj[i + 1].CardValue ||
                    pickedCardsWnj[i].CardValue + JokerCards.Count + 1 == pickedCardsWnj[i + 1].CardValue)
                    series = true;
                if (pickedCardsWnj[i].CardValue == pickedCardsWnj[i + 1].CardValue)
                    equalValue = true;
            }

            Cards.Sort();
            // If an illegal move is been picked, discard it and throw the largest card
            if (series && equalValue || ((!series || PickedCards.Count < 3) && (!equalValue || PickedCards.Count < 2) &&
                                         PickedCards.Count != 1))
            {
                foreach (var pCard in PickedCards)
                {
                    pCard.Picked = false;
                    pCard.CardState = CardState.PUT_DOWN;
                }
                PickedCards.Clear();
                if (Cards.Any(c => c.CardValue != newCard.CardValue))
                    PickedCards.Add(Cards[Cards.Count - 1]);
                

            }#1#
            List<Card> thrownCards;
            if (PickedCardsRule == ThrowingRule.SERIES)
            {
                var pickedCardsWnj = new List<Card>(PickedCards);
                pickedCardsWnj.RemoveAll(c => c.CardShape == Shapes.JOKER);
                // Organize cards in a sorted matter, including finding the 
                // right place for the joker, if it replaces a middle card in the series.
                thrownCards = new List<Card>();
                pickedCardsWnj.Sort();
                for (int i = 0; i < pickedCardsWnj.Count - 1; i++)
                {
                    if (pickedCardsWnj[i].CardValue + 1 == pickedCardsWnj[i + 1].CardValue)
                    {
                        thrownCards.Add(pickedCardsWnj[i]);
                        thrownCards.Add(pickedCardsWnj[i + 1]);
                        i++;
                    } else
                    {
                        thrownCards.Add(pickedCardsWnj[i]);
                        for (var j = 0; j < pickedCardsWnj[i + 1].CardValue - pickedCardsWnj[i].CardValue - 1; j++)
                        {
                            var joker = JokerCards.Find(card => card.Picked);
                            joker.Picked = false;
                            thrownCards.Add(joker);
                        }
                        thrownCards.Add(pickedCardsWnj[i + 1]);
                    }
                }

                // If joker replaces the smallest card or the largest card, 
                // We need to add it.
                if (JokerCards.Exists(card => card.Picked))
                    thrownCards.AddRange(JokerCards.GetRange(JokerCards.FindIndex(card => card.Picked),
                        JokerCards.FindLastIndex(card => card.Picked)));
                
            } else
            {
                thrownCards = new List<Card>(PickedCards);
            }*/
            
            var thrownCards = new List<Card>(PickedCards);
            thrownCards.Sort();
            foreach (var card in thrownCards.Where(card => card.CardShape == Shapes.JOKER))
                card.CardValue = 0;
            
            foreach (var pCard in PickedCards)
            {
                Cards.Remove(pCard);
                ClubsCards.Remove(pCard);
                SpadesCards.Remove(pCard);
                HeartsCards.Remove(pCard);
                DiamondCards.Remove(pCard);
                JokerCards.Remove(pCard);
                pCard.Picked = false;
                
            }
            PickedCards.Clear();
            SetCards(new List<Card>{newCard});
            return thrownCards;
        }

        public void FixList(Card[] cards)
        {
            this.Cards.Clear();
            this.Cards.AddRange(cards);
        }
        
        public void ScorePlayer(bool penalty)
        {
            Score += CardSum();
            if (penalty) Score += 30;
            if (Score == 50) Score = 0; 
            else if (Score % 50 == 0) Score /= 2;
        }
    }
    
    
}
