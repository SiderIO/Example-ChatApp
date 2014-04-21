using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Sider.IO.Client.DataProvider.Net;
using Sider.IO.Client.DataProvider.Net.Commands;

namespace Blabbling.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Connection connection;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Publish publishCommand = new Publish(channelName: ChannelName.Text, publicationId: Username.Text, message: DraftMessage.Text);
            this.connection.Execute(publishCommand);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            this.connection = new Connection("localhost", 11000);
            this.connection.OnReceivedPubSubMessage += connection_OnReceivedPubSubMessage;
            this.connection.Open();

            //Publish to make sure the channel exists
            Publish publishCommand = new Publish(channelName: ChannelName.Text, publicationId: Username.Text, message: String.Format("{0} has entered channel {1}", Username.Text, ChannelName.Text));
            Sider.IO.Api.Dto.Message.SiderResponse response = connection.Execute(publishCommand);

            //Subscribe
            Subscribe subscribeCommand = new Subscribe(channelName: ChannelName.Text, subscriptionId: Username.Text);
            response = connection.Execute(subscribeCommand);
            Conversation.Text = Conversation.Text + String.Format("Logging into channel {0} status: {1} ", ChannelName.Text, response.ReturnCode);
        }

        public void connection_OnReceivedPubSubMessage(string channelName, string publicationId, string message)
        {
            Application.Current.Dispatcher.BeginInvoke(new ThreadStart(() => HandleReceivedPubSubMessage(channelName, publicationId, message)), null);
        }

        public void HandleReceivedPubSubMessage(string channelName, string publicationId, string message)
        {
            Conversation.Text = Conversation.Text + publicationId + "@" + DateTime.Now.ToShortTimeString() + ":" + message;
            Conversation.ScrollToEnd();
        }

        private void Unsubscribe_Click(object sender, RoutedEventArgs e)
        {
            //Unsubscribe unsubscribeCommand = new Unsubscribe(channelName: ChannelName.Text, subscriptionId: Username.Text);
            //Sider.IO.Api.Dto.Message.SiderResponse response = connection.Execute(unsubscribeCommand);

            //Conversation.Text = Conversation.Text + DateTime.Now.ToShortTimeString() + "You have unsubscribed from channel " + ChannelName.Text;
        }
    }
}
