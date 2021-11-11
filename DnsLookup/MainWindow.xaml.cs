using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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

namespace DnsLookup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string selectedDomainName = "a";

        public event PropertyChangedEventHandler PropertyChanged;

        protected UserSettings UserSettings { get; set; } = new();
        protected IEnumerable<string> DomainNames => UserSettings?.DomainNames;
        
        // TODO move to viewmodel
        public string SelectedDomainName
        {
            get => selectedDomainName; set
            {
                selectedDomainName = value;
                OnPropertyChanged(nameof(SelectedDomainName));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainWindow()
        {
            InitializeComponent();
            Task.Run(async () =>
            {
                UserSettings = await UserSettings.LoadAsync();
            });
        }

        private void btnGetIp_Click(object sender, RoutedEventArgs e)
        {
            string domainName = txtDomainName.Text;
            if (!string.IsNullOrWhiteSpace(domainName))
            {
                try
                {
                    var hostEntry = Dns.GetHostEntry(domainName);
                    if (hostEntry is null || !hostEntry.AddressList.Any())
                        return;

                    var ipAddress = hostEntry.AddressList[0].MapToIPv4().ToString();
                    txtIpAddress.Text = ipAddress;

                    if (!UserSettings.DomainNames.Contains(domainName))
                    {
                        UserSettings.DomainNames.Add(domainName);
                        Task.Run(async () => await UserSettings.SaveAsync());
                    }

                    if (UserSettings.CopyToClipboard)
                        Clipboard.SetData(DataFormats.Text, ipAddress);
                }
                catch (Exception)
                {
                }
            }
        }

        private void checkCopyToClipboard_Checked(object sender, RoutedEventArgs e)
        {
            UserSettings.CopyToClipboard = checkCopyToClipboard.IsChecked ?? false;
        }
    }
}
