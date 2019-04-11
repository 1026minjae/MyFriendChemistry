using System;
using System.Windows;
using System.Net.Mail;

namespace MyFriendChemistry
{
    /// <summary>
    /// Options.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Communication : Window
    {
        public Communication()
        {
            InitializeComponent();
        }

        private void Options_MouseDown(object sender, RoutedEventArgs e)
        {
            //창을 누르면 드래그할 수 있게 된다.
            this.DragMove();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            /*
            보내기 버튼을 눌렀을 때 호출되는 함수
            제목, 내용, 답장을 받을 주소를 메일에 붙여넣어 전송한다.
             */
            Loading.Visibility = Visibility.Visible;
            label1.Visibility = Visibility.Visible;
            image.Visibility = Visibility.Visible;
            if (Text.Text == "")
            {
                this.Close();
            }
            else if (Text.Text != "")
            {
                string text = Text.Text;
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new System.Net.NetworkCredential("myfriendchemistry", "2#myfriendchemistry");
                MailAddress from = new MailAddress("myfriendchemistry@gamil.com", "[MYFRIENDCHEMISTRY!]", System.Text.Encoding.UTF8);
                MailAddress to = new MailAddress("1026minjae@naver.com");
                MailMessage message = new MailMessage(from, to);
                message.Subject = "[MyFriendChemistry]" + Mail_Title.Text;
                message.Body = Text.Text + "\n" + AnswerEmail.Text;
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.BodyEncoding = System.Text.Encoding.UTF8;

                try
                {
                    //메시지 전송
                    client.Send(message);
                    message.Dispose();
                    
                }
                catch (Exception ex)
                {
                    //오류가 발생하였을 때
                    MessageBox.Show(ex.ToString());
                    MessageBox.Show("제가 생각하기에는 메일을 보낼 수 없는 것 같군요! 인터넷이 안 좋을 수도 있으니 참고하세요!");
                }
                this.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //취소 버튼을 눌렀을 때 창을 닫는다.
            this.Close();
        }
    }
}
