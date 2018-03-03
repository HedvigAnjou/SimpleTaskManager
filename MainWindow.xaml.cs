using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace TaskManager
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel model;
        public List<Process> chosenProcesses=new List<Process>();
        public Dictionary<int, string> paths=new Dictionary<int, string>();
     
        public MainWindow()
        {
            InitializeComponent();
            model =  new ViewModel(this);
            DataContext = model;
           
        }

 

        private void SelectionChangedMaster(object sender, SelectionChangedEventArgs e)
        {
            if(((sender as ListView).SelectedItem as Process)!= null){
                Console.WriteLine(((sender as ListView).SelectedItem as Process).Id);
                model.SelectedItem = ((sender as ListView).SelectedItem as Process);
                Process proca = ((sender as ListView).SelectedItem as Process);
                Console.WriteLine(proca.Id);
            }
        }

        private void KillProcessClick(object sender, RoutedEventArgs e)
        {
           (model.SelectedItem as Process).Kill();
           // (model.SelectedItem as Process).Threads[0].
        }

        private void ChangePriority(object sender, RoutedEventArgs e)
        {
            PriorityComboBox.Visibility = Visibility.Visible;
            PriorityOK.Visibility = Visibility.Visible;
        }

        private void ChangePriority_OK(object sender, RoutedEventArgs e)
        {
            var a = PriorityComboBox.SelectedItem.ToString();
            if (PriorityComboBox.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: Idle")
                (model.SelectedItem as Process).PriorityClass = ProcessPriorityClass.Idle;
            else if (PriorityComboBox.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: Normal")
                (model.SelectedItem as Process).PriorityClass = ProcessPriorityClass.Normal;
            else if (PriorityComboBox.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: High")
                (model.SelectedItem as Process).PriorityClass = ProcessPriorityClass.High;
            else if (PriorityComboBox.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: RealTime")
                (model.SelectedItem as Process).PriorityClass = ProcessPriorityClass.RealTime;
            //  (model.SelectedItem as Process).PriorityClass = PriorityTextBox.Text;
            model.UpdateProcPriorityChanged();
            MasterListView.GetBindingExpression(ListView.ItemsSourceProperty)
                .UpdateTarget();
        }

        private void UpdateChosenProcesses(object sender, RoutedEventArgs e)
        {
            if (((sender as Button).Content) as string == "Dodaj")
            {
                int a = Int32.Parse((sender as Button).Tag.ToString());
                Process b = model.Processes.FirstOrDefault(l => l.Id == a);
                chosenProcesses.Add(b);
                paths.Add(b.Id, b.MainModule.FileName);
                MessageBox.Show("Process " + a + " added to your list");
            }
            else if ((sender as Button).Content as string == "Usun")
            {
                int a = Int32.Parse((sender as Button).Tag.ToString());
                Process b = chosenProcesses.FirstOrDefault(l => l.Id == a);
                chosenProcesses.Remove(b);
                MessageBox.Show("Process "+a+" deleted from your special list");
            }
        }

        private void seeModulesClick(object sender, RoutedEventArgs e)
        {
            if (modulesPanel.Visibility == Visibility.Collapsed)
                modulesPanel.Visibility = Visibility.Visible;
            else
            {
                modulesPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}
