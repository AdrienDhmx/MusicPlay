using IconButton;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TextBtn;

namespace MessageControl
{
    public class MessageControl : Control
    {
        private const string PART_GridMain = "PART_GridMain";
        private const string PART_Interaction = "PART_Interaction";
        private const string PART_BtnClose = "PART_BtnClose";
        private const string PART_BtnUndo = "PART_BtnUndo";
        private const string PART_BtnCancel = "PART_BtnCancel";
        private const string PART_BtnConfirm = "PART_BtnConfirm";
        private const string PART_DateTime = "PART_DateTime";

        public Geometry Icon
        {
            get { return (Geometry)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public Brush FillColor
        {
            get { return (Brush)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }

        public Brush StrokeColor
        {
            get { return (Brush)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Stretch IconStretch
        {
            get { return (Stretch)GetValue(IconStretchProperty); }
            set { SetValue(IconStretchProperty, value); }
        }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public bool IsTimeDisplayed
        {
            get { return (bool)GetValue(IsTimeDisplayedProperty); }
            set { SetValue(IsTimeDisplayedProperty, value); }
        }

        public bool IsInteractive
        {
            get { return (bool)GetValue(IsInteractiveProperty); }
            set { SetValue(IsInteractiveProperty, value); }
        }

        public bool IsInteractiveWithCancel
        {
            get { return (bool)GetValue(IsInteractiveWithCancelProperty); }
            set { SetValue(IsInteractiveWithCancelProperty, value); }
        }

        public static readonly DependencyProperty IsInteractiveWithCancelProperty =
            DependencyProperty.Register("IsInteractiveWithCancel", typeof(bool), typeof(MessageControl), new PropertyMetadata(false));


        public Action<bool> InteractionCallBack
        {
            get { return (Action<bool>)GetValue(InteractionCallBackProperty); }
            set { SetValue(InteractionCallBackProperty, value); }
        }

        public string ConfirmBtnText
        {
            get { return (string)GetValue(ConfirmBtnTextProperty); }
            set { SetValue(ConfirmBtnTextProperty, value); }
        }

        public string CancelBtnText
        {
            get { return (string)GetValue(CancelBtnTextProperty); }
            set { SetValue(CancelBtnTextProperty, value); }
        }

        public bool IsUndoBtnVisible
        {
            get { return (bool)GetValue(IsUndoBtnVisibleProperty); }
            set { SetValue(IsUndoBtnVisibleProperty, value); }
        }

        public string UndoBtnText
        {
            get { return (string)GetValue(UndoBtnTextProperty); }
            set { SetValue(UndoBtnTextProperty, value); }
        }

        public Func<bool> UndoCallBack
        {
            get { return (Func<bool>)GetValue(UndoCallBackProperty); }
            set { SetValue(UndoCallBackProperty, value); }
        }

        public string UndoneMessage
        {
            get { return (string)GetValue(UndoneMessageProperty); }
            set { SetValue(UndoneMessageProperty, value); }
        }

        public string UndoneFailedMessage
        {
            get { return (string)GetValue(UndoneFailedMessageProperty); }
            set { SetValue(UndoneFailedMessageProperty, value); }
        }

        public static readonly DependencyProperty UndoneFailedMessageProperty =
            DependencyProperty.Register("UndoneFailedMessage", typeof(string), typeof(MessageControl), new PropertyMetadata("The action could not be undone."));

        public bool StaysOpen
        {
            get { return (bool)GetValue(StaysOpenProperty); }
            set { SetValue(StaysOpenProperty, value); }
        }

        public Brush MouseOverBackground
        {
            get { return (Brush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        public Action OnCloseCallBack
        {
            get { return (Action)GetValue(OnCloseCallBackProperty); }
            set { SetValue(OnCloseCallBackProperty, value); }
        }

        public static readonly DependencyProperty OnCloseCallBackProperty =
            DependencyProperty.Register("OnCloseCallBack", typeof(Action), typeof(MessageControl), new PropertyMetadata(null));

        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(MessageControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0,0,0,0))));

        public static readonly DependencyProperty StaysOpenProperty =
            DependencyProperty.Register("StaysOpen", typeof(bool), typeof(MessageControl), new PropertyMetadata(false));

        public static readonly DependencyProperty UndoneMessageProperty =
            DependencyProperty.Register("UndoneMessage", typeof(string), typeof(MessageControl), new PropertyMetadata("The action has been undone."));

        public static readonly DependencyProperty UndoCallBackProperty =
            DependencyProperty.Register("UndoCallBack", typeof(Func<bool>), typeof(MessageControl), new PropertyMetadata(null));

        public static readonly DependencyProperty UndoBtnTextProperty =
            DependencyProperty.Register("UndoBtnText", typeof(string), typeof(MessageControl), new PropertyMetadata("undo"));

        public static readonly DependencyProperty IsUndoBtnVisibleProperty =
            DependencyProperty.Register("IsUndoBtnVisible", typeof(bool), typeof(MessageControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsConfirmBtnVisibleProperty =
            DependencyProperty.Register("IsConfirmBtnVisible", typeof(bool), typeof(MessageControl), new PropertyMetadata(true));

        public static readonly DependencyProperty CancelBtnTextProperty =
            DependencyProperty.Register("CancelBtnText", typeof(string), typeof(MessageControl), new PropertyMetadata("Cancel"));

        public static readonly DependencyProperty ConfirmBtnTextProperty =
            DependencyProperty.Register("ConfirmBtnText", typeof(string), typeof(MessageControl), new PropertyMetadata("Confirm"));

        public static readonly DependencyProperty InteractionCallBackProperty =
            DependencyProperty.Register("InteractionCallBack", typeof(Action<bool>), typeof(MessageControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IsInteractiveProperty =
            DependencyProperty.Register("IsInteractive", typeof(bool), typeof(MessageControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsTimeDisplayedProperty =
            DependencyProperty.Register("IsTimeDisplayed", typeof(bool), typeof(MessageControl), new PropertyMetadata(true));

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(MessageControl), new PropertyMetadata(""));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Geometry), typeof(MessageControl), new PropertyMetadata(new PathGeometry()));

        public static readonly DependencyProperty FillColorProperty =
            DependencyProperty.Register("FillColor", typeof(Brush), typeof(MessageControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250, 250, 250, 250))));

        public static readonly DependencyProperty StrokeColorProperty =
            DependencyProperty.Register("StrokeColor", typeof(Brush), typeof(MessageControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(250, 250, 250, 250))));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MessageControl), new PropertyMetadata(new CornerRadius(0)));

        public static readonly DependencyProperty IconStretchProperty =
            DependencyProperty.Register("IconStretch", typeof(Stretch), typeof(MessageControl), new PropertyMetadata(Stretch.None));

        private Grid _gridMain;
        private IconButton.IconButton _buttonClose;
        private Panel _panelInteraction;
        private TextButton _buttonUndo;
        private TextButton _buttonCancel;
        private TextButton _buttonConfirm;
        private TextBlock _textDateTime;

        private bool _staysOpen;
        private int _waitTime = 6;
        private int _tickCount;
        private DispatcherTimer _timerClose;

        static MessageControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageControl), new FrameworkPropertyMetadata(typeof(MessageControl)));
        }

        public MessageControl()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _panelInteraction = GetTemplateChild(PART_Interaction) as Panel;
            _gridMain = GetTemplateChild(PART_GridMain) as Grid;
            _buttonClose = GetTemplateChild(PART_BtnClose) as IconButton.IconButton;
            _buttonUndo = GetTemplateChild(PART_BtnUndo) as TextButton;
            _buttonCancel = GetTemplateChild(PART_BtnCancel) as TextButton;
            _buttonConfirm = GetTemplateChild(PART_BtnConfirm) as TextButton;
            _textDateTime = GetTemplateChild(PART_DateTime) as TextBlock;

            CheckNull();
            Update();
        }

        private void CheckNull()
        {
            if (_panelInteraction == null || _gridMain == null || _buttonClose == null || _buttonUndo == null || _textDateTime == null
                || _buttonCancel == null || _buttonConfirm == null) throw new Exception();
        }

        private void Update()
        {
            _panelInteraction.Visibility = IsInteractive ? Visibility.Visible : Visibility.Collapsed;
            _textDateTime.Visibility = IsTimeDisplayed ? Visibility.Visible : Visibility.Collapsed;
            _buttonUndo.Visibility = IsUndoBtnVisible ? Visibility.Visible : Visibility.Collapsed;
            _buttonCancel.Visibility = IsInteractiveWithCancel ? Visibility.Visible : Visibility.Collapsed;

            _buttonClose.Visibility = _buttonUndo.Visibility;

            _buttonClose.Command = new RelayCommand(Close);
            _buttonCancel.Command = new RelayCommand(() => Close(false));
            _buttonConfirm.Command = new RelayCommand(() => Close(true));
            _buttonUndo.Command = new RelayCommand(UndoClose);

            if (IsTimeDisplayed)
            {
                _textDateTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            }
            _staysOpen = StaysOpen;

            var transform = new TranslateTransform
            {
                X = FlowDirection == FlowDirection.LeftToRight ? MaxWidth : -MaxWidth
            };
            _gridMain.RenderTransform = transform;
            transform.BeginAnimation(TranslateTransform.XProperty, CreateAnimation(0));
            if (!_staysOpen) StartTimer();
        }

        private void StartTimer()
        {
            _timerClose = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timerClose.Tick += delegate
            {
                if (IsMouseOver)
                {
                    _tickCount = 0;
                    return;
                }

                _tickCount++;
                if (_tickCount >= _waitTime)
                {
                    Close();
                }
            };
            _timerClose.Start();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            _buttonClose.Visibility = Visibility.Visible;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if(!IsUndoBtnVisible)
                _buttonClose.Visibility = Visibility.Collapsed;
        }

        private void Close()
        {
            _timerClose?.Stop();
            var transform = new TranslateTransform();
            _gridMain.RenderTransform = transform;
            var animation = CreateAnimation(FlowDirection == FlowDirection.LeftToRight ? ActualWidth : -ActualWidth);
            animation.Completed += (s, e) =>
            {
                if (Parent is Panel panel)
                {
                    panel.Children.Remove(this);
                }
            };
            transform.BeginAnimation(TranslateTransform.XProperty, animation);

            OnCloseCallBack?.Invoke();
        }

        private void UndoClose()
        {
            if (UndoCallBack.Invoke())
            {
                Message = UndoneMessage;
            }
            else
            {
                Message = UndoneFailedMessage;
            }
            _buttonUndo.Visibility = Visibility.Collapsed;

            // reset tick
            _tickCount = 0;
        }

        private void Close(bool confirm)
        {
            if (IsInteractive)
            {
                InteractionCallBack?.Invoke(confirm);
                Close();
            }
        }

        public static DoubleAnimation CreateAnimation(double toValue, double milliseconds = 200)
        {
            return new(toValue, new Duration(TimeSpan.FromMilliseconds(milliseconds)))
            {
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut }
            };
        }
    }

    internal class RelayCommand : ICommand
    {
        #region Fields

        readonly Action _execute = null;
        readonly Predicate<string> _canExecute = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommand{T}"/>.
        /// </summary>
        /// <param name="execute">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Predicate<string> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        ///<summary>
        ///Defines the method that determines whether the command can execute in its current state.
        ///</summary>
        ///<param name="parameter">Data used by the command.  If the command does not require fft to be passed, this object can be set to null.</param>
        ///<returns>
        ///true if this command can be executed; otherwise, false.
        ///</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        ///<summary>
        ///Occurs when changes occur that affect whether or not the command should execute.
        ///</summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        ///<summary>
        ///Defines the method to be called when the command is invoked.
        ///</summary>
        ///<param name="parameter">Data used by the command. If the command does not require fft to be passed, this object can be set to <see langword="null" />.</param>
        public void Execute(object parameter)
        {
            _execute();
        }

        #endregion
    }

}
