using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using TaskManager.Annotations;

namespace TaskManager
{
    class ViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Process> _processes;
        private MainWindow Parent;

        public ObservableCollection<Process> Processes {
            get { return _processes; }
            set
            {
                OnPropertyChanged("Processes");
                _processes = value;
                
            }
        }

        private Process _selectedItem;
        public Process SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
        public ViewModel(MainWindow parent)
        {
            Parent = parent;

            Processes = new ObservableCollection<Process>();
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Tick += UpdateProcPriorityChangedEvent;
            timer.Start();
        }

        private void UpdateProcesses(object sender, EventArgs e)
        {
            var currentIds = Processes.Select(p => p.Id).ToList();

            foreach (var p in Process.GetProcesses())
            {
                var d = Processes.FirstOrDefault(l => l.Id == p.Id);
                try
                {
                    if (p.Id == 9292)
                    {
                        Console.WriteLine("duia");
                    }
                    if (d != null)
                        if ((d as Process).PriorityClass != p.PriorityClass)
                        {
                            d.PriorityClass = p.PriorityClass;

                        }
                }
                catch (Exception a)
                {
                    Console.WriteLine("Nie pyklo");
                }
                if (!currentIds.Remove(p.Id)) // it's a new process id
                {
                    Processes.Add(p);
                }
            }

            foreach (var id in currentIds) // these do not exist any more
            {
                Processes.Remove(Processes.First(p => p.Id == id));
            }

            Parent.MasterListView.GetBindingExpression(ListView.ItemsSourceProperty)
                .UpdateSource();
        }

        public void UpdateProcPriorityChanged()
        {

            Processes.Clear();

            foreach (var p in Process.GetProcesses())
            {
               // p.Modules[0].
                Processes.Add(p);

            }
        }


        public void UpdateProcPriorityChangedEvent(object sender, EventArgs e)
        {

            var newProcesses = Process.GetProcesses();
            List<Process> toRemove=new List<Process>();
            List<Process> toAdd = new List<Process>();

            foreach (var a in Parent.chosenProcesses)
            {
                if (newProcesses.Count(c => c.Id==a.Id)==0)
                {
                    Console.WriteLine("dupa");
                    var p = new Process();

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = Parent.paths[a.Id]; //path/process name
                    p.StartInfo = startInfo;
                    var started = p.Start();
                    Thread.Sleep(10);
                    toRemove.Add(a);
                    toAdd.Add(p);
                    Parent.paths.Add(p.Id,p.MainModule.FileName);

                }

                
            }
            foreach (var a in toRemove)
            {
                Parent.chosenProcesses.Remove(a);
            }

            foreach (var a in toAdd)
            {
                Parent.chosenProcesses.Add(a);
            }
            Processes.Clear();
            foreach (var p in newProcesses)
            {

                Processes.Add(p);

            }
           
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
