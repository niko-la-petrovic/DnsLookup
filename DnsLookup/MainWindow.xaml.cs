using DnsLookup.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;

namespace DnsLookup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected UserSettings UserSettings { get; set; } = new();
        protected DnsViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new DnsViewModel();
            DataContext = viewModel;
            Task.Run(async () =>
            {
                UserSettings = await UserSettings.LoadAsync();

                viewModel.SelectedDomainName = UserSettings.DomainNames.LastOrDefault() ?? "";
                viewModel.DomainNames = UserSettings.DomainNames;
            });
        }

        protected void UpdateSuggestions()
        {
            viewModel.DomainNames = UserSettings?.DomainNames;
        }

        private void BtnGetIp_Click(object sender, RoutedEventArgs e)
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
                        UpdateSuggestions();
                    }
                    else
                    {
                        UserSettings.DomainNames.Remove(domainName);
                        UserSettings.DomainNames.Add(domainName);
                    }
                    Task.Run(async () => await UserSettings.SaveAsync());

                    if (UserSettings.CopyToClipboard)
                        Clipboard.SetData(DataFormats.Text, ipAddress);
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(this, ex.Message, "DNS Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtIpAddress.Text = "";
                }
            }
        }

        private void CheckCopyToClipboard_Checked(object sender, RoutedEventArgs e)
        {
            UserSettings.CopyToClipboard = checkCopyToClipboard.IsChecked ?? false;
        }
    }
}
