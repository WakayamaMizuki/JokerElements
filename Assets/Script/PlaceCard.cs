using System.Collections.Generic;

public class PlaceCard
{
    public int cardNum;
    public List<Card> cards;
    public bool stair; //階段か
    public bool multi; //数字が同じカードが複数か
    public int CountJoker;
    public int count7;
    public int count8;
    public int count10;
    public bool[] IsMark = new bool[4];



    public PlaceCard(List<Card> cards)
    {
        this.cards = cards;
        cardNum = cards.Count;
        count7 = FuncCountImageNum(7);
        count8 = FuncCountImageNum(8);
        count10 = FuncCountImageNum(10);
        CountJoker = FuncCountJoker();
        multi = CheckMulti();
        stair = CheckStairs();
        CheckMark();
    }

    private int FuncCountImageNum(int imageNum)
    {
        int count = 0;
        for (int i = 0; i < this.cards.Count; i++)
        {
            if (this.cards[i].getImageNum() == imageNum)
            {
                count++;
            }
        }
        return count;
    }


    private bool CheckMulti()
    {
        if (cards.Count <= 1) return false;
        int num = cards[0].getNum();
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].getNum() != num && cards[i].getMark() != 4) return false;
        }
        return true;
    }

    private int FuncCountJoker()
    {
        int sum = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].getMark() == 4) sum++;
        }
        return sum;
    }

    private bool CheckStairs()
    {
        if (cards.Count < 3) return false;
        for (int i = 0; i < cards.Count - 1; i++)
        {
            if ((cards[i].getNum() == cards[i + 1].getNum() && cards[i].getMark() == cards[i].getMark()) || (cards[i + 1].getMark() == 4))
            {

            }
            else
            {
                return true;
            }
        }
        return true;
    }

    private void CheckMark()
    {
        for (int i = 0; i < IsMark.Length; i++)
        {
            IsMark[i] = false;
        }
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].getMark() != 4) IsMark[cards[i].getMark()] = true;
        }
    }
}