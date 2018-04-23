using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.NavBar;

namespace NavBar_DataBinding {
    public partial class Window1 : Window {

        ObservableCollection<NavBarGroup> groups;

        public Window1() {
            InitializeComponent();

            groups = new ObservableCollection<NavBarGroup>();
            for (int i = 0; i < 5; i++)
                groups.Add(new NavBarGroup() { Header = "Group" + count++ });

            DataContext = groups;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        int count = 0;
        Random random = new Random();

        void timer_Tick(object sender, EventArgs e) {
            if ((random.Next(2) == 1 || groups.Count == 0) && groups.Count < 10)
                groups.Add(new NavBarGroup() { Header = "Group" + count++ });
            else
                groups.RemoveAt(groups.Count - 1);
        }
    }
}
