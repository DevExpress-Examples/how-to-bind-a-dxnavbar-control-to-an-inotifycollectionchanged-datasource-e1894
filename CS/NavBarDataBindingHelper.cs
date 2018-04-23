using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using DevExpress.Xpf.NavBar;

namespace NavBar_DataBinding {

    public class NavBarDataBindingHelper : DependencyObject {
        public static readonly DependencyProperty ItemsSourceProperty;

        static NavBarDataBindingHelper() {
            ItemsSourceProperty = DependencyProperty.RegisterAttached("ItemsSource", 
                typeof(IEnumerable), typeof(NavBarDataBindingHelper), 
                new FrameworkPropertyMetadata(null, OnItemsSourceChanged));
        }

        public static void SetItemsSource(DependencyObject element, IEnumerable<NavBarGroup> value) {
            element.SetValue(NavBarDataBindingHelper.ItemsSourceProperty, value);
        }

        public static IEnumerable GetItemsSource(DependencyObject element) {
            return (IEnumerable)element.GetValue(NavBarDataBindingHelper.ItemsSourceProperty);
        }

        static Dictionary<NavBarControl, Binder> navBars = new Dictionary<NavBarControl, Binder>();

        static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            NavBarControl navBar = d as NavBarControl;
            if (navBar == null)
                return;
            if (navBars.ContainsKey(navBar)) {
                navBars[navBar].UnsubscribeEventsIfNeeded();
                navBars.Remove(navBar);
            }
            if (e.NewValue != null)
                navBars[navBar] = new Binder(navBar, NavBarDataBindingHelper.GetItemsSource(navBar));
        }
    }

    class Binder {
        NavBarControl navBar;
        INotifyCollectionChanged collection;

        public Binder(NavBarControl navBar, IEnumerable source) {
            this.navBar = navBar;

            navBar.Groups.Clear();
            foreach (NavBarGroup group in source)
                navBar.Groups.Add(group);

            collection = source as INotifyCollectionChanged;
            if (collection != null)
                collection.CollectionChanged += 
                    new NotifyCollectionChangedEventHandler(BindingHelper_CollectionChanged);
        }

        public void UnsubscribeEventsIfNeeded() {
            if (collection != null)
                collection.CollectionChanged -= 
                    new NotifyCollectionChangedEventHandler(BindingHelper_CollectionChanged);
        }

        void BindingHelper_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.NewItems != null)
                foreach (NavBarGroup group in e.NewItems)
                    navBar.Groups.Add(group);
            if (e.OldItems != null)
                foreach (NavBarGroup group in e.OldItems)
                    navBar.Groups.Remove(group);
        }
    }
}