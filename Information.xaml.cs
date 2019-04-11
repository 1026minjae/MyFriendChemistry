using System.Windows;
using System.Windows.Input;

namespace MyFriendChemistry
{
    /// <summary>
    /// Information.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Information : Window
    {
        public Information()
        {
            InitializeComponent();

            //탭에 따라 설명을 바꾼다
            MYFRIEND.Text = "내친구 화학이는 화학을 배우고싶은 분들을 위한 '기초화학교육용 프로그램'입니다. 해상도가 '1280x720' 이하이신 분들은 기초화학지식평가와 오답노트의 화면이 잘릴 수 있으니 주의해주시기바랍니다!";
            PERIODIC.Text = "원소주기율표는 총 118가지의 원소들에 대해서 여러가지를 알려주는 곳입니다. 버튼들을 누르면 각 원소들에 대한 설명이 나오고, 원자량, 녹는점, 끓는점등의 성질들도 알 수 있습니다. 오른쪽 위에는 검색창이 있어서 원소의 이름을 치면 원소가 바로 나오기도 합니다.";
            SCHOOL.Text = "화학마당에서는 화학에 대한 지식을 얻을 수 있는 곳입니다. 화학이 무엇인지 설명하는 것부터 시작합니다. 다양한 주제들로 쓰여진 글들을 읽으면서 화학을 열심히 공부해보세요!";
            TEST.Text = "기초화학지식평가에서는 자신의 화학적인 지식을 시험해볼 수 있습니다. 다만 원소주기율표와 화학마당을 전부 다 꼼꼼히 읽어본 다음에 보는 것을 추천합니다. 범위가 전체거든요. 해상도가 '1024x720'이하이신 분들은 화면이 잘릴 수가 있습니다.";
            FAILURE.Text = "오답노트에서는 기초화학지식평가에서 틀렸던 문제들을 다시 한번 풀어볼 수 있습니다. 오답노트에서 푼 문제를 다시 한번 틀렸다고 해서 좌절하지마세요! 다시 풀 수 있습니다. 해상도가 '1024x720'이하이신 분들은 화면이 잘릴 수가 있습니다.";
            COMMUNICATION.Text = "의사소통에서는 제작자에게 메일을 보내서 건의사항을 보낼 수 있습니다. 기초화학지식평가 문제를 내서 보낼 수도 있고, 원소주기율표나 화학마당에 있는 글에 잘못된 점이 있다는 것을 알려줄 수도 있습니다. 프로그램에 문제가 있으면 제작자에게 어서 고쳐달라고 할 수도 있습니다! 많은 이용 바랍니다.";
        }

        private void Information_MouseDown(object sender, MouseEventArgs e)
        {
            //창을 클릭했을 때 드래그를 할 수 있게 한다.
            this.DragMove();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //창 닫기
            this.Close();
        }
    }
}
