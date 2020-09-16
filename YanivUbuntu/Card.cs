using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YanivUbuntu
{

    public enum Shapes { CLUBS, DIAMONDS, HEARTS, SPADES, JOKER }

    internal class Card  : Sprite ,IComparable<Card> 
    {
        public Card(Shapes shape, int value, Texture2D texture)
        {
            CardShape = shape;
            CardValue = value;
            SpriteTexture = texture;
            SetRectangle();
            Picked = false;
            CardState = CardState.NONE;
        }
        
        public Card(Card other)
        {
            CardShape = other.CardShape;
            CardValue = other.CardValue;
            SpriteTexture = other.SpriteTexture;
            Picked = other.Picked;
            spriteVector = new Vector2(other.spriteVector.X, other.spriteVector.Y);
            SetRectangle();
        }
        public CardState CardState { get; set; }
        public int CardValue { get; set; }
        public Shapes CardShape { get; set; }
        public bool Picked { get; set; }

        private void SetRectangle()
        {
            spriteRectangle = new Rectangle(79 * CardValue, 123 * (int) CardShape, 79, 123);
        } 
        public int CompareTo(Card other)
        {
            return CardValue - other.CardValue;
        }

        public static int CardSum(List<Card> listOfCards)
        {
            var sum = 0;
            foreach (var card in listOfCards.Where(card => card.CardShape != Shapes.JOKER))
            {
                if (card.CardValue % 13 < 11) sum += (card.CardValue % 13 + 1);
                else sum += 10;
            }
            return sum;
        }

        /*public static ThrowingRule Sort(List<Card> cards)
        {
            switch (cards.Count)
            {
                case 0:
                    return ThrowingRule.NONE;
                case 1:
                    return ThrowingRule.SINGLE;
            }

            var nonJokerCards = new List<Card>(cards);
            nonJokerCards.RemoveAll(c => c.CardShape == Shapes.JOKER);
            cards.RemoveAll(card => card.CardShape != Shapes.JOKER);
            var jokerCards = new List<Card>(cards);
            var throwingRule = ThrowingRule.NONE;

            // Organize cards in a sorted matter, including finding the 
            // right place for the joker, if it replaces a middle card in the series.
            var sortedCards = new List<Card>();
            nonJokerCards.Sort();
            for (var i = 0; i < nonJokerCards.Count - 1; i++)
            {
                if (nonJokerCards[i].CardValue == nonJokerCards[i + 1].CardValue)
                    if (throwingRule != ThrowingRule.NONE)
                        return ThrowingRule.NONE; 
                    else throwingRule = ThrowingRule.SAME_VALUE;
                else if (nonJokerCards[i].CardValue + 1 == nonJokerCards[i + 1].CardValue)
                {
                    if (throwingRule != ThrowingRule.NONE) return ThrowingRule.NONE;
                    if(!sortedCards.Contains(nonJokerCards[i]))
                        sortedCards.Add(nonJokerCards[i]);
                    sortedCards.Add(nonJokerCards[i + 1]);
                    throwingRule = ThrowingRule.SERIES;
                } else
                {
                    if (jokerCards.Count == 0)
                        return ThrowingRule.NONE;
                    throwingRule = ThrowingRule.SERIES;
                    sortedCards.Add(nonJokerCards[i]);
                    for (var j = 0; j < nonJokerCards[i + 1].CardValue - nonJokerCards[i].CardValue - 1; j++)
                    {
                        var joker = jokerCards.Find(card => card.Picked);
                        joker.Picked = false;
                        sortedCards.Add(joker);
                    }
                    sortedCards.Add(nonJokerCards[i + 1]);
                }
            }
            // If joker replaces the smallest card or the largest card, 
            // We need to add it.
            foreach (var jokerCard in jokerCards.Where(jokerCard => !sortedCards.Contains(jokerCard)))
            {
                sortedCards.Add(jokerCard);
            }

            cards = sortedCards;
            return throwingRule;
        }*/

        public override string ToString()
        {
            return "    " +(CardValue + 1) + " " + CardShape;
        }
    }
    
}
