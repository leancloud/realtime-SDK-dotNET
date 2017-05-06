using LeanCloud.Realtime.Test.Integration.WPFNetFx45.ViewModel;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace LeanCloud.Realtime.Test.Integration.WPFNetFx45
{
    /// <summary>
    /// Interaction logic for ConversationGroup.xaml
    /// </summary>
    public partial class ConversationGroup : UserControl
    {
        public ConversationGroup()
        {
            InitializeComponent();
        }
        private async void On_UserSelectedDialog_Closing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true)) return;

            var userSelectUserControl = UserSelectBox.Content as UserControl;
            var userSelectVM = userSelectUserControl.DataContext as UserSelectViewModel;
            var chatVM = this.DataContext as ChatViewModel;
            await chatVM.CreateConversationExecuteAsync(userSelectVM);
            userSelectVM.Reset();
        }
    }
}
