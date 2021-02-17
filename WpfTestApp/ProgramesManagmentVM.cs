using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WpfTestApp
{
    [Notify]
    public class ProgramesManagmentVM : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        public ProgramesManagmentVM()
        {
            SelectedProgramesList = new Kemorave.Collection.ObservableList<object>();
            SelectedProgramesList.CollectionChanged += SelectedProgramesList_CollectionChanged;
            ProgramesList = new Kemorave.Collection.ObservableList<Kemorave.Win.RegistryTools.ProgramInfo>();
            UninstallApplicationCommand = new Kemorave.Command.RelayCommand<object>(UninstallApplication);
            UpdateApplicationCommand = new Kemorave.Command.RelayCommand<object>(UpdateApplication);
            RefreshCommand = new Kemorave.Command.RelayCommand(Refresh);
            Refresh();
            this.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.ShowSystemComponentApps))
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            if (IsBusy)
            {
                return;
            }
            IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    TotalProgramsSize = 0;
                    if (ShowSystemComponentApps)
                    {
                        ProgramesList.AddRange(Kemorave.Win.RegistryTools.RegistryHelper.GetAllInstalledPrograms(), true);
                    }
                    else
                    {
                        ProgramesList.AddRange(Kemorave.Win.RegistryTools.RegistryHelper.GetAllInstalledPrograms().Where(a => !a.IsSystemComponent), true);
                    }

                    foreach (Kemorave.Win.RegistryTools.ProgramInfo item in ProgramesList)
                    {
                        if (item.EstimatedSize != null)
                        {
                            TotalProgramsSize += item.EstimatedSize ?? 0;
                        }
                    }
                }
                catch (Exception ex)
                {

                    System.Windows.MessageBox.Show("Error", ex.Message, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        private void SelectedProgramesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (SelectedProgramesList.Count <= 0)
            {
                return;
            }
            TotalSelectionSize = 0;
            CanUninstall = false;
            this.CanOpenInfo = false;
            this.CanUpdate = false;
            CanQuiteUninstall = false;
            try
            {
                foreach (Kemorave.Win.RegistryTools.ProgramInfo item in SelectedProgramesList)
                {
                    if (item == null)
                    {
                        return;
                    }
                    if (!string.IsNullOrEmpty(item.UninstallString))
                    {
                        CanUninstall = true;
                    }
                    if (!string.IsNullOrEmpty(item.UpdateLink))
                    {
                        CanUpdate = true;
                    }
                    if (!string.IsNullOrEmpty(item.HelpLink))
                    {
                        CanOpenInfo = true;
                    }
                    if (!string.IsNullOrEmpty(item.QuietUninstallString))
                    {
                        CanQuiteUninstall = true;
                    }
                    if (item.EstimatedSize != null)
                    {

                        TotalSelectionSize += item.EstimatedSize;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }
            finally { }
        }

        private void UpdateApplication(object obj)
        {
            if (IsBusy)
            {
                return;
            }
            IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    ProgramesList.AddRange(Kemorave.Win.RegistryTools.RegistryHelper.GetAllInstalledPrograms(), true);
                }
                catch (Exception ex)
                {

                    System.Windows.MessageBox.Show("Error", ex.Message, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        private void UninstallApplication(object obj)
        {
            if (IsBusy)
            {
                return;
            }
            IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    ProgramesList.AddRange(Kemorave.Win.RegistryTools.RegistryHelper.GetAllInstalledPrograms(), true);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error", ex.Message, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        /// <summary>
        /// zdbfg
        /// </summary>
        [NonNotify]
        public Kemorave.Collection.ObservableList<object> SelectedProgramesList { get; }
        [NonNotify]
        public Kemorave.Collection.ObservableList<Kemorave.Win.RegistryTools.ProgramInfo> ProgramesList { get; }
        [NonNotify]
        public Kemorave.Command.RelayCommand<object> UninstallApplicationCommand { get; set; }
        [NonNotify]
        public Kemorave.Command.RelayCommand<object> UpdateApplicationCommand { get; set; }
        [NonNotify]
        public Kemorave.Command.RelayCommand<object> OpenApplicationInfoCommand { get; set; }
        [NonNotify]
        public Kemorave.Command.RelayCommand<object> QuiteUninstallApplicationCommand { get; set; }
        [NonNotify]
        public Kemorave.Command.RelayCommand RefreshCommand { get; set; }
        public bool ShowSystemComponentApps { get => showSystemComponentApps; set => SetProperty(ref showSystemComponentApps, value, showSystemComponentAppsPropertyChangedEventArgs); }
        public long TotalProgramsSize { get => totalProgramsSize; set => SetProperty(ref totalProgramsSize, value, totalProgramsSizePropertyChangedEventArgs); }
        /// <summary>
        /// Selected applications size
        /// </summary>
        public long? TotalSelectionSize { get => totalSelectionSize; private set => SetProperty(ref totalSelectionSize, value, totalSelectionSizePropertyChangedEventArgs); }
        /// <summary>
        /// Sees if can uninstall selected applications
        /// </summary>
        public bool CanQuiteUninstall { get => canQuiteUninstall; set => SetProperty(ref canQuiteUninstall, value, canQuiteUninstallPropertyChangedEventArgs); }
        public bool CanUninstall { get => canUninstall; set => SetProperty(ref canUninstall, value, canUninstallPropertyChangedEventArgs); }
        public bool CanOpenInfo { get => canOpenInfo; set => SetProperty(ref canOpenInfo, value, canOpenInfoPropertyChangedEventArgs); }
        public bool CanUpdate { get => canUpdate; set => SetProperty(ref canUpdate, value, canUpdatePropertyChangedEventArgs); }
        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value, isBusyPropertyChangedEventArgs); }

        #region NotifyPropertyChangedGenerator

        public event PropertyChangedEventHandler PropertyChanged;

        private bool showSystemComponentApps;
        private static readonly PropertyChangedEventArgs showSystemComponentAppsPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(ShowSystemComponentApps));
        private long totalProgramsSize;
        private static readonly PropertyChangedEventArgs totalProgramsSizePropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(TotalProgramsSize));
        private long? totalSelectionSize;
        private static readonly PropertyChangedEventArgs totalSelectionSizePropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(TotalSelectionSize));
        private bool canQuiteUninstall;
        private static readonly PropertyChangedEventArgs canQuiteUninstallPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(CanQuiteUninstall));
        private bool canUninstall;
        private static readonly PropertyChangedEventArgs canUninstallPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(CanUninstall));
        private bool canOpenInfo;
        private static readonly PropertyChangedEventArgs canOpenInfoPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(CanOpenInfo));
        private bool canUpdate;
        private static readonly PropertyChangedEventArgs canUpdatePropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(CanUpdate));
        private bool isBusy;
        private static readonly PropertyChangedEventArgs isBusyPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsBusy));

        private void SetProperty<T>(ref T field, T value, PropertyChangedEventArgs ev)
        {
            if (!System.Collections.Generic.EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, ev);
            }
        }

        #endregion
    }
}