using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace MyFriendChemistry
{
    /// <summary>
    /// Test.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Test : Window
    {
        private void Test_MouseDown(object sender, MouseButtonEventArgs e)//드래그를 할 수 있게 한다!
        {
            this.DragMove();
        }

        string iniLocation =AppDomain.CurrentDomain.BaseDirectory + "TextFile.ini";
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(String section, String key, String def, StringBuilder retVal, int size, String filePath);

        private string Readini(String Section, String Key)//ini파일 읽는 함수
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", temp, 500, iniLocation);
            return temp.ToString();
        }

        private class Define
        {
            public const int QMC_Max = 20;//객관식 출제 문제수 (20개)
            public const int QSQ_Max = 5;//주관식 출제 문제수 (5개)
            public static int MC_Max;
            public static int SQ_Max;
        }

        public Test()
        {
            InitializeComponent();
            Define.MC_Max = Convert.ToUInt16(Readini("MultipleChoice", "Number"));//현재 문제목록에 있는 객관식 문제 수
            Define.SQ_Max = Convert.ToUInt16(Readini("Subjective", "Number"));//현재 문제목록에 있는 주관식 문제 수
        }

        public class QMC
        {
            public string question;
            public string a1;
            public string a2;
            public string a3;
            public string a4;
            public string a5;
            public string answer;
        }//객관식 문제 클래스
        public class QSQ
        {
            public string question;
            public string answer;
        }//주관식 문제 클래스
        private QMC[] qmc = new QMC[20];//객관식 문제 배열
        private QSQ[] qsq = new QSQ[5];//주관식 문제 배열
        private string[] choices = new string[25];//사용자가 선택한 답 배열

       
        private QMC[] Make_QMC()//객관식 문제 배열에 객관식 문제를 채운다
        {
            //72~81줄의 코드는 문제 목록에서 어떠한 번호의 문제를 출제할 것인지 난수를 이용하여 결정하는 알고리즘이다.
            List<int> list = new List<int>();
            int[] NumList = new int[Define.QMC_Max];
            Random ran = new Random();
            for (int i = 0; i < Define.MC_Max; i++) list.Add(i+1);
            for (int i = 0; i < Define.QMC_Max; i++)
            {
                int x = ran.Next(0, list.Count());
                NumList[i] = list[x];
                list.RemoveAt(x);
            }
            
            QMC[] qmc = new QMC[20];
            for(int i=0;i<20;i++)
            {
                QMC qm = new QMC();
                qm.question=Readini("MultipleChoice","Q"+NumList[i].ToString());
                qm.a1 = Readini("MultipleChoice", "A" + NumList[i].ToString() + "_1");
                qm.a2 = Readini("MultipleChoice", "A" + NumList[i].ToString() + "_2");
                qm.a3 = Readini("MultipleChoice", "A" + NumList[i].ToString() + "_3");
                qm.a4 = Readini("MultipleChoice", "A" + NumList[i].ToString() + "_4");
                qm.a5 = Readini("MultipleChoice", "A" + NumList[i].ToString() + "_5");
                qm.answer = Readini("MultipleChoice", "Answer" + NumList[i].ToString());
                qmc[i] = qm;
            }
            return qmc;     //만든 객관식 문제 배열을 리턴
        }

        private QSQ[] Make_QSQ()//주관식 문제 배열에 주관식 문제를 채운다.
        {
            //102~111줄의 코드는 문제 목록에서 어떠한 번호의 문제를 출제할 것인지 난수를 이용하여 결정하는 알고리즘이다.
            List<int> list = new List<int>();
            int[] NumList = new int[Define.SQ_Max];
            Random ran = new Random();
            for (int i = 0; i < Define.SQ_Max; i++) list.Add(i+1);
            for(int i=0; i<Define.QSQ_Max; i++)
            {
                int x = ran.Next(0, list.Count());
                NumList[i]=list[x];
                list.RemoveAt(x);
            }

            QSQ[] qsq = new QSQ[5];
            for (int i = 0; i < 5; i++)
            {
                QSQ qs = new QSQ();
                qs.question = Readini("Subjective", "Q" + NumList[i].ToString());
                qs.answer = Readini("Subjective", "Answer" + NumList[i].ToString());
                qsq[i] = qs;
            }
            return qsq;     //만든 주관식 문제 배열을 리턴
        }

        private void Start_Click(object sender, RoutedEventArgs e)//시작버튼 클릭시 호출됨
        {
            //문제지 가리고 있던 오브젝트들을 투명화시키고 첫문제를 세팅
            Start_Page.Visibility = Visibility.Hidden;
            S1.Visibility = Visibility.Hidden;
            S2.Visibility = Visibility.Hidden;
            S3.Visibility = Visibility.Hidden;
            Start.Visibility = Visibility.Hidden;
            Mistake.Visibility = Visibility.Hidden;
            qmc = Make_QMC();//객관식 문제를 준비
            qsq = Make_QSQ();//주관식 문제를 준비

            //첫 문제를 세팅한다.
            Question.Text = qmc[0].question;
            A1.Content = qmc[0].a1;
            A2.Content = qmc[0].a2;
            A3.Content = qmc[0].a3;
            A4.Content = qmc[0].a4;
            A5.Content = qmc[0].a5;
        }

        private void Mistake_Click(object sender, RoutedEventArgs e)//실수로 기초화학지식평가에 들어온 사람들을 위해서 준비한 기능
        {
            this.Close();
        }

        private bool boo = true;

        private void Check(int page)
        {
            boo = false;
            if (choices[page] == "1") A1.IsChecked = true;
            else if (choices[page] == "2") A2.IsChecked = true;
            else if (choices[page] == "3") A3.IsChecked = true;
            else if (choices[page] == "4") A4.IsChecked = true;
            else if (choices[page] == "5") A5.IsChecked = true;
        }//이전에 체크한 답안을 다시 체크하여 보여주는 기능(그러니까 페이지를 앞뒤로 이동하면서 풀 때를 말하는 것이다)

        private void Write(int page)
        {
            boo = false;
            if (choices[page] != "") W.Text = choices[page];          
        }//위 함수 주관식 버전

        private void Previous_Click(object sender, RoutedEventArgs e)//이전 페이지 버튼을 클릭하였을 때 호출됨
        {
            A1.IsChecked = false;
            A2.IsChecked = false;
            A3.IsChecked = false;
            A4.IsChecked = false;
            A5.IsChecked = false;

            //이전 페이지 문제가 무엇인지를 판단하여 세팅한다.
            if (Page.Text == "2/25")
            {
                Page.Text = "1/25";
                Number.Content = "1";
                Question.Text = qmc[0].question;
                A1.Content = qmc[0].a1;
                A2.Content = qmc[0].a2;
                A3.Content = qmc[0].a3;
                A4.Content = qmc[0].a4;
                A5.Content = qmc[0].a5;
                Check(0);
                boo = true;
            }
            else if (Page.Text == "3/25")
            {
                Page.Text = "2/25";
                Number.Content = "2";
                Question.Text = qmc[1].question;
                A1.Content = qmc[1].a1;
                A2.Content = qmc[1].a2;
                A3.Content = qmc[1].a3;
                A4.Content = qmc[1].a4;
                A5.Content = qmc[1].a5;
                Check(1);
                boo = true;
            }
            else if (Page.Text == "4/25")
            {
                Page.Text = "3/25";
                Number.Content = "3";
                Question.Text = qmc[2].question;
                A1.Content = qmc[2].a1;
                A2.Content = qmc[2].a2;
                A3.Content = qmc[2].a3;
                A4.Content = qmc[2].a4;
                A5.Content = qmc[2].a5;
                Check(2);
                boo = true;
            }
            else if (Page.Text == "5/25")
            {
                Page.Text = "4/25";
                Number.Content = "4";
                Question.Text = qmc[3].question;
                A1.Content = qmc[3].a1;
                A2.Content = qmc[3].a2;
                A3.Content = qmc[3].a3;
                A4.Content = qmc[3].a4;
                A5.Content = qmc[3].a5;
                Check(3);
                boo = true;
            }
            else if (Page.Text == "6/25")
            {
                Page.Text = "5/25";
                Number.Content = "5";
                Question.Text = qmc[4].question;
                A1.Content = qmc[4].a1;
                A2.Content = qmc[4].a2;
                A3.Content = qmc[4].a3;
                A4.Content = qmc[4].a4;
                A5.Content = qmc[4].a5;
                Check(4);
                boo = true;
            }
            else if (Page.Text == "7/25")
            {
                Page.Text = "6/25";
                Number.Content = "6";
                Question.Text = qmc[5].question;
                A1.Content = qmc[5].a1;
                A2.Content = qmc[5].a2;
                A3.Content = qmc[5].a3;
                A4.Content = qmc[5].a4;
                A5.Content = qmc[5].a5;
                Check(5);
                boo = true;
            }
            else if (Page.Text == "8/25")
            {
                Page.Text = "7/25";
                Number.Content = "7";
                Question.Text = qmc[6].question;
                A1.Content = qmc[6].a1;
                A2.Content = qmc[6].a2;
                A3.Content = qmc[6].a3;
                A4.Content = qmc[6].a4;
                A5.Content = qmc[6].a5;
                Check(6);
                boo = true;
            }
            else if (Page.Text == "9/25")
            {
                Page.Text = "8/25";
                Number.Content = "8";
                Question.Text = qmc[7].question;
                A1.Content = qmc[7].a1;
                A2.Content = qmc[7].a2;
                A3.Content = qmc[7].a3;
                A4.Content = qmc[7].a4;
                A5.Content = qmc[7].a5;
                Check(7);
                boo = true;
            }
            else if (Page.Text == "10/25")
            {
                Page.Text = "9/25";
                Number.Content = "9";
                Question.Text = qmc[8].question;
                A1.Content = qmc[8].a1;
                A2.Content = qmc[8].a2;
                A3.Content = qmc[8].a3;
                A4.Content = qmc[8].a4;
                A5.Content = qmc[8].a5;
                Check(8);
                boo = true;
            }
            else if (Page.Text == "11/25")
            {
                Page.Text = "10/25";
                Number.Content = "10";
                Question.Text = qmc[9].question;
                A1.Content = qmc[9].a1;
                A2.Content = qmc[9].a2;
                A3.Content = qmc[9].a3;
                A4.Content = qmc[9].a4;
                A5.Content = qmc[9].a5;
                Check(9);
                boo = true;
            }
            else if (Page.Text == "12/25")
            {
                Page.Text = "11/25";
                Number.Content = "11";
                Question.Text = qmc[10].question;
                A1.Content = qmc[10].a1;
                A2.Content = qmc[10].a2;
                A3.Content = qmc[10].a3;
                A4.Content = qmc[10].a4;
                A5.Content = qmc[10].a5;
                Check(10);
                boo = true;
            }
            else if (Page.Text == "13/25")
            {
                Page.Text = "12/25";
                Number.Content = "12";
                Question.Text = qmc[11].question;
                A1.Content = qmc[11].a1;
                A2.Content = qmc[11].a2;
                A3.Content = qmc[11].a3;
                A4.Content = qmc[11].a4;
                A5.Content = qmc[11].a5;
                Check(11);
                boo = true;
            }
            else if (Page.Text == "14/25")
            {
                Page.Text = "13/25";
                Number.Content = "13";
                Question.Text = qmc[12].question;
                A1.Content = qmc[12].a1;
                A2.Content = qmc[12].a2;
                A3.Content = qmc[12].a3;
                A4.Content = qmc[12].a4;
                A5.Content = qmc[12].a5;
                Check(12);
                boo = true;
            }
            else if (Page.Text == "15/25")
            {
                Page.Text = "14/25";
                Number.Content = "14";
                Question.Text = qmc[13].question;
                A1.Content = qmc[13].a1;
                A2.Content = qmc[13].a2;
                A3.Content = qmc[13].a3;
                A4.Content = qmc[13].a4;
                A5.Content = qmc[13].a5;
                Check(13);
                boo = true;
            }
            else if (Page.Text == "16/25")
            {
                Page.Text = "15/25";
                Number.Content = "15";
                Question.Text = qmc[14].question;
                A1.Content = qmc[14].a1;
                A2.Content = qmc[14].a2;
                A3.Content = qmc[14].a3;
                A4.Content = qmc[14].a4;
                A5.Content = qmc[14].a5;
                Check(14);
                boo = true;
            }
            else if (Page.Text == "17/25")
            {
                Page.Text = "16/25";
                Number.Content = "16";
                Question.Text = qmc[15].question;
                A1.Content = qmc[15].a1;
                A2.Content = qmc[15].a2;
                A3.Content = qmc[15].a3;
                A4.Content = qmc[15].a4;
                A5.Content = qmc[15].a5;
                Check(15);
                boo = true;
            }
            else if (Page.Text == "18/25")
            {
                Page.Text = "17/25";
                Number.Content = "17";
                Question.Text = qmc[16].question;
                A1.Content = qmc[16].a1;
                A2.Content = qmc[16].a2;
                A3.Content = qmc[16].a3;
                A4.Content = qmc[16].a4;
                A5.Content = qmc[16].a5;
                Check(16);
                boo = true;
            }
            else if (Page.Text == "19/25")
            {
                Page.Text = "18/25";
                Number.Content = "18";
                Question.Text = qmc[17].question;
                A1.Content = qmc[17].a1;
                A2.Content = qmc[17].a2;
                A3.Content = qmc[17].a3;
                A4.Content = qmc[17].a4;
                A5.Content = qmc[17].a5;
                Check(17);
                boo = true;
            }
            else if (Page.Text == "20/25")
            {
                Page.Text = "19/25";
                Number.Content = "19";
                Question.Text = qmc[18].question;
                A1.Content = qmc[18].a1;
                A2.Content = qmc[18].a2;
                A3.Content = qmc[18].a3;
                A4.Content = qmc[18].a4;
                A5.Content = qmc[18].a5;
                Check(18);
                boo = true;
            }
            else if (Page.Text == "21/25")
            {
                Page.Text = "20/25";
                Number.Content = "20";
                A1.Visibility = Visibility.Visible;
                A2.Visibility = Visibility.Visible;
                A3.Visibility = Visibility.Visible;
                A4.Visibility = Visibility.Visible;
                A5.Visibility = Visibility.Visible;
                W.Visibility = Visibility.Hidden;
                Question.Text = qmc[19].question;
                A1.Content = qmc[19].a1;
                A2.Content = qmc[19].a2;
                A3.Content = qmc[19].a3;
                A4.Content = qmc[19].a4;
                A5.Content = qmc[19].a5;
                Check(19);
                boo = true;
            }
            else if (Page.Text == "22/25")
            {
                Page.Text = "21/25";
                Number.Content = "21";
                Question.Text = qsq[0].question;
                Write(20);
                boo = true;
            }
            else if (Page.Text == "23/25")
            {
                Page.Text = "22/25";
                Number.Content = "22";
                Question.Text = qsq[1].question;
                Write(21);
                boo = true;
            }
            else if (Page.Text == "24/25")
            {
                Page.Text = "23/25";
                Number.Content = "23";
                Question.Text = qsq[2].question;
                Write(22);
                boo = true;
            }
            else if (Page.Text == "25/25")
            {
                Page.Text = "24/25";
                Number.Content = "24";
                Question.Text = qsq[3].question;
                Write(23);
                boo = true;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)//다음 페이지 버튼을 눌렀을 때 호출됨
        {
            A1.IsChecked = false;
            A2.IsChecked = false;
            A3.IsChecked = false;
            A4.IsChecked = false;
            A5.IsChecked = false;
            //다음 페이지의 문제가 어떤 것인지 판단하여 세팅한다.
            if (Page.Text == "1/25")
            {
                Page.Text = "2/25";
                Number.Content = "2";
                Question.Text = qmc[1].question;
                A1.Content = qmc[1].a1;
                A2.Content = qmc[1].a2;
                A3.Content = qmc[1].a3;
                A4.Content = qmc[1].a4;
                A5.Content = qmc[1].a5;
                Check(1);
                boo = true;
            }
            else if (Page.Text == "2/25")
            {
                Page.Text = "3/25";
                Number.Content = "3";
                Question.Text = qmc[2].question;
                A1.Content = qmc[2].a1;
                A2.Content = qmc[2].a2;
                A3.Content = qmc[2].a3;
                A4.Content = qmc[2].a4;
                A5.Content = qmc[2].a5;
                Check(2);
                boo = true;
            }
            else if (Page.Text == "3/25")
            {
                Page.Text = "4/25";
                Number.Content = "4";
                Question.Text = qmc[3].question;
                A1.Content = qmc[3].a1;
                A2.Content = qmc[3].a2;
                A3.Content = qmc[3].a3;
                A4.Content = qmc[3].a4;
                A5.Content = qmc[3].a5;
                Check(3);
                boo = true;
            }
            else if (Page.Text == "4/25")
            {
                Page.Text = "5/25";
                Number.Content = "5";
                Question.Text = qmc[4].question;
                A1.Content = qmc[4].a1;
                A2.Content = qmc[4].a2;
                A3.Content = qmc[4].a3;
                A4.Content = qmc[4].a4;
                A5.Content = qmc[4].a5;
                Check(4);
                boo = true;
            }
            else if (Page.Text == "5/25")
            {
                Page.Text = "6/25";
                Number.Content = "6";
                Question.Text = qmc[5].question;
                A1.Content = qmc[5].a1;
                A2.Content = qmc[5].a2;
                A3.Content = qmc[5].a3;
                A4.Content = qmc[5].a4;
                A5.Content = qmc[5].a5;
                Check(5);
                boo = true;
            }
            else if (Page.Text == "6/25")
            {
                Page.Text = "7/25";
                Number.Content = "7";
                Question.Text = qmc[6].question;
                A1.Content = qmc[6].a1;
                A2.Content = qmc[6].a2;
                A3.Content = qmc[6].a3;
                A4.Content = qmc[6].a4;
                A5.Content = qmc[6].a5;
                Check(6);
                boo = true;
            }
            else if (Page.Text == "7/25")
            {
                Page.Text = "8/25";
                Number.Content = "8";
                Question.Text = qmc[7].question;
                A1.Content = qmc[7].a1;
                A2.Content = qmc[7].a2;
                A3.Content = qmc[7].a3;
                A4.Content = qmc[7].a4;
                A5.Content = qmc[7].a5;
                Check(7);
                boo = true;
            }
            else if (Page.Text == "8/25")
            {
                Page.Text = "9/25";
                Number.Content = "9";
                Question.Text = qmc[8].question;
                A1.Content = qmc[8].a1;
                A2.Content = qmc[8].a2;
                A3.Content = qmc[8].a3;
                A4.Content = qmc[8].a4;
                A5.Content = qmc[8].a5;
                Check(8);
                boo = true;
            }
            else if (Page.Text == "9/25")
            {
                Page.Text = "10/25";
                Number.Content = "10";
                Question.Text = qmc[9].question;
                A1.Content = qmc[9].a1;
                A2.Content = qmc[9].a2;
                A3.Content = qmc[9].a3;
                A4.Content = qmc[9].a4;
                A5.Content = qmc[9].a5;
                Check(9);
                boo = true;
            }
            else if (Page.Text == "10/25")
            {
                Page.Text = "11/25";
                Number.Content = "11";
                Question.Text = qmc[10].question;
                A1.Content = qmc[10].a1;
                A2.Content = qmc[10].a2;
                A3.Content = qmc[10].a3;
                A4.Content = qmc[10].a4;
                A5.Content = qmc[10].a5;
                Check(10);
                boo = true;
            }
            else if (Page.Text == "11/25")
            {
                Page.Text = "12/25";
                Number.Content = "12";
                Question.Text = qmc[11].question;
                A1.Content = qmc[11].a1;
                A2.Content = qmc[11].a2;
                A3.Content = qmc[11].a3;
                A4.Content = qmc[11].a4;
                A5.Content = qmc[11].a5;
                Check(11);
                boo = true;
            }
            else if (Page.Text == "12/25")
            {
                Page.Text = "13/25";
                Number.Content = "13";
                Question.Text = qmc[12].question;
                A1.Content = qmc[12].a1;
                A2.Content = qmc[12].a2;
                A3.Content = qmc[12].a3;
                A4.Content = qmc[12].a4;
                A5.Content = qmc[12].a5;
                Check(12);
                boo = true;
            }
            else if (Page.Text == "13/25")
            {
                Page.Text = "14/25";
                Number.Content = "14";
                Question.Text = qmc[13].question;
                A1.Content = qmc[13].a1;
                A2.Content = qmc[13].a2;
                A3.Content = qmc[13].a3;
                A4.Content = qmc[13].a4;
                A5.Content = qmc[13].a5;
                Check(13);
                boo = true;
            }
            else if (Page.Text == "14/25")
            {
                Page.Text = "15/25";
                Number.Content = "15";
                Question.Text = qmc[14].question;
                A1.Content = qmc[14].a1;
                A2.Content = qmc[14].a2;
                A3.Content = qmc[14].a3;
                A4.Content = qmc[14].a4;
                A5.Content = qmc[14].a5;
                Check(14);
                boo = true;
            }
            else if (Page.Text == "15/25")
            {
                Page.Text = "16/25";
                Number.Content = "16";
                Question.Text = qmc[15].question;
                A1.Content = qmc[15].a1;
                A2.Content = qmc[15].a2;
                A3.Content = qmc[15].a3;
                A4.Content = qmc[15].a4;
                A5.Content = qmc[15].a5;
                Check(15);
                boo = true;
            }
            else if (Page.Text == "16/25")
            {
                Page.Text = "17/25";
                Number.Content = "17";
                Question.Text = qmc[16].question;
                A1.Content = qmc[16].a1;
                A2.Content = qmc[16].a2;
                A3.Content = qmc[16].a3;
                A4.Content = qmc[16].a4;
                A5.Content = qmc[16].a5;
                Check(16);
                boo = true;
            }
            else if (Page.Text == "17/25")
            {
                Page.Text = "18/25";
                Number.Content = "18";
                Question.Text = qmc[17].question;
                A1.Content = qmc[17].a1;
                A2.Content = qmc[17].a2;
                A3.Content = qmc[17].a3;
                A4.Content = qmc[17].a4;
                A5.Content = qmc[17].a5;
                Check(17);
                boo = true;
            }
            else if (Page.Text == "18/25")
            {
                Page.Text = "19/25";
                Number.Content = "19";
                Question.Text = qmc[18].question;
                A1.Content = qmc[18].a1;
                A2.Content = qmc[18].a2;
                A3.Content = qmc[18].a3;
                A4.Content = qmc[18].a4;
                A5.Content = qmc[18].a5;
                Check(18);
                boo = true;
            }
            else if (Page.Text == "19/25")
            {
                Page.Text = "20/25";
                Number.Content = "20";
                Question.Text = qmc[19].question;
                A1.Content = qmc[19].a1;
                A2.Content = qmc[19].a2;
                A3.Content = qmc[19].a3;
                A4.Content = qmc[19].a4;
                A5.Content = qmc[19].a5;
                Check(19);
                boo = true;
            }
            else if (Page.Text == "20/25")
            {
                Page.Text = "21/25";
                Number.Content = "21";
                A1.Visibility = Visibility.Hidden;
                A2.Visibility = Visibility.Hidden;
                A3.Visibility = Visibility.Hidden;
                A4.Visibility = Visibility.Hidden;
                A5.Visibility = Visibility.Hidden;
                W.Visibility = Visibility.Visible;
                Question.Text = qsq[0].question;
                W.Text = "";
                Write(20);
                boo = true;
            }
            else if (Page.Text == "21/25")
            {
                Page.Text = "22/25";
                Number.Content = "22";
                Question.Text = qsq[1].question;
                W.Text = "";
                Write(21);
                boo = true;
            }
            else if (Page.Text == "22/25")
            {
                Page.Text = "23/25";
                Number.Content = "23";
                Question.Text = qsq[2].question;
                W.Text = "";
                Write(22);
                boo = true;
            }
            else if (Page.Text == "23/25")
            {
                Page.Text = "24/25";
                Number.Content = "24";
                Question.Text = qsq[3].question;
                W.Text = "";
                Write(23);
                boo = true;
            }
            else if (Page.Text == "24/25")
            {
                Page.Text = "25/25";
                Number.Content = "25";
                Question.Text = qsq[4].question;
                W.Text = "";
                Write(24);
                boo = true;
            }
        }

        private void Quit_Click(object sender, RoutedEventArgs e)//답안 제출 버튼을 눌렀을 때 호출됨
        {
            Finish_Test.Visibility = Visibility.Visible;
            button.Visibility = Visibility.Visible;
            button2.Visibility = Visibility.Visible;
            textBlock.Visibility = Visibility.Visible;
            textBlock2.Visibility = Visibility.Visible;
        }

        private void button_Click(object sender, RoutedEventArgs e)//한번만 더 살펴보죠
        { 
            Finish_Test.Visibility = Visibility.Hidden;
            button.Visibility = Visibility.Hidden;
            button2.Visibility = Visibility.Hidden;
            textBlock.Visibility = Visibility.Hidden;
            textBlock2.Visibility = Visibility.Hidden;
        }

        private void button2_Click(object sender, RoutedEventArgs e)//더이상 살펴볼게 없군요 제출하겠습니다.
        {
            ScoreBoard score = new ScoreBoard(qmc, qsq, choices);
            score.Show();
            this.Close();           
        }


        //아래 5개 함수는 객관식에서 해당번호를 체크했을 때 문제지 옆쪽에 있는 사용자답안지에 체크를 해주는 역할을 한다.
        private void A1_Checked(object sender, RoutedEventArgs e)
        {
            if(boo==false)
            {
                boo = true;
                return;
            }
            A2.IsChecked = false;
            A3.IsChecked = false;
            A4.IsChecked = false;
            A5.IsChecked = false;
            if (Page.Text == "1/25")
            {
                choices[0] = "1";
                _1R1.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "2/25")
            {
                choices[1] = "1";
                _1R2.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "3/25")
            {
                choices[2] = "1";
                _1R3.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "4/25")
            {
                choices[3] = "1";
                _1R4.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "5/25")
            {
                choices[4] = "1";
                _1R5.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "6/25")
            {
                choices[5] = "1";
                _1R6.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "7/25")
            {
                choices[6] = "1";
                _1R7.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "8/25")
            {
                choices[7] = "1";
                _1R8.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "9/25")
            {
                choices[8] = "1";
                _1R9.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "10/25")
            {
                choices[9] = "1";
                _1R10.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "11/25")
            {
                choices[10] = "1";
                _1R11.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "12/25")
            {
                choices[11] = "1";
                _1R12.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "13/25")
            {
                choices[12] = "1";
                _1R13.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "14/25")
            {
                choices[13] = "1";
                _1R14.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "15/25")
            {
                choices[14] = "1";
                _1R15.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "16/25")
            {
                choices[15] = "1";
                _1R16.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "17/25")
            {
                choices[16] = "1";
                _1R17.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "18/25")
            {
                choices[17] = "1";
                _1R18.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "19/25")
            {
                choices[18] = "1";
                _1R19.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "20/25")
            {
                choices[19] = "1";
                _1R20.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _2R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
        }

        private void A2_Checked(object sender, RoutedEventArgs e)
        {
            if (boo == false)
            {
                boo = true;
                return;
            }
            A1.IsChecked = false;
            A3.IsChecked = false;
            A4.IsChecked = false;
            A5.IsChecked = false;
            if (Page.Text == "1/25")
            {
                choices[0] = "2";
                _1R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R1.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "2/25")
            {
                choices[1] = "2";
                _1R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R2.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "3/25")
            {
                choices[2] = "2";
                _1R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R3.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "4/25")
            {
                choices[3] = "2";
                _1R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R4.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "5/25")
            {
                choices[4] = "2";
                _1R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R5.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "6/25")
            {
                choices[5] = "2";
                _1R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R6.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "7/25")
            {
                choices[6] = "2";
                _1R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R7.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "8/25")
            {
                choices[7] = "2";
                _1R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R8.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "9/25")
            {
                choices[8] = "2";
                _1R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R9.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "10/25")
            {
                choices[9] = "2";
                _1R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R10.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "11/25")
            {
                choices[10] = "2";
                _1R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R11.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "12/25")
            {
                choices[11] = "2";
                _1R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R12.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "13/25")
            {
                choices[12] = "2";
                _1R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R13.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "14/25")
            {
                choices[13] = "2";
                _1R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R14.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "15/25")
            {
                choices[14] = "2";
                _1R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R15.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "16/25")
            {
                choices[15] = "2";
                _1R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R16.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "17/25")
            {
                choices[16] = "2";
                _1R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R17.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "18/25")
            {
                choices[17] = "2";
                _1R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R18.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "19/25")
            {
                choices[18] = "2";
                _1R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R19.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "20/25")
            {
                choices[19] = "2";
                _1R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R20.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _3R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
        }

        private void A3_Checked(object sender, RoutedEventArgs e)
        {
            if (boo == false)
            {
                boo = true;
                return;
            }
            A1.IsChecked = false;
            A2.IsChecked = false;
            A4.IsChecked = false;
            A5.IsChecked = false;
            if (Page.Text == "1/25")
            {
                choices[0] = "3";
                _1R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R1.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "2/25")
            {
                choices[1] = "3";
                _1R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R2.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "3/25")
            {
                choices[2] = "3";
                _1R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R3.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "4/25")
            {
                choices[3] = "3";
                _1R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R4.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "5/25")
            {
                choices[4] = "3";
                _1R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R5.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "6/25")
            {
                choices[5] = "3";
                _1R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R6.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "7/25")
            {
                choices[6] = "3";
                _1R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R7.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "8/25")
            {
                choices[7] = "3";
                _1R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R8.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "9/25")
            {
                choices[8] = "3";
                _1R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R9.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "10/25")
            {
                choices[9] = "3";
                _1R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R10.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "11/25")
            {
                choices[10] = "3";
                _1R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R11.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "12/25")
            {
                choices[11] = "3";
                _1R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R12.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "13/25")
            {
                choices[12] = "3";
                _1R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R13.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "14/25")
            {
                choices[13] = "3";
                _1R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R14.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "15/25")
            {
                choices[14] = "3";
                _1R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R15.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "16/25")
            {
                choices[15] = "3";
                _1R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R16.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "17/25")
            {
                choices[16] = "3";
                _1R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R17.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "18/25")
            {
                choices[17] = "3";
                _1R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R18.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "19/25")
            {
                choices[18] = "3";
                _1R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R19.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "20/25")
            {
                choices[19] = "3";
                _1R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R20.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _4R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
        }

        private void A4_Checked(object sender, RoutedEventArgs e)
        {
            if (boo == false)
            {
                boo = true;
                return;
            }
            A1.IsChecked = false;
            A2.IsChecked = false;
            A3.IsChecked = false;
            A5.IsChecked = false;
            if (Page.Text == "1/25")
            {
                choices[0] = "4";
                _1R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R1.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "2/25")
            {
                choices[1] = "4";
                _1R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R2.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "3/25")
            {
                choices[2] = "4";
                _1R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R3.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "4/25")
            {
                choices[3] = "4";
                _1R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R4.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "5/25")
            {
                choices[4] = "4";
                _1R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R5.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "6/25")
            {
                choices[5] = "4";
                _1R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R6.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "7/25")
            {
                choices[6] = "4";
                _1R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R7.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "8/25")
            {
                choices[7] = "4";
                _1R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R8.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "9/25")
            {
                choices[8] = "4";
                _1R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R9.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "10/25")
            {
                choices[9] = "4";
                _1R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R10.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "11/25")
            {
                choices[10] = "4";
                _1R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R11.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "12/25")
            {
                choices[11] = "4";
                _1R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R12.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "13/25")
            {
                choices[12] = "4";
                _1R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R13.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "14/25")
            {
                choices[13] = "4";
                _1R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R14.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "15/25")
            {
                choices[14] = "4";
                _1R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R15.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "16/25")
            {
                choices[15] = "4";
                _1R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R16.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "17/25")
            {
                choices[16] = "4";
                _1R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R17.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "18/25")
            {
                choices[17] = "4";
                _1R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R18.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "19/25")
            {
                choices[18] = "4";
                _1R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R19.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else if (Page.Text == "20/25")
            {
                choices[19] = "4";
                _1R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R20.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                _5R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
        }

        private void A5_Checked(object sender, RoutedEventArgs e)
        {
            if (boo == false)
            {
                boo = true;
                return;
            }
            A1.IsChecked = false;
            A2.IsChecked = false;
            A3.IsChecked = false;
            A4.IsChecked = false;
            if (Page.Text == "1/25")
            {
                choices[0] = "5";
                _1R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R1.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "2/25")
            {
                choices[1] = "5";
                _1R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R2.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "3/25")
            {
                choices[2] = "5";
                _1R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R3.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "4/25")
            {
                choices[3] = "5";
                _1R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R4.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "5/25")
            {
                choices[4] = "5";
                _1R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R5.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "6/25")
            {
                choices[5] = "5";
                _1R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R6.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "7/25")
            {
                choices[6] = "5";
                _1R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R7.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "8/25")
            {
                choices[7] = "5";
                _1R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R8.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "9/25")
            {
                choices[8] = "5";
                _1R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R9.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "10/25")
            {
                choices[9] = "5";
                _1R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R10.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "11/25")
            {
                choices[10] = "5";
                _1R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R11.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "12/25")
            {
                choices[11] = "5";
                _1R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R12.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "13/25")
            {
                choices[12] = "5";
                _1R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R13.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "14/25")
            {
                choices[13] = "5";
                _1R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R14.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "15/25")
            {
                choices[14] = "5";
                _1R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R15.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "16/25")
            {
                choices[15] = "5";
                _1R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R16.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "17/25")
            {
                choices[16] = "5";
                _1R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R17.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "18/25")
            {
                choices[17] = "5";
                _1R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R18.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "19/25")
            {
                choices[18] = "5";
                _1R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R19.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
            else if (Page.Text == "20/25")
            {
                choices[19] = "5";
                _1R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _2R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _3R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _4R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                _5R20.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
            }
        }

        //서술형답안을 작성하면 옆에 사용자답안지에 옮겨적어준다.
        private void W_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(boo==false)
            {
                boo = true;
                return;
            }
            if (Page.Text == "21/25")
            {
                W1.Content = W.Text;
                choices[20] = W.Text;
            }
            else if (Page.Text == "22/25")
            {
                W2.Content = W.Text;
                choices[21] = W.Text;
            }
            else if (Page.Text == "23/25")
            {
                W3.Content = W.Text;
                choices[22] = W.Text;
            }
            else if (Page.Text == "24/25")
            {
                W4.Content = W.Text;
                choices[23] = W.Text;
            }
            else if (Page.Text == "25/25")
            {
                W5.Content = W.Text;
                choices[24] = W.Text;
            }
        }
    }
}
