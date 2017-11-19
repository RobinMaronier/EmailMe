using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using MailKit.Net.Imap;
using MailKit;

// Pour plus d'informations sur le modèle d'élément Boîte de dialogue de contenu, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace MailProject
{
    public partial class AddMailAccount : Window
    {
        private Pivot pivot;
        private Button nextButton;
        private Button cancelButton;

        //Step 1
        private TextBox fullNameBox;
        private TextBox emailBox;
        private PasswordBox passwordBox;
        private CheckBox rememberPasswordCheck;
        // Step 3
        private TextBlock titleStep3;
        private TextBox identityConfirm;
        private TextBox serverConfirm;
        private CheckBox SSLConfirm;
        private PasswordBox passwordConfirm;
        private TextBox portConfirm;
        private Button validateConfirmButton;
        private CheckBox rememberPasswordConfirm;

        private Account inCreationAccount;

        private int stepAddAccount = 0;

        public AddMailAccount()
        {
            this.InitializeComponent();
            getAllView();
            pivot.SelectedIndex = 0;
        }

        private void getAllView()
        {
            /*pivot = (Pivot)FindName("flipViewAddAccount");
            nextButton = (Button)FindName("NextButton");
            cancelButton = (Button)FindName("CancelButton");
            //Step 1
            fullNameBox = (TextBox)FindName("nameUserBox");
            emailBox = (TextBox)FindName("emailUserBox");
            passwordBox = (PasswordBox)FindName("passwordUserBox");
            rememberPasswordBox = (CheckBox)FindName("rememberPasswordBox");
            // Step 3
            titleStep3 = (TextBlock)FindName("TitleStep3");
            identityConfirm = (TextBox)FindName("identityConfirmBox");
            serverConfirm = (TextBox)FindName("serverConfirmBox");
            SSLConfirm = (CheckBox)FindName("SSLConfirmCheck");
            passwordConfirm = (PasswordBox)FindName("passwordConfirmBox");
            portConfirm = (TextBox)FindName("portConfirmBox");
            validateConfirmButton = (Button)FindName("validateButtonConfirm");
            rememberPasswordConfirm = (CheckBox)FindName("passwordRememberConfirmCheck");*/
        }

        private void NextActionAddAccount(object sender, RoutedEventArgs e)
        {
            Boolean canNext = false;
            switch (stepAddAccount)
            {
                case 0:
                    canNext = doStepOne();
                    break;
                case 1:
                    canNext = doStepTwo();
                    break;
                default:
                    break;
            }
            if (canNext)
            {
                goNextStep();
            }
        }

        private void goNextStep()
        {
            stepAddAccount++;
            pivot.SelectedIndex = pivot.SelectedIndex + 1;
        }

        private Boolean doStepOne()
        {
            Account newAccount = new Account(fullNameBox.Text, emailBox.Text, passwordBox.Password, rememberPasswordCheck.IsChecked.Value);
            Boolean wrongField = false;

            if (newAccount.fullName == String.Empty)
            {
                fullNameBox.BorderBrush = new SolidColorBrush(Colors.Red);
                //fullNameBox.Focus(FocusState.Programmatic);
                wrongField = true;
            }
            if (newAccount.password == String.Empty)
            {
                passwordBox.BorderBrush = new SolidColorBrush(Colors.Red);
                //passwordBox.Focus(FocusState.Programmatic);
                wrongField = true;
            }
            if (newAccount.getEmail() == String.Empty)
            {
                emailBox.BorderBrush = new SolidColorBrush(Colors.Red);
                //emailBox.Focus(FocusState.Programmatic);
                wrongField = true;
            }
            if (!newAccount.getEmail().Contains("@"))
            {
                emailBox.BorderBrush = new SolidColorBrush(Colors.Red);
                wrongField = true;
            }
            if (!wrongField)
            {
                inCreationAccount = newAccount;
                goNextStep();
                return doStepTwo();
            }
            return false;
        }

        private Boolean doStepTwo()
        {

            Boolean result = tryToConnectWith(inCreationAccount, "imap");

            if (result)
            {
                fillStep3Information(true);
                return true;
            }

            fillStep3Information(false);
            return false;

            /*var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadOnly);

            Console.WriteLine("Total messages: {0}", inbox.Count);
            Console.WriteLine("Recent messages: {0}", inbox.Recent);*/


        }

        private Boolean tryToConnectWith(Account thatAccount, String type)
        {
            using (var client = new ImapClient())
            {
                String server = thatAccount.getServer(type);
                int port = thatAccount.port;
                Boolean useSSL = thatAccount.useSSL;

                try
                {
                    client.Connect(server, port, useSSL);
                }
                catch (MailKit.ServiceNotConnectedException)
                {
                    System.Diagnostics.Debug.WriteLine("Error: Connection error");
                    return false;
                }
                catch (System.Exception)
                {
                    System.Diagnostics.Debug.WriteLine("Error: Connection error");
                    return false;
                }

                String identity = thatAccount.getIdentity();

                try
                {
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(identity, thatAccount.password);
                }
                catch (MailKit.Net.Imap.ImapProtocolException)
                {
                    System.Diagnostics.Debug.WriteLine("Error: Authentificate error");
                    return false;
                }
            }
            return true;
        }

        private void fillStep3Information(Boolean isAuthentificate)
        {
            nextButton.Visibility = Visibility.Collapsed;
            cancelButton.Visibility = Visibility.Collapsed;
            if (!isAuthentificate)
            {
                titleStep3.Text = "Impossible de se connecter au serveur automatiquement, aide moi !";
            }
            identityConfirm.Text = inCreationAccount.getIdentity();
            serverConfirm.Text = inCreationAccount.getServer("imap");
            portConfirm.Text = inCreationAccount.port + "";
            if (inCreationAccount.useSSL)
            {
                SSLConfirm.IsChecked = true;
            }
            else
            {
                SSLConfirm.IsChecked = false;
            }
            passwordConfirm.Password = inCreationAccount.password;
        }

        private void CancelActionAddAccount(object sender, RoutedEventArgs e)
        {
           // this.Hide();
        }

        private void validateButtonClick(object sender, RoutedEventArgs e)
        {
            titleStep3.Text = "Tentative de connexion ...";
            String email = identityConfirm.Text + "@" + serverConfirm.Text.Substring(serverConfirm.Text.IndexOf(".") + 1);
            int port = Int32.Parse(portConfirm.Text);
            String fullName = "ttt"; //TODO
            Boolean SSLCheck = SSLConfirm.IsChecked.Value;
            String password = passwordConfirm.Password;
            Boolean rememberPassword = rememberPasswordConfirm.IsChecked.Value;

            Account newTryAccount = new Account(fullName, email, password, rememberPassword);
            newTryAccount.useSSL = SSLCheck;
            newTryAccount.port = port;
            if (tryToConnectWith(newTryAccount, "imap"))
            {
                goNextStep();
                goNextStep();
            } else
            {

            }
        }

        private void finalValidateClick(object sender, RoutedEventArgs e)
        {
            /*Save new account*/
            //this.Hide();
        }
    }
}
