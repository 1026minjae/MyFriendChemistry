using System.Windows;
using System.Windows.Input;

namespace MyFriendChemistry
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e) //창을 클릭했을 때 드래그할 수 있게 해준다
        {
            this.DragMove();
        }

        private void ch_board1_Click(object sender, RoutedEventArgs e) //원소주기율표버튼 누름
        {
            AtomBoard atomboard = new AtomBoard();
            atomboard.Show();
        }

        private void ch_board2_Click(object sender, RoutedEventArgs e) //기초화학지식평가버튼 누름
        {
            Test test = new Test();
            test.Show();
        }

        private void ch_board3_Click(object sender, RoutedEventArgs e) //화학마당버튼 누름
        {
            School school = new School();
            school.Show();
        }

        private void ch_board4_Click(object sender, RoutedEventArgs e) //오답노트버튼 누름
        {
            FailureNote failurenote = new FailureNote();
            failurenote.Show();
        }
        private void ch_board5_Click(object sender, RoutedEventArgs e) //의사소통버튼 누름
        {
            Communication options = new Communication();
            options.Show();
        }

        private void Inform_Click(object sender, RoutedEventArgs e) //도움말버튼 누름
        {
            Information inform = new Information();
            inform.Show();
        }

        private void Close_button_Click(object sender, RoutedEventArgs e) //종료버튼 누름
        {
            //종료하시겠습니까 창을 보여준다
            Close.Visibility = Visibility.Visible;
            label.Visibility = Visibility.Visible;
            yes.Visibility = Visibility.Visible;
            no.Visibility = Visibility.Visible;
        }

        private void yes_Click(object sender, RoutedEventArgs e) //종료한다
        {
            Application.Current.Shutdown();
        }

        private void no_Click(object sender, RoutedEventArgs e) //종료하지 않는다
        {
            Close.Visibility = Visibility.Hidden;
            label.Visibility = Visibility.Hidden;
            yes.Visibility = Visibility.Hidden;
            no.Visibility = Visibility.Hidden;
        }
    }
}
