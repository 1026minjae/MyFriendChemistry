using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace MyFriendChemistry
{
    /// <summary>
    /// ScoreBoard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ScoreBoard : Window
    {
        //필요한 배열 및 변수를 선언하는 부분. 많이 쓰이는 것만 담았다.
        private string[] UserAnswer = null;
        private Test.QMC[] QMC = null;
        private Test.QSQ[] QSQ = null;
        private int count;

        public ScoreBoard(Test.QMC[] qmc, Test.QSQ[] qsq, string[] useranswer)
        {
            InitializeComponent();
            UserAnswer = useranswer;//인수로 받아온 사용자의 답안 배열을 저장한다.
            QMC = qmc;//인수로 받아온 객관식 배열을 저장한다
            QSQ = qsq;//인수로 받아온 주관식 배열을 저장한다
            MCscore();//객관식 채점
            textBlock.Text = qsq[0].answer;
            textBlock2.Text = UserAnswer[20];//서술형이 21번부터니까 21번째 답을 불러온다.
            count = Convert.ToInt16(Readini("WrongQSQ", "Number"));//현재 오답목록에 몇개의 주관식 문제가 저장되어있는지 확인한다. 그 이유는 오답목록에 문제를 이어서 저장하기 위해서이다. (오답목록에 10개의 문제가 저장되어았었다고 치면 새로 저장하는 문제는 11번부터 저장된다)
        }

        string iniLocation = AppDomain.CurrentDomain.BaseDirectory + "TextFile.ini";
        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filepath);
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        private string Readini(string Section, string Key)//ini파일 읽는 함수
        {
            StringBuilder temp = new StringBuilder(32);
            int i = GetPrivateProfileString(Section, Key, "", temp, 32, iniLocation);
            return temp.ToString();
        }

        private void MCscore()//객관식 채점 함수
        {
            int score = 0;
            int j = Convert.ToInt16(Readini("WrongQMC", "Number"));//현재 오답목록에 몇개의 객관식 문제가 저장되어있는지 확인한다. 그 이유는 오답목록에 문제를 이어서 저장하기 위해서이다. (오답목록에 10개의 문제가 저장되어았었다고 치면 새로 저장하는 문제는 11번부터 저장된다)
            for (int i=0; i<20; i++)
            {
                if ((UserAnswer[i] != null) && (QMC[i].answer == UserAnswer[i])) score += 4; //문제를 맞혔을 때 점수를 준다.
                else if ((UserAnswer[i] == null) || (QMC[i].answer != UserAnswer[i]))//문제를 틀렸을 때 오답목록에 문제를 저장한다.
                {
                    j++;
                    WritePrivateProfileString("WrongQMC", "Q" + j.ToString(), QMC[i].question, iniLocation);
                    WritePrivateProfileString("WrongQMC", "A" + j.ToString() + "_1", QMC[i].a1, iniLocation);
                    WritePrivateProfileString("WrongQMC", "A" + j.ToString() + "_2", QMC[i].a2, iniLocation);
                    WritePrivateProfileString("WrongQMC", "A" + j.ToString() + "_3", QMC[i].a3, iniLocation);
                    WritePrivateProfileString("WrongQMC", "A" + j.ToString() + "_4", QMC[i].a4, iniLocation);
                    WritePrivateProfileString("WrongQMC", "A" + j.ToString() + "_5", QMC[i].a5, iniLocation);
                    WritePrivateProfileString("WrongQMC", "Answer" + j.ToString(), QMC[i].answer, iniLocation);
                }
            }
            mcscore.Content = score.ToString();
            if(j != 0) WritePrivateProfileString("WrongQMC", "Number", j.ToString(), iniLocation);
        }

        private void Close_Click(object sender, RoutedEventArgs e)//종료버튼를 눌렀을 때 호출되는 함수
        {
            this.Close();
        }     

        private void button_Click(object sender, RoutedEventArgs e)//오답버튼 눌렀을 때
        {
            //다음 문제를 세팅하며 오답목록에 문제를 저장한다.
            if(number.Text=="1/5")
            {
                textBlock.Text = QSQ[1].answer;
                textBlock2.Text = UserAnswer[21];
                number.Text = "2/5";
                count++;
                WritePrivateProfileString("WrongQSQ", "Q" + count.ToString(), QSQ[0].question, iniLocation);
                WritePrivateProfileString("WrongQSQ", "Answer" + count.ToString(), QSQ[count - 1].question, iniLocation);
            }
            else if (number.Text == "2/5")
            {
                textBlock.Text = QSQ[2].answer;
                textBlock2.Text = UserAnswer[22];
                number.Text = "3/5";
                count++;
                WritePrivateProfileString("WrongQSQ", "Q" + count.ToString(), QSQ[1].question, iniLocation);
                WritePrivateProfileString("WrongQSQ", "Answer" + count.ToString(), QSQ[count - 1].question, iniLocation);
            }
            else if (number.Text == "3/5")
            {
                textBlock.Text = QSQ[3].answer;
                textBlock2.Text = UserAnswer[23];
                number.Text = "4/5";
                count++;
                WritePrivateProfileString("WrongQSQ", "Q" + count.ToString(), QSQ[2].question, iniLocation);
                WritePrivateProfileString("WrongQSQ", "Answer" + count.ToString(), QSQ[count - 1].question, iniLocation);
            }
            else if (number.Text == "4/5")
            {
                textBlock.Text = QSQ[4].answer;
                textBlock2.Text = UserAnswer[24];
                number.Text = "5/5";
                count++;
                WritePrivateProfileString("WrongQSQ", "Q" + count.ToString(), QSQ[3].question, iniLocation);
                WritePrivateProfileString("WrongQSQ", "Answer" + count.ToString(), QSQ[count - 1].question, iniLocation);
            }
            else if (number.Text == "5/5")
            {
                count++;
                WritePrivateProfileString("WrongQSQ", "Q" + count.ToString(), QSQ[4].question, iniLocation);
                WritePrivateProfileString("WrongQSQ", "Answer" + count.ToString(), QSQ[4].answer, iniLocation);
                int i, j;
                i = Convert.ToInt32(mcscore.Content);
                j = Convert.ToInt32(sqscore.Content);
                string str = "최종점수 : " + (i + j).ToString();
                textBlock.Text = str;
                number.Text = "채점완료!";
                if (i + j == 100) textBlock2.Text = "모두 정답을 맞히셨습니다! 축하합니다! 지금까지의 자세로 정진하세요!";
                else if (i + j != 100) textBlock2.Text = "안타깝네요. 다음번에는 꼭 만점을 받으시길 바랍니다. 더 노력하세요!";
            }
            WritePrivateProfileString("WrongQSQ", "Number", count.ToString(), iniLocation);
        }

        private void button2_Click(object sender, RoutedEventArgs e)//정답버튼 눌렀을 때
        {
            int i, j;
            //다음 문제를 세팅하며 점수를 더한다.
            if (number.Text == "1/5")
            {
                textBlock.Text = QSQ[1].answer;
                textBlock2.Text = UserAnswer[21];
                i = Convert.ToInt32(sqscore.Content) + 4;
                sqscore.Content = i.ToString();
                number.Text = "2/5";
            }
            else if (number.Text == "2/5")
            {
                textBlock.Text = QSQ[2].answer;
                textBlock2.Text = UserAnswer[22];
                i = Convert.ToInt32(sqscore.Content) + 4;
                sqscore.Content = i.ToString();
                number.Text = "3/5";
            }
            else if (number.Text == "3/5")
            {
                textBlock.Text = QSQ[3].answer;
                textBlock2.Text = UserAnswer[23];
                i = Convert.ToInt32(sqscore.Content) + 4;
                sqscore.Content = i.ToString();
                number.Text = "4/5";
            }
            else if (number.Text == "4/5")
            {
                textBlock.Text = QSQ[4].answer;
                textBlock2.Text = UserAnswer[24];
                i = Convert.ToInt32(sqscore.Content) + 4;
                sqscore.Content = i.ToString();
                number.Text = "5/5";
            }
            else if (number.Text == "5/5")
            {
                i = Convert.ToInt32(sqscore.Content) + 4;
                sqscore.Content = i.ToString();
                j = Convert.ToInt32(mcscore.Content);
                string str = "최종점수 : " + (i + j).ToString();
                textBlock.Text = str;
                number.Text = "채점완료!";
                if (i + j == 100) textBlock2.Text = "모두 정답을 맞히셨습니다! 축하합니다! 지금까지의 자세로 정진하세요!";
                else if (i + j != 100) textBlock2.Text = "안타깝네요. 다음번에는 꼭 만점을 받으시길 바랍니다. 더 노력하세요!";
            }
        }
    }
}
