using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Converter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
{
        private readonly IConfiguration _configuration;
        private readonly GrpcChannel _channel;
        private readonly Services.Converter.ConverterClient _client;
        public MainWindow()
        {
            InitializeComponent();
            _configuration = CreateConfiguration();
            _channel = CreateAuthorizedChannel();
            _client = CreateConverterClient();
        }
        private IConfiguration CreateConfiguration() => new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        private Services.Converter.ConverterClient CreateConverterClient() => new Services.Converter.ConverterClient(_channel);
        private GrpcChannel CreateAuthorizedChannel()
        {
            //Create channel authorized to prevent adding token each calling.
            var accessToken = CreateAccessToken();

            var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (!string.IsNullOrEmpty(accessToken))
                {
                    metadata.Add("Authorization", $"Bearer {accessToken}");
                }
                return Task.CompletedTask;
            });

            var channel = GrpcChannel.ForAddress(_configuration.GetSection("ServerEndpoint").Value, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), callCredentials)
            });
            return channel;
        }
        private string CreateAccessToken()
        {
            //Create simple token locally 
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void Number_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Prevent UI Freezing
            var worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                txtStatus.Text = string.Empty;
                txtResult.Text = e.Result.ToString();
                txtNumber.IsEnabled = true;
                Keyboard.Focus(txtNumber);
            });
        }
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var amount = string.Empty;
            Dispatcher.Invoke(() =>
            {
                txtStatus.Text = "Calculating ...";
                txtNumber.IsEnabled = false;
                amount = txtNumber.Text;
            });
            // Call Service
            var reply = _client.ConvertToWord(new Services.ConvertRequest { Amount = amount });
            e.Result = reply.Description;
        }
    }
}
