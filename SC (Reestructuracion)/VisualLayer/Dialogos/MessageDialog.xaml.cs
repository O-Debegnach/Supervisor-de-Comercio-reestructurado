using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using DataLayer;

namespace VisualLayer.Dialogos
{
    /// <summary>
    /// Lógica de interacción para MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : UserControl, INotifyPropertyChanged
    {
        #region Propiedades

        #region MessageDialogState
        public MessageDialogState MessageDialogState
        {
            get { return (MessageDialogState)GetValue(MessageDialogStateProperty); }
            set { SetValue(MessageDialogStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MessageDialogState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageDialogStateProperty =
            DependencyProperty.Register("MessageDialogState", typeof(MessageDialogState), typeof(MessageDialog), new PropertyMetadata(default(MessageDialogState), MessageDialogStateChanged));

        private static void MessageDialogStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            switch (e.NewValue)
            {
                case MessageDialogState.OK:
                    (d as MessageDialog).Icono.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckCircle;
                    break;

                case MessageDialogState.Error:
                    (d as MessageDialog).Icono.Kind = MaterialDesignThemes.Wpf.PackIconKind.Error;
                    break;

                case MessageDialogState.Warning:
                    (d as MessageDialog).Icono.Kind = MaterialDesignThemes.Wpf.PackIconKind.Warning;
                    break;

                case MessageDialogState.Info:
                    (d as MessageDialog).Icono.Kind = MaterialDesignThemes.Wpf.PackIconKind.Help;
                    break;
            }
        }
        #endregion MessageDialogState

        #region Message
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(MessageDialog), new PropertyMetadata(string.Empty));
        #endregion Message


        #region Title

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MessageDialog), new PropertyMetadata("Listo"));
        #endregion Title


        #region Description



        public string Description
        {
            get { return (string)GetValue(DescritionProperty); }
            set { SetValue(DescritionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Descrition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescritionProperty =
            DependencyProperty.Register("Descrition", typeof(string), typeof(MessageDialog), new PropertyMetadata(string.Empty));



        #endregion Description


        #region MessageDialogButtons


        public MessageDialogButtons MessageDialogButtons
        {
            get { return (MessageDialogButtons)GetValue(MessageDialogButtonsProperty); }
            set { SetValue(MessageDialogButtonsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MessageDialogButtons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageDialogButtonsProperty =
            DependencyProperty.Register("MessageDialogButtons", typeof(MessageDialogButtons), typeof(MessageDialog), new PropertyMetadata(MessageDialogButtons.Nothing));
        private bool _isShowingDescription;




        #endregion MessageDialogButtons

        #region IsShowingDescription
        public bool IsShowingDescription 
        { 
            get => _isShowingDescription;
            private set
            { 
                if(value != _isShowingDescription)
                {
                    _isShowingDescription = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsShowingDescription)));
                }
            }
        }
        #endregion IsShowingDescription

        #endregion Propiedades


        #region Campos
        public event Action<MessageDialogResult> OnClosing;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Campos


        #region Constructores
        public MessageDialog()
        {
            InitializeComponent();
        }

        public MessageDialog(string message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            InitializeComponent();
        }

        public MessageDialog(string message, MessageDialogState messageDialogState) : this(message)
        {
            MessageDialogState = messageDialogState;
        }

        public MessageDialog(string message, MessageDialogState messageDialogState, MessageDialogButtons messageDialogButtons) : this(message, messageDialogState)
        {
            MessageDialogButtons = messageDialogButtons;
        }


        public MessageDialog(Error error)
        {
            InitializeComponent();
            Message = error.Message;
            Description = error.Description;
            MessageDialogState = error.MessageDialogState;
            Title = error.Title;

        }
        #endregion Constructores

        private void Evento_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button b)
            {
                if (b.Name.Equals(btAccept.Name))
                {
                    OnClosing?.Invoke(MessageDialogResult.OK);
                }
                else if (b.Name.Equals(btCancel.Name))
                {
                    OnClosing?.Invoke(MessageDialogResult.Cancel);
                }
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsShowingDescription = !IsShowingDescription;
        }
    }
}
