using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo
{
    public partial class ListViewEx : System.Windows.Forms.ListView
    {
        public ListViewEx()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        private Color mProgressColor = Color.Red;
        public Color ProgressColor
        {
            get
            {
                return this.mProgressColor;
            }
            set
            {
                this.mProgressColor = value;
            }
        }
        private Color mProgressTextColor = Color.Black;
        public Color ProgressTextColor
        {
            get
            {
                return mProgressTextColor;
            }
            set
            {
                mProgressTextColor = value;
            }
        }
        public int ProgressColumIndex
        {
            set
            {
                progressIndex = value;
            }
            get
            {
                return progressIndex;
            }
        }
        int progressIndex = -1;
        /// <summary>
        /// 检查是否可以转化为一个浮点数
        /// </summary>
        const string numberstring = "0123456789.";
        private bool CheckIsFloat(String s)
        {
            foreach (char c in s)
            {
                if (numberstring.IndexOf(c) > -1)
                {
                    continue;
                }
                else
                    return false;
            }
            return true;
        }
 
        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            base.OnDrawColumnHeader(e);
        }
        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            if (e.ColumnIndex != this.progressIndex)
            {
                e.DrawDefault = true;
                base.OnDrawSubItem(e);
            }
            else
            {
                if (CheckIsFloat(e.Item.SubItems[e.ColumnIndex].Text))//判断当前subitem文本是否可以转为浮点数
                {
                    float per = float.Parse(e.Item.SubItems[e.ColumnIndex].Text);
                    if (per >= 1.0f)
                    {
                        per = per / 100.0f;
                    }
                    Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                    DrawProgress(rect, per, e.Graphics);
                }
            }
        }
        ///绘制进度条列的subitem
        private void DrawProgress(Rectangle rect, float percent, Graphics g)
        {
            if (rect.Height > 2 && rect.Width > 2)
            {
                //if ((rect.Top > 0 && rect.Top < this.Height) &&(rect.Left > this.Left && rect.Left < this.Width))
                {
                    //绘制进度
                    int width = (int)(rect.Width * percent);
                    Rectangle newRect = new Rectangle(rect.Left + 1, rect.Top + 1, width - 2, rect.Height - 2);
                    using (Brush tmpb = new SolidBrush(this.mProgressColor))
                    {
                        g.FillRectangle(tmpb, newRect);
                    }
                    newRect = new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 2, rect.Height - 2);
                    g.DrawRectangle(Pens.RoyalBlue, newRect);
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    newRect = new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 2, rect.Height - 2);
                    using (Brush b = new SolidBrush(mProgressTextColor))
                    {
                        g.DrawString(percent.ToString("p1"), this.Font, b, newRect, sf);
                    }
                }
            }
            else
            {
                return;
            }
        }
    }
}
