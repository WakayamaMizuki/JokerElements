public class Card
{
    private int mark;
    private string MarkString;//デバッグ用
    private int num;  //強さ
    private int imageNum; //カードの数字

    private int imageIndex;

    private bool IsPrepare;

    public Card(int mark, int imageNum)
    {
        this.mark = mark;
        this.imageNum = imageNum;
        IsPrepare = false;
        if (mark != 4) imageIndex = mark * 15 + imageNum - 1;
        else imageIndex = 13;
        if (imageNum >= 3)
        {
            this.num = imageNum;
        }
        else
        {
            if (mark != 4)
            {
                this.num = imageNum + 13;
            }
            else
            {
                this.num = 16;
            }
        }
        ChangeMarkString();
    }

    private void ChangeMarkString()
    {
        switch (this.mark)
        {
            case 0:
                this.MarkString = "スペード";
                break;
            case 1:
                this.MarkString = "クラブ";
                break;
            case 2:
                this.MarkString = "ハート";
                break;
            case 3:
                this.MarkString = "ダイヤ";
                break;
            case 4:
                this.MarkString = "ジョーカー";
                break;
        }
    }

    public void setMark(int mark)
    {
        this.mark = mark;
    }

    public void setNum(int num)
    {
        this.num = num;
    }
    public void setIsPrepare(bool IsPrepare)
    {
        this.IsPrepare = IsPrepare;
    }

    public int getMark()
    {
        return mark;
    }

    public int getNum()
    {
        return num;
    }

    public int getImageNum()
    {
        return imageNum;
    }
    public int getImageIndex()
    {
        return imageIndex;
    }

    public bool getIsPrepare()
    {
        return IsPrepare;
    }
}