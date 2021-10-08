using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace GmailSend
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        //メールアドレスのリスト
        public static ArrayList members = new ArrayList();
        //送信先リストの更新
        public void load()
        {
            members.Clear();
            System.IO.StreamReader sr2 = new System.IO.StreamReader(@"members.data", Encoding.GetEncoding("Shift_JIS"));
            string str2 = sr2.ReadToEnd();
            sr2.Close();
            string[] str3 = str2.Split(',');
            for (int i = 0; i < str3.Length - 1; i++)
            {
                str3[i].Replace("\r\n", "");
                members.Add(str3[i]);
            }
            checkedListBox1.Items.Clear();
            for (int g = 0; g < members.Count; g++)
            {
                checkedListBox1.Items.Add(members[g]);
            }
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
        }
        //Main
        public Form1()
        {
            InitializeComponent();
            //以前使ったパスワードの復元
            if (System.IO.File.Exists(@"pass.data"))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(@"pass.data", Encoding.GetEncoding("Shift_JIS"));
                string str = sr.ReadToEnd();
                sr.Close();
                pass.Text = str;
            }
            if (System.IO.File.Exists(@"members.data"))
            {
                load();
            }
            else
            {
                Encoding enc = Encoding.GetEncoding("Shift_JIS");
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"members.data", false, enc);
                writer.Write("");
                writer.Close();
            }
        }
        //送信ボタン
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (address.Text == "" || pass.Text == "" || toaddres.Text == "" || titlebox.Text == "" || contents.Text == "")
                {
                    MessageBox.Show("内容が不足しています。", "警告");
                }
                else
                {
                    if (checkedListBox1.CheckedIndices.Count == 0)
                    {
                        MessageBox.Show("送信する人がいません。\r\nチェックリストで一人以上チェックが入っていることを確認してください。","警告");
                    }
                    for (int i = 0; i < checkedListBox1.CheckedIndices.Count; i++)
                    {
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.Credentials = new System.Net.NetworkCredential(address.Text, pass.Text);
                        smtp.EnableSsl = true;
                        MailMessage msg = new MailMessage(address.Text, checkedListBox1.CheckedItems[i].ToString(), titlebox.Text, contents.Text);
                        smtp.Send(msg);
                        Encoding enc = Encoding.GetEncoding("Shift_JIS");
                        System.IO.StreamWriter writer = new System.IO.StreamWriter(@"pass.data", false, enc);
                        writer.WriteLine(pass.Text);
                        writer.Close();
                    }
                    MessageBox.Show("全員への送信が完了しました。\r\n計:" + checkedListBox1.CheckedIndices.Count + "人", "報告");

                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "例外的なエラー");
            }
        }
        //詳細情報
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Version:1+α\n開発言語:C#\nEditer:Visual Studio2019\nAuthor:SanaeProject.\n参考サイト:https://qiita.com/R_TES_/items/d746e0e8197ce36a14fe","詳細");
        }
        //help
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://ublog.web.fc2.com/help/?program=GS");
        }
        //送信先リストへ
        private void button2_Click(object sender, EventArgs e)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(@"members.data", Encoding.GetEncoding("Shift_JIS"));
            string str2 = sr.ReadToEnd();
            sr.Close();
            if (-1 != str2.IndexOf(toaddres.Text))
            {
                MessageBox.Show("同じメールアドレスが既に登録されています。", "警告");
            }
            else
            {
                Encoding enc = Encoding.GetEncoding("Shift_JIS");
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"members.data", false, enc);
                writer.Write(str2 + toaddres.Text + ",");
                writer.Close();
                MessageBox.Show(toaddres.Text + "を追加しました。");
                toaddres.Text = "";
                load();
            }
        }
        //送信先リスト
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("送信先リストは\"チェックされている\"メールアドレスに送信します。\r\nそのため送りたくない人にはチェックを外してください。","説明");
        }
        bool cleanmode = false;
        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!cleanmode)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                }
                MessageBox.Show("消したいメールアドレスをチェックしてください。", "操作");
                cleanmode = true;
                linkLabel4.Text = "消去する";
            }
            else
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(@"members.data", Encoding.GetEncoding("Shift_JIS"));
                string str = sr.ReadToEnd();
                sr.Close();
                for (int s = 0; s < checkedListBox1.CheckedIndices.Count; s++)
                {
                    str =str.Replace(checkedListBox1.CheckedItems[s] + ",", "");
                    Encoding enc = Encoding.GetEncoding("Shift_JIS");
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(@"members.data", false, enc);
                    writer.Write(str);
                    writer.Close();
                }
                MessageBox.Show("消去完了しました。", "報告");
                cleanmode = false;
                linkLabel4.Text = "リストから消す";
                load();
            }
        }
        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://support.google.com/accounts/answer/185833?hl=ja");
        }
    }
}
