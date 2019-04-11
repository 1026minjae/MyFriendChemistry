using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace MyFriendChemistry
{
    /// <summary>
    /// FailureScore.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FailureScore : Window
    {
        private string[] UserAnswer = null;
        private Test.QMC[] QMC = null;
        private Test.QSQ[] QSQ = null;
        private int count = 1;
        private int QMCsize;
        private int QSQsize;

        public FailureScore(Test.QMC[] qmc, Test.QSQ[] qsq, string[] useranswer, int qmcsize, int qsqsize)
        {
            InitializeComponent();
            UserAnswer = useranswer;//오답노트 창에서 사용자의 답을 불러온다
            QMC = qmc; 
            QSQ = qsq;
            QMCsize = qmcsize;
            QSQsize = qsqsize;
            MCscore();//객관식 채점
            if (QSQ!=null)
            {
                textBlock.Text = qsq[0].answer;//주관식 1번 문제를 표시한다
                textBlock2.Text = UserAnswer[qmcsize];
                int i = Convert.ToInt16(Readini("WrongQSQ", "Number"));
                for (int j = 0; j < QSQsize; j++)//출제된 문제를 오답목록에서 삭제한다.
                {
                    Delini("WrongQSQ", "Q" + i.ToString());
                    Delini("WrongQSQ", "Answer" + i.ToString());
                    i--;
                }
                number.Text = "1/" + QSQsize.ToString();
                return;
            }
            //만약 주관식이 없다면 바로 채점완료
            float f, l;
            f = 0;
            l = Convert.ToInt32(mcscore.Content);//객관식 점수를 불러온다.
            string str = "최종점수 : " + (f + l).ToString();
            textBlock.Text = str;
            number.Text = "채점완료!";
            if (f + l == 100) textBlock2.Text = "이번에는 모두 정답을 맞히셨습니다! 축하합니다! 지금까지의 자세로 정진하세요!";
            else if (f + l != 100) textBlock2.Text = "안타깝네요. 이미 틀렸던 것을 다시 틀렸군요. 더 노력하세요! 원소주기율표와 화학마당을 꼼꼼히 읽어보는 것을 추천합니다.";
        }

        string iniLocation = AppDomain.CurrentDomain.BaseDirectory + "TextFile.ini";
        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filepath);
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        private string Readini(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(32);
            int i = GetPrivateProfileString(Section, Key, "", temp, 32, iniLocation);
            return temp.ToString();
        }

        private void Delini(string section, string key)
        {
            WritePrivateProfileString(section, key, null, iniLocation);
        }

        private void MCscore()//객관식 채점 함수
        {
            int score = 0;
            int j = Convert.ToInt16(Readini("WrongQMC", "Number"));
            for(int i=0; i<QMCsize; i++)//출제된 객관식 문제를 오답목록에서 삭제한다.
            {
                Delini("WrongQMC", "Q" + j.ToString());
                Delini("WrongQMC", "A" + j.ToString() + "_1");
                Delini("WrongQMC", "A" + j.ToString() + "_2");
                Delini("WrongQMC", "A" + j.ToString() + "_3");
                Delini("WrongQMC", "A" + j.ToString() + "_4");
                Delini("WrongQMC", "A" + j.ToString() + "_5");
                Delini("WrongQMC", "Answer" + j.ToString());
                j--;
            }
            
            for (int i = 0; i < QMCsize; i++)//다시 틀린 문제를 오답목록에 추가한다.
            {
                if ((UserAnswer[i] != null) && (QMC[i].answer == UserAnswer[i])) score += (100/(QMCsize+QSQsize));
                else if ((UserAnswer[i] == null) || (QMC[i].answer != UserAnswer[i]))
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
            WritePrivateProfileString("WrongQMC", "Number", j.ToString(), iniLocation);
        }

        private void Close_Click(object sender, RoutedEventArgs e)//나가기버튼를 눌렀을때 호출됨
        {
            this.Close();
        }

        private int wrong = 0;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (QSQsize == 0) return;
            //오답 버튼을 눌렀으므로 문제를 오답목록에 추가한다.
            WritePrivateProfileString("WrongQSQ", "Q" + count.ToString(), QSQ[count - 1].question, iniLocation);
            WritePrivateProfileString("WrongQSQ", "Answer" + count.ToString(), QSQ[count - 1].answer, iniLocation);
            wrong++;
            if(count==QSQsize)//주관식채점이 끝났다는 뜻
            {
                number.Text = "채점완료!";
                int i, j;
                i = Convert.ToInt32(mcscore.Content);
                j = Convert.ToInt32(sqscore.Content);
                string str = "최종점수 : " + (i + j).ToString();
                textBlock.Text = str;
                if (i + j == 100) textBlock2.Text = "모두 정답을 맞히셨습니다! 축하합니다! 지금까지의 자세로 정진하세요!";
                else if (i + j != 100) textBlock2.Text = "안타깝네요. 다음번에는 꼭 만점을 받으시길 바랍니다. 더 노력하세요!";
                WritePrivateProfileString("WrongQSQ", "Number", wrong.ToString(), iniLocation);//주관식 문제중 틀린 문제가 몇문제인지 ini파일안의 Number에 입력한다.
                return;
            }
            textBlock.Text = QSQ[count].answer;
            textBlock2.Text = UserAnswer[QMCsize + 1];
            number.Text = (count + 1).ToString() +"/"+ QSQsize.ToString();                       
            count++;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (QSQsize == 0) return;
            float i = Convert.ToInt32(sqscore.Content), j;
            
            if (count == QSQsize)//주관식 채점이 끝났다는 뜻
            {
                j = Convert.ToInt32(mcscore.Content);
                string str = "최종점수 : " + (i + j).ToString();
                textBlock.Text = str;
                number.Text = "채점완료!";
                if (i + j == 100) textBlock2.Text = "이번에는 모두 정답을 맞히셨습니다! 축하합니다! 지금까지의 자세로 정진하세요!";
                else if (i + j != 100) textBlock2.Text = "안타깝네요. 이미 틀렸던 것을 다시 틀렸군요. 더 노력하세요! 원소주기율표와 화학마당을 꼼꼼히 읽어보는 것을 추천합니다.";
                WritePrivateProfileString("WrongQSQ", "Number", wrong.ToString(), iniLocation);//주관식 문제중 틀린 문제가 몇문제인지 ini파일안의 Number에 입력한다.
                return;
            }
            i = (100 / (QMCsize + QSQsize));
            sqscore.Content = i.ToString();
            textBlock.Text = QSQ[count-1].answer;
            textBlock2.Text = UserAnswer[QMCsize + 1];
            number.Text = (count + 1).ToString() +"/"+ QSQsize.ToString();                                
            count++;
        }
    }
}
