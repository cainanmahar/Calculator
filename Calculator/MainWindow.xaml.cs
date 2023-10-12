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

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        public static RoutedCommand DigitCmd { get { return _digitCmd; } }
        public static RoutedCommand OperationCmd { get { return _operationCmd; } }
        public string LeftOperand
        {
            get { return _leftOperand ?? ""; }
            set { _leftOperand = value; OnPropertyChanged("LeftOperand"); }
        }
        public string RightOperand
        {
            get { return _rightOperand; }
            set { _rightOperand = value; OnPropertyChanged("RightOperand"); }
        }
        public char Operation
        {
            get { return _operation; }
            set { _operation = value; OnPropertyChanged("Operation"); }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DigitCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RightOperand += e.Parameter as string;
            //CommandManager.InvalidateRequerySuggested();
        }

        private void DigitCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            char param = '\0';
            if (e.Parameter != null && ((string)e.Parameter).Length == 1)
            {
                param = ((string)e.Parameter)[0];
            }

            // param is '\0' if command parameter is not a length 1 string

            if (char.IsAsciiDigit(param))
            {
                // Can always add a digit.
                e.CanExecute = true;
            }
            else if (param == '.')
            {
                // Can't add a decimal to a number that already has one.
                e.CanExecute = !RightOperand.Contains('.');
            }
            else
            {
                // Not decimal or digit. Not allowed.
                e.CanExecute = false;
            }
        }

        private void OperationCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var newOp = ((string)e.Parameter)[0];
            execCurrentOperation();
            if (newOp == '=')
            {
                newOp = '\0';
            }
            else
            {
                LeftOperand = RightOperand;
                RightOperand = "";
            }
            Operation = newOp;
            //CommandManager.InvalidateRequerySuggested();
        }

        private void OperationCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter is string && ((string)e.Parameter).Length == 1)
            {
                switch (((string)e.Parameter)[0])
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '=':
                        e.CanExecute = RightOperand != "";
                        break;
                    default:
                        e.CanExecute = false;
                        break;
                }
            }
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void AllClear_Click(object sender, RoutedEventArgs e)
        {
            LeftOperand = "";
            RightOperand = "";
            Operation = '\0';
        }

        public void Clear_Click(object sender, RoutedEventArgs e)
        {
            RightOperand = "";
        }

        public void Sign_Click(object sender, RoutedEventArgs e)
        {
            if (RightOperand.Length > 0)
            {
                if (RightOperand[0] != '-')
                {
                    RightOperand = "-" + RightOperand;
                }
                else
                {
                    RightOperand = RightOperand.Substring(1);
                }
            }
        }

        static readonly RoutedCommand _digitCmd = new();
        static readonly RoutedCommand _operationCmd = new();
        string _leftOperand = "";
        string _rightOperand = "";
        char _operation = '\0';

        private void execCurrentOperation()
        {
            if (Operation == '\0') { return; }

            var left = double.Parse(LeftOperand);
            var right = double.Parse(RightOperand);
            LeftOperand = "";
            switch (_operation)
            {
                case '+':
                    RightOperand = (left + right).ToString();
                    break;
                case '-':
                    RightOperand = (left - right).ToString();
                    break;
                case '*':
                    RightOperand = (left * right).ToString();
                    break;
                case '/':
                    RightOperand = (left / right).ToString();
                    break;
                default:
                    // Unreachable
                    Console.Error.WriteLine("Invalid operand type when attempting to execute.");
                    break;
            }
        }
    }
}
