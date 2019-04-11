using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;

namespace MyFriendChemistry
{
    /// <summary>
    /// FailureNote.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FailureNote : Window
    {
        private class Define
        {
            //앞으로 쓸 변수를 정의
            public int QMC_Max;
            public int QSQ_Max;
            public int MC_Max;
            public int SQ_Max;
        }
        private Define defi = new Define();
        private Test.QMC[] qmc = null;
        private Test.QSQ[] qsq = null;
        private string[] choices = null;
        int count = 1;

        public FailureNote()
        {
            InitializeComponent();
            defi.MC_Max = Convert.ToInt16(Readini("WrongQMC", "Number")); //오답목록에 저장된 객관식 개수를 알아옴
            defi.SQ_Max = Convert.ToInt16(Readini("WrongQSQ", "Number")); //오답목록에 저장된 주관식 개수를 알아옴
            if (defi.MC_Max < 20) defi.QMC_Max = defi.MC_Max; //오답목록에 있는 객관식 문제가 20문제보다 적을 경우 그만큼의 객관식을 출제함
            else if (defi.MC_Max >= 20) defi.QMC_Max = 20; //20문제보다 많을 경우 20문제를 출제함
            if (defi.SQ_Max < 5) defi.QSQ_Max = defi.SQ_Max; //오답목록에 있는 주관식 문제가 5문제보다 적을 경우 그만큼의 주관식을 출제함
            else if (defi.SQ_Max >= 5) defi.QSQ_Max = 5; //5문제보다 많을 경우 5문제를 출제함
            S2.Content = "객관식 " + Readini("WrongQMC", "Number") + "개"; //오답목록에 저장된 객관식 개수를 출력
            S3.Content = "주관식 " + Readini("WrongQSQ", "Number") + "개"; //오답목록에 저장된 주관식 개수를 출력
            Page.Text = "1/" + (defi.QMC_Max + defi.QSQ_Max).ToString(); //페이지 번호 출력
            choices = new string[defi.QMC_Max + defi.QSQ_Max]; //사용자의 답을 저장할 배열 선언
        }

        private void FailureNote_MouseDown(object sender, MouseEventArgs e)
        {
            //창을 클릭했을 때 드래그하여 옮길 수 있음
            this.DragMove();
        }

        string iniLocation = AppDomain.CurrentDomain.BaseDirectory + "TextFile.ini"; //TextFile.ini의 위치를 받아옴
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder value, int size, string filePath); 
        //위 두 줄의 코드는 어떤 역할을 하는지 자세히는 모름. ini파일 입출력에 필요하다고만 알고있음.

        public string Readini(string Section, string Key)
        {
            //이름만 보아도 ini파일 읽어오는 함수이다
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", temp, 500, iniLocation);
            return temp.ToString();
        }

        private Test.QMC[] Make_QMC()
        {
            if (defi.QMC_Max == 0) return null;
            Test.QMC[] qmc = new Test.QMC[defi.QMC_Max]; //객관식 문제 배열 선언
            for (int i = 0; i < defi.QMC_Max; i++) //객관식 문제를 오답목록에서 받아와서 배열에 저장
            {
                Test.QMC qm = new Test.QMC();
                qm.question = Readini("WrongQMC", "Q" + (defi.MC_Max-i).ToString());
                qm.a1 = Readini("WrongQMC", "A" + (defi.MC_Max - i).ToString() + "_1");
                qm.a2 = Readini("WrongQMC", "A" + (defi.MC_Max - i).ToString() + "_2");
                qm.a3 = Readini("WrongQMC", "A" + (defi.MC_Max - i).ToString() + "_3");
                qm.a4 = Readini("WrongQMC", "A" + (defi.MC_Max - i).ToString() + "_4");
                qm.a5 = Readini("WrongQMC", "A" + (defi.MC_Max - i).ToString() + "_5");
                qm.answer = Readini("WrongQMC", "Answer" + (defi.MC_Max - i).ToString());
                qmc[i] = qm;
            }
            return qmc; //배열을 리턴
        }

        private Test.QSQ[] Make_QSQ()
        {
            if (defi.QSQ_Max == 0) return null;
            Test.QSQ[] qsq = new Test.QSQ[defi.QSQ_Max]; //주관식 문제 배열 선언
            for (int i = 0; i < defi.QSQ_Max; i++) //주관식 문제를 오답목록에서 받아와서 배열에 저장
            {
                Test.QSQ qs = new Test.QSQ();
                qs.question = Readini("WrongQSQ", "Q" + (defi.SQ_Max - i).ToString());
                qs.answer = Readini("WrongQSQ", "Answer" + (defi.SQ_Max - i).ToString());
                qsq[i] = qs;
            }
            return qsq; //배열을 리턴
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if(defi.MC_Max+defi.SQ_Max==0) //오답목록에 저장된 문제가 없음
            {
                Start.Content = "다시 풀어야할 문제가 없습니다!";
                Mistake.Content = "이 버튼을 눌러 돌아가세요.";
                return;
            }
            //문제지를 덮고있던 잡다한 것들을 다 숨김처리한다
            Start_Page.Visibility = Visibility.Hidden;
            S1.Visibility = Visibility.Hidden;
            S2.Visibility = Visibility.Hidden;
            S3.Visibility = Visibility.Hidden;
            Start.Visibility = Visibility.Hidden;
            Mistake.Visibility = Visibility.Hidden;
            qmc = Make_QMC(); //객관식 문제 만들기
            qsq = Make_QSQ(); //주관식 문제 만들기
            if (defi.QMC_Max != 0) //객관식 1번 문제를 세팅
            {
                Question.Text = qmc[0].question;
                A1.Content = qmc[0].a1;
                A2.Content = qmc[0].a2;
                A3.Content = qmc[0].a3;
                A4.Content = qmc[0].a4;
                A5.Content = qmc[0].a5;
                return;
            }
            //객관식이 없다면? 주관식 1번 문제 세팅
            A1.Visibility = Visibility.Hidden;
            A2.Visibility = Visibility.Hidden;
            A3.Visibility = Visibility.Hidden;
            A4.Visibility = Visibility.Hidden;
            A5.Visibility = Visibility.Hidden;
            W.Visibility = Visibility.Visible;
            Question.Text = qsq[0].question;
            return;
        }

        private void Mistake_Click(object sender, RoutedEventArgs e)
        {
            //시작 버튼 밑에 잘못눌렀어요 버튼을 눌렀을 때 창을 닫는다.
            this.Close();
        }

        private bool boo = true; // 

        private void Check(int page) //객관식문제에서 전 문제로 다시 돌아갈 때 골랐던 답 다시 보여주기
        {
            boo = false;
            if (choices[page] == "1") A1.IsChecked = true;
            else if (choices[page] == "2") A2.IsChecked = true;
            else if (choices[page] == "3") A3.IsChecked = true;
            else if (choices[page] == "4") A4.IsChecked = true;
            else if (choices[page] == "5") A5.IsChecked = true;
        }

        private void Write(int page) //주관식문제에서 전 문제로 다시 돌아갈 때 썼던 답 다시 보여주기
        {
            boo = false;
            if (choices[page] != "") W.Text = choices[page];
        }

        private void Previous_Click(object sender, RoutedEventArgs e) //이전 버튼 눌렀을 때 이전 문제로 세팅해주며ㅕㅛㅅ
        {
            if (count != 1) count--;
            A1.IsChecked = false;
            A2.IsChecked = false;
            A3.IsChecked = false;
            A4.IsChecked = false;
            A5.IsChecked = false;
            Page.Text = count.ToString() + "/" + (defi.QMC_Max + defi.QSQ_Max).ToString();
            Number.Content = count.ToString();

            //전페이지가 객관식인지 주관식인지 알아낸 뒤 경우에 따라 다르게 동작
            if (count < defi.QMC_Max)
            {
                Question.Text = qmc[count - 1].question;
                A1.Content = qmc[count - 1].a1;
                A2.Content = qmc[count - 1].a2;
                A3.Content = qmc[count - 1].a3;
                A4.Content = qmc[count - 1].a4;
                A5.Content = qmc[count - 1].a5;
                Check(count - 1);
            }
            else if (count == defi.QMC_Max)
            {
                A1.Visibility = Visibility.Visible;
                A2.Visibility = Visibility.Visible;
                A3.Visibility = Visibility.Visible;
                A4.Visibility = Visibility.Visible;
                A5.Visibility = Visibility.Visible;
                W.Visibility = Visibility.Hidden;
                Question.Text = qmc[count - 1].question;
                A1.Content = qmc[count - 1].a1;
                A2.Content = qmc[count - 1].a2;
                A3.Content = qmc[count - 1].a3;
                A4.Content = qmc[count - 1].a4;
                A5.Content = qmc[count - 1].a5;
                Check(count - 1);
            }
            else if (count > defi.QMC_Max) 
            {
                Question.Text = qsq[count - defi.QMC_Max - 1].question;
                Write(count - 1);
            }
            boo = true;
        }

        private void Next_Click(object sender, RoutedEventArgs e) //다음 버튼을 눌렀을 때 다음 문제를 세팅
        {
            if (count == defi.QMC_Max + defi.QSQ_Max) return;
            count++;
            A1.IsChecked = false;
            A2.IsChecked = false;
            A3.IsChecked = false;
            A4.IsChecked = false;
            A5.IsChecked = false;
            Page.Text = count.ToString() + "/" + (defi.QMC_Max + defi.QSQ_Max).ToString();
            Number.Content = count.ToString();

            //다음 문제가 객관식인지 주관식인지 판단하여 경우에 따라 다르게 동작
            if (count <= defi.QMC_Max) 
            {
                Question.Text = qmc[count - 1].question;
                A1.Content = qmc[count - 1].a1;
                A2.Content = qmc[count - 1].a2;
                A3.Content = qmc[count - 1].a3;
                A4.Content = qmc[count - 1].a4;
                A5.Content = qmc[count - 1].a5;
                Check(count - 1);
            }
            else if (count == defi.QMC_Max + 1)
            {
                A1.Visibility = Visibility.Hidden;
                A2.Visibility = Visibility.Hidden;
                A3.Visibility = Visibility.Hidden;
                A4.Visibility = Visibility.Hidden;
                A5.Visibility = Visibility.Hidden;
                W.Visibility = Visibility.Visible;
                Question.Text = qsq[count - defi.QMC_Max - 1].question;
                Write(defi.QMC_Max);
            }
            else if (count > defi.QMC_Max + 1) 
            {
                Question.Text = qsq[count - defi.QMC_Max - 1].question;
                Write(count-1);
            }
            boo = true;
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            //제출 버튼을 눌렀을 때
            Finish_Test.Visibility = Visibility.Visible;
            button1.Visibility = Visibility.Visible;
            button2.Visibility = Visibility.Visible;
            textBlock.Visibility = Visibility.Visible;
            textBlock2.Visibility = Visibility.Visible;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //한번더 살펴보겠습니다 눌렀을 때
            Finish_Test.Visibility = Visibility.Hidden;
            button1.Visibility = Visibility.Hidden;
            button2.Visibility = Visibility.Hidden;
            textBlock.Visibility = Visibility.Hidden;
            textBlock2.Visibility = Visibility.Hidden;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //더이상 살펴볼게 없습니다 눌렀을 때
            //채점을 실시한다
            FailureScore score = new FailureScore(qmc, qsq, choices, defi.QMC_Max, defi.QSQ_Max);
            score.Show();//주관식 채점창 띄우기
            this.Close();//오답노트 닫기
        }

        private void A1_Checked(object sender, RoutedEventArgs e) //보기1번이 체크됨
        {
            if (boo == false)
            {
                boo = true;
                return;
            }
            A2.IsChecked = false;
            A3.IsChecked = false;
            A4.IsChecked = false;
            A5.IsChecked = false;

            choices[count - 1] = "1";
            switch (count)
            {
                case 1:
                    _1R1.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 2:
                    _1R2.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 3:
                    _1R3.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 4:
                    _1R4.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 5:
                    _1R5.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 6:
                    _1R6.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 7:
                    _1R7.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 8:
                    _1R8.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 9:
                    _1R9.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 10:
                    _1R10.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 11:
                    _1R11.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 12:
                    _1R12.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 13:
                    _1R13.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 14:
                    _1R14.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 15:
                    _1R15.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 16:
                    _1R16.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 17:
                    _1R17.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 18:
                    _1R18.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 19:
                    _1R19.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 20:
                    _1R20.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _2R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
            }
        }

        private void A2_Checked(object sender, RoutedEventArgs e) //보기 2번이 체크됨
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

            choices[count - 1] = "2";
            switch (count)
            {
                case 1:
                    _1R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R1.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 2:
                    _1R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R2.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 3:
                    _1R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R3.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 4:
                    _1R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R4.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 5:
                    _1R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R5.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 6:
                    _1R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R6.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 7:
                    _1R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R7.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 8:
                    _1R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R8.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 9:
                    _1R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R9.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 10:
                    _1R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R10.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 11:
                    _1R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R11.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 12:
                    _1R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R12.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 13:
                    _1R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R13.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 14:
                    _1R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R14.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 15:
                    _1R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R15.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 16:
                    _1R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R16.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 17:
                    _1R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R17.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 18:
                    _1R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R18.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 19:
                    _1R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R19.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 20:
                    _1R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R20.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _3R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
            }
        }

        private void A3_Checked(object sender, RoutedEventArgs e) //보기 3번이 체크됨
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

            choices[count - 1] = "3";
            switch (count)
            {
                case 1:
                    _1R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R1.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 2:
                    _1R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R2.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 3:
                    _1R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R3.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 4:
                    _1R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R4.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 5:
                    _1R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R5.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 6:
                    _1R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R6.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 7:
                    _1R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R7.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 8:
                    _1R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R8.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 9:
                    _1R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R9.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 10:
                    _1R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R10.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 11:
                    _1R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R11.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 12:
                    _1R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R12.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 13:
                    _1R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R13.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 14:
                    _1R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R14.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 15:
                    _1R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R15.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 16:
                    _1R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R16.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 17:
                    _1R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R17.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 18:
                    _1R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R18.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 19:
                    _1R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R19.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 20:
                    _1R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R20.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _4R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
            }
        }

        private void A4_Checked(object sender, RoutedEventArgs e) //보기 4번이 체크됨
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

            choices[count - 1] = "4";
            switch (count)
            {
                case 1:
                    _1R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R1.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 2:
                    _1R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R2.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 3:
                    _1R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R3.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 4:
                    _1R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R4.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 5:
                    _1R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R5.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 6:
                    _1R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R6.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 7:
                    _1R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R7.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 8:
                    _1R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R8.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 9:
                    _1R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R9.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 10:
                    _1R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R10.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 11:
                    _1R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R11.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 12:
                    _1R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R12.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 13:
                    _1R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R13.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 14:
                    _1R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R14.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 15:
                    _1R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R15.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 16:
                    _1R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R16.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 17:
                    _1R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R17.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 18:
                    _1R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R18.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 19:
                    _1R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R19.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
                case 20:
                    _1R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R20.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    _5R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    break;
            }
        }

        private void A5_Checked(object sender, RoutedEventArgs e) //보기 5번이 체크됨
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

            choices[count - 1] = "5";
            switch (count)
            {
                case 1:
                    _1R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R1.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R1.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 2:
                    _1R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R2.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R2.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 3:
                    _1R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R3.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R3.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 4:
                    _1R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R4.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R4.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 5:
                    _1R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R5.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R5.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 6:
                    _1R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R6.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R6.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 7:
                    _1R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R7.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R7.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 8:
                    _1R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R8.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R8.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 9:
                    _1R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R9.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R9.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 10:
                    _1R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R10.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R10.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 11:
                    _1R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R11.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R11.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 12:
                    _1R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R12.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R12.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 13:
                    _1R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R13.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R13.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 14:
                    _1R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R14.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R14.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 15:
                    _1R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R15.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R15.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 16:
                    _1R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R16.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R16.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 17:
                    _1R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R17.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R17.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 18:
                    _1R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R18.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R18.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 19:
                    _1R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R19.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R19.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
                case 20:
                    _1R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _2R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _3R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _4R20.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                    _5R20.Fill = new SolidColorBrush(Color.FromRgb(0xBC, 0xE8, 0x89));
                    break;
            }
        }

        private void W_TextChanged(object sender, TextChangedEventArgs e) //서술형 답이 입력됨
        {
            if (boo == false)
            {
                boo = true;
                return;
            }

            choices[count-1] = W.Text;
            if (count == defi.QMC_Max + 1) W1.Content = W.Text;
            else if (count == defi.QMC_Max + 2) W2.Content = W.Text;
            else if (count == defi.QMC_Max + 3) W3.Content = W.Text;
            else if (count == defi.QMC_Max + 4) W4.Content = W.Text;
            else if (count == defi.QMC_Max + 5) W5.Content = W.Text;

        }
    }
}
